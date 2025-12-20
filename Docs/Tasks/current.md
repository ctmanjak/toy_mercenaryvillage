## 선행 태스크

- [object_pool.md](object_pool.md) - 오브젝트 풀링 시스템 (필수)

---

## 목표

공격 시 데미지 숫자를 표시하여 전투 피드백을 강화한다.

---

## 산출물

- `DamagePopup.cs`
- `DamagePopupSpawner.cs`
- `DamagePopup.prefab`

---

## 요구사항

### 표시 요소

- 데미지 숫자 (ex: "25")
- 색상 구분: 아군 피해(빨강), 적군 피해(흰색)
- 크리티컬 히트 표시 (확장용, v0.1에서는 제외)

### 애니메이션

- 위로 떠오르며 페이드아웃
- 지속 시간: 0.5~0.8초
- 약간의 랜덤 오프셋 (겨침 방지)

---

## 설계 가이드

### DamagePopup

```csharp
public class DamagePopup : MonoBehaviour, IPoolable
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _duration = 0.6f;
    [SerializeField] private float _floatSpeed = 1f;

    public void Setup(int damage, bool isAllyDamage)
    {
        _text.text = damage.ToString();
        _text.color = isAllyDamage ? Color.red : Color.white;
        StartCoroutine(AnimateAndReturn());
    }

    private IEnumerator AnimateAndReturn()
    {
        float elapsed = 0;
        Vector3 startPos = transform.position;
        Color startColor = _text.color;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _duration;

            transform.position = startPos + Vector3.up * (_floatSpeed * t);
            _text.color = new Color(startColor.r, startColor.g, startColor.b, 1 - t);

            yield return null;
        }
        PoolManager.Instance.Release(this); // 풀에 반환
    }

    public void OnGetFromPool() { }

    public void OnReleaseToPool()
    {
        StopAllCoroutines();
    }
}
```

### 호출 위치 (BattleUnit.TakeDamage)

```csharp
public void TakeDamage(float damage)
{
    _currentHealth -= damage;
    // isAllyDamage: 피해를 받는 쪽이 아군이면 true (빨간색 표시)
    DamagePopupSpawner.Instance.Show(transform.position, (int)damage, team == Team.Ally);
    // ...
}
```

### DamagePopupSpawner (PoolManager 활용)

```csharp
public class DamagePopupSpawner : MonoBehaviour
{
    public static DamagePopupSpawner Instance { get; private set; }

    [SerializeField] private DamagePopup _prefab;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(Vector3 position, int damage, bool isAllyDamage)
    {
        var popup = PoolManager.Instance.Get(_prefab);
        popup.transform.position = position + GetRandomOffset();
        popup.Setup(damage, isAllyDamage);
    }

    private Vector3 GetRandomOffset()
    {
        return new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0f, 0.3f), 0);
    }
}
```

---

## 수락 기준

- [ ] 공격 시 데미지 숫자 표시
- [ ] 아군/적군 피해 색상 구분
- [ ] 위로 떠오르며 페이드아웃
- [ ] 여러 데미지 동시 표시 시 겹침 없음
- [ ] PoolManager를 통한 생성/반환 (Destroy 사용 금지)

---

## 후속 태스크 연결

- [S4] 공격 이펙트 (확장용)