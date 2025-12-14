using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Data;

public class StageButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _labelText;
    [SerializeField] private Button _button;
    [SerializeField] private Image _background;
    [SerializeField] private Color _normalColor = new Color(0.25f, 0.25f, 0.3f, 1f);
    [SerializeField] private Color _selectedColor = new Color(0.4f, 0.35f, 0.2f, 1f);

    private StageData _stageData;
    public event Action<StageData> OnStageSelected;

    public StageData StageData => _stageData;

    public void Setup(StageData data)
    {
        _stageData = data;
        _labelText.text = $"[{data.StageId}] {data.StageName}";
        _button.onClick.AddListener(() => OnStageSelected?.Invoke(_stageData));
    }

    public void SetSelected(bool selected)
    {
        if (_background != null)
        {
            _background.color = selected ? _selectedColor : _normalColor;
        }
    }
}
