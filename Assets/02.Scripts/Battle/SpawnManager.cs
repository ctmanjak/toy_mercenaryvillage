using UI;
using UnityEngine;

namespace Battle
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private SpawnPoint[] _allySpawns = new SpawnPoint[4];
        [SerializeField] private SpawnPoint[] _enemySpawns = new SpawnPoint[4];

        [Header("UI")]
        [SerializeField] private UnitHealthBar _healthBarPrefab;
        
        public Vector3 GetSpawnPosition(Team team, int index)
        {
            var spawns = team == Team.Ally ? _allySpawns : _enemySpawns;

            if (index < 0 || index >= spawns.Length)
            {
                Debug.LogWarning($"[SpawnManager] Invalid spawn index: {index}");
                return Vector3.zero;
            }

            if (spawns[index] == null)
            {
                Debug.LogWarning($"[SpawnManager] Spawn point not assigned: {team} {index}");
                return Vector3.zero;
            }

            return spawns[index].transform.position;
        }
        
        public BattleUnit SpawnUnit(BattleUnit prefab, Team team, int index)
        {
            var position = GetSpawnPosition(team, index);
            var unit = Instantiate(prefab, position, Quaternion.identity);

            CreateHealthBar(unit);

            return unit;
        }

        public BattleUnit SpawnUnit(GameObject prefab, Team team, int index)
        {
            var position = GetSpawnPosition(team, index);
            var go = Instantiate(prefab, position, Quaternion.identity);
            var unit = go.GetComponent<BattleUnit>();

            if (unit != null)
            {
                CreateHealthBar(unit);
            }

            return unit;
        }

        private void CreateHealthBar(BattleUnit unit)
        {
            if (_healthBarPrefab == null || unit == null) return;

            var healthBar = Instantiate(_healthBarPrefab, unit.transform.position, Quaternion.identity);
            unit.SetHealthBar(healthBar);
        }
        
        public int GetSpawnCount(Team team)
        {
            var spawns = team == Team.Ally ? _allySpawns : _enemySpawns;
            return spawns.Length;
        }
        
        public bool IsSpawnValid(Team team, int index)
        {
            var spawns = team == Team.Ally ? _allySpawns : _enemySpawns;
            return index >= 0 && index < spawns.Length && spawns[index] != null;
        }
    }
}
