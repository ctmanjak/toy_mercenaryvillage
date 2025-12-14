using TMPro;
using UnityEngine;

namespace UI
{
    public class GoldUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _goldText;

        private void Start()
        {
            if (PlayerResourceManager.Instance != null)
            {
                PlayerResourceManager.Instance.OnGoldChanged += UpdateGoldDisplay;
                UpdateGoldDisplay(PlayerResourceManager.Instance.Gold);
            }
        }

        private void OnDestroy()
        {
            if (PlayerResourceManager.Instance != null)
            {
                PlayerResourceManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
            }
        }

        private void UpdateGoldDisplay(int gold)
        {
            if (_goldText != null)
            {
                _goldText.text = $"{gold:N0} G";
            }
        }
    }
}
