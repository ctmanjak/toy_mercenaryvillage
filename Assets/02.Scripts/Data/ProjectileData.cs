using UnityEngine;

namespace Data
{
    public enum ProjectileMovementType
    {
        Linear,
        Parabolic
    }

    [CreateAssetMenu(fileName = "Projectile_", menuName = "Game/Projectile Data")]
    public class ProjectileData : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("이동 타입")]
        public ProjectileMovementType MovementType = ProjectileMovementType.Linear;

        [Tooltip("이동 속도")]
        public float Speed = 10f;

        [Tooltip("포물선 강도 (Parabolic 전용) - 값이 클수록 더 높은 포물선")]
        public float ArcIntensity = 2f;

        [Tooltip("진행 방향으로 회전")]
        public bool RotateToDirection = true;

        [Header("Visual")]
        [Tooltip("투사체 프리팹")]
        public GameObject Prefab;

        [Tooltip("충돌 이펙트 프리팹 (선택)")]
        public GameObject ImpactEffect;
    }
}
