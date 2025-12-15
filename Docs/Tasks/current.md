## 목표

선택한 용병의 상세 정보를 표시하고, 레벨업 기능을 제공한다.

---

## 산출물

- `MercenaryDetailPanel.cs`
- 레벨업 UI 통합

---

## 요구사항

### 상세 정보 표시

- 용병 이름
- 직업
- 현재 레벨 / 최대 레벨 (예: Lv.3 / 10)
- 스탯: HP, ATK, AtkSpd, Range
- (선택) 초상화/아이콘

### 레벨업 기능

- 레벨업 버튼
- 필요 골드 표시
- 골드 부족 시 버튼 비활성화
- 최대 레벨 도달 시 버튼 숨김

### 레벨업 비용 테이블

| 레벨 | 비용 |
| --- | --- |
| 1→2 | 50G |
| 2→3 | 75G |
| 3→4 | 100G |
| 4→5 | 150G |
| 5→6 | 200G |
| 6→7 | 300G |
| 7→8 | 400G |
| 8→9 | 500G |
| 9→10 | 750G |

**공식**: `cost = 50 * (1 + (level-1) * 0.5)` 근사

---

## 설계 가이드

### 레벨업 로직

```csharp
public static int GetLevelUpCost(int currentLevel)
{
    int[] costs = { 50, 75, 100, 150, 200, 300, 400, 500, 750 };
    if (currentLevel >= 10) return -1; // 최대 레벨
    return costs[currentLevel - 1];
}

public bool TryLevelUp(MercenaryData merc)
{
    int cost = GetLevelUpCost(merc.level);
    if (cost < 0 || !PlayerData.Instance.SpendGold(cost))
        return false;
    
    merc.level++;
    OnMercenaryLevelUp?.Invoke(merc);
    return true;
}
```

---

## 수락 기준

- [ ]  선택 용병 정보 표시
- [ ]  레벨 적용된 스탯 표시
- [ ]  레벨업 버튼 동작
- [ ]  골드 차감 및 레벨 증가
- [ ]  UI 즉시 갱신
- [ ]  최대 레벨 처리

---

## 후속 태스크 연결

- [S3] 파티 편성 UI