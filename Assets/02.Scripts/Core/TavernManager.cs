using System;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class TavernManager : MonoBehaviour
    {
        public static TavernManager Instance { get; private set; }
        
        [Header("Settings")]
        [SerializeField] private UnitData[] _unitTemplates;
        
        [Header("Hire Costs")]
        [SerializeField] private int _tankHireCost = 100;
        [SerializeField] private int _damageHireCost = 80;
        [SerializeField] private int _supportHireCost = 80;
        
        private MercenaryData[] _candidates = new MercenaryData[3];
        
        public event Action OnCandidatesChanged;
        public event Action<int> OnHireSuccess;
        public event Action<int> OnHireFailed;
        
        public MercenaryData[] Candidates => _candidates;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            GenerateCandidates();
        }
        
        public void GenerateCandidates()
        {
            if (_unitTemplates == null || _unitTemplates.Length == 0)
            {
                Debug.LogWarning("[TavernManager] No unit templates assigned!");
                return;
            }
            
            for (int i = 0; i < _candidates.Length; i++)
            {
                int randomIndex = Random.Range(0, _unitTemplates.Length);
                _candidates[i] = new MercenaryData(_unitTemplates[randomIndex]);
            }
            
            OnCandidatesChanged?.Invoke();
        }
        
        public MercenaryData GetCandidate(int index)
        {
            if (index < 0 || index >= _candidates.Length)
                return null;
            return _candidates[index];
        }
        
        public int GetHireCost(MercenaryData mercenary)
        {
            if (mercenary?.UnitData == null)
                return 0;
            
            return GetHireCostByRole(mercenary.UnitData.Role);
        }
        
        public int GetHireCostByRole(UnitRole role)
        {
            return role switch
            {
                UnitRole.Tank => _tankHireCost,
                UnitRole.Damage => _damageHireCost,
                UnitRole.Support => _supportHireCost,
                _ => _damageHireCost
            };
        }
        
        public bool TryHire(int candidateIndex)
        {
            if (candidateIndex < 0 || candidateIndex >= _candidates.Length)
            {
                OnHireFailed?.Invoke(candidateIndex);
                return false;
            }
            
            var candidate = _candidates[candidateIndex];
            if (candidate == null)
            {
                OnHireFailed?.Invoke(candidateIndex);
                return false;
            }
            
            int cost = GetHireCost(candidate);
            
            if (!PlayerResourceManager.Instance.SpendGold(cost))
            {
                Debug.Log($"[TavernManager] Not enough gold. Required: {cost}G");
                OnHireFailed?.Invoke(candidateIndex);
                return false;
            }
            
            PlayerResourceManager.Instance.AddMercenary(candidate);
            _candidates[candidateIndex] = null;
            
            Debug.Log($"[TavernManager] Hired {candidate.DisplayName} for {cost}G");
            OnHireSuccess?.Invoke(candidateIndex);
            OnCandidatesChanged?.Invoke();
            
            return true;
        }
        
        public bool CanAfford(int candidateIndex)
        {
            var candidate = GetCandidate(candidateIndex);
            if (candidate == null) return false;
            
            int cost = GetHireCost(candidate);
            return PlayerResourceManager.Instance.HasEnoughGold(cost);
        }
    }
}
