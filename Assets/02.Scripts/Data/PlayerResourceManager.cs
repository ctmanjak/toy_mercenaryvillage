using System;
using UnityEngine;

public class PlayerResourceManager : MonoBehaviour
{
    public static PlayerResourceManager Instance { get; private set; }

    [SerializeField] private int _startingGold = 100;

    private int _gold;
    public int Gold => _gold;

    public event Action<int> OnGoldChanged;

    private const string GOLD_KEY = "PlayerGold";

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
    }

    [ContextMenu("Reset Gold")]
    public void ResetGold()
    {
        _gold = _startingGold;
        OnGoldChanged?.Invoke(_gold);
        SaveData();
    }
}
