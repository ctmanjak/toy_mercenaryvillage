using Core;
using Data;
using UnityEngine;

namespace Battle
{
    public class ProjectileManager : MonoBehaviour
    {
        public static ProjectileManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void FireProjectile(Vector3 startPosition, BattleUnit target, ProjectileData data, float damage)
        {
            if (target == null || target.IsDead) return;
            if (data == null || data.Prefab == null) return;

            var projectileObj = PoolManager.Instance.Get(data.Prefab);
            projectileObj.transform.position = startPosition;

            var projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(data, target, damage);
        }
    }
}
