using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;
using Data;

namespace UI
{
    public class MercenaryDetailPanel : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject _panel;
        
        [Header("Info Display")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _roleText;
        [SerializeField] private TextMeshProUGUI _levelText;
        
        [Header("Stats Display")]
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _attackDamageText;
        [SerializeField] private TextMeshProUGUI _attackSpeedText;
        [SerializeField] private TextMeshProUGUI _rangeText;
        
        [Header("Level Up")]
        [SerializeField] private Button _levelUpButton;
        [SerializeField] private TextMeshProUGUI _levelUpCostText;
        [SerializeField] private GameObject _levelUpContainer;
        
        private MercenaryData _currentMercenary;

        private void Start()
        {
            if (_levelUpButton != null)
            {
                _levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
            }
            
            if (PlayerResourceManager.Instance != null)
            {
                PlayerResourceManager.Instance.OnGoldChanged += HandleGoldChanged;
            }
            
            Hide();
        }
        
        private void OnDestroy()
        {
            if (_levelUpButton != null)
            {
                _levelUpButton.onClick.RemoveListener(OnLevelUpButtonClicked);
            }
            
            if (PlayerResourceManager.Instance != null)
            {
                PlayerResourceManager.Instance.OnGoldChanged -= HandleGoldChanged;
            }
        }
        
        public void Show(MercenaryData mercenary)
        {
            if (mercenary == null)
            {
                Hide();
                return;
            }
            
            _currentMercenary = mercenary;
            
            if (_panel != null)
            {
                _panel.SetActive(true);
            }
            
            UpdateDisplay();
        }
        
        public void Hide()
        {
            if (_panel != null)
            {
                _panel.SetActive(false);
            }
            
            _currentMercenary = null;
        }
        
        public void Refresh()
        {
            if (_currentMercenary != null)
            {
                UpdateDisplay();
            }
        }
        
        private void UpdateDisplay()
        {
            if (_currentMercenary == null) return;
            
            var unitData = _currentMercenary.UnitData;
            if (unitData == null) return;
            
            if (_nameText != null)
            {
                _nameText.text = _currentMercenary.DisplayName;
            }
            
            if (_roleText != null)
            {
                _roleText.text = GetRoleDisplayText(unitData.Role);
            }
            
            if (_levelText != null)
            {
                int maxLevel = MercenaryManager.Instance.GetMaxLevel();
                _levelText.text = $"Lv.{_currentMercenary.Level} / {maxLevel}";
            }
            
            var stats = _currentMercenary.GetCurrentStats();
            
            if (_healthText != null)
            {
                _healthText.text = $"Health: {stats.MaxHealth:F0}";
            }
            
            if (_attackDamageText != null)
            {
                _attackDamageText.text = $"AttackDamage: {stats.AttackDamage:F0}";
            }
            
            if (_attackSpeedText != null)
            {
                _attackSpeedText.text = $"AttackSpeed: {stats.AttackSpeed:F1}";
            }
            
            if (_rangeText != null)
            {
                _rangeText.text = $"Range: {stats.AttackRange:F1}";
            }
            
            UpdateLevelUpUI();
        }
        
        private void UpdateLevelUpUI()
        {
            if (_currentMercenary == null) return;
            if (MercenaryManager.Instance == null) return;

            int cost = MercenaryManager.Instance.GetLevelUpCost(_currentMercenary.Level);
            bool isMaxLevel = MercenaryManager.Instance.IsMaxLevel(_currentMercenary.Level);

            if (_levelUpContainer != null)
            {
                _levelUpContainer.SetActive(!isMaxLevel);
            }

            if (isMaxLevel) return;

            if (_levelUpCostText != null)
            {
                _levelUpCostText.text = $"{cost} G";
            }

            if (_levelUpButton != null)
            {
                _levelUpButton.interactable = MercenaryManager.Instance.CanLevelUp(_currentMercenary);
            }
        }
        
        private void OnLevelUpButtonClicked()
        {
            if (_currentMercenary == null) return;
            if (MercenaryManager.Instance == null) return;

            if (MercenaryManager.Instance.TryLevelUp(_currentMercenary))
            {
                UpdateDisplay();
            }
        }
        
        private void HandleGoldChanged(int newGold)
        {
            UpdateLevelUpUI();
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
