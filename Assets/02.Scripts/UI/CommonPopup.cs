using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CommonPopup : MonoBehaviour
    {
        public static CommonPopup Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject _popupPanel;

        [Header("Content")]
        [SerializeField] private TMP_Text _messageText;

        [Header("Buttons")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TMP_Text _confirmButtonText;
        [SerializeField] private TMP_Text _cancelButtonText;

        private Action _onConfirm;
        private Action _onCancel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_confirmButton != null)
            {
                _confirmButton.onClick.AddListener(OnConfirmClicked);
            }

            if (_cancelButton != null)
            {
                _cancelButton.onClick.AddListener(OnCancelClicked);
            }

            if (_popupPanel != null)
            {
                _popupPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.RemoveListener(OnConfirmClicked);
            }

            if (_cancelButton != null)
            {
                _cancelButton.onClick.RemoveListener(OnCancelClicked);
            }
        }
        
        public void ShowAlert(string message, Action onConfirm = null, string confirmText = "확인")
        {
            _messageText.text = message;
            _onConfirm = onConfirm;
            _onCancel = null;

            if (_confirmButtonText != null)
            {
                _confirmButtonText.text = confirmText;
            }

            _confirmButton.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(false);
            _popupPanel.SetActive(true);
        }
        
        public void ShowConfirm(string message, Action onConfirm, Action onCancel = null,
            string confirmText = "확인", string cancelText = "취소")
        {
            _messageText.text = message;
            _onConfirm = onConfirm;
            _onCancel = onCancel;

            if (_confirmButtonText != null)
            {
                _confirmButtonText.text = confirmText;
            }

            if (_cancelButtonText != null)
            {
                _cancelButtonText.text = cancelText;
            }

            _confirmButton.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(true);
            _popupPanel.SetActive(true);
        }
        
        public void Hide()
        {
            _popupPanel.SetActive(false);
            _onConfirm = null;
            _onCancel = null;
        }

        private void OnConfirmClicked()
        {
            var callback = _onConfirm;
            Hide();
            callback?.Invoke();
        }

        private void OnCancelClicked()
        {
            var callback = _onCancel;
            Hide();
            callback?.Invoke();
        }
        
        public bool IsVisible => _popupPanel != null && _popupPanel.activeSelf;
    }
}
