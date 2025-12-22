using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Battle
{
    public static class SkillTargetSelector
    {
        public static BattleUnit SelectTarget(BattleUnit caster, SkillData skillData)
        {
            if (caster == null || skillData == null) return null;

            return skillData.TargetType switch
            {
                TargetType.NearestEnemy => FindNearestEnemy(caster, skillData.Range),
                TargetType.LowestHpEnemy => FindLowestHpEnemy(caster, skillData.Range),
                TargetType.LowestHpPercentageEnemy => FindLowestHpPercentageEnemy(caster, skillData.Range),
                TargetType.Self => caster,
                TargetType.LowestHpAlly => FindLowestHpAlly(caster, skillData.Range),
                TargetType.LowestHpPercentageAlly => FindLowestHpPercentageAlly(caster, skillData.Range),
                TargetType.AllEnemiesInRange => FindNearestEnemy(caster, skillData.Range),
                TargetType.AllAlliesInRange => FindLowestHpAlly(caster, skillData.Range),
                _ => null
            };
        }
        
        public static List<BattleUnit> SelectTargets(BattleUnit caster, SkillData skillData)
        {
            if (caster == null || skillData == null) return new List<BattleUnit>();

            return skillData.TargetType switch
            {
                TargetType.NearestEnemy => SingleToList(FindNearestEnemy(caster, skillData.Range)),
                TargetType.LowestHpEnemy => SingleToList(FindLowestHpEnemy(caster, skillData.Range)),
                TargetType.LowestHpPercentageEnemy => SingleToList(FindLowestHpPercentageEnemy(caster, skillData.Range)),
                TargetType.Self => SingleToList(caster),
                TargetType.LowestHpAlly => SingleToList(FindLowestHpAlly(caster, skillData.Range)),
                TargetType.LowestHpPercentageAlly => SingleToList(FindLowestHpPercentageAlly(caster, skillData.Range)),
                TargetType.AllEnemiesInRange => FindAllEnemiesInRange(caster, skillData.Range),
                TargetType.AllAlliesInRange => FindAllAlliesInRange(caster, skillData.Range),
                _ => new List<BattleUnit>()
            };
        }
        
        public static List<BattleUnit> FindEffectTargets(
            BattleUnit caster,
            BattleUnit primaryTarget,
            SkillData skillData,
            bool targetEnemies = true)
        {
            if (caster == null || skillData == null) return new List<BattleUnit>();
            
            if (skillData.EffectRadius <= 0f)
            {
                return SingleToList(primaryTarget);
            }
            
            BattleUnit origin = skillData.EffectOrigin == EffectOrigin.Target && primaryTarget != null
                ? primaryTarget
                : caster;
            
            var candidates = targetEnemies
                ? BattleManager.Instance.GetEnemiesOf(caster.Team)
                : GetAlliesOf(caster);

            return FindAllInRange(origin, candidates, skillData.EffectRadius);
        }
        
        public static BattleUnit FindNearestEnemy(BattleUnit caster, float range)
        {
            var enemies = BattleManager.Instance.GetEnemiesOf(caster.Team);
            return FindNearest(caster, enemies, range);
        }
        
        public static BattleUnit FindLowestHpEnemy(BattleUnit caster, float range)
        {
            var enemies = BattleManager.Instance.GetEnemiesOf(caster.Team);
            return FindLowestHp(caster, enemies, range);
        }
        
        public static BattleUnit FindLowestHpAlly(BattleUnit caster, float range)
        {
            var allies = GetAlliesOf(caster);
            return FindLowestHp(caster, allies, range);
        }

        public static BattleUnit FindLowestHpPercentageEnemy(BattleUnit caster, float range)
        {
            var enemies = BattleManager.Instance.GetEnemiesOf(caster.Team);
            return FindLowestHpPercentage(caster, enemies, range);
        }

        public static BattleUnit FindLowestHpPercentageAlly(BattleUnit caster, float range)
        {
            var allies = GetAlliesOf(caster);
            return FindLowestHpPercentage(caster, allies, range);
        }
        
        public static List<BattleUnit> FindAllEnemiesInRange(BattleUnit caster, float range)
        {
            var enemies = BattleManager.Instance.GetEnemiesOf(caster.Team);
            return FindAllInRange(caster, enemies, range);
        }
        
        public static List<BattleUnit> FindAllAlliesInRange(BattleUnit caster, float range)
        {
            var allies = GetAlliesOf(caster);
            return FindAllInRange(caster, allies, range);
        }

        private static List<BattleUnit> GetAlliesOf(BattleUnit caster)
        {
            return caster.Team == Team.Ally
                ? BattleManager.Instance.GetAllies()
                : BattleManager.Instance.GetEnemies();
        }

        private static BattleUnit FindNearest(BattleUnit caster, List<BattleUnit> candidates, float range)
        {
            BattleUnit nearest = null;
            float nearestDistance = float.MaxValue;
            Vector3 casterPos = caster.transform.position;

            foreach (var candidate in candidates)
            {
                if (candidate == null || candidate.IsDead) continue;

                float distance = Vector2.Distance(casterPos, candidate.transform.position);
                if (distance <= range && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = candidate;
                }
            }

            return nearest;
        }

        private static BattleUnit FindLowestHp(BattleUnit caster, List<BattleUnit> candidates, float range)
        {
            BattleUnit lowestHpUnit = null;
            float lowestHp = float.MaxValue;
            Vector3 casterPos = caster.transform.position;

            foreach (var candidate in candidates)
            {
                if (candidate == null || candidate.IsDead) continue;

                float distance = Vector2.Distance(casterPos, candidate.transform.position);
                if (distance <= range && candidate.Health < lowestHp)
                {
                    lowestHp = candidate.Health;
                    lowestHpUnit = candidate;
                }
            }

            return lowestHpUnit;
        }

        private static BattleUnit FindLowestHpPercentage(BattleUnit caster, List<BattleUnit> candidates, float range)
        {
            BattleUnit lowestHpUnit = null;
            float lowestHpRatio = float.MaxValue;
            Vector3 casterPos = caster.transform.position;

            foreach (var candidate in candidates)
            {
                if (candidate == null || candidate.IsDead) continue;

                float distance = Vector2.Distance(casterPos, candidate.transform.position);
                if (distance <= range && candidate.HealthRatio < lowestHpRatio)
                {
                    lowestHpRatio = candidate.HealthRatio;
                    lowestHpUnit = candidate;
                }
            }

            return lowestHpUnit;
        }

        private static List<BattleUnit> FindAllInRange(BattleUnit caster, List<BattleUnit> candidates, float range)
        {
            var result = new List<BattleUnit>();
            Vector3 casterPos = caster.transform.position;

            foreach (var candidate in candidates)
            {
                if (candidate == null || candidate.IsDead) continue;

                float distance = Vector2.Distance(casterPos, candidate.transform.position);
                if (distance <= range)
                {
                    result.Add(candidate);
                }
            }

            return result;
        }

        private static List<BattleUnit> SingleToList(BattleUnit unit)
        {
            var list = new List<BattleUnit>();
            if (unit != null)
            {
                list.Add(unit);
            }
            return list;
        }
    }
}
