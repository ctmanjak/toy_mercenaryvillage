using System.Collections.Generic;
using Core;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ExpeditionSelectUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _titleText;

        [Header("Expedition List")]
        [SerializeField] private ExpeditionCard _expeditionCardPrefab;
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private ExpeditionData[] _expeditions;

        [Header("Detail Panel")]
        [SerializeField] private GameObject _detailPanel;
        [SerializeField] private TextMeshProUGUI _detailNameText;
        [SerializeField] private TextMeshProUGUI _detailDescriptionText;
        [SerializeField] private TextMeshProUGUI _detailBattleCountText;
        [SerializeField] private TextMeshProUGUI _detailPowerText;
        [SerializeField] private TextMeshProUGUI _detailRewardText;
        [SerializeField] private GameObject _detailPowerWarning;

        [Header("Actions")]
        [SerializeField] private Button _startButton;
        [SerializeField] private TextMeshProUGUI _startButtonText;

        private List<ExpeditionCard> _cards = new();
        private ExpeditionCard _selectedCard;
        private ExpeditionData _selectedExpedition;

        private void Start()
        {
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);

            if (_startButton != null)
            {
                _startButton.onClick.AddListener(OnStartClicked);
                _startButton.interactable = false;
            }

            if (_titleText != null)
                _titleText.text = "원정 선택";

            GenerateCards();
            HideDetailPanel();
        }

        private void OnEnable()
        {
            RefreshAllCards();
            ClearSelection();
        }

        private void OnDestroy()
        {
            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);

            if (_startButton != null)
                _startButton.onClick.RemoveListener(OnStartClicked);

            foreach (var card in _cards)
            {
                if (card != null)
                    card.OnCardClicked -= HandleCardClicked;
            }
        }

        private void GenerateCards()
        {
            if (_expeditionCardPrefab == null || _cardContainer == null) return;

            foreach (var expedition in _expeditions)
            {
                if (expedition == null) continue;

                var card = Instantiate(_expeditionCardPrefab, _cardContainer);
                card.Setup(expedition);
                card.OnCardClicked += HandleCardClicked;
                _cards.Add(card);
            }
        }

        private void RefreshAllCards()
        {
            foreach (var card in _cards)
            {
                card?.Refresh();
            }
        }

        private void HandleCardClicked(ExpeditionCard card)
        {
            if (card == null || card.ExpeditionData == null) return;

            if (!card.IsUnlocked)
            {
                ShowLockedPopup(card.ExpeditionData);
                return;
            }

            SelectCard(card);
        }

        private void SelectCard(ExpeditionCard card)
        {
            foreach (var c in _cards)
            {
                c.SetSelected(c == card);
            }

            _selectedCard = card;
            _selectedExpedition = card.ExpeditionData;

            ShowDetailPanel();
            UpdateStartButton();
        }

        private void ClearSelection()
        {
            foreach (var card in _cards)
            {
                card.SetSelected(false);
            }

            _selectedCard = null;
            _selectedExpedition = null;

            HideDetailPanel();
            UpdateStartButton();
        }

        private void ShowDetailPanel()
        {
            if (_detailPanel == null || _selectedExpedition == null) return;

            _detailPanel.SetActive(true);

            if (_detailNameText != null)
                _detailNameText.text = _selectedExpedition.ExpeditionName;

            if (_detailDescriptionText != null)
                _detailDescriptionText.text = _selectedExpedition.Description;

            if (_detailBattleCountText != null)
                _detailBattleCountText.text = $"전투 {_selectedExpedition.Battles?.Count ?? 0}회";

            if (_detailPowerText != null)
                _detailPowerText.text = $"권장 전투력: {_selectedExpedition.RecommendedPower}";

            if (_detailRewardText != null)
            {
                int minReward = CalculateMinReward(_selectedExpedition);
                int maxReward = CalculateMaxReward(_selectedExpedition);
                _detailRewardText.text = $"보상: {minReward}~{maxReward}G";
            }

            if (_detailPowerWarning != null)
            {
                int partyPower = GetPartyPower();
                _detailPowerWarning.SetActive(partyPower < _selectedExpedition.RecommendedPower);
            }
        }

        private void HideDetailPanel()
        {
            if (_detailPanel != null)
                _detailPanel.SetActive(false);
        }

        private void UpdateStartButton()
        {
            if (_startButton != null)
                _startButton.interactable = _selectedExpedition != null;

            if (_startButtonText != null)
                _startButtonText.text = _selectedExpedition != null ? "출발!" : "원정을 선택하세요";
        }

        private void ShowLockedPopup(ExpeditionData expedition)
        {
            if (CommonPopup.Instance == null || expedition == null) return;

            string message = $"이 원정은 잠겨있습니다.\n\n";
            if (ExpeditionProgressManager.Instance != null)
            {
                message += ExpeditionProgressManager.Instance.GetUnlockRequirementText(expedition);
            }

            CommonPopup.Instance.ShowAlert(message);
        }

        private void OnBackClicked()
        {
            TownUIManager.Instance?.ShowTown();
        }

        private void OnStartClicked()
        {
            if (_selectedExpedition == null) return;

            if (!PartyValidator.HasAnyMember())
            {
                CommonPopup.Instance?.ShowAlert("파티에 용병이 없습니다.\n길드 하우스에서 편성해주세요.");
                return;
            }

            string message = $"{_selectedExpedition.ExpeditionName} 원정을 시작하시겠습니까?";

            int partyPower = GetPartyPower();
            if (partyPower < _selectedExpedition.RecommendedPower)
            {
                message += $"\n\n⚠️ 파티 전투력({partyPower})이 권장 전투력({_selectedExpedition.RecommendedPower})보다 낮습니다.";
            }

            CommonPopup.Instance?.ShowConfirm(
                message,
                onConfirm: StartExpedition,
                confirmText: "출발",
                cancelText: "취소"
            );
        }

        private void StartExpedition()
        {
            if (_selectedExpedition == null) return;

            if (ExpeditionManager.Instance != null)
            {
                ExpeditionManager.Instance.StartExpedition(_selectedExpedition);
            }
            else
            {
                Debug.LogError("[ExpeditionSelectUI] ExpeditionManager not found!");
            }
        }

        private int CalculateMinReward(ExpeditionData expedition)
        {
            if (expedition?.Battles == null) return 0;

            int total = 0;
            foreach (var stage in expedition.Battles)
            {
                if (stage != null)
                    total += stage.GoldReward;
            }
            return total + expedition.CompletionBonus;
        }

        private int CalculateMaxReward(ExpeditionData expedition)
        {
            return CalculateMinReward(expedition) + expedition.FirstClearBonus;
        }

        private int GetPartyPower()
        {
            return PartyManager.Instance?.GetTotalCombatPower() ?? 0;
        }
    }
}
