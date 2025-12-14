using System;

namespace Data
{
    [Serializable]
    public struct UnitStats
    {
        public float MaxHealth;
        public float AttackDamage;
        public float AttackSpeed;
        public float MoveSpeed;
        public float AttackRange;

        public UnitStats(float maxHealth, float attackDamage, float attackSpeed, float moveSpeed, float attackRange)
        {
            MaxHealth = maxHealth;
            AttackDamage = attackDamage;
            AttackSpeed = attackSpeed;
            MoveSpeed = moveSpeed;
            AttackRange = attackRange;
        }
        
        public static UnitStats Default => new UnitStats(
            maxHealth: 100f,
            attackDamage: 10f,
            attackSpeed: 1f,
            moveSpeed: 3f,
            attackRange: 1.5f
        );

        public static UnitStats FromUnitData(UnitData data, int level)
        {
            return new UnitStats(
                maxHealth: data.GetHealth(level),
                attackDamage: data.GetAttackDamage(level),
                attackSpeed: data.BaseAttackSpeed,
                moveSpeed: data.BaseMoveSpeed,
                attackRange: data.BaseAttackRange
            );
        }
    }
}
