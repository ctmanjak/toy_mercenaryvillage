using System.Collections.Generic;
using Core;
using Data;
using UI;
using UnityEngine;

namespace Battle
{
    public enum BattlePhase { Ready, Fighting, Ended }
    public enum BattleResult { InProgress, Victory, Defeat }

    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private SpawnManager _spawnManager;
        [SerializeField] private BattleResultUI _resultUI;

        private List<BattleUnit> _allies = new();
        private List<BattleUnit> _enemies = new();
        private List<BattleUnit> _allUnits = new();
        private BattlePhase _phase = BattlePhase.Ready;
        private BattleResult _battleResult = BattleResult.InProgress;
        private StageData _currentStage;

        public BattlePhase Phase => _phase;
        public BattleResult BattleResult => _battleResult;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentStage != null)
            {
                StartBattle(GameManager.Instance.CurrentStage);
            }
        }

        private void Update()
        {
            if (_phase != BattlePhase.Fighting) return;

            foreach (var unit in _allUnits)
            {
                if (unit.IsAlive)
                {
                    unit.UpdateAI();
                }
            }

            _battleResult = CheckBattleResult();
            if (_battleResult != BattleResult.InProgress)
            {
                EndBattle(_battleResult);
            }
        }

        public BattleResult CheckBattleResult()
        {
            bool allAlliesDead = true;
            bool allEnemiesDead = true;

            foreach (var ally in _allies)
            {
                if (ally.IsAlive)
                {
                    allAlliesDead = false;
                    break;
                }
            }

            foreach (var enemy in _enemies)
            {
                if (enemy.IsAlive)
                {
                    allEnemiesDead = false;
                    break;
                }
            }

            if (allEnemiesDead) return BattleResult.Victory;
            if (allAlliesDead) return BattleResult.Defeat;
            return BattleResult.InProgress;
        }

        private void EndBattle(BattleResult result)
        {
            if (_phase == BattlePhase.Ended) return;

            _phase = BattlePhase.Ended;

            int reward = 0;
            if (result == BattleResult.Victory && _currentStage != null)
            {
                reward = _currentStage.GoldReward;

                if (PlayerResourceManager.Instance != null)
                {
                    PlayerResourceManager.Instance.AddGold(reward);
                }
            }

            if (_resultUI != null)
            {
                _resultUI.Show(result, reward);
            }

            Debug.Log($"[BattleManager] Battle ended: {result}, Reward: {reward}G");
        }
        
        public List<BattleUnit> GetAllUnits() => _allUnits;
        
        public List<BattleUnit> GetAllies() => _allies;
        
        public List<BattleUnit> GetEnemies() => _enemies;
        
        public List<BattleUnit> GetEnemiesOf(Team team)
        {
            return team == Team.Ally ? _enemies : _allies;
        }
        
        public void RegisterUnit(BattleUnit unit)
        {
            if (unit == null) return;
            if (_allUnits.Contains(unit)) return;

            _allUnits.Add(unit);

            if (unit.Team == Team.Ally)
                _allies.Add(unit);
            else
                _enemies.Add(unit);
        }
        
        public void UnregisterUnit(BattleUnit unit)
        {
            if (unit == null) return;

            _allUnits.Remove(unit);
            _allies.Remove(unit);
            _enemies.Remove(unit);
        }

        public BattleUnit SpawnUnit(UnitData unitData, int level, Team team, int spawnIndex)
        {
            if (_spawnManager == null)
            {
                Debug.LogError("[BattleManager] SpawnManager not assigned");
                return null;
            }

            if (unitData == null || unitData.Prefab == null)
            {
                Debug.LogWarning("[BattleManager] UnitData or Prefab is null, skipping");
                return null;
            }

            var unit = _spawnManager.SpawnUnit(unitData.Prefab, team, spawnIndex);
            if (unit == null) return null;

            unit.Initialize(unitData, level, team);
            RegisterUnit(unit);

            return unit;
        }
        
        public void ClearAllUnits()
        {
            foreach (var unit in _allUnits)
            {
                if (unit != null)
                    Destroy(unit.gameObject);
            }

            _allUnits.Clear();
            _allies.Clear();
            _enemies.Clear();
            _phase = BattlePhase.Ready;
            _battleResult = BattleResult.InProgress;
            _currentStage = null;

            if (_resultUI != null)
            {
                _resultUI.Hide();
            }
        }

        public void StartBattle(StageData stage)
        {
            if (_phase == BattlePhase.Fighting)
            {
                Debug.LogWarning("[BattleManager] Battle already in progress");
                return;
            }

            if (PartyManager.Instance == null)
            {
                Debug.LogError("[BattleManager] PartyManager not found!");
                return;
            }

            if (!PartyManager.Instance.CanStartBattle())
            {
                Debug.LogWarning("[BattleManager] Cannot start battle: Party is empty!");
                return;
            }

            ClearAllUnits();
            _currentStage = stage;

            SpawnEnemies(stage);
            SpawnAlliesFromParty();

            foreach (var unit in _allUnits)
            {
                unit.UpdateTarget();
            }

            _phase = BattlePhase.Fighting;
            _battleResult = BattleResult.InProgress;

            Debug.Log($"[BattleManager] Battle started: {stage.StageName}");
        }

        public List<BattleUnit> SpawnEnemies(StageData stage)
        {
            if (stage == null)
            {
                Debug.LogError("[BattleManager] StageData is null");
                return new List<BattleUnit>();
            }

            var spawnedEnemies = new List<BattleUnit>();

            foreach (var spawnInfo in stage.Enemies)
            {
                var unit = SpawnUnit(spawnInfo.UnitData, spawnInfo.Level, Team.Enemy, spawnInfo.SpawnIndex);
                if (unit != null)
                {
                    spawnedEnemies.Add(unit);
                }
            }

            Debug.Log($"[BattleManager] Spawned {spawnedEnemies.Count} enemies from stage: {stage.StageName}");
            return spawnedEnemies;
        }

        private List<BattleUnit> SpawnAlliesFromParty()
        {
            var spawnedAllies = new List<BattleUnit>();
            var party = PartyManager.Instance.GetParty();

            for (int i = 0; i < party.Length; i++)
            {
                if (party[i] == null) continue;

                var merc = party[i];
                var unit = SpawnUnit(merc.UnitData, merc.Level, Team.Ally, i);

                if (unit != null)
                {
                    spawnedAllies.Add(unit);
                }
            }

            Debug.Log($"[BattleManager] Spawned {spawnedAllies.Count} allies from party");
            return spawnedAllies;
        }
    }
}
