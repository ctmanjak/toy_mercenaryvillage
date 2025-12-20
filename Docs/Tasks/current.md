## 목표

원정 진행 상황(완료 횟수, 첫 완료 여부)을 저장/로드한다.

---

## 산출물

- PlayerData 확장 (원정 진행 데이터)
- SaveData 확장
- SaveManager 수정

---

## 요구사항

### PlayerData 확장

- List of ExpeditionProgress 추가
- GetCompletionCount(expeditionId): 완료 횟수 조회
- AddCompletion(expeditionId): 완료 횟수 증가
- IsFirstClearClaimed(expeditionId): 첫 완료 보너스 수령 여부
- ClaimFirstClearBonus(expeditionId): 첫 완료 보너스 수령 처리
- **IsUnlocked(ExpeditionData)**: 해금 여부 확인

### 해금 체크 로직

```csharp
public bool IsUnlocked(ExpeditionData expedition)
{
    if (expedition.unlockRequirement == null)
        return true; // 기본 해금
    
    // 해금 조건 원정 첫 완료 여부 확인
    return GetCompletionCount(expedition.unlockRequirement.expeditionId) > 0;
}
```

### 저장 데이터

- JSON 형식으로 원정 진행 상황 저장
- 기존 SaveData에 expeditionProgress 필드 추가

---

## 수락 기준

- 원정 완료 시 완료 횟수 저장
- 첫 완료 보너스 수령 상태 저장
- 게임 재시작 시 진행 상황 복원
- IsUnlocked() 메서드로 해금 여부 확인 가능
- 원정 선택 화면에서 완료 횟수 표시