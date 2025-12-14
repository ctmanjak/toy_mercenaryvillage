using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GuildHouseUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button _backButton;

        [Header("Panels (Placeholder for future tasks)")]
        [SerializeField] private RectTransform _mercenaryListPanel;
        [SerializeField] private RectTransform _detailPanel;
        [SerializeField] private RectTransform _partySlotPanel;

        private void Start()
        {
            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackButtonClicked);
            }
        }

        private void OnBackButtonClicked()
        {
            GameManager.Instance.GoToTown();
        }
    }
}
