using System;
using Core;
using Data;
using UnityEngine;

namespace UI
{
    public class PartyFormationUI : MonoBehaviour
    {
        [Header("Slot References")]
        [SerializeField] private PartySlotUI[] _partySlots = new PartySlotUI[PartyManager.PARTY_SIZE];

        private MercenaryData _selectedMercenary;

        public event Action OnPartyUpdated;

        private void Awake()
        {
            InitializeSlots();
        }

        private void OnEnable()
        {
            if (PartyManager.Instance != null)
            {
                PartyManager.Instance.OnPartyChanged += RefreshUI;
            }

            RefreshUI();
        }

        private void OnDisable()
        {
            if (PartyManager.Instance != null)
            {
                PartyManager.Instance.OnPartyChanged -= RefreshUI;
            }
        }

        private void InitializeSlots()
        {
            for (int i = 0; i < _partySlots.Length; i++)
            {
                if (_partySlots[i] != null)
                {
                    _partySlots[i].SetSlotIndex(i);
                    _partySlots[i].OnSlotClicked += OnPartySlotClick;
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var slot in _partySlots)
            {
                if (slot != null)
                {
                    slot.OnSlotClicked -= OnPartySlotClick;
                }
            }
        }
        
        public void OnMercenarySelected(MercenaryData mercenary)
        {
            _selectedMercenary = mercenary;
            HighlightAvailableSlots();
        }
        
        public void ClearSelection()
        {
            _selectedMercenary = null;
            ClearHighlights();
        }
        
        private void OnPartySlotClick(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= PartyManager.PARTY_SIZE)
            {
                return;
            }

            var currentMercenary = PartyManager.Instance.GetPartySlot(slotIndex);

            if (currentMercenary != null)
            {
                PartyManager.Instance.ClearPartySlot(slotIndex);
                Debug.Log($"[PartyFormationUI] Removed mercenary from slot {slotIndex}");
            }
            else if (_selectedMercenary != null)
            {
                bool success = PartyManager.Instance.SetPartySlot(slotIndex, _selectedMercenary);
                if (success)
                {
                    Debug.Log($"[PartyFormationUI] Placed {_selectedMercenary.DisplayName} in slot {slotIndex}");
                }
                else
                {
                    Debug.LogWarning($"[PartyFormationUI] Failed to place mercenary in slot {slotIndex}");
                }
            }
            
            ClearSelection();
            RefreshUI();
            OnPartyUpdated?.Invoke();
        }
        
        public void RefreshUI()
        {
            if (PartyManager.Instance == null)
            {
                return;
            }

            var party = PartyManager.Instance.GetParty();

            for (int i = 0; i < _partySlots.Length; i++)
            {
                if (_partySlots[i] != null && i < party.Length)
                {
                    _partySlots[i].SetMercenary(party[i]);
                }
            }

            ClearHighlights();
        }
        
        private void HighlightAvailableSlots()
        {
            if (_selectedMercenary == null)
            {
                ClearHighlights();
                return;
            }

            bool isAlreadyInParty = PartyManager.Instance.IsInParty(_selectedMercenary);

            foreach (var partySlot in _partySlots)
            {
                if (partySlot == null) continue;
                
                bool canPlace = partySlot.IsEmpty && !isAlreadyInParty;
                partySlot.SetHighlight(canPlace);
            }
        }
        
        private void ClearHighlights()
        {
            foreach (var slot in _partySlots)
            {
                if (slot != null)
                {
                    slot.SetHighlight(false);
                }
            }
        }
        
        public bool CanStartBattle()
        {
            return PartyManager.Instance != null && PartyManager.Instance.CanStartBattle();
        }
        
        public int GetPartyCount()
        {
            return PartyManager.Instance?.GetPartyCount() ?? 0;
        }
    }
}