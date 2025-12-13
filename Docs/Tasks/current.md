## 목표

유닛이 타겟 방향으로 직선 이동하는 로직을 구현한다. v0.1에서는 복잡한 길찾기 없이 단순 직선 이동.

---

## 산출물

- `Assets/02.Scripts/Battle/UnitMovement.cs`에 이동 로직 추가

---

## 요구사항

### 이동 조건

- 타겟이 있고, 사거리 밖일 때만 이동
- 사거리 안에 들어오면 이동 중단 → Attack 상태로 전환
- 타겟 없으면 Idle 상태로 대기

### 이동 방식

- `Transform.position` 직접 변경 (Rigidbody 미사용)
- `MoveSpeed * Time.deltaTime` 으로 프레임 독립적 이동
- 타겟 방향으로 단순 직선 이동

### 거리 계산

```csharp
float distance = Vector2.Distance(transform.position, target.position);
bool inRange = distance <= attackRange;
```

---

## 설계 가이드

### 상태 전환 흐름

```
Idle (타겟 없음)
  ↓ 타겟 발견
Move (사거리 밖)
  ↓ 사거리 진입
Attack
```

### v0.1 단순화

- 충돌 처리: 검침 허용 (나중에 밀어내기 추가)
- 회전: 없음 (2D 탑다운이라 스프라이트 flip만 고려)

---

## 수락 기준

- [ ]  Move 상태에서 타겟 방향으로 이동
- [ ]  MoveSpeed 스탯에 따라 속도 변화 확인
- [ ]  사거리 진입 시 이동 중단
- [ ]  타겟 없을 때 Idle 상태 유지
- [ ]  테스트: 2개 유닛이 서로 접근하는지 확인

---

## 참고 코드 스니펫

```csharp
// BattleUnit.cs 내부 또는 별도 컴포넌트
private void UpdateMovement()
{
    if (currentTarget == null || currentTarget.IsDead)
    {
        state = UnitState.Idle;
        return;
    }
    
    float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
    
    if (distance <= attackRange)
    {
        state = UnitState.Attack;
        return;
    }
    
    // 타겟 방향으로 이동
    state = UnitState.Move;
    Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
    transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
}
```

---

## 후속 태스크 연결

이 태스크 완료 후 진행 가능:

- [S1] 공격 로직
- [S2] 검침 처리 최소 구현
- [S2] 사거리 경계 떨림 방지