using System;
using Data;
using UI;
using UnityEngine;

namespace Battle
{
    public enum UnitState { Idle, Move, Attack, Dead }
    public enum Team { Ally, Enemy }
    
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
        #endregion

        public event Action<BattleUnit> OnDeath;

        private void Awake()
        {
            _movement = GetComponent<UnitMovement>();
            _combat = GetComponent<UnitCombat>();
            _unitAnimator = GetComponent<UnitAnimator>();

            _unitAnimator.OnDeathComplete += HandleDeathComplete;
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
            Initialize(stats, team);
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

            UpdateTarget();

            if (_currentTarget == null || _currentTarget.IsDead)
            {
                SetState(UnitState.Idle);
                return;
            }

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

#if UNITY_EDITOR
        [ContextMenu("Initialize with Default Stats")]
        private void InitializeWithDefaultStats()
        {
            Initialize(UnitStats.Default, Team.Ally);
        }
#endif
    }
}
