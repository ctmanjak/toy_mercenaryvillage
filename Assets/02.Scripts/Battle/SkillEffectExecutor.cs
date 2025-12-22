using System.Collections.Generic;
using Data;

namespace Battle
{
    public static class SkillEffectExecutor
    {
        public static void Execute(
            BattleUnit caster,
            BattleUnit primaryTarget,
            SkillData skillData,
            List<BattleUnit> effectTargets)
        {
            if (caster == null || skillData == null) return;
            
            effectTargets ??= new List<BattleUnit>();
            
            if (effectTargets.Count > 0)
            {
                float effectValue = skillData.EffectType switch
                {
                    EffectType.Damage or EffectType.DamageAoE => skillData.GetParam(SkillParamKey.Damage),
                    EffectType.Heal or EffectType.HealAoE => skillData.GetParam(SkillParamKey.HealAmount),
                    _ => 0f
                };
                
                ProjectileData projectileData = skillData.ProjectileData;
                if (projectileData == null && skillData.UseUnitProjectile)
                {
                    projectileData = caster.ProjectileData;
                }
                
                caster.RegisterPendingSkillEffect(skillData.EffectType, effectValue, effectTargets, skillData.HitCount, projectileData);
            }
            
            skillData.CustomBehavior?.Execute(caster, primaryTarget, skillData, effectTargets);

            SpawnSkillEffect(caster, primaryTarget, skillData, effectTargets);
        }

        private static void SpawnSkillEffect(
            BattleUnit caster,
            BattleUnit primaryTarget,
            SkillData skillData,
            List<BattleUnit> effectTargets)
        {
            if (skillData.EffectPrefab == null) return;

            bool isAoE = skillData.EffectType == EffectType.DamageAoE ||
                         skillData.EffectType == EffectType.HealAoE ||
                         skillData.EffectRadius > 0f;

            if (isAoE)
            {
                SkillEffectSpawner.SpawnEffect(caster, primaryTarget, skillData, effectTargets);
            }
            else
            {
                SkillEffectSpawner.SpawnEffectOnTargets(skillData, effectTargets);
            }
        }
    }
}
