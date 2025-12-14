using Battle;
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
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TextMeshProUGUI _rewardText;

        [Header("Buttons")]
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _townButton;

        [Header("Colors")]
        [SerializeField] private Color _victoryColor = Color.yellow;
        [SerializeField] private Color _defeatColor = Color.gray;

        private void Awake()
        {
            Hide();
        }

        private void Start()
        {
            if (_retryButton != null)
            {
                _retryButton.onClick.AddListener(OnRetryClicked);
            }
            if (_townButton != null)
            {
                _townButton.onClick.AddListener(OnTownClicked);
            }
        }

        private void OnDestroy()
        {
            if (_retryButton != null)
            {
                _retryButton.onClick.RemoveListener(OnRetryClicked);
            }
            if (_townButton != null)
            {
                _townButton.onClick.RemoveListener(OnTownClicked);
            }
        }

        private void OnRetryClicked()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentStage != null)
            {
                GameManager.Instance.StartBattle(GameManager.Instance.CurrentStage);
            }
        }

        private void OnTownClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToTown();
            }
        }

        public void Show(BattleResult result, int goldReward = 0)
        {
            _panel.SetActive(true);

            if (result == BattleResult.Victory)
            {
                _resultText.text = "승리!";
                _resultText.color = _victoryColor;
                _rewardText.text = $"획득 골드: {goldReward}G";
                _rewardText.gameObject.SetActive(true);
            }
            else
            {
                _resultText.text = "패배..";
                _resultText.color = _defeatColor;
                _rewardText.gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            if (_panel != null)
            {
                _panel.SetActive(false);
            }
        }
    }
}
