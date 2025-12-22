using System;
using System.Collections;
using Data;
using UnityEngine;

namespace Battle
{
    public class UnitAnimator : MonoBehaviour
    {
        [Header("Hit Flash")]
        [SerializeField] private float _flashDuration = 0.1f;

        private Animator _animator;
        private SpriteRenderer[] _spriteRenderers;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashCoroutine;
        
        private static readonly int _moveTrigger = Animator.StringToHash("1_Move");
        private static readonly int _attackTrigger = Animator.StringToHash("2_Attack");
        private static readonly int _attackBowTrigger = Animator.StringToHash("Attack_Bow");
        private static readonly int _attackStaffTrigger = Animator.StringToHash("Attack_Staff");
        private static readonly int _deathTrigger = Animator.StringToHash("4_Death");
        private static readonly int _isDeath = Animator.StringToHash("isDeath");
        private static readonly int _isJumping = Animator.StringToHash("isJumping");
        private static readonly int _whirlwindTrigger = Animator.StringToHash("Whirlwind");

        private static readonly int _hitEffectBlend = Shader.PropertyToID("_HitEffectBlend");
        private static readonly int _fadeAmount = Shader.PropertyToID("_FadeAmount");

        [Header("Death Fade")]
        [SerializeField] private float _deathFadeDuration = 1.0f;

        private Coroutine _deathFadeCoroutine;

        public event Action OnAttackHit;
        public event Action OnAttackEnd;
        public event Action OnSkillHit;

        public event Action OnDeathComplete;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();

            if (_animator == null)
            {
                Debug.LogError($"[UnitAnimator] Animator not found in children of {gameObject.name}");
                return;
            }
            
            var animatorObject = _animator.gameObject;
            var relay = animatorObject.GetComponent<AnimationEventRelay>();
            if (relay == null)
            {
                relay = animatorObject.AddComponent<AnimationEventRelay>();
            }
            relay.Initialize(this);
            
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void SetMoving(bool isMoving)
        {
            _animator.SetBool(_moveTrigger, isMoving);
        }

        public void SetJumping(bool isJumping)
        {
            _animator.SetBool(_isJumping, isJumping);
        }

        public void PlayWhirlwind()
        {
            _animator.SetTrigger(_whirlwindTrigger);
        }

        public void PlaySkillAnimation(string triggerName)
        {
            if (string.IsNullOrEmpty(triggerName)) return;
            _animator.SetTrigger(triggerName);
        }

        public void PlayAttack(AttackType attackType = AttackType.Melee)
        {
            switch (attackType)
            {
                case AttackType.Melee:
                    _animator.SetTrigger(_attackTrigger);
                    break;
                case AttackType.Bow:
                    _animator.SetTrigger(_attackBowTrigger);
                    break;
                case AttackType.Staff:
                    _animator.SetTrigger(_attackStaffTrigger);
                    break;
            }
        }
        
        private bool _waitingForAttackEnd;

        public void PlayRepeatedAttack(AttackType attackType, int count, float speedMultiplier, Action onComplete)
        {
            StartCoroutine(RepeatedAttackCoroutine(attackType, count, speedMultiplier, onComplete));
        }

        private IEnumerator RepeatedAttackCoroutine(AttackType attackType, int count, float speedMultiplier, Action onComplete)
        {
            float originalSpeed = _animator.speed;
            _animator.speed = speedMultiplier;

            OnAttackEnd += HandleAttackEndForRepeated;

            for (int i = 0; i < count; i++)
            {
                _waitingForAttackEnd = true;
                PlayAttack(attackType);

                while (_waitingForAttackEnd)
                {
                    yield return null;
                }
            }

            OnAttackEnd -= HandleAttackEndForRepeated;

            _animator.speed = originalSpeed;
            onComplete?.Invoke();
        }

        private void HandleAttackEndForRepeated()
        {
            _waitingForAttackEnd = false;
        }

        public void PlayDeath()
        {
            _animator.SetTrigger(_deathTrigger);
            _animator.SetBool(_isDeath, true);
        }
        
        public void AnimEvent_AttackHit()
        {
            OnAttackHit?.Invoke();
        }

        public void AnimEvent_AttackEnd()
        {
            OnAttackEnd?.Invoke();
        }

        public void AnimEvent_SkillHit()
        {
            OnSkillHit?.Invoke();
        }

        public void AnimEvent_DeathComplete()
        {
            OnDeathComplete?.Invoke();
        }

        #region Hit Flash

        public void PlayHitFlash()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }
            _flashCoroutine = StartCoroutine(HitFlashCoroutine());
        }

        public void StopHitFlash()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                _flashCoroutine = null;
            }
            SetHitEffectBlend(0f);
        }

        private IEnumerator HitFlashCoroutine()
        {
            SetHitEffectBlend(1f);
            yield return new WaitForSeconds(_flashDuration);
            SetHitEffectBlend(0f);
            _flashCoroutine = null;
        }

        private void SetHitEffectBlend(float value)
        {
            foreach (var sr in _spriteRenderers)
            {
                if (sr == null) continue;

                sr.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(_hitEffectBlend, value);
                sr.SetPropertyBlock(_propertyBlock);
            }
        }

        #endregion

        #region Death Fade

        public void PlayDeathFade(Action onComplete)
        {
            if (_deathFadeCoroutine != null)
            {
                StopCoroutine(_deathFadeCoroutine);
            }
            _deathFadeCoroutine = StartCoroutine(DeathFadeCoroutine(onComplete));
        }

        private IEnumerator DeathFadeCoroutine(Action onComplete)
        {
            float elapsed = 0f;

            while (elapsed < _deathFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _deathFadeDuration;
                SetFadeAmount(t);
                yield return null;
            }

            SetFadeAmount(1f);
            _deathFadeCoroutine = null;
            onComplete?.Invoke();
        }

        private void SetFadeAmount(float value)
        {
            foreach (var sr in _spriteRenderers)
            {
                if (sr == null) continue;

                sr.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(_fadeAmount, value);
                sr.SetPropertyBlock(_propertyBlock);
            }
        }

        public void ResetFade()
        {
            SetFadeAmount(0f);
        }

        #endregion
    }
}
