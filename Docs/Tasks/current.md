## 목표

원정 전투 결과(승리/패배)를 표시하고, 마지막 전투 완료 시 보상 내역을 보여주는 UI를 구현한다.

---

## 산출물

- BattleResultUI.cs (기존 파일 수정)
- ExpeditionManager.cs (첫 완료 보너스 지급 로직 추가)

---

## 요구사항

### UI 구성

#### 일반 전투 승리 시
- "전투 승리!" 메시지
- 획득 골드 표시 (ex: +50G)
- 현재 진행도 (ex: 3/5 전투)
- 누적 보상 표시 (ex: 총 145G)
- "다음 전투" 버튼
- "귀환" 버튼

#### 마지막 전투 승리 시 (원정 완료)
- "원정 완료!" 메시지
- 진행도 (ex: 5/5 전투 완료)
- 보상 내역:
    - 전투 보상: NNG
    - 완료 보너스: NNG
    - 첫 완료 보너스: NNG (해당 시)
    - 총 획득: NNG
- "원정 완료" 버튼

#### 패배 시
- "패배..." 메시지
- "마을로" 버튼

### 동작

#### 이벤트 구독 (Event Pattern)
- `ExpeditionManager.OnBattleVictory` → `ShowVictoryResult()`
- `ExpeditionManager.OnBattleDefeat` → `ShowDefeatResult()`

#### 버튼 동작
- 다음 전투: `ExpeditionManager.LoadNextBattle()` 호출
- 귀환: `ExpeditionManager.ReturnToTown()` 호출
- 원정 완료: `ExpeditionManager.CompleteExpedition()` 호출
    - 완료 보너스 지급
    - 첫 완료면 첫 완료 보너스 지급
    - 완료 횟수 증가
- 마을로 (패배): `ExpeditionManager.ReturnToTown()` 호출

---

## 수락 기준

- 전투 승리 후 결과 UI 표시
- 전투 패배 후 결과 UI 표시
- 진행도 정확히 표시
- 획득 골드 표시
- "다음 전투" 버튼 정상 동작
- "귀환" 버튼 정상 동작
- 마지막 전투 시 보상 내역 표시
- 완료 보너스 정상 지급
- 첫 완료 보너스 정상 지급 (1회성)
- 패배 시 "마을로" 버튼 정상 동작
