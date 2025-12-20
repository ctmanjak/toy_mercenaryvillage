using UnityEngine;

namespace Battle
{
    [RequireComponent(typeof(BattleUnit))]
    public class UnitCombat : MonoBehaviour
    {
        private BattleUnit _unit;
        private float _attackTimer;
        private bool _isAttacking;
        private BattleUnit _attackTarget;

        private void Awake()
        {
            _unit = GetComponent<BattleUnit>();
        }

        private void Start()
        {
            _unit.UnitAnimator.OnAttackHit += HandleAttackHit;
        }

        public void UpdateCombat(BattleUnit target)
        {
            if (target == null || target.IsDead) return;
            if (_unit.IsDead) return;

            if (_isAttacking) return;

            if (!CanAttack(target))
            {
                _unit.SetState(UnitState.Move);
                return;
            }

            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0f)
            {
                PerformAttack(target);
                _attackTimer = _unit.AttackSpeed;
            }
        }

        public bool CanAttack(BattleUnit target)
        {
            if (target == null || target.IsDead) return false;

            float distance = Vector2.Distance(transform.position, target.transform.position);
            return distance <= _unit.AttackRange;
        }

        public bool TryAttack(BattleUnit target)
        {
            if (!CanAttack(target)) return false;
            if (_attackTimer > 0f) return false;

            PerformAttack(target);
            _attackTimer = _unit.AttackSpeed;
            return true;
        }

        private void PerformAttack(BattleUnit target)
        {
            _isAttacking = true;
            _attackTarget = target;
            _unit.UnitAnimator.PlayAttack();
        }

        private void HandleAttackHit()
        {
            if (_attackTarget != null && _attackTarget.IsAlive)
            {
                _attackTarget.TakeDamage(_unit.AttackDamage);
            }

            _isAttacking = false;
            _attackTarget = null;
        }

        public void ResetAttackTimer()
        {
            _attackTimer = 0f;
        }
    }
}
