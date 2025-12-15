using System;
using Core;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HireableSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _roleText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Button _button;
        [SerializeField] private Image _background;
        [SerializeField] private GameObject _emptyState;
        [SerializeField] private GameObject _filledState;

        [Header("Colors")]
        [SerializeField] private Color _normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color _selectedColor = new Color(0.4f, 0.6f, 0.8f, 1f);
        [SerializeField] private Color _affordableColor = Color.white;
        [SerializeField] private Color _unaffordableColor = Color.red;

        private int _slotIndex;
        private MercenaryData _candidate;

        public event Action<HireableSlotUI> OnSlotClicked;

        public int SlotIndex => _slotIndex;
        public MercenaryData Candidate => _candidate;
        public bool IsEmpty => _candidate == null;

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

        public void Setup(int slotIndex, MercenaryData candidate)
        {
            _slotIndex = slotIndex;
            _candidate = candidate;

            if (candidate == null)
            {
                SetEmptyState();
                return;
            }

            SetFilledState();

            if (_nameText != null)
            {
                _nameText.text = candidate.DisplayName;
            }

            if (_levelText != null)
            {
                _levelText.text = $"Lv.{candidate.Level}";
            }

            if (_roleText != null && candidate.UnitData != null)
            {
                _roleText.text = GetRoleDisplayText(candidate.UnitData.Role);
            }

            UpdateCostDisplay();
        }

        public void UpdateCostDisplay()
        {
            if (_costText == null || _candidate == null) return;

            int cost = TavernManager.Instance != null
                ? TavernManager.Instance.GetHireCost(_candidate)
                : 0;

            _costText.text = $"{cost}G";

            bool canAfford = PlayerResourceManager.Instance != null
                && PlayerResourceManager.Instance.HasEnoughGold(cost);

            _costText.color = canAfford ? _affordableColor : _unaffordableColor;
        }

        public void SetSelected(bool selected)
        {
            if (_background != null)
            {
                _background.color = selected ? _selectedColor : _normalColor;
            }
        }

        private void SetEmptyState()
        {
            if (_emptyState != null) _emptyState.SetActive(true);
            if (_filledState != null) _filledState.SetActive(false);
            if (_button != null) _button.interactable = false;
        }

        private void SetFilledState()
        {
            if (_emptyState != null) _emptyState.SetActive(false);
            if (_filledState != null) _filledState.SetActive(true);
            if (_button != null) _button.interactable = true;
        }

        private void HandleClick()
        {
            if (_candidate != null)
            {
                OnSlotClicked?.Invoke(this);
            }
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
