using System.Collections.Generic;
using Core;
using Data;
using UnityEngine;

namespace Battle
{
    public static class SkillEffectSpawner
    {
        public static void SpawnEffect(
            BattleUnit caster,
            BattleUnit primaryTarget,
            SkillData skillData,
            List<BattleUnit> effectTargets)
        {
            if (skillData.EffectPrefab == null) return;

            Vector3 spawnPosition = GetEffectPosition(caster, primaryTarget, skillData);

            var effectInstance = PoolManager.Instance.Get(skillData.EffectPrefab);
            effectInstance.transform.position = spawnPosition;
            effectInstance.transform.rotation = Quaternion.identity;

            InitializeVFX(effectInstance, skillData.EffectDuration);
        }

        public static void SpawnEffectOnTargets(
            SkillData skillData,
            List<BattleUnit> targets)
        {
            if (skillData.EffectPrefab == null || targets == null) return;

            foreach (var target in targets)
            {
                if (target == null || target.IsDead) continue;

                var effectInstance = PoolManager.Instance.Get(skillData.EffectPrefab);
                effectInstance.transform.position = target.HitPosition;
                effectInstance.transform.rotation = Quaternion.identity;

                InitializeVFX(effectInstance, skillData.EffectDuration);
            }
        }

        private static void InitializeVFX(GameObject effectInstance, float duration)
        {
            if (effectInstance.TryGetComponent<SkillVFX>(out var vfx))
            {
                vfx.Initialize(duration);
            }
        }

        private static Vector3 GetEffectPosition(BattleUnit caster, BattleUnit primaryTarget, SkillData skillData)
        {
            if (skillData.EffectOrigin == EffectOrigin.Target && primaryTarget != null)
            {
                return primaryTarget.HitPosition;
            }

            return caster.transform.position;
        }
    }
}
