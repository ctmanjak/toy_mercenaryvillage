## 목표

스테이지 선택 시 해당 스테이지 데이터를 전투 씬에 전달하고 씬을 전환한다.

---

## 산출물

- 씬 전환 로직 구현 (GameManager 활용)

---

## 요구사항

### 데이터 전달 흐름

```
스테이지 선택 → GameManager에 저장 → 씬 전환 → BattleManager에서 읽어서 사용
```

### GameManager 역할

- 현재 선택된 StageData 보관
- 씬 간 데이터 전달 중개
- DontDestroyOnLoad

---

## 설계 가이드

### 씬 전환 순서

1. 사용자가 스테이지 버튼 클릭
2. StageListUI에서 OnStageSelected 이벤트 발생
3. DungeonSelectUI에서 GameManager.SetCurrentStage() 호출
4. SceneManager.LoadScene("BattleScene")
5. BattleScene의 BattleManager에서 GameManager.CurrentStage 참조

### v0.1 단순화

- 파티 편성 화면 생략
- 고정된 테스트 파티로 바로 전투 진입

---

## 수락 기준

- [ ]  스테이지 선택 시 데이터 저장
- [ ]  전투 씬으로 정상 전환
- [ ]  BattleManager에서 선택된 스테이지 데이터 사용 확인
- [ ]  전투 정상 진행 확인

---

## 참고 코드 스니펫

```csharp
// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public StageData CurrentStage { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void StartBattle(StageData stage)
    {
        CurrentStage = stage;
        SceneManager.LoadScene("BattleScene");
    }
    
    public void ReturnToTown()
    {
        SceneManager.LoadScene("TownScene");
    }
    
    public void GoToDungeonSelect()
    {
        SceneManager.LoadScene("DungeonSelectScene");
    }
}
```

```csharp
// DungeonSelectUI.cs 수정
private void Start()
{
    _stageListUI.OnStageSelected += OnStageSelected;
    _backButton.onClick.AddListener(() => GameManager.Instance.ReturnToTown());
}

private void OnStageSelected(StageData stage)
{
    GameManager.Instance.StartBattle(stage);
}
```

```csharp
// BattleManager.cs 수정 (Start 부분)
private void Start()
{
    if (GameManager.Instance != null && GameManager.Instance.CurrentStage != null)
    {
        StartBattle(GameManager.Instance.CurrentStage);
    }
}
```

---

## 후속 태스크 연결

이 태스크 완료 후 진행 가능:

- [S2] 전체 루프 플레이테스트