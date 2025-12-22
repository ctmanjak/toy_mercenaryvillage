using UnityEngine;

namespace Battle
{
    [RequireComponent(typeof(BattleUnit))]
    public class UnitMovement : MonoBehaviour
    {
        private BattleUnit _unit;
        private Transform _transform;

        private void Awake()
        {
            _unit = GetComponent<BattleUnit>();
            _transform = transform;
        }

        public void UpdateMovement(BattleUnit target)
        {
            if (target == null || target.IsDead)
            {
                _unit.SetState(UnitState.Idle);
                return;
            }

            float distance = Vector2.Distance(_transform.position, target.transform.position);
            float attackRange = _unit.AttackRange;

            if (distance <= attackRange)
            {
                _unit.SetState(UnitState.Attack);
                return;
            }

            _unit.SetState(UnitState.Move);
            _unit.FaceTarget(target);
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)_transform.position).normalized;
            _transform.position += (Vector3)(direction * _unit.MoveSpeed * Time.deltaTime);
        }
    }
}
