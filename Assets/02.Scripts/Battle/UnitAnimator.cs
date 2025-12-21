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
        
        private static readonly int _isMoving = Animator.StringToHash("1_Move");
        private static readonly int _attackTrigger = Animator.StringToHash("2_Attack");
        private static readonly int _attackBowTrigger = Animator.StringToHash("Attack_Bow");
        private static readonly int _attackStaffTrigger = Animator.StringToHash("Attack_Staff");
        private static readonly int _deathTrigger = Animator.StringToHash("4_Death");
        
        private static readonly int _hitEffectBlend = Shader.PropertyToID("_HitEffectBlend");

        public event Action OnAttackHit;

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
            
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void SetMoving(bool isMoving)
        {
            _animator.SetBool(_isMoving, isMoving);
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

        public void PlayDeath()
        {
            _animator.SetTrigger(_deathTrigger);
        }
        
        public void AnimEvent_AttackHit()
        {
            OnAttackHit?.Invoke();
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
    }
}
