using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GuildHouseUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button _backButton;

        [Header("Panels")]
        [SerializeField] private RectTransform _mercenaryListPanel;
        [SerializeField] private RectTransform _detailPanel;
        [SerializeField] private RectTransform _partySlotPanel;

        [Header("Components")]
        [SerializeField] private MercenaryListUI _mercenaryListUI;
        [SerializeField] private MercenaryDetailPanel _mercenaryDetailPanel;

        private void Start()
        {
            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackButtonClicked);
            }

            // 용병 선택 이벤트 연결
            if (_mercenaryListUI != null)
            {
                _mercenaryListUI.OnMercenarySelected += HandleMercenarySelected;
            }

            // 레벨업 이벤트 연결 (목록 갱신용)
            if (MercenaryManager.Instance != null)
            {
                MercenaryManager.Instance.OnMercenaryLevelUp += HandleMercenaryLevelUp;
            }
        }

        private void OnDestroy()
        {
            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackButtonClicked);
            }

            if (_mercenaryListUI != null)
            {
                _mercenaryListUI.OnMercenarySelected -= HandleMercenarySelected;
            }

            if (MercenaryManager.Instance != null)
            {
                MercenaryManager.Instance.OnMercenaryLevelUp -= HandleMercenaryLevelUp;
            }
        }

        private void HandleMercenarySelected(Data.MercenaryData mercenary)
        {
            if (_mercenaryDetailPanel != null)
            {
                _mercenaryDetailPanel.Show(mercenary);
            }
        }

        private void HandleMercenaryLevelUp(Data.MercenaryData mercenary)
        {
            // 목록 갱신 (레벨 텍스트 업데이트)
            if (_mercenaryListUI != null)
            {
                _mercenaryListUI.Refresh();
                // 같은 용병 다시 선택하여 상세 패널 유지
                _mercenaryListUI.SelectMercenary(mercenary);
            }
        }

        private void OnBackButtonClicked()
        {
            GameManager.Instance.GoToTown();
        }
    }
}
