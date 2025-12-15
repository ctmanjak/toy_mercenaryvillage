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
        [SerializeField] private PartyFormationUI _partyFormationUI;

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

            // 파티 변경 시 용병 목록 갱신 (배지 업데이트)
            if (_partyFormationUI != null)
            {
                _partyFormationUI.OnPartyUpdated += HandlePartyUpdated;
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

            if (_partyFormationUI != null)
            {
                _partyFormationUI.OnPartyUpdated -= HandlePartyUpdated;
            }
        }

        private void HandleMercenarySelected(Data.MercenaryData mercenary)
        {
            if (_mercenaryDetailPanel != null)
            {
                _mercenaryDetailPanel.Show(mercenary);
            }

            // 파티 편성 UI에 선택된 용병 전달
            if (_partyFormationUI != null)
            {
                _partyFormationUI.OnMercenarySelected(mercenary);
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

        private void HandlePartyUpdated()
        {
            // 파티 변경 시 용병 목록 새로고침 (배지 상태 업데이트)
            if (_mercenaryListUI != null)
            {
                _mercenaryListUI.Refresh();
            }
        }

        private void OnBackButtonClicked()
        {
            GameManager.Instance.GoToTown();
        }
    }
}
