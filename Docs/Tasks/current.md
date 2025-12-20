## 목표

원거리 유닛(궁수/메이지)의 공격이 시각적으로 보이도록 투사체 시스템을 구현한다.

## 산출물

- ProjectileData (ScriptableObject)
- Projectile 프리팹 (화살/마법탄)
- ProjectileManager (오브젝트 풀링 연동)
- BattleUnit 원거리 공격 로직 수정

## ProjectileData 구조

```
ProjectileData (ScriptableObject)
├─ MovementType: Linear / Parabolic
├─ Speed (float)
├─ ArcHeight (float, Parabolic일 때 포물선 높이)
├─ RotateToDirection (bool, 진행 방향으로 회전)
├─ Sprite
└─ ImpactEffect (선택)
```

## MovementType별 동작

| 타입 | 궤적 | 사용 | 특징 |
| --- | --- | --- | --- |
| Linear | 직선 | 메이지 | 타겟 방향 직진 |
| Parabolic | 포물선 | 궁수 | 위로 올라갔다 떨어짐, 회전 |

## 수락 기준

- 화살이 포물선으로 날아감
- 마법탄이 직선으로 날아감
- 투사체가 타겟에 도달하면 데미지 적용 후 사라짐
- 타겟이 먼저 죽으면 투사체 자동 소멸