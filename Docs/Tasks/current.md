## 목표

술집에서 고용 가능한 용병 후보를 생성하고, 골드를 지불해 고용하는 기능을 구현한다.

---

## 산출물

- `TavernManager.cs`
- `HireableSlotUI.cs`
- 고용 로직

---

## 요구사항

### 고용 후보 생성

- 후보 3명 표시
- 직업 랜덤 (탱커/근접딜러/원거리딜러)
- 모두 Lv.1
- v0.1: 고정 라인업 (새로고침 없음)

### 고용 비용

| 직업 | 비용 |
| --- | --- |
| 탱커 | 100G |
| 근접딜러 | 80G |
| 원거리딜러 | 80G |

### 고용 흐름

1. 후보 용병 클릭 → 상세 정보 표시
2. "고용하기" 버튼 클릭
3. 골드 확인 → 차감 → 소유 목록에 추가
4. 해당 후보 슬롯 비움 (또는 새 후보 생성)

---

## 설계 가이드

### TavernManager

```csharp
public class TavernManager : MonoBehaviour
{
    [SerializeField] private UnitData[] _unitTemplates; // 3종
    [SerializeField] private int[] _hireCosts; // 직업별 비용
    
    private MercenaryData[] _candidates = new MercenaryData[3];
    
    private void Start()
    {
        GenerateCandidates();
    }
    
    private void GenerateCandidates()
    {
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, _unitTemplates.Length);
            _candidates[i] = new MercenaryData(_unitTemplates[randomIndex]);
        }
    }
    
    public bool TryHire(int candidateIndex)
    {
        var candidate = _candidates[candidateIndex];
        if (candidate == null) return false;
        
        int cost = GetHireCost(candidate.unitData);
        if (!PlayerData.Instance.SpendGold(cost))
            return false;
        
        PlayerData.Instance.AddMercenary(candidate);
        _candidates[candidateIndex] = null;
        return true;
    }
}
```

---

## 수락 기준

- [ ]  술집 진입 시 후보 3명 생성
- [ ]  후보 클릭 시 상세 정보 표시
- [ ]  고용 버튼 클릭 시 골드 차감
- [ ]  고용 성공 시 소유 목록에 추가
- [ ]  고용된 슬롯 비움 처리
- [ ]  골드 부족 시 고용 실패

---

## 후속 태스크 연결

- [S3] 용병 시스템 통합 플레이테스트