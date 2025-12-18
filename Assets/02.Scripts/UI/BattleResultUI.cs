using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class BattleResultUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private TextMeshProUGUI _resultText;

        [Header("Expedition Info")]
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private TextMeshProUGUI _goldEarnedText;
        [SerializeField] private TextMeshProUGUI _totalGoldText;

        [Header("Bonus Breakdown (Last Battle)")]
        [SerializeField] private GameObject _bonusBreakdownPanel;
        [SerializeField] private TextMeshProUGUI _battleRewardText;
        [SerializeField] private TextMeshProUGUI _completionBonusText;
        [SerializeField] private TextMeshProUGUI _firstClearBonusText;
        [SerializeField] private TextMeshProUGUI _grandTotalText;

        [Header("Buttons")]
        [SerializeField] private Button _nextBattleButton;
        [SerializeField] private Button _townButton;
        [SerializeField] private TextMeshProUGUI _townButtonText;

        [Header("Colors")]
        [SerializeField] private Color _victoryColor = Color.yellow;
        [SerializeField] private Color _defeatColor = Color.gray;

        private bool _isLastBattle;
        private bool _isDefeat;

        private void Awake()
        {
            Hide();
        }

        private void Start()
        {
            if (_nextBattleButton != null)
            {
                _nextBattleButton.onClick.AddListener(OnNextBattleClicked);
            }
            if (_townButton != null)
            {
                _townButton.onClick.AddListener(OnTownClicked);
            }
        }

        private void OnEnable()
        {
            if (ExpeditionManager.Instance != null)
            {
                ExpeditionManager.Instance.OnBattleVictory += HandleBattleVictory;
                ExpeditionManager.Instance.OnBattleDefeat += HandleBattleDefeat;
            }
        }

        private void OnDisable()
        {
            if (ExpeditionManager.Instance != null)
            {
                ExpeditionManager.Instance.OnBattleVictory -= HandleBattleVictory;
                ExpeditionManager.Instance.OnBattleDefeat -= HandleBattleDefeat;
            }
        }

        private void OnDestroy()
        {
            if (_nextBattleButton != null)
            {
                _nextBattleButton.onClick.RemoveListener(OnNextBattleClicked);
            }
            if (_townButton != null)
            {
                _townButton.onClick.RemoveListener(OnTownClicked);
            }
        }

        private void HandleBattleVictory(int battleIndex, int totalBattles, int goldEarned, int totalGold, bool isLastBattle)
        {
            ShowVictoryResult(battleIndex, totalBattles, goldEarned, totalGold, isLastBattle);
        }

        private void HandleBattleDefeat(int battleIndex, int totalBattles, int totalGold)
        {
            ShowDefeatResult(battleIndex, totalBattles, totalGold);
        }

        private void OnNextBattleClicked()
        {
            Hide();
            ExpeditionManager.Instance?.LoadNextBattle();
        }

        private void OnTownClicked()
        {
            bool wasLastBattle = _isLastBattle;
            bool wasDefeat = _isDefeat;
            Hide();

            if (wasDefeat)
            {
                ExpeditionManager.Instance?.FailExpedition();
            }
            else if (wasLastBattle)
            {
                ExpeditionManager.Instance?.CompleteExpedition();
            }
            else
            {
                ExpeditionManager.Instance?.ReturnToTown();
            }
        }

        private void ShowVictoryResult(int battleIndex, int totalBattles, int goldEarned, int totalGold, bool isLastBattle)
        {
            _isLastBattle = isLastBattle;
            _isDefeat = false;

            _panel.SetActive(true);
            _mainPanel.SetActive(true);

            if (isLastBattle)
            {
                ShowExpeditionComplete(totalBattles, totalGold);
            }
            else
            {
                ShowBattleVictory(battleIndex, totalBattles, goldEarned, totalGold);
            }

            if (_nextBattleButton != null)
                _nextBattleButton.gameObject.SetActive(!isLastBattle);

            if (_townButtonText != null)
                _townButtonText.text = isLastBattle ? "원정 완료" : "귀환";
        }

        private void ShowBattleVictory(int battleIndex, int totalBattles, int goldEarned, int totalGold)
        {
            _resultText.text = "전투 승리!";
            _resultText.color = _victoryColor;

            _goldEarnedText.text = $"+{goldEarned}G";
            _goldEarnedText.gameObject.SetActive(true);

            if (_progressText != null)
            {
                _progressText.text = $"{battleIndex + 1}/{totalBattles} 전투";
                _progressText.gameObject.SetActive(true);
            }

            if (_totalGoldText != null)
            {
                _totalGoldText.text = $"총 {totalGold}G";
                _totalGoldText.gameObject.SetActive(true);
            }

            HideBonusBreakdown();
        }

        private void ShowExpeditionComplete(int totalBattles, int battleReward)
        {
            _resultText.text = "원정 완료!";
            _resultText.color = _victoryColor;

            _mainPanel.SetActive(false);

            ShowBonusBreakdown(battleReward);
        }

        private void ShowBonusBreakdown(int battleReward)
        {
            var expedition = ExpeditionManager.Instance?.CurrentExpedition;
            if (expedition == null) return;

            int completionBonus = expedition.CompletionBonus;
            int firstClearBonus = 0;
            bool isFirstClear = ExpeditionProgressManager.Instance?.WillReceiveFirstClearBonus(expedition.ExpeditionId) ?? false;

            if (isFirstClear)
            {
                firstClearBonus = expedition.FirstClearBonus;
            }

            int grandTotal = battleReward + completionBonus + firstClearBonus;

            if (_bonusBreakdownPanel != null)
                _bonusBreakdownPanel.SetActive(true);

            if (_battleRewardText != null)
                _battleRewardText.text = $"전투 보상: {battleReward}G";

            if (_completionBonusText != null)
            {
                bool hasCompletionBonus = completionBonus > 0;
                _completionBonusText.gameObject.SetActive(hasCompletionBonus);
                if (hasCompletionBonus)
                    _completionBonusText.text = $"완료 보너스: +{completionBonus}G";
            }

            if (_firstClearBonusText != null)
            {
                _firstClearBonusText.gameObject.SetActive(isFirstClear);
                if (isFirstClear)
                    _firstClearBonusText.text = $"첫 완료 보너스: +{firstClearBonus}G";
            }

            if (_grandTotalText != null)
                _grandTotalText.text = $"총 획득: {grandTotal}G";
        }

        private void HideBonusBreakdown()
        {
            if (_bonusBreakdownPanel != null)
                _bonusBreakdownPanel.SetActive(false);
        }

        private void ShowDefeatResult(int battleIndex, int totalBattles, int totalGold)
        {
            _isLastBattle = false;
            _isDefeat = true;

            _panel.SetActive(true);
            _mainPanel.SetActive(true);

            _resultText.text = "패배...";
            _resultText.color = _defeatColor;

            _goldEarnedText.text = $"+0G";
            _goldEarnedText.gameObject.SetActive(true);

            if (_progressText != null)
            {
                _progressText.text = $"{battleIndex}/{totalBattles} 전투";
                _progressText.gameObject.SetActive(true);
            }

            if (_totalGoldText != null)
            {
                _totalGoldText.text = $"총 {totalGold}G";
                _totalGoldText.gameObject.SetActive(true);
            }

            HideBonusBreakdown();

            if (_nextBattleButton != null)
                _nextBattleButton.gameObject.SetActive(false);

            if (_townButtonText != null)
                _townButtonText.text = "마을로";
        }

        public void Hide()
        {
            if (_panel != null)
            {
                _panel.SetActive(false);
            }
            _isLastBattle = false;
            _isDefeat = false;
        }
    }
}
