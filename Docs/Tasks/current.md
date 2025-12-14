## 목표

전투 종료 후 승리/패배 결과와 획득 보상을 표시하는 UI를 구현한다.

---

## 산출물

- `Assets/02.Scripts/UI/BattleResultUI.cs`
- 결과 화면 UI 프리팩

---

## 요구사항

### UI 구성 요소

| 요소 | 설명 |
| --- | --- |
| 결과 텍스트 | "승리!" 또는 "패배..." |
| 보상 표시 | "획득 골드: 150G" |
| 배경 | 반투명 패널 또는 오버레이 |

### 기능

- `Show(BattleResult result, int goldReward)`: 결과 화면 표시
- `Hide()`: 결과 화면 숨김

### 표시 조건

- 승리 시: 택스트 + 보상 골드 표시
- 패배 시: 텍스트만 표시 (보상 없음)

---

## 설계 가이드

### UI 계층 구조

```
BattleResultUI (Panel)
├── Background (Image, 반투명)
├── ResultText (TextMeshPro)
└── RewardText (TextMeshPro)
```

### 초기 상태

- `SetActive(false)`로 숨겨둔 상태
- 전투 종료 시 BattleManager에서 호출

### 색상 구분

- 승리: 금색/노란색 텍스트
- 패배: 회색/빨간색 텍스트

---

## 수락 기준

- [ ]  BattleResultUI.cs 컴파일 성공
- [ ]  결과 화면 UI 프리팩 생성
- [ ]  승리 시 "승리!" + 골드 표시
- [ ]  패배 시 "패배..." 표시
- [ ]  BattleManager에서 EndBattle() 시 UI 호출 연동
- [ ]  테스트: 승리/패배 각각 UI 확인

---

## 참고 코드 스니펫

```csharp
using UnityEngine;
using TMPro;

public class BattleResultUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TextMeshProUGUI _rewardText;
    
    [Header("Colors")]
    [SerializeField] private Color _victoryColor = Color.yellow;
    [SerializeField] private Color _defeatColor = Color.gray;
    
    private void Awake()
    {
        Hide();
    }
    
    public void Show(BattleResult result, int goldReward = 0)
    {
        _panel.SetActive(true);
        
        if (result == BattleResult.Victory)
        {
            _resultText.text = "승리!";
            _resultText.color = _victoryColor;
            _rewardText.text = $"획득 골드: {goldReward}G";
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
}

// BattleManager.cs 연동
[SerializeField] private BattleResultUI _resultUI;

private void EndBattle(BattleResult result)
{
    phase = BattlePhase.Ended;
    
    int reward = 0;
    if (result == BattleResult.Victory)
    {
        reward = _currentStage.goldReward;
        // TODO: 실제 골드 지급
    }
    
    resultUI.Show(result, reward);
}
```

---

## 후속 태스크 연결

이 태스크 완료 후 진행 가능:

- [S2] 결과 화면 버튼: 다시 도전/마을로 돌아가기