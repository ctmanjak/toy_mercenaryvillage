using System;
using Core;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MercenarySlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _roleText;
        [SerializeField] private GameObject _partyBadge;
        [SerializeField] private Button _button;
        [SerializeField] private Image _background;
    
        [Header("Colors")]
        [SerializeField] private Color _normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color _selectedColor = new Color(0.4f, 0.6f, 0.8f, 1f);
    
        private MercenaryData _mercenary;
    
        public event Action<MercenarySlotUI> OnSlotClicked;
    
        public MercenaryData Mercenary => _mercenary;
    
        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(HandleClick);
            }
        }
    
        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(HandleClick);
            }
        }
    
        public void Setup(MercenaryData mercenary)
        {
            _mercenary = mercenary;
        
            if (_nameText != null)
            {
                _nameText.text = mercenary.DisplayName;
            }
        
            if (_levelText != null)
            {
                _levelText.text = $"Lv.{mercenary.Level}";
            }
        
            if (_roleText != null && mercenary.UnitData != null)
            {
                _roleText.text = GetRoleDisplayText(mercenary.UnitData.Role);
            }
        
            UpdatePartyBadge();
        }
    
        public void UpdatePartyBadge()
        {
            if (_partyBadge != null && _mercenary != null)
            {
                bool inParty = PartyManager.Instance != null && PartyManager.Instance.IsInParty(_mercenary);
                _partyBadge.SetActive(inParty);
            }
        }
    
        public void SetSelected(bool selected)
        {
            if (_background != null)
            {
                _background.color = selected ? _selectedColor : _normalColor;
            }
        }
    
        private void HandleClick()
        {
            OnSlotClicked?.Invoke(this);
        }
    
        private string GetRoleDisplayText(UnitRole role)
        {
            return role switch
            {
                UnitRole.Tank => "탱커",
                UnitRole.Damage => "딜러",
                UnitRole.Support => "서포터",
                _ => role.ToString()
            };
        }
    }
}
