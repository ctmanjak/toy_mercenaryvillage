using Core;
using Data;
using UnityEngine;

namespace Battle
{
    public class Projectile : MonoBehaviour, IPoolable
    {
        private ProjectileData _data;
        private BattleUnit _target;
        private float _damage;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _travelTime;
        private float _elapsedTime;
        private float _arcHeight;
        private bool _isActive;

        public void Initialize(ProjectileData data, BattleUnit target, float damage)
        {
            _data = data;
            _target = target;
            _damage = damage;

            _startPosition = transform.position;
            _targetPosition = target.HitPosition;

            float distance = Vector2.Distance(_startPosition, _targetPosition);
            
            float estimatedTime = distance / _data.Speed;
            
            _arcHeight = _data.ArcIntensity * estimatedTime * estimatedTime;
            
            float pathLength = CalculatePathLength(distance);
            _travelTime = pathLength / _data.Speed;

            _elapsedTime = 0f;
            _isActive = true;

            if (_data.RotateToDirection)
            {
                RotateTowardsTarget();
            }
        }

        private float CalculatePathLength(float linearDistance)
        {
            if (_data.MovementType != ProjectileMovementType.Parabolic)
            {
                return linearDistance;
            }
            
            const int segments = 10;
            float length = 0f;
            Vector3 prevPos = _startPosition;

            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 pos = CalculatePositionInternal(t);
                length += Vector3.Distance(prevPos, pos);
                prevPos = pos;
            }

            return length;
        }

        private Vector3 CalculatePositionInternal(float t)
        {
            Vector3 linearPosition = Vector3.Lerp(_startPosition, _targetPosition, t);

            if (_data.MovementType == ProjectileMovementType.Parabolic)
            {
                float arcOffset = _arcHeight * 4f * t * (1f - t);
                linearPosition.y += arcOffset;
            }

            return linearPosition;
        }

        private void Update()
        {
            if (!_isActive) return;

            if (_target == null || _target.IsDead)
            {
                Release();
                return;
            }

            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / _travelTime);

            Vector3 newPosition = CalculatePosition(t);
            
            if (_data.RotateToDirection)
            {
                Vector3 direction = newPosition - transform.position;
                if (direction.sqrMagnitude > 0.0001f)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }

            transform.position = newPosition;

            if (t >= 1f)
            {
                OnHitTarget();
            }
        }

        private Vector3 CalculatePosition(float t)
        {
            return CalculatePositionInternal(t);
        }

        private void RotateTowardsTarget()
        {
            Vector2 direction = (_targetPosition - _startPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void OnHitTarget()
        {
            if (_target != null && _target.IsAlive)
            {
                _target.TakeDamage(_damage);
            }

            if (_data.ImpactEffect != null)
            {
                var effect = PoolManager.Instance.Get(_data.ImpactEffect);
                effect.transform.position = transform.position;
            }

            Release();
        }

        private void Release()
        {
            _isActive = false;
            PoolManager.Instance.Release(gameObject);
        }

        public void OnGetFromPool()
        {
            _isActive = false;
            _target = null;
            _data = null;
        }

        public void OnReleaseToPool()
        {
            _isActive = false;
            _target = null;
        }
    }
}
