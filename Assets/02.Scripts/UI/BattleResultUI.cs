using Battle;
using UnityEngine;
using TMPro;

namespace UI
{
    public class BattleResultUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TextMeshProUGUI _rewardText;

        private void Awake()
        {
            Hide();
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
