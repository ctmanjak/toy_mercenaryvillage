## 목표

용병 목록, 파티 구성, 골드 등 플레이어 데이터를 JSON 파일로 저장/로드한다.

---

## 산출물

- `SaveManager.cs`
- `SaveData.cs` (직렬화 클래스)
- 저장 파일: `Application.persistentDataPath/save.json`

---

## 요구사항

### 저장 데이터 구조

```csharp
[System.Serializable]
public class SaveData
{
    public int gold;
    public List<MercenarySaveData> mercenaries;
    public string[] partyIds; // 파티 슬롯별 용병 ID
}

[System.Serializable]
public class MercenarySaveData
{
    public string id;
    public string unitDataId; // UnitData 식별용
    public string customName;
    public int level;
}
```

### 저장 시점

- 자동 저장:
    - 골드 변경 시
    - 용병 추가/제거 시
    - 파티 변경 시
    - 레벨업 시
- 수동 저장: 없음 (v0.1)

### 로드 시점

- 게임 시작 시 (PlayerData.Awake)
- 저장 파일 없으면 초기 데이터 생성

---

## 설계 가이드

### SaveManager

```csharp
public class SaveManager : MonoBehaviour
{
    private static string SavePath => 
        Path.Combine(Application.persistentDataPath, "save.json");
    
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }
    
    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
            return null;
        
        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }
    
    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}
```

### UnitData 참조 복원

- UnitData는 ScriptableObject이므로 직접 직렬화 불가
- unitDataId로 저장 → 로드 시 Resources나 레지스트리에서 검색

```csharp
// UnitDataRegistry.cs
public class UnitDataRegistry : MonoBehaviour
{
    [SerializeField] private UnitData[] _allUnitData;
    
    public UnitData GetById(string id) =>
        _allUnitData.FirstOrDefault(u => [u.id](http://u.id) == id);
}
```

---

## 수락 기준

- [ ]  게임 종료/재시작 시 데이터 유지
- [ ]  골드 저장/로드
- [ ]  용병 목록 저장/로드 (레벨 포함)
- [ ]  파티 구성 저장/로드
- [ ]  저장 파일 손상 시 초기화 처리

---

## 후속 태스크 연결

- [S3] 용병 시스템 통합 플레이테스트