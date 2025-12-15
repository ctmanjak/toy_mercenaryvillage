using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class TownUI : MonoBehaviour
    {
        [Header("Building Buttons")]
        [SerializeField] private Button _dungeonButton;
        [SerializeField] private Button _guildButton;
        [SerializeField] private Button _tavernButton;

        [Header("Popups")]
        [SerializeField] private GameObject _comingSoonPopup;
        [SerializeField] private TextMeshProUGUI _comingSoonText;
        [SerializeField] private Button _comingSoonCloseButton;

        private void Start()
        {
            if (_dungeonButton != null)
            {
                _dungeonButton.onClick.AddListener(OnDungeonButtonClicked);
            }

            if (_guildButton != null)
            {
                _guildButton.onClick.AddListener(OnGuildButtonClicked);
            }

            if (_tavernButton != null)
            {
                _tavernButton.onClick.AddListener(OnTavernButtonClicked);
            }

            if (_comingSoonCloseButton != null)
            {
                _comingSoonCloseButton.onClick.AddListener(HideComingSoonPopup);
            }

            HideComingSoonPopup();
        }

        private void OnDestroy()
        {
            if (_dungeonButton != null)
            {
                _dungeonButton.onClick.RemoveListener(OnDungeonButtonClicked);
            }

            if (_guildButton != null)
            {
                _guildButton.onClick.RemoveListener(OnGuildButtonClicked);
            }

            if (_tavernButton != null)
            {
                _tavernButton.onClick.RemoveListener(OnTavernButtonClicked);
            }

            if (_comingSoonCloseButton != null)
            {
                _comingSoonCloseButton.onClick.RemoveListener(HideComingSoonPopup);
            }
        }

        private void OnDungeonButtonClicked()
        {
            GameManager.Instance.GoToDungeonSelect();
        }

        private void OnGuildButtonClicked()
        {
            GameManager.Instance.GoToGuildHouse();
        }

        private void OnTavernButtonClicked()
        {
            GameManager.Instance.GoToTavern();
        }

        private void ShowComingSoonPopup(string buildingName)
        {
            if (_comingSoonPopup != null)
            {
                _comingSoonPopup.SetActive(true);

                if (_comingSoonText != null)
                {
                    _comingSoonText.text = $"{buildingName}: 준비 중입니다.";
                }
            }
            else
            {
                Debug.Log($"{buildingName}: 준비 중입니다.");
            }
        }

        private void HideComingSoonPopup()
        {
            if (_comingSoonPopup != null)
            {
                _comingSoonPopup.SetActive(false);
            }
        }
    }
}
