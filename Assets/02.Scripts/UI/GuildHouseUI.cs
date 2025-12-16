using UnityEngine;
using UnityEngine.UI;
using Core;

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
            
            if (_mercenaryListUI != null)
            {
                _mercenaryListUI.OnMercenarySelected += HandleMercenarySelected;
            }
            
            if (MercenaryManager.Instance != null)
            {
                MercenaryManager.Instance.OnMercenaryLevelUp += HandleMercenaryLevelUp;
            }
            
            if (_partyFormationUI != null)
            {
                _partyFormationUI.OnPartyUpdated += HandlePartyUpdated;
            }
        }

        private void OnEnable()
        {
            if (_mercenaryListUI != null)
            {
                _mercenaryListUI.Refresh();
            }

            if (_mercenaryDetailPanel != null)
            {
                _mercenaryDetailPanel.Hide();
            }

            if (_partyFormationUI != null)
            {
                _partyFormationUI.RefreshUI();
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
            
            if (_partyFormationUI != null)
            {
                _partyFormationUI.OnMercenarySelected(mercenary);
            }
        }

        private void HandleMercenaryLevelUp(Data.MercenaryData mercenary)
        {
            if (_mercenaryListUI != null)
            {
                _mercenaryListUI.Refresh();
                _mercenaryListUI.SelectMercenary(mercenary);
            }
        }

        private void HandlePartyUpdated()
        {
            if (_mercenaryListUI != null)
            {
                _mercenaryListUI.Refresh();
            }
        }

        private void OnBackButtonClicked()
        {
            if (!PartyValidator.HasAnyMember())
            {
                CommonPopup.Instance?.ShowAlert("파티를 편성해야 마을로 돌아갈 수 있습니다.");
                return;
            }
            TownUIManager.Instance?.ShowTown();
        }
    }
}
