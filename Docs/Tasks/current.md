## 목표

4인 파티 슬롯을 관리하고, 전투 시 실제 파티 데이터를 사용하도록 연동한다.

---

## 산출물

- `PartyManager.cs`
- BattleManager 파티 연동 수정

---

## 요구사항

### 파티 슬롯 구조

| 필드 | 타입 | 설명 |
| --- | --- | --- |
| partySlots | MercenaryData[4] | 출전 파티 (null = 빈 슬롯) |
| OnPartyChanged | Action | 파티 변경 시 이벤트 |

### 핵심 메서드

```csharp
public bool SetPartySlot(int slotIndex, MercenaryData merc);
public void ClearPartySlot(int slotIndex);
public MercenaryData[] GetParty();
public int GetPartyCount(); // null 제외
public bool IsInParty(MercenaryData merc);
```

### 전투 연동

- BattleManager.StartBattle()에서 하드코딩된 아군 생성 제거
- PartyManager.GetParty()로 실제 파티 가져오기
- 각 MercenaryData.CreateBattleUnit() 호출

---

## 설계 가이드

### 초기 파티 설정

- 게임 시작 시 초기 용병 2명을 자동으로 파티에 배치
- 슬롯 0, 1에 배치

### 빈 파티 검증

```csharp
public bool CanStartBattle()
{
    return GetPartyCount() > 0;
}
```

### BattleManager 수정

```csharp
// 기존: 하드코딩된 테스트 유닛
// 변경:
private void SpawnAllies()
{
    var party = PartyManager.Instance.GetParty();
    for (int i = 0; i < party.Length; i++)
    {
        if (party[i] == null) continue;
        var unit = party[i].CreateBattleUnit(_allySpawnPoints[i].position);
        RegisterUnit(unit, isAlly: true);
    }
}
```

---

## 수락 기준

- [ ]  파티 슬롯 4개 정상 관리
- [ ]  같은 용병 중복 배치 불가
- [ ]  전투 시작 시 파티 용병으로 아군 생성
- [ ]  빈 파티로 전투 시작 시 경고/차단
- [ ]  파티 변경 이벤트 발생

---

## 후속 태스크 연결

이 태스크 완료 후 진행 가능:

- [S3] 파티 편성 UI
- [S3] 길드 하우스 화면