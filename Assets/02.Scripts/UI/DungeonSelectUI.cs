using Core;
using Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class DungeonSelectUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _regionTitle;

        [Header("Stage List")]
        [SerializeField] private StageListUI _stageListUI;

        [Header("Stage Info Panel")]
        [SerializeField] private GameObject _stageInfoPanel;
        [SerializeField] private TextMeshProUGUI _selectedStageTitle;
        [SerializeField] private TextMeshProUGUI _selectedStagePower;
        [SerializeField] private TextMeshProUGUI _selectedStageGold;
        [SerializeField] private TextMeshProUGUI _selectedStageEnemies;

        [Header("Action")]
        [SerializeField] private Button _startBattleButton;
        [SerializeField] private TextMeshProUGUI _startButtonText;

        private StageData _selectedStage;

        private void Start()
        {
            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackButtonClicked);
            }

            if (_startBattleButton != null)
            {
                _startBattleButton.onClick.AddListener(OnStartBattleClicked);
                _startBattleButton.interactable = false;
            }

            if (_stageListUI != null)
            {
                _stageListUI.OnStageSelected += OnStageSelected;
            }

            if (_regionTitle != null)
            {
                _regionTitle.text = "초원 지역";
            }

            if (_stageInfoPanel != null)
            {
                _stageInfoPanel.SetActive(false);
            }

            UpdateStartButton();
        }

        private void OnDestroy()
        {
            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackButtonClicked);
            }

            if (_startBattleButton != null)
            {
                _startBattleButton.onClick.RemoveListener(OnStartBattleClicked);
            }

            if (_stageListUI != null)
            {
                _stageListUI.OnStageSelected -= OnStageSelected;
            }
        }

        private void OnStageSelected(StageData stage)
        {
            _selectedStage = stage;
            UpdateStageInfoPanel();
            UpdateStartButton();
        }

        private void UpdateStageInfoPanel()
        {
            if (_stageInfoPanel == null) return;

            if (_selectedStage == null)
            {
                _stageInfoPanel.SetActive(false);
                return;
            }

            _stageInfoPanel.SetActive(true);

            if (_selectedStageTitle != null)
                _selectedStageTitle.text = $"[{_selectedStage.StageId}] {_selectedStage.StageName}";

            if (_selectedStagePower != null)
                _selectedStagePower.text = $"권장 전투력: {_selectedStage.RecommendedPower}";

            if (_selectedStageGold != null)
                _selectedStageGold.text = $"보상: {_selectedStage.GoldReward} Gold";

            if (_selectedStageEnemies != null)
                _selectedStageEnemies.text = $"적 수: {_selectedStage.Enemies?.Length ?? 0}";
        }

        private void UpdateStartButton()
        {
            if (_startBattleButton != null)
            {
                _startBattleButton.interactable = _selectedStage != null;
            }

            if (_startButtonText != null)
            {
                _startButtonText.text = _selectedStage != null ? "전투 시작!" : "스테이지를 선택하세요";
            }
        }

        private void OnBackButtonClicked()
        {
            GameManager.Instance.GoToTown();
        }

        private void OnStartBattleClicked()
        {
            if (_selectedStage != null)
            {
                GameManager.Instance.StartBattle(_selectedStage);
            }
        }
    }
}
