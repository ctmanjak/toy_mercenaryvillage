using System;
using Core;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ExpeditionCard : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _battleCountText;
        [SerializeField] private TextMeshProUGUI _recommendedPowerText;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private TextMeshProUGUI _completionCountText;
        [SerializeField] private Image _iconImage;

        [Header("Lock State")]
        [SerializeField] private GameObject _lockOverlay;
        [SerializeField] private TextMeshProUGUI _unlockConditionText;

        [Header("Warning")]
        [SerializeField] private GameObject _powerWarning;

        [Header("Interaction")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _cardBackground;
        [SerializeField] private Color _normalColor = new Color(0.25f, 0.25f, 0.3f, 1f);
        [SerializeField] private Color _lockedColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        [SerializeField] private Color _selectedColor = new Color(0.4f, 0.35f, 0.2f, 1f);

        private ExpeditionData _expeditionData;
        private bool _isUnlocked;
        private bool _isSelected;

        public ExpeditionData ExpeditionData => _expeditionData;
        public bool IsUnlocked => _isUnlocked;

        public event Action<ExpeditionCard> OnCardClicked;

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

        public void Setup(ExpeditionData data)
        {
            _expeditionData = data;
            _isSelected = false;

            UpdateDisplay();
            UpdateLockState();
            UpdatePowerWarning();
            UpdateBackground();
        }

        public void Refresh()
        {
            if (_expeditionData == null) return;

            UpdateLockState();
            UpdatePowerWarning();
            UpdateCompletionCount();
            UpdateBackground();
        }

        private void UpdateDisplay()
        {
            if (_expeditionData == null) return;

            if (_nameText != null)
                _nameText.text = _expeditionData.ExpeditionName;

            if (_battleCountText != null)
                _battleCountText.text = $"전투 {_expeditionData.Battles?.Count ?? 0}회";

            if (_recommendedPowerText != null)
                _recommendedPowerText.text = $"권장 {_expeditionData.RecommendedPower}";

            if (_rewardText != null)
            {
                int minReward = CalculateMinReward();
                int maxReward = CalculateMaxReward();
                _rewardText.text = $"{minReward}~{maxReward}G";
            }

            if (_iconImage != null && _expeditionData.Icon != null)
                _iconImage.sprite = _expeditionData.Icon;

            UpdateCompletionCount();
        }

        private void UpdateCompletionCount()
        {
            if (_completionCountText != null && _expeditionData != null)
            {
                int count = 0;
                if (ExpeditionProgressManager.Instance != null)
                {
                    count = ExpeditionProgressManager.Instance.GetCompletionCount(_expeditionData.ExpeditionId);
                }
                _completionCountText.text = count > 0 ? $"완료 {count}회" : "미완료";
            }
        }

        private void UpdateLockState()
        {
            _isUnlocked = true;

            if (ExpeditionProgressManager.Instance != null && _expeditionData != null)
            {
                _isUnlocked = ExpeditionProgressManager.Instance.IsUnlocked(_expeditionData);
            }

            if (_lockOverlay != null)
                _lockOverlay.SetActive(!_isUnlocked);

            if (_unlockConditionText != null && !_isUnlocked)
            {
                if (ExpeditionProgressManager.Instance != null)
                {
                    _unlockConditionText.text = ExpeditionProgressManager.Instance.GetUnlockRequirementText(_expeditionData);
                }
            }
        }

        private void UpdatePowerWarning()
        {
            if (_powerWarning == null || _expeditionData == null) return;

            int partyPower = GetPartyPower();
            bool showWarning = _isUnlocked && partyPower < _expeditionData.RecommendedPower;
            _powerWarning.SetActive(showWarning);
        }

        private void UpdateBackground()
        {
            if (_cardBackground == null) return;

            if (!_isUnlocked)
            {
                _cardBackground.color = _lockedColor;
            }
            else if (_isSelected)
            {
                _cardBackground.color = _selectedColor;
            }
            else
            {
                _cardBackground.color = _normalColor;
            }
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            UpdateBackground();
        }

        private void HandleClick()
        {
            OnCardClicked?.Invoke(this);
        }

        private int CalculateMinReward()
        {
            if (_expeditionData == null || _expeditionData.Battles == null)
                return 0;

            int total = 0;
            foreach (var stage in _expeditionData.Battles)
            {
                if (stage != null)
                    total += stage.GoldReward;
            }
            return total + _expeditionData.CompletionBonus;
        }

        private int CalculateMaxReward()
        {
            return CalculateMinReward() + _expeditionData.FirstClearBonus;
        }

        private int GetPartyPower()
        {
            return PartyManager.Instance?.GetTotalCombatPower() ?? 0;
        }
    }
}
