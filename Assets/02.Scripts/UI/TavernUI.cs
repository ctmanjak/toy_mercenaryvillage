using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TavernUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button _backButton;

        [Header("Panels")]
        [SerializeField] private RectTransform _candidateListPanel;
        [SerializeField] private RectTransform _detailPanel;

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
