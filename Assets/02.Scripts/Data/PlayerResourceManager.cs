using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

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

    private const string GOLD_KEY = "PlayerGold";
    private const string MERCENARIES_KEY = "PlayerMercenaries";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
    }

    public void AddGold(int amount)
    {
        _gold += amount;
        OnGoldChanged?.Invoke(_gold);
        SaveData();
    }

    public bool SpendGold(int amount)
    {
        if (_gold < amount) return false;

        _gold -= amount;
        OnGoldChanged?.Invoke(_gold);
        SaveData();
        return true;
    }

    public bool HasEnoughGold(int amount) => _gold >= amount;

    private void SaveData()
    {
        PlayerPrefs.SetInt(GOLD_KEY, _gold);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        _gold = PlayerPrefs.GetInt(GOLD_KEY, _startingGold);
        LoadMercenaries();
    }

    [ContextMenu("Reset Gold")]
    public void ResetGold()
    {
        _gold = _startingGold;
        OnGoldChanged?.Invoke(_gold);
        SaveData();
    }

    #region Mercenary System
    
    public void AddMercenary(MercenaryData merc)
    {
        if (merc == null) return;
        if (_mercenaries.Any(m => m.Id == merc.Id)) return;

        _mercenaries.Add(merc);
        OnMercenaryAdded?.Invoke(merc);
        SaveMercenaries();
    }
    
    public void RemoveMercenary(MercenaryData merc)
    {
        if (merc == null) return;

        var existing = _mercenaries.FirstOrDefault(m => m.Id == merc.Id);
        if (existing != null)
        {
            _mercenaries.Remove(existing);
            OnMercenaryRemoved?.Invoke(existing);
            SaveMercenaries();
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

        SaveData();
        SaveMercenaries();

        Debug.Log($"[PlayerResourceManager] 초기 용병 지급 완료: {_mercenaries.Count}명");
    }

    private void SaveMercenaries()
    {
        var json = JsonUtility.ToJson(new MercenaryListWrapper { mercenaries = _mercenaries });
        PlayerPrefs.SetString(MERCENARIES_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadMercenaries()
    {
        var json = PlayerPrefs.GetString(MERCENARIES_KEY, "");
        if (string.IsNullOrEmpty(json))
        {
            InitializeNewGame();
            return;
        }

        try
        {
            var wrapper = JsonUtility.FromJson<MercenaryListWrapper>(json);
            _mercenaries = wrapper?.mercenaries ?? new List<MercenaryData>();

            if (_mercenaries.Count == 0)
            {
                InitializeNewGame();
            }
        }
        catch
        {
            InitializeNewGame();
        }
    }

    [ContextMenu("Reset All Data (Gold + Mercenaries)")]
    public void ResetAllData()
    {
        PlayerPrefs.DeleteKey(GOLD_KEY);
        PlayerPrefs.DeleteKey(MERCENARIES_KEY);
        InitializeNewGame();
        OnGoldChanged?.Invoke(_gold);
        Debug.Log("[PlayerResourceManager] 모든 데이터 초기화 완료");
    }

    [System.Serializable]
    private class MercenaryListWrapper
    {
        public List<MercenaryData> mercenaries;
    }

    #endregion
}
