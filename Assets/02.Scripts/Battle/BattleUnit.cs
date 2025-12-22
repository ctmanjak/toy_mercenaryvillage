using System;
using System.Collections.Generic;
using Data;
using UI;
using UnityEngine;

namespace Battle
{
    public enum UnitState { Idle, Move, Attack, Dead }
    public enum Team { Ally, Enemy }

    public struct PendingSkillEffect
    {
        public EffectType EffectType;
        public float Value;
        public List<BattleUnit> Targets;
        public int RemainingHits;
        public ProjectileData ProjectileData;
        public Vector3 FirePosition;

        public bool IsValid => Targets != null && Targets.Count > 0 && RemainingHits > 0;
        public bool HasProjectile => ProjectileData != null;

        public void Clear()
        {
            EffectType = EffectType.None;
            Value = 0f;
            Targets = null;
            RemainingHits = 0;
            ProjectileData = null;
            FirePosition = Vector3.zero;
        }
    }
    
    [RequireComponent(typeof(UnitMovement))]
    [RequireComponent(typeof(UnitCombat))]
    [RequireComponent(typeof(UnitAnimator))]
    public class BattleUnit : MonoBehaviour
    {
        [Header("Status")]
        [SerializeField] private Team _team;
        [SerializeField] private UnitState _state = UnitState.Idle;

        [Header("Stats")]
        [SerializeField] private UnitStats _stats;
        [SerializeField] private float _health;

        [Header("Combat")]
        [SerializeField] private BattleUnit _currentTarget;
        [SerializeField] private ProjectileData _projectileData;
        [SerializeField] private AttackType _attackType;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Transform _hitPoint;

        [Header("Visual")]
        [SerializeField] private Transform _shadow;
        [SerializeField] private Transform _spriteRoot;

        [Header("Skills")]
        [SerializeField] private List<SkillData> _skills = new List<SkillData>();
        [SerializeField] private List<SkillInstance> _skillInstances = new List<SkillInstance>();
        private bool _isSkillActivating;
        private PendingSkillEffect _pendingSkillEffect;

        private UnitMovement _movement;
        private UnitCombat _combat;
        private UnitAnimator _unitAnimator;
        private UnitHealthBar _healthBar;

        #region Properties
        public Team Team => _team;
        public UnitState State => _state;
        public UnitStats Stats => _stats;
        public float MaxHealth => _stats.MaxHealth;
        public float Health => _health;
        public float AttackDamage => _stats.AttackDamage;
        public float AttackSpeed => _stats.AttackSpeed;
        public float MoveSpeed => _stats.MoveSpeed;
        public float AttackRange => _stats.AttackRange;
        public BattleUnit CurrentTarget => _currentTarget;
        public ProjectileData ProjectileData => _projectileData;
        public AttackType AttackType => _attackType;
        public bool IsRanged => _projectileData != null;
        public Vector3 FirePosition => _firePoint != null ? _firePoint.position : transform.position;
        public Vector3 HitPosition => _hitPoint != null ? _hitPoint.position : transform.position;
        public bool IsDead => _state == UnitState.Dead;
        public bool IsAlive => !IsDead;
        public float HealthRatio => _stats.MaxHealth > 0 ? _health / _stats.MaxHealth : 0f;
        public IReadOnlyList<SkillData> Skills => _skills;
        public IReadOnlyList<SkillInstance> SkillInstances => _skillInstances;
        public bool IsSkillActivating => _isSkillActivating;
        #endregion

        public event Action<BattleUnit> OnDeath;

        private void Awake()
        {
            _movement = GetComponent<UnitMovement>();
            _combat = GetComponent<UnitCombat>();
            _unitAnimator = GetComponent<UnitAnimator>();

            _unitAnimator.OnDeathComplete += HandleDeathComplete;
            _unitAnimator.OnSkillHit += HandleSkillHit;
            _unitAnimator.OnAttackHit += HandleAttackHitForSkill;
        }

        private void HandleAttackHitForSkill()
        {
            if (_pendingSkillEffect.IsValid)
            {
                HandleSkillHit();
            }
        }

        public UnitAnimator UnitAnimator => _unitAnimator;

        public void Initialize(UnitStats stats, Team team)
        {
            _team = team;
            _stats = stats;
            _health = stats.MaxHealth;

            _state = UnitState.Idle;
            _currentTarget = null;

            _unitAnimator.ResetFade();
            _healthBar?.Initialize(this);
        }

        public void SetHealthBar(UnitHealthBar healthBar)
        {
            _healthBar = healthBar;
            _healthBar?.Initialize(this);
        }

        public void Initialize(UnitData data, int level, Team team)
        {
            var stats = UnitStats.FromUnitData(data, level);
            _projectileData = data.ProjectileData;
            _attackType = data.AttackType;
            SetSkills(data.Skills);
            Initialize(stats, team);
        }

        public void SetSkills(List<SkillData> skills)
        {
            _skills.Clear();
            _skillInstances.Clear();

            if (skills != null)
            {
                foreach (var skillData in skills)
                {
                    if (skillData != null)
                    {
                        _skills.Add(skillData);
                        _skillInstances.Add(new SkillInstance(skillData));
                    }
                }
            }
        }
        
        public void TakeDamage(float damage)
        {
            if (IsDead) return;

            _health -= damage;
            _healthBar?.UpdateHealthBar();
            _unitAnimator.PlayHitFlash();

            DamagePopupSpawner.Instance?.Show(transform.position, (int)damage, _team == Team.Ally);

            if (_health <= 0f)
            {
                _health = 0f;
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;

            float prevHealth = _health;
            _health = Mathf.Min(_health + amount, MaxHealth);
            float actualHeal = _health - prevHealth;

            if (actualHeal > 0f)
            {
                _healthBar?.UpdateHealthBar();
                DamagePopupSpawner.Instance?.ShowHeal(transform.position, (int)actualHeal);
            }
        }

        public void SetState(UnitState newState)
        {
            if (IsDead && newState != UnitState.Dead) return;

            var prevState = _state;
            _state = newState;

            if (prevState != newState)
            {
                _unitAnimator.SetMoving(newState == UnitState.Move);
            }
        }
        
        public void SetTarget(BattleUnit target)
        {
            _currentTarget = target;
        }

        public void UpdateAI()
        {
            if (IsDead) return;

            UpdateSkillCooldowns(Time.deltaTime);
            TryActivateSkills();

            UpdateTarget();

            if (_currentTarget == null || _currentTarget.IsDead)
            {
                SetState(UnitState.Idle);
                return;
            }

            if (_isSkillActivating) return;

            switch (_state)
            {
                case UnitState.Idle:
                case UnitState.Move:
                    _movement.UpdateMovement(_currentTarget);
                    break;
                case UnitState.Attack:
                    _combat.UpdateCombat(_currentTarget);
                    break;
            }
        }

        private void UpdateSkillCooldowns(float deltaTime)
        {
            foreach (var skillInstance in _skillInstances)
            {
                skillInstance.UpdateCooldown(deltaTime);
            }
        }

        private void TryActivateSkills()
        {
            if (_isSkillActivating) return;
            if (_combat.IsAttacking) return;

            foreach (var skillInstance in _skillInstances)
            {
                if (skillInstance.CheckTriggerCondition(this))
                {
                    ActivateSkill(skillInstance);
                    break;
                }
            }
        }
        
        public void TriggerBattleStartSkills()
        {
            foreach (var skillInstance in _skillInstances)
            {
                if (skillInstance.Data.TriggerType == TriggerType.BattleStart && skillInstance.IsReady)
                {
                    Debug.Log($"[BattleUnit] {gameObject.name} triggers BattleStart skill: {skillInstance.Data.SkillName}");
                    ActivateSkill(skillInstance);
                    break;
                }
            }
        }

        private void ActivateSkill(SkillInstance skillInstance)
        {
            _isSkillActivating = true;
            _combat.CancelAttack();

            var skillData = skillInstance.Data;
            
            var primaryTarget = SkillTargetSelector.SelectTarget(this, skillData);

            if (primaryTarget == null)
            {
                Debug.Log($"[BattleUnit] {gameObject.name} skill {skillData.SkillName}: no valid target");
                OnSkillComplete();
                return;
            }
            
            bool targetEnemies = IsEnemyTargetingEffect(skillData.EffectType);
            var effectTargets = SkillTargetSelector.FindEffectTargets(this, primaryTarget, skillData, targetEnemies);

            Debug.Log($"[BattleUnit] {gameObject.name} activates {skillData.SkillName} on {effectTargets.Count} target(s)");

            SkillEffectExecutor.Execute(this, primaryTarget, skillData, effectTargets);

            skillInstance.ResetCooldown();

            bool handlesCompletion = skillData.CustomBehavior != null && skillData.CustomBehavior.HandlesCompletion;
            if (!handlesCompletion)
            {
                OnSkillComplete();
            }
        }

        private bool IsEnemyTargetingEffect(EffectType effectType)
        {
            return effectType switch
            {
                EffectType.Damage => true,
                EffectType.DamageAoE => true,
                EffectType.DebuffAtk => true,
                EffectType.Stun => true,
                EffectType.Heal => false,
                EffectType.HealAoE => false,
                EffectType.BuffAtk => false,
                EffectType.Shield => false,
                EffectType.None => true,
                _ => true
            };
        }

        public void OnSkillComplete()
        {
            _isSkillActivating = false;
        }

        public void RegisterPendingSkillEffect(EffectType effectType, float value, List<BattleUnit> targets, int hitCount = 1, ProjectileData projectileData = null)
        {
            _pendingSkillEffect = new PendingSkillEffect
            {
                EffectType = effectType,
                Value = value,
                Targets = targets,
                RemainingHits = hitCount,
                ProjectileData = projectileData,
                FirePosition = FirePosition
            };
        }

        private void HandleSkillHit()
        {
            if (!_pendingSkillEffect.IsValid) return;

            foreach (var target in _pendingSkillEffect.Targets)
            {
                if (target == null || !target.IsAlive) continue;

                switch (_pendingSkillEffect.EffectType)
                {
                    case EffectType.Damage:
                    case EffectType.DamageAoE:
                        if (_pendingSkillEffect.HasProjectile)
                        {
                            ProjectileManager.Instance.FireProjectile(
                                _pendingSkillEffect.FirePosition,
                                target,
                                _pendingSkillEffect.ProjectileData,
                                _pendingSkillEffect.Value
                            );
                        }
                        else
                        {
                            target.TakeDamage(_pendingSkillEffect.Value);
                        }
                        break;
                    case EffectType.Heal:
                    case EffectType.HealAoE:
                        target.Heal(_pendingSkillEffect.Value);
                        break;
                }
            }

            _pendingSkillEffect.RemainingHits--;
            if (_pendingSkillEffect.RemainingHits <= 0)
            {
                _pendingSkillEffect.Clear();
            }
        }

        private void Die()
        {
            _state = UnitState.Dead;
            _currentTarget = null;

            _healthBar?.Hide();
            _unitAnimator.StopHitFlash();
            _unitAnimator.PlayDeath();
        }

        private void HandleDeathComplete()
        {
            _unitAnimator.PlayDeathFade(() =>
            {
                OnDeath?.Invoke(this);
                gameObject.SetActive(false);
            });
        }
        
        public void UpdateTarget()
        {
            if (_currentTarget == null || _currentTarget.IsDead)
            {
                _currentTarget = FindClosestEnemy();
            }
        }

        private BattleUnit FindClosestEnemy()
        {
            var enemies = BattleManager.Instance.GetEnemiesOf(_team);

            BattleUnit closest = null;
            float closestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                if (enemy.IsDead) continue;

                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = enemy;
                }
            }

            return closest;
        }
        
        public void Leap(BattleUnit target, float stopDistance, float duration, float jumpHeight, LeapSettings settings, Action onComplete = null)
        {
            if (target == null || IsDead)
            {
                onComplete?.Invoke();
                return;
            }

            LeapAnimator.Play(
                root: transform,
                visualRoot: _spriteRoot,
                shadow: _shadow,
                targetPos: target.transform.position,
                stopDistance: stopDistance,
                duration: duration,
                jumpHeight: jumpHeight,
                settings: settings,
                unitAnimator: _unitAnimator,
                onComplete: () =>
                {
                    Debug.Log($"[BattleUnit] {gameObject.name} completed leap to {target.gameObject.name}");
                    onComplete?.Invoke();
                }
            );
        }

#if UNITY_EDITOR
        [ContextMenu("Initialize with Default Stats")]
        private void InitializeWithDefaultStats()
        {
            Initialize(UnitStats.Default, Team.Ally);
        }
#endif
    }
}
