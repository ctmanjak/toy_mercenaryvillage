## 목표

전투 중 화면 상단에 현재 스테이지 정보를 표시한다.

---

## 산출물

- `Assets/02.Scripts/UI/BattleHUD.cs`
- 전투 HUD UI 프리팩

---

## 요구사항

### UI 구성 요소

| 요소 | 예시 |
| --- | --- |
| 스테이지 번호 | "1-3" |
| 스테이지 이름 | "초원 깊은 곳" |

### 표시 형식

```
[1-3] 초원 깊은 곳
```

또는

```
Stage 1-3
초원 깊은 곳
```

---

## 설계 가이드

### UI 계층 구조

```
BattleHUD (Panel, 상단)
├─ StageInfoPanel
│   ├─ StageNumberText ("1-3")
│   └─ StageNameText ("초원 깊은 곳")
└─ GoldUI (프리팩, 우측)
```

### 위치

- Anchor: Top-Left 또는 Top-Center
- 여백: (20, 20) from corner

### 데이터 소스

- GameManager.CurrentStage에서 읽어옴
- 전투 시작 시 1회 설정

---

## 수락 기준

- [ ]  BattleHUD.cs 컴파일 성공
- [ ]  전투 시작 시 스테이지 정보 표시
- [ ]  스테이지 번호 정상 표시
- [ ]  스테이지 이름 정상 표시

---

## 참고 코드 스니펫

```csharp
using UnityEngine;
using TMPro;

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
        if (GameManager.Instance == null || 
            GameManager.Instance.CurrentStage == null)
        {
            _stageNumberText.text = "---";
            _stageNameText.text = "Unknown Stage";
            return;
        }
        
        var stage = GameManager.Instance.CurrentStage;
        _stageNumberText.text = $"{stage.regionIndex}-{stage.stageIndex}";
        _stageNameText.text = stage.stageName;
    }
}
```

### StageData 확장 (필요시)

```csharp
// StageData.cs에 추가
public int regionIndex = 1;
public int stageIndex = 1;
```

---

## 후속 태스크 연결

이 태스크는 독립적으로 완결됨.