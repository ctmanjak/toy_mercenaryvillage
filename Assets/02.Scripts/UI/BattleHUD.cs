using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BattleHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _stageNumberText;
        [SerializeField] private TextMeshProUGUI _stageNameText;

        private void Start()
        {
            SetupStageInfo();
        }

        private void SetupStageInfo()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentStage == null)
            {
                if (_stageNumberText != null) _stageNumberText.text = "---";
                if (_stageNameText != null) _stageNameText.text = "Unknown Stage";
                return;
            }

            var stage = GameManager.Instance.CurrentStage;
            if (_stageNumberText != null) _stageNumberText.text = $"Stage {stage.StageId}";
            if (_stageNameText != null) _stageNameText.text = stage.StageName;
        }
    }
}
