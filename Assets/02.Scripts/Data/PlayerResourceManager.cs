using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    public class PlayerResourceManager : MonoBehaviour
    {
        public static PlayerResourceManager Instance { get; private set; }

        [Header("Gold Settings")]
        [SerializeField] private int _startingGold = 100;

        [Header("Initial Mercenaries")]
        [SerializeField] private UnitData _tankerTemplate;
        [SerializeField] private UnitData _meleeDealerTemplate;

        private int _gold;
        public int Gold => _gold;

        private List<MercenaryData> _mercenaries = new List<MercenaryData>();

        public event Action<int> OnGoldChanged;
        public event Action<MercenaryData> OnMercenaryAdded;
        public event Action<MercenaryData> OnMercenaryRemoved;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _gold = _startingGold;
        }

        #region Save/Load Interface (SaveManager에서 호출)

        public void ApplyLoadedData(int? gold, List<MercenaryData> mercenaries)
        {
            if (gold.HasValue && mercenaries != null && mercenaries.Count > 0)
            {
                _gold = gold.Value;
                _mercenaries = mercenaries;
                Debug.Log($"[PlayerResourceManager] 데이터 적용: Gold={_gold}, 용병={_mercenaries.Count}명");
            }
            else
            {
                InitializeNewGame();
            }

            OnGoldChanged?.Invoke(_gold);
        }

        public List<MercenarySaveData> GetMercenariesForSave()
        {
            return _mercenaries.Select(m => new MercenarySaveData(m)).ToList();
        }

        #endregion

        #region Gold

        public void AddGold(int amount)
        {
            _gold += amount;
            OnGoldChanged?.Invoke(_gold);
            SaveManager.Instance?.SaveGame();
        }

        public bool SpendGold(int amount)
        {
            if (_gold < amount) return false;

            _gold -= amount;
            OnGoldChanged?.Invoke(_gold);
            SaveManager.Instance?.SaveGame();
            return true;
        }

        public bool HasEnoughGold(int amount) => _gold >= amount;

        [ContextMenu("Reset Gold")]
        public void ResetGold()
        {
            _gold = _startingGold;
            OnGoldChanged?.Invoke(_gold);
            SaveManager.Instance?.SaveGame();
        }

        #endregion

        #region Mercenary System

        public void AddMercenary(MercenaryData merc)
        {
            if (merc == null) return;
            if (_mercenaries.Any(m => m.Id == merc.Id)) return;

            _mercenaries.Add(merc);
            OnMercenaryAdded?.Invoke(merc);
            SaveManager.Instance?.SaveGame();
        }

        public void RemoveMercenary(MercenaryData merc)
        {
            if (merc == null) return;

            var existing = _mercenaries.FirstOrDefault(m => m.Id == merc.Id);
            if (existing != null)
            {
                _mercenaries.Remove(existing);
                OnMercenaryRemoved?.Invoke(existing);
                SaveManager.Instance?.SaveGame();
            }
        }

        public MercenaryData GetMercenaryById(string id)
        {
            return _mercenaries.FirstOrDefault(m => m.Id == id);
        }

        public List<MercenaryData> GetAllMercenaries()
        {
            return new List<MercenaryData>(_mercenaries);
        }

        public int MercenaryCount => _mercenaries.Count;

        public void SaveMercenaries()
        {
            SaveManager.Instance?.SaveGame();
        }

        #endregion

        #region Private Methods

        private void InitializeNewGame()
        {
            _gold = _startingGold;
            _mercenaries.Clear();

            if (_tankerTemplate != null)
            {
                var tanker = new MercenaryData(_tankerTemplate);
                _mercenaries.Add(tanker);
            }

            if (_meleeDealerTemplate != null)
            {
                var meleeDealer = new MercenaryData(_meleeDealerTemplate);
                _mercenaries.Add(meleeDealer);
            }

            Debug.Log($"[PlayerResourceManager] 새 게임 시작: Gold={_gold}, 초기 용병={_mercenaries.Count}명");
        }

        #endregion

        [ContextMenu("Reset All Data")]
        public void ResetAllData()
        {
            SaveManager.Instance?.DeleteSave();
            InitializeNewGame();
            OnGoldChanged?.Invoke(_gold);

            Core.PartyManager.Instance?.ClearAllSlots();

            SaveManager.Instance?.SaveGame();

            Debug.Log("[PlayerResourceManager] 모든 데이터 초기화 완료");
        }
    }
}
