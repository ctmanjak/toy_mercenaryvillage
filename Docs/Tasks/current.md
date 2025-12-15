## 목표

빈 파티 상태에서 게임 진행을 차단하고, 재사용 가능한 공통 경고 팝업 시스템을 구현한다.

---

## 산출물

- `CommonPopup.cs` / `CommonPopup.prefab` (공통 팝업)
- `PartyValidator.cs` (파티 검증 유틸)

---

## 요구사항

### 1. 공통 경고 팝업 시스템

- 메시지 + 확인 버튼 (기본)
- 메시지 + 확인/취소 버튼 (선택)
- 씬 전환 시에도 유지되는 싱글톤 or DontDestroyOnLoad
- 콜백 지원 (확인/취소 시 액션 실행)

### 2. GuildHouseScene - 뒤로가기 차단

- 파티가 비어있으면 마을로 복귀 불가
- "파티를 편성해야 마을로 돌아갈 수 있습니다" 팝업 표시
- 최소 1명 이상 편성 시 정상 복귀

### 3. DungeonSelectScene - 전투 시작 차단 (이중 안전)

- TownScene → DungeonSelectScene 직행 경로 대비
- 빈 파티로 전투 시작 버튼 클릭 시 차단
- "파티에 용병이 없습니다. 길드 하우스에서 편성해주세요" 팝업 표시

---

## 설계 가이드

### CommonPopup 구조

```csharp
public class CommonPopup : MonoBehaviour
{
    public static CommonPopup Instance;
    
    [SerializeField] private GameObject _popupPanel;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;
    
    private System.Action _onConfirm;
    private System.Action _onCancel;
    
    // 경고 팝업 (확인만)
    public void ShowAlert(string message, System.Action onConfirm = null)
    {
        _messageText.text = message;
        _confirmButton.gameObject.SetActive(true);
        _cancelButton.gameObject.SetActive(false);
        _onConfirm = onConfirm;
        _popupPanel.SetActive(true);
    }
    
    // 확인/취소 팝업
    public void ShowConfirm(string message, System.Action onConfirm, System.Action onCancel = null)
    {
        _messageText.text = message;
        _confirmButton.gameObject.SetActive(true);
        _cancelButton.gameObject.SetActive(true);
        _onConfirm = onConfirm;
        _onCancel = onCancel;
        _popupPanel.SetActive(true);
    }
}
```

### PartyValidator 유틸

```csharp
public static class PartyValidator
{
    public static bool HasAnyMember()
    {
        return PartyManager.Instance.GetPartyMembers().Any(m => m != null);
    }
    
    public static int GetMemberCount()
    {
        return PartyManager.Instance.GetPartyMembers().Count(m => m != null);
    }
}
```

### 사용 예시 (GuildHouseScene)

```csharp
public void OnBackButtonClick()
{
    if (!PartyValidator.HasAnyMember())
    {
        CommonPopup.Instance.ShowAlert("파티를 편성해야 마을로 돌아갈 수 있습니다.");
        return;
    }
    GameManager.Instance.GoToTown();
}
```

---

## 수락 기준

- [ ]  CommonPopup 싱글톤 구현 및 DontDestroyOnLoad 설정
- [ ]  ShowAlert / ShowConfirm 메서드 동작
- [ ]  GuildHouseScene: 빈 파티 시 뒤로가기 차단 + 팝업
- [ ]  DungeonSelectScene: 빈 파티 시 전투 시작 차단 + 팝업
- [ ]  정상 파티 상태에서는 기존 플로우 유지

---

## 참고

- 길드 하우스 진입 시점에는 이미 초기 용병 2명이 파티에 배치되어 있음
- 사용자가 모두 제거하고 나가려는 경우를 방지하는 목적