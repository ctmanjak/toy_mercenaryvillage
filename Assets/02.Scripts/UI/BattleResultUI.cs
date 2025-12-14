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
        [SerializeField] private Button _returnButton;

        private void Awake()
        {
            Hide();
        }

        private void Start()
        {
            if (_returnButton != null)
            {
                _returnButton.onClick.AddListener(OnReturnButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (_returnButton != null)
            {
                _returnButton.onClick.RemoveListener(OnReturnButtonClicked);
            }
        }

        private void OnReturnButtonClicked()
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
                _rewardText.text = $"획득 골드: {goldReward}G";
                _rewardText.gameObject.SetActive(true);
            }
            else
            {
                _resultText.text = "패배..";
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
