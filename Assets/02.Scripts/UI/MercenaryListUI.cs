using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace UI
{
    public class MercenaryListUI : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private MercenarySlotUI _slotPrefab;
    
        private List<MercenarySlotUI> _slots = new();
        private MercenarySlotUI _selectedSlot;
    
        public event Action<MercenaryData> OnMercenarySelected;
    
        public MercenaryData SelectedMercenary => _selectedSlot?.Mercenary;

        private void Start()
        {
            Refresh();
        }

        private void OnDestroy()
        {
            ClearSlots();
        }
    
        public void Refresh()
        {
            ClearSlots();
        
            if (PlayerResourceManager.Instance == null)
            {
                Debug.LogWarning("PlayerResourceManager not found");
                return;
            }
        
            var mercenaries = PlayerResourceManager.Instance.GetAllMercenaries();
            foreach (var merc in mercenaries)
            {
                CreateSlot(merc);
            }
        }
    
        private void CreateSlot(MercenaryData mercenary)
        {
            if (_slotPrefab == null || _content == null) return;
        
            var slot = Instantiate(_slotPrefab, _content);
            slot.Setup(mercenary);
            slot.OnSlotClicked += HandleSlotClicked;
            _slots.Add(slot);
        }
    
        private void ClearSlots()
        {
            foreach (var slot in _slots)
            {
                if (slot != null)
                {
                    slot.OnSlotClicked -= HandleSlotClicked;
                    Destroy(slot.gameObject);
                }
            }
            _slots.Clear();
            _selectedSlot = null;
        }
    
        private void HandleSlotClicked(MercenarySlotUI slot)
        {
            SelectSlot(slot);
        }
    
        private void SelectSlot(MercenarySlotUI slot)
        {
            if (_selectedSlot != null)
            {
                _selectedSlot.SetSelected(false);
            }
        
            _selectedSlot = slot;
        
            if (_selectedSlot != null)
            {
                _selectedSlot.SetSelected(true);
                OnMercenarySelected?.Invoke(_selectedSlot.Mercenary);
            }
        }
    
        public void SelectMercenary(MercenaryData mercenary)
        {
            var slot = _slots.Find(s => s.Mercenary == mercenary);
            if (slot != null)
            {
                SelectSlot(slot);
            }
        }
    }
}
