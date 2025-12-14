## 목표

전투 결과 화면에 "다시 도전", "마을로" 버튼을 추가하고 기능을 연결한다.

---

## 산출물

- BattleResultUI에 버튼 2개 추가
- 버튼 클릭 로직 구현

---

## 요구사항

### 버튼 구성

| 버튼 | 동작 |
| --- | --- |
| 다시 도전 | 같은 스테이지로 재전투 |
| 마을로 | TownScene으로 이동 |

### UI 구성

```
BattleResultUI (Panel)
├─ ResultText ("승리!" / "패배...")
├─ RewardText ("획득: 50G")
└─ ButtonContainer
    ├─ RetryButton ("다시 도전")
    └─ TownButton ("마을로")
```

---

## 설계 가이드

### 다시 도전 처리

- GameManager.CurrentStage 유지
- BattleScene 재로드

### 마을 복귀 처리

- GameManager.ReturnToTown() 호출

### 버튼 배치

- 가로 나란히 또는 세로 배치
- HorizontalLayoutGroup 사용 권장

---

## 수락 기준

- [ ]  "다시 도전" 버튼 UI 추가
- [ ]  "마을로" 버튼 UI 추가
- [ ]  다시 도전 클릭 시 전투 재시작
- [ ]  마을로 클릭 시 TownScene 전환
- [ ]  승리/패배 모두 버튼 정상 동작

---

## 참고 코드 스니펫

```csharp
// BattleResultUI.cs 확장
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleResultUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TextMeshProUGUI _rewardText;
    
    [Header("Buttons")]
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _townButton;
    
    [Header("Colors")]
    [SerializeField] private Color _victoryColor = Color.yellow;
    [SerializeField] private Color _defeatColor = Color.gray;
    
    private void Awake()
    {
        Hide();
        
        _retryButton.onClick.AddListener(OnRetryClicked);
        _townButton.onClick.AddListener(OnTownClicked);
    }
    
    public void Show(BattleResult result, int goldReward = 0)
    {
        _panel.SetActive(true);
        
        if (result == BattleResult.Victory)
        {
            _resultText.text = "승리!";
            _resultText.color = _victoryColor;
            _rewardText.text = $"획득 곢8드: {goldReward}G";
            _rewardText.gameObject.SetActive(true);
        }
        else
        {
            _resultText.text = "패배...";
            _resultText.color = _defeatColor;
            _rewardText.gameObject.SetActive(false);
        }
    }
    
    public void Hide()
    {
        _panel.SetActive(false);
    }
    
    private void OnRetryClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartBattle(GameManager.Instance.CurrentStage);
        }
    }
    
    private void OnTownClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToTown();
        }
    }
}
```

---

## 후속 태스크 연결

이 태스크 완료 후 진행 가능:

- [S2] 전투 결과 → 마을 복귀 연결