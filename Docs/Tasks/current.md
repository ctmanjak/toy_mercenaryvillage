## 목표

기존 별도 씬(GuildHouseScene, TavernScene)을 TownScene 내 패널로 통합하여 씬 수를 줄이고 마을 내 전환을 빠르게 한다.

---

## 배경

- 현재 5개 씬: TownScene, GuildHouseScene, TavernScene, DungeonSelectScene, BattleScene
- 길드하우스/술집은 풀스크린 UI일 뿐, 별도 게임 로직 없음
- 마을 ↔ 건물 전환이 잦은데 매번 씬 로드는 과함

---

## 산출물

- `TownUIManager.cs` (패널 전환 관리)
- TownScene 내 GuildPanel, TavernPanel 통합
- GuildHouseScene.unity, TavernScene.unity 삭제
- Build Settings 업데이트 (3개 씬만 유지)

---

## 요구사항

### 패널 구조

```
TownScene
├─ Main Camera
├─ Canvas
│   ├─ TownPanel (기본 마을 화면)
│   │   ├─ Background
│   │   ├─ BuildingButtons (던전/길드/술집)
│   │   └─ GoldUI
│   ├─ GuildPanel (비활성 상태로 시작)
│   │   ├─ Header (뒤로가기, 타이틀, 골드)
│   │   ├─ MercenaryListUI
│   │   ├─ MercenaryDetailPanel
│   │   └─ PartySlotUI
│   └─ TavernPanel (비활성 상태로 시작)
│       ├─ Header
│       ├─ CandidateListUI
│       └─ HireDetailPanel
└─ EventSystem
```

### 전환 로직

```csharp
public class TownUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _townPanel;
    [SerializeField] private GameObject _guildPanel;
    [SerializeField] private GameObject _tavernPanel;
    
    private GameObject _currentPanel;
    
    private void Start()
    {
        ShowTown();
    }
    
    public void ShowTown()
    {
        SwitchPanel(_townPanel);
    }
    
    public void ShowGuild()
    {
        SwitchPanel(_guildPanel);
    }
    
    public void ShowTavern()
    {
        SwitchPanel(_tavernPanel);
    }
    
    private void SwitchPanel(GameObject panel)
    {
        _townPanel.SetActive(panel == _townPanel);
        _guildPanel.SetActive(panel == _guildPanel);
        _tavernPanel.SetActive(panel == _tavernPanel);
        _currentPanel = panel;
    }
}
```

### GameManager 수정

```csharp
// 기존 (삭제)
public void LoadGuildHouse() => SceneManager.LoadScene("GuildHouseScene");
public void LoadTavern() => SceneManager.LoadScene("TavernScene");

// 변경 (TownUIManager 참조로 대체하거나 이벤트 사용)
```

---

## 작업 순서

1. TownUIManager.cs 생성
2. TownScene에 GuildPanel, TavernPanel 빈 오브젝트 추가
3. 기존 GuildHouseScene의 UI 계층을 GuildPanel 하위로 이동
4. 기존 TavernScene의 UI 계층을 TavernPanel 하위로 이동
5. 버튼 연결 수정 (씬 전환 → 패널 전환)
6. 뒤로가기 버튼 수정 (TownUIManager.ShowTown 호출)
7. GuildHouseScene.unity, TavernScene.unity 삭제
8. Build Settings에서 제거

---

## 수락 기준

- [ ]  TownScene에서 길드하우스 버튼 → GuildPanel 표시
- [ ]  TownScene에서 술집 버튼 → TavernPanel 표시
- [ ]  각 패널에서 뒤로가기 → TownPanel 표시
- [ ]  기존 기능 정상 동작 (용병 목록, 고용 등)
- [ ]  GoldUI가 모든 패널에서 표시
- [ ]  Build Settings에 3개 씬만 존재
- [ ]  GuildHouseScene.unity, TavernScene.unity 삭제됨

---

## 주의사항

- 기존 UI 프리팹/스크립트는 최대한 재사용
- 씬 전환 관련 코드만 패널 전환으로 변경
- DontDestroyOnLoad 객체 정리 확인