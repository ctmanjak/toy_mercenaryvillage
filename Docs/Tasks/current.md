## 목표

원정 내 연속 전투를 관리하는 ExpeditionManager를 구현한다.

---

## 산출물

- ExpeditionManager.cs
- 기존 BattleManager와 연동

---

## 요구사항

### 핵심 로직

- 현재 원정 데이터 보관
- 현재 전투 인덱스 추적
- 누적 골드 계산
- 전투 결과 처리 (BattleManager 콜백)

### 주요 메서드

- StartExpedition(ExpeditionData): 원정 시작
- LoadNextBattle(): 다음 전투 로드
- OnBattleEnd(victory, gold): 전투 종료 처리
- ReturnToTown(): 중도 귀환

### 전투 종료 처리

- 승리 시: 골드 누적 → 마지막 전투면 결과화면, 아니면 계속/귀환 선택
- 패배 시: 원정 중단 → 결과 화면 (누적 골드 유지)

---

## 설계 가이드

### 상태 관리

- currentExpedition: ExpeditionData
- currentBattleIndex: int
- totalGoldEarned: int
- isExpeditionActive: bool

### BattleManager 연동

- BattleManager.OnBattleEnd 이벤트 구독
- StageData는 ExpeditionData.battles[index]에서 가져옴

---

## 수락 기준

- 원정 시작 시 첫 전투 로드
- 전투 승리 시 골드 누적
- 마지막 전투 승리 시 원정 완료 처리
- 전투 패배 시 원정 중단 (골드 유지)
- BattleManager와 정상 연동