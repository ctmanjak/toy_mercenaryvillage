using System;
using System.Linq;
using Data;
using UnityEngine;

namespace Core
{
    public class PartyManager : MonoBehaviour
    {
        public static PartyManager Instance { get; private set; }

        private const int PARTY_SIZE = 4;

        private MercenaryData[] _partySlots = new MercenaryData[PARTY_SIZE];

        public event Action OnPartyChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializePartyFromMercenaries();
        }
        
        public void InitializePartyFromMercenaries()
        {
            if (PlayerResourceManager.Instance == null) return;
            
            if (GetPartyCount() > 0) return;

            var mercenaries = PlayerResourceManager.Instance.GetAllMercenaries();
            int slotIndex = 0;

            foreach (var merc in mercenaries)
            {
                if (slotIndex >= PARTY_SIZE) break;

                _partySlots[slotIndex] = merc;
                slotIndex++;
            }

            if (slotIndex > 0)
            {
                OnPartyChanged?.Invoke();
                Debug.Log($"[PartyManager] 초기 파티 설정 완료: {slotIndex}명");
            }
        }
        
        public bool SetPartySlot(int slotIndex, MercenaryData merc)
        {
            if (slotIndex < 0 || slotIndex >= PARTY_SIZE)
            {
                Debug.LogWarning($"[PartyManager] Invalid slot index: {slotIndex}");
                return false;
            }
            
            if (merc != null && IsInParty(merc))
            {
                Debug.LogWarning($"[PartyManager] Mercenary already in party: {merc.DisplayName}");
                return false;
            }

            _partySlots[slotIndex] = merc;
            OnPartyChanged?.Invoke();

            if (merc != null)
            {
                Debug.Log($"[PartyManager] Slot {slotIndex}: {merc.DisplayName}");
            }
            else
            {
                Debug.Log($"[PartyManager] Slot {slotIndex}: Cleared");
            }

            return true;
        }
        
        public void ClearPartySlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= PARTY_SIZE) return;

            if (_partySlots[slotIndex] != null)
            {
                _partySlots[slotIndex] = null;
                OnPartyChanged?.Invoke();
            }
        }
        
        public MercenaryData[] GetParty()
        {
            return (MercenaryData[])_partySlots.Clone();
        }
        
        public MercenaryData GetPartySlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= PARTY_SIZE) return null;
            return _partySlots[slotIndex];
        }
        
        public int GetPartyCount()
        {
            return _partySlots.Count(m => m != null);
        }
        
        public bool IsInParty(MercenaryData merc)
        {
            if (merc == null) return false;
            return _partySlots.Any(m => m != null && m.Id == merc.Id);
        }
        
        public bool CanStartBattle()
        {
            return GetPartyCount() > 0;
        }
        
        public void ClearAllSlots()
        {
            for (int i = 0; i < PARTY_SIZE; i++)
            {
                _partySlots[i] = null;
            }
            OnPartyChanged?.Invoke();
        }
        
        public int GetFirstEmptySlot()
        {
            for (int i = 0; i < PARTY_SIZE; i++)
            {
                if (_partySlots[i] == null) return i;
            }
            return -1;
        }
        
        public bool AddToParty(MercenaryData merc)
        {
            if (merc == null) return false;
            if (IsInParty(merc)) return false;

            int emptySlot = GetFirstEmptySlot();
            if (emptySlot < 0) return false;

            return SetPartySlot(emptySlot, merc);
        }
        
        public bool RemoveFromParty(MercenaryData merc)
        {
            if (merc == null) return false;

            for (int i = 0; i < PARTY_SIZE; i++)
            {
                if (_partySlots[i] != null && _partySlots[i].Id == merc.Id)
                {
                    _partySlots[i] = null;
                    OnPartyChanged?.Invoke();
                    return true;
                }
            }
            return false;
        }
        
        public int PartySize => PARTY_SIZE;
    }
}
