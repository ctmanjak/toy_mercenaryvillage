using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 개별 파티 슬롯 UI 컴포넌트
    /// </summary>
    public class PartySlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _roleText;
        [SerializeField] private GameObject _emptySlotIcon;
        [SerializeField] private GameObject _mercenaryInfo;
        [SerializeField] private Button _button;
        [SerializeField] private Image _background;

        [Header("Settings")]
        [SerializeField] private Color _normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color _highlightColor = new Color(0.3f, 0.5f, 0.3f, 1f);

        private int _slotIndex;
        private MercenaryData _mercenary;

        public event Action<int> OnSlotClicked;

        public int SlotIndex => _slotIndex;
        public MercenaryData Mercenary => _mercenary;
        public bool IsEmpty => _mercenary == null;

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

        /// <summary>
        /// 슬롯 인덱스 설정
        /// </summary>
        public void SetSlotIndex(int index)
        {
            _slotIndex = index;
        }

        /// <summary>
        /// 슬롯에 용병 설정
        /// </summary>
        public void SetMercenary(MercenaryData mercenary)
        {
            _mercenary = mercenary;
            UpdateDisplay();
        }

        /// <summary>
        /// 슬롯 비우기
        /// </summary>
        public void Clear()
        {
            _mercenary = null;
            UpdateDisplay();
        }

        /// <summary>
        /// 하이라이트 설정
        /// </summary>
        public void SetHighlight(bool highlighted)
        {
            if (_background != null)
            {
                _background.color = highlighted ? _highlightColor : _normalColor;
            }
        }

        private void UpdateDisplay()
        {
            bool hasMercenary = _mercenary != null;

            if (_emptySlotIcon != null)
            {
                _emptySlotIcon.SetActive(!hasMercenary);
            }

            if (_mercenaryInfo != null)
            {
                _mercenaryInfo.SetActive(hasMercenary);
            }

            if (hasMercenary)
            {
                if (_nameText != null)
                {
                    _nameText.text = _mercenary.DisplayName;
                }

                if (_levelText != null)
                {
                    _levelText.text = $"Lv.{_mercenary.Level}";
                }

                if (_roleText != null && _mercenary.UnitData != null)
                {
                    _roleText.text = GetRoleDisplayName(_mercenary.UnitData.Role);
                }
            }
        }

        private string GetRoleDisplayName(UnitRole role)
        {
            return role switch
            {
                UnitRole.Tank => "탱커",
                UnitRole.Damage => "딜러",
                UnitRole.Support => "서포터",
                _ => role.ToString()
            };
        }

        private void HandleClick()
        {
            OnSlotClicked?.Invoke(_slotIndex);
        }
    }
}