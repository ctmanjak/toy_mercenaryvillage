using Core;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TavernUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button _backButton;

        [Header("Candidate List")]
        [SerializeField] private Transform _candidateListContent;
        [SerializeField] private HireableSlotUI _hireableSlotPrefab;

        [Header("Detail Panel")]
        [SerializeField] private GameObject _detailPanel;
        [SerializeField] private TextMeshProUGUI _detailNameText;
        [SerializeField] private TextMeshProUGUI _detailLevelText;
        [SerializeField] private TextMeshProUGUI _detailRoleText;
        [SerializeField] private TextMeshProUGUI _detailHealthText;
        [SerializeField] private TextMeshProUGUI _detailAttackText;
        [SerializeField] private TextMeshProUGUI _detailCostText;
        [SerializeField] private Button _hireButton;
        [SerializeField] private TextMeshProUGUI _hireButtonText;

        private HireableSlotUI[] _hireableSlots;
        private HireableSlotUI _selectedSlot;

        private void Start()
        {
            SetupButtons();
            SubscribeEvents();
            CreateSlots();
            RefreshCandidates();
            HideDetailPanel();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
            CleanupButtons();
        }

        private void SetupButtons()
        {
            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackButtonClicked);
            }

            if (_hireButton != null)
            {
                _hireButton.onClick.AddListener(OnHireButtonClicked);
            }
        }

        private void CleanupButtons()
        {
            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackButtonClicked);
            }

            if (_hireButton != null)
            {
                _hireButton.onClick.RemoveListener(OnHireButtonClicked);
            }
        }

        private void SubscribeEvents()
        {
            if (TavernManager.Instance != null)
            {
                TavernManager.Instance.OnCandidatesChanged += RefreshCandidates;
                TavernManager.Instance.OnHireSuccess += HandleHireSuccess;
                TavernManager.Instance.OnHireFailed += HandleHireFailed;
            }

            if (PlayerResourceManager.Instance != null)
            {
                PlayerResourceManager.Instance.OnGoldChanged += HandleGoldChanged;
            }
        }

        private void UnsubscribeEvents()
        {
            if (TavernManager.Instance != null)
            {
                TavernManager.Instance.OnCandidatesChanged -= RefreshCandidates;
                TavernManager.Instance.OnHireSuccess -= HandleHireSuccess;
                TavernManager.Instance.OnHireFailed -= HandleHireFailed;
            }

            if (PlayerResourceManager.Instance != null)
            {
                PlayerResourceManager.Instance.OnGoldChanged -= HandleGoldChanged;
            }
        }

        private void CreateSlots()
        {
            if (_candidateListContent == null || _hireableSlotPrefab == null)
            {
                Debug.LogWarning("[TavernUI] Missing candidate list content or prefab!");
                return;
            }

            _hireableSlots = new HireableSlotUI[3];

            for (int i = 0; i < 3; i++)
            {
                var slot = Instantiate(_hireableSlotPrefab, _candidateListContent);
                slot.OnSlotClicked += HandleSlotClicked;
                _hireableSlots[i] = slot;
            }
        }

        private void RefreshCandidates()
        {
            if (_hireableSlots == null || TavernManager.Instance == null) return;

            var candidates = TavernManager.Instance.Candidates;

            for (int i = 0; i < _hireableSlots.Length; i++)
            {
                var candidate = i < candidates.Length ? candidates[i] : null;
                _hireableSlots[i].Setup(i, candidate);
            }

            if (_selectedSlot != null && _selectedSlot.IsEmpty)
            {
                HideDetailPanel();
                _selectedSlot = null;
            }
        }

        private void HandleSlotClicked(HireableSlotUI slot)
        {
            if (_selectedSlot != null)
            {
                _selectedSlot.SetSelected(false);
            }

            _selectedSlot = slot;
            _selectedSlot.SetSelected(true);

            ShowDetailPanel(slot.Candidate);
        }

        private void ShowDetailPanel(MercenaryData mercenary)
        {
            if (_detailPanel != null)
            {
                _detailPanel.SetActive(true);
            }

            if (mercenary == null) return;

            if (_detailNameText != null)
            {
                _detailNameText.text = mercenary.DisplayName;
            }

            if (_detailLevelText != null)
            {
                _detailLevelText.text = $"Lv.{mercenary.Level}";
            }

            if (_detailRoleText != null && mercenary.UnitData != null)
            {
                _detailRoleText.text = GetRoleDisplayText(mercenary.UnitData.Role);
            }

            if (_detailHealthText != null)
            {
                _detailHealthText.text = $"HP: {mercenary.GetCurrentHealth():F0}";
            }

            if (_detailAttackText != null)
            {
                _detailAttackText.text = $"ATK: {mercenary.GetCurrentAttackDamage():F0}";
            }

            int cost = TavernManager.Instance != null
                ? TavernManager.Instance.GetHireCost(mercenary)
                : 0;

            if (_detailCostText != null)
            {
                _detailCostText.text = $"비용: {cost}G";
            }

            UpdateHireButton(cost);
        }

        private void HideDetailPanel()
        {
            if (_detailPanel != null)
            {
                _detailPanel.SetActive(false);
            }

            if (_selectedSlot != null)
            {
                _selectedSlot.SetSelected(false);
                _selectedSlot = null;
            }
        }

        private void UpdateHireButton(int cost)
        {
            if (_hireButton == null) return;

            bool canAfford = PlayerResourceManager.Instance != null
                && PlayerResourceManager.Instance.HasEnoughGold(cost);

            _hireButton.interactable = canAfford;

            if (_hireButtonText != null)
            {
                _hireButtonText.text = canAfford ? "고용하기" : "골드 부족";
            }
        }

        private void OnHireButtonClicked()
        {
            if (_selectedSlot == null || TavernManager.Instance == null) return;

            TavernManager.Instance.TryHire(_selectedSlot.SlotIndex);
        }

        private void HandleHireSuccess(int slotIndex)
        {
            Debug.Log($"[TavernUI] Hire success at slot {slotIndex}");
            HideDetailPanel();
        }

        private void HandleHireFailed(int slotIndex)
        {
            Debug.Log($"[TavernUI] Hire failed at slot {slotIndex}");
        }

        private void HandleGoldChanged(int newGold)
        {
            UpdateAllSlotsCost();

            if (_selectedSlot != null && _selectedSlot.Candidate != null)
            {
                int cost = TavernManager.Instance != null
                    ? TavernManager.Instance.GetHireCost(_selectedSlot.Candidate)
                    : 0;
                UpdateHireButton(cost);
            }
        }

        private void UpdateAllSlotsCost()
        {
            if (_hireableSlots == null) return;

            foreach (var slot in _hireableSlots)
            {
                if (slot != null)
                {
                    slot.UpdateCostDisplay();
                }
            }
        }

        private void OnBackButtonClicked()
        {
            GameManager.Instance.GoToTown();
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
