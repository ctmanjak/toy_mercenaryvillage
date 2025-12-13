# 용병단 마을 (Mercenary Village)

## 프로젝트 개요

- **장르**: 2D 오토배틀러 + 타운빌딩
- **플랫폼**: PC

## 참고 문서

- `Docs/game_concept.md` - 게임 디자인 전체
- `Docs/dev_checklist.md` - 구현 항목 상세

## 코딩 컨벤션

### 네이밍
- `PascalCase`: 클래스, 메서드, 프로퍼티
- `camelCase`: 지역변수, 매개변수
- `_camelCase`: private 필드
- `I` 접두어: 인터페이스

### 코드 품질
- 클래스는 하나의 역할만 (단일 책임)
- 깊은 체이닝 피하기 (`a.b.c.d` 금지)
- 구체 클래스보다 인터페이스 의존 (필요 시)
- 과도한 추상화보다 작동하는 코드 우선

### Unity 규칙
- `Awake`: 자기 자신 초기화
- `Start`: 다른 객체 참조, 초기 로직
- `Update`: 최소한으로, 무거운 로직 금지
- `[SerializeField]` private 필드 노출 권장 (public 필드 지양)

## 금지 사항

- `GameObject.Find()`, `FindObjectOfType()` 런타임 남용
- 문자열 경로 하드코딩 (Resources.Load 등)
- Update에서 매 프레임 GetComponent
- 매직넘버 직접 사용

## 작업 원칙

1. **기획서 우선**: 구현 전 관련 기획서 확인
2. **MVP 범위 준수**: 현재 마일스톤에 없는 기능은 구현하지 않음
3. **점진적 구현**: 작동하는 최소 버전 먼저, 이후 개선
4. **확장성 고려**: 하드코딩 피하고 데이터 기반 설계