using UnityEngine;

namespace UI
{
    public class TownUIManager : MonoBehaviour
    {
        public static TownUIManager Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private GameObject _townPanel;
        [SerializeField] private GameObject _guildPanel;
        [SerializeField] private GameObject _tavernPanel;
        [SerializeField] private GameObject _expeditionSelectPanel;

        private GameObject _currentPanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            ShowTown();
        }

        public void ShowTown()
        {
            SwitchPanel(_townPanel);
        }

        public void ShowGuild()
        {
            SwitchPanel(_guildPanel);
        }

        public void ShowTavern()
        {
            SwitchPanel(_tavernPanel);
        }

        public void ShowExpeditionSelect()
        {
            SwitchPanel(_expeditionSelectPanel);
        }

        private void SwitchPanel(GameObject panel)
        {
            if (_townPanel != null) _townPanel.SetActive(panel == _townPanel);
            if (_guildPanel != null) _guildPanel.SetActive(panel == _guildPanel);
            if (_tavernPanel != null) _tavernPanel.SetActive(panel == _tavernPanel);
            if (_expeditionSelectPanel != null) _expeditionSelectPanel.SetActive(panel == _expeditionSelectPanel);

            _currentPanel = panel;
        }

        public bool IsShowingTown()
        {
            return _currentPanel == _townPanel;
        }
    }
}
