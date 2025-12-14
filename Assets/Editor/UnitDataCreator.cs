using Battle;
using UnityEngine;
using UnityEditor;
using Data;

public static class UnitDataCreator
{
    private const string FolderPath = "Assets/09.ScriptableObjects/Units";
    private const string AllyPrefabPath = "Assets/03.Prefabs/Units/Ally/Swordsman.prefab";

    [MenuItem("Tools/Create Mercenary Unit Data")]
    public static void CreateMercenaryUnits()
    {
        EnsureFolderExists();

        var prefab = AssetDatabase.LoadAssetAtPath<BattleUnit>(AllyPrefabPath);

        // 탱커: HP 150, ATK 10, AtkSpd 0.8, MoveSpd 2.5, Range 1.5
        CreateUnit("tank_01", "탱커", UnitRole.Tank,
            baseHealth: 150f, baseAttackDamage: 10f, baseAttackSpeed: 0.8f,
            baseMoveSpeed: 2.5f, baseAttackRange: 1.5f, prefab, "Unit_Tank.asset");

        // 근접딜러: HP 80, ATK 25, AtkSpd 1.2, MoveSpd 3.5, Range 1.5
        CreateUnit("melee_damage_01", "근접딜러", UnitRole.Damage,
            baseHealth: 80f, baseAttackDamage: 25f, baseAttackSpeed: 1.2f,
            baseMoveSpeed: 3.5f, baseAttackRange: 1.5f, prefab, "Unit_MeleeDamage.asset");

        // 원거리딜러: HP 60, ATK 20, AtkSpd 1.0, MoveSpd 3.0, Range 5.0
        CreateUnit("ranged_damage_01", "원거리딜러", UnitRole.Damage,
            baseHealth: 60f, baseAttackDamage: 20f, baseAttackSpeed: 1.0f,
            baseMoveSpeed: 3.0f, baseAttackRange: 5.0f, prefab, "Unit_RangedDamage.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[UnitDataCreator] 용병 UnitData 에셋 생성 완료: 탱커, 근접딜러, 원거리딜러");
    }

    private static void CreateUnit(string id, string name, UnitRole role,
        float baseHealth, float baseAttackDamage, float baseAttackSpeed,
        float baseMoveSpeed, float baseAttackRange, BattleUnit prefab, string fileName = null)
    {
        var unit = ScriptableObject.CreateInstance<UnitData>();
        unit.UnitId = id;
        unit.UnitName = name;
        unit.Role = role;
        unit.Prefab = prefab;
        unit.BaseHealth = baseHealth;
        unit.BaseAttackDamage = baseAttackDamage;
        unit.BaseAttackSpeed = baseAttackSpeed;
        unit.BaseMoveSpeed = baseMoveSpeed;
        unit.BaseAttackRange = baseAttackRange;

        if (string.IsNullOrEmpty(fileName))
        {
            fileName = $"Unit_{role}_{id.Split('_')[1]}.asset";
        }

        string fullPath = $"{FolderPath}/{fileName}";

        // 기존 에셋이 있으면 삭제
        var existing = AssetDatabase.LoadAssetAtPath<UnitData>(fullPath);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(fullPath);
        }

        AssetDatabase.CreateAsset(unit, fullPath);
    }

    private static void EnsureFolderExists()
    {
        if (!AssetDatabase.IsValidFolder("Assets/09.ScriptableObjects"))
        {
            AssetDatabase.CreateFolder("Assets", "09.ScriptableObjects");
        }
        if (!AssetDatabase.IsValidFolder(FolderPath))
        {
            AssetDatabase.CreateFolder("Assets/09.ScriptableObjects", "Units");
        }
    }
}
