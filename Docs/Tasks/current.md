## 목표

전투 시 간단한 시각적 피드백을 추가한다.

---

## 산출물

- 사망 페이드 이펙트 (All In 1 Sprite Shader 활용)

---

## 구현 내용

### 공격 이펙트

- 근접 공격: 스킵 (애니메이션만 사용)
- 원거리 공격: 기존 ProjectileManager 투사체 시스템 활용

### 사망 이펙트

- All In 1 Sprite Shader의 `_FadeAmount` 프로퍼티 활용
- Death 애니메이션 완료 후 페이드아웃 시작
- 지속 시간: 1.0초 (Inspector에서 조정 가능)

---

## 구현 상세

### UnitAnimator.cs

```csharp
// Death Fade 관련 필드
private static readonly int _fadeAmount = Shader.PropertyToID("_FadeAmount");
[SerializeField] private float _deathFadeDuration = 1.0f;

// 페이드아웃 실행
public void PlayDeathFade(Action onComplete)
{
    StartCoroutine(DeathFadeCoroutine(onComplete));
}

private IEnumerator DeathFadeCoroutine(Action onComplete)
{
    float elapsed = 0f;
    while (elapsed < _deathFadeDuration)
    {
        elapsed += Time.deltaTime;
        SetFadeAmount(elapsed / _deathFadeDuration);
        yield return null;
    }
    onComplete?.Invoke();
}
```

### BattleUnit.cs

```csharp
// 애니메이션 완료 후 페이드아웃 시작
private void HandleDeathComplete()
{
    _unitAnimator.PlayDeathFade(() =>
    {
        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    });
}

// 유닛 재사용 시 페이드 초기화
public void Initialize(UnitStats stats, Team team)
{
    // ...
    _unitAnimator.ResetFade();
}
```

---

## 수락 기준

- [x]  원거리 공격 시 투사체 이펙트 재생
- [x]  사망 시 페이드아웃
- [x]  유닛 재사용 시 페이드 초기화
- [x]  이펙트가 전투 성능에 영향 없음