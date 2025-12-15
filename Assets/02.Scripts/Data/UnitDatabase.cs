using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UnitDatabase", menuName = "Game/Unit Database")]
    public class UnitDatabase : ScriptableObject
    {
        [SerializeField] private UnitData[] _units;

        public UnitData GetById(string unitId)
        {
            if (string.IsNullOrEmpty(unitId)) return null;
            return _units?.FirstOrDefault(u => u != null && u.UnitId == unitId);
        }

        public IReadOnlyList<UnitData> GetAll() => _units ?? System.Array.Empty<UnitData>();

        public IEnumerable<UnitData> GetByRole(UnitRole role)
        {
            if (_units == null) yield break;
            foreach (var unit in _units)
            {
                if (unit != null && unit.Role == role)
                    yield return unit;
            }
        }

        public int Count => _units?.Length ?? 0;

#if UNITY_EDITOR
        [ContextMenu("Auto-Collect UnitData from Project")]
        private void AutoCollectUnits()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:UnitData");
            var units = new List<UnitData>();
            
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var unit = UnityEditor.AssetDatabase.LoadAssetAtPath<UnitData>(path);
                if (unit != null)
                    units.Add(unit);
            }
            
            _units = units.ToArray();
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"[UnitDatabase] {_units.Length}개의 UnitData를 수집했습니다.");
        }
#endif
    }
}
