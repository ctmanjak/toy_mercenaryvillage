using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace UI
{
    public class StageListUI : MonoBehaviour
    {
        [SerializeField] private StageButton _stageButtonPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private StageData[] _stages;

        private List<StageButton> _buttons = new();
        private StageButton _selectedButton;

        public event Action<StageData> OnStageSelected;

        private void Start()
        {
            GenerateStageButtons();
        }

        private void GenerateStageButtons()
        {
            foreach (var stage in _stages)
            {
                var button = Instantiate(_stageButtonPrefab, _container);
                button.Setup(stage);
                button.OnStageSelected += HandleStageSelected;
                _buttons.Add(button);
            }
        }

        private void HandleStageSelected(StageData stage)
        {
            foreach (var btn in _buttons)
            {
                btn.SetSelected(btn.StageData == stage);
            }

            _selectedButton = _buttons.Find(b => b.StageData == stage);
            OnStageSelected?.Invoke(stage);
        }

        private void OnDestroy()
        {
            foreach (var button in _buttons)
            {
                if (button != null)
                {
                    button.OnStageSelected -= HandleStageSelected;
                }
            }
        }
    }
}
