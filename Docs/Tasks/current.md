## 목표

전투 승리 시 스테이지에 설정된 골드 보상을 플레이어에게 지급한다.

---

## 산출물

- BattleManager 보상 지급 로직
- PlayerData 연동

---

## 요구사항

### 보상 지급 조건

| 결과 | 보상 |
| --- | --- |
| 승리 | StageData.goldReward 지급 |
| 패배 | 보상 없음 (0G) |

### 보상 흐름

```
전투 종료 (Victory)
    ↓
BattleManager.EndBattle()
    ↓
PlayerData.AddGold(reward)
    ↓
BattleResultUI.Show(result, reward)
```

---

## 설계 가이드

### StageData 필드 확인

```csharp
public int goldReward = 50;  // S1에서 이미 정의됨
```

### 보상 지급 시점

- EndBattle() 내부에서 승리 판정 직후
- UI 표시 전에 지급 완료

### 중복 지급 방지

- EndBattle()은 1회만 호출되도록 BattlePhase 체크

---

## 수락 기준

- [ ]  승리 시 PlayerData에 골드 추가
- [ ]  패배 시 골드 추가 없음
- [ ]  결과 화면에 획득 골드 표시
- [ ]  마을 복귀 후 GoldUI에 반영 확인
- [ ]  다시 도전 시 추가 보상 정상 지급

---

## 참고 코드 스니펫

```csharp
// BattleManager.cs 수정
private void EndBattle(BattleResult result)
{
    if (phase == BattlePhase.Ended) return;  // 중복 호출 방지
    
    phase = BattlePhase.Ended;
    
    int reward = 0;
    
    if (result == BattleResult.Victory)
    {
        reward = _currentStage.goldReward;
        
        // 골드 지급
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.AddGold(reward);
        }
    }
    
    // 결과 UI 표시
    if (_resultUI != null)
    {
        _resultUI.Show(result, reward);
    }
    
    Debug.Log($"Battle Ended: {result}, Reward: {reward}G");
}
```

### 스테이지별 보상 예시 (v0.1)

| 스테이지 | 보상 골드 |
| --- | --- |
| 1-1 | 30G |
| 1-2 | 40G |
| 1-3 | 50G |
| 1-4 | 60G |
| 1-5 | 80G |

---

## 후속 태스크 연결

이 태스크는 독립적으로 완결됨.

관련 태스크:

- [S2] PlayerData: 골드 보유량 관리 (선행)