using UnityEngine;
using UnityEditor;
using Data;

public static class UnitDataCreator
{
    private const string FolderPath = "Assets/09.ScriptableObjects/Units";

    [MenuItem("Tools/Create Test Units (Tank, Damage, Support)")]
    public static void CreateTestUnits()
    {
        EnsureFolderExists();

        CreateUnit("tank_01", "Iron Guard", UnitRole.Tank,
            baseHealth: 150f, baseAttackDamage: 8f, baseAttackSpeed: 0.8f,
            baseMoveSpeed: 2.5f, baseAttackRange: 1.5f);

        CreateUnit("damage_01", "Swift Blade", UnitRole.Damage,
            baseHealth: 80f, baseAttackDamage: 15f, baseAttackSpeed: 1.2f,
            baseMoveSpeed: 3.5f, baseAttackRange: 1.5f);

        CreateUnit("support_01", "Field Medic", UnitRole.Support,
            baseHealth: 100f, baseAttackDamage: 5f, baseAttackSpeed: 1f,
            baseMoveSpeed: 3f, baseAttackRange: 2f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Test UnitData assets created: Tank, Damage, Support");
    }

    private static void CreateUnit(string id, string name, UnitRole role,
        float baseHealth, float baseAttackDamage, float baseAttackSpeed,
        float baseMoveSpeed, float baseAttackRange)
    {
        var unit = ScriptableObject.CreateInstance<UnitData>();
        unit.UnitId = id;
        unit.UnitName = name;
        unit.Role = role;
        unit.BaseHealth = baseHealth;
        unit.BaseAttackDamage = baseAttackDamage;
        unit.BaseAttackSpeed = baseAttackSpeed;
        unit.BaseMoveSpeed = baseMoveSpeed;
        unit.BaseAttackRange = baseAttackRange;

        string fileName = $"Unit_{role}_{id.Split('_')[1]}.asset";
        AssetDatabase.CreateAsset(unit, $"{FolderPath}/{fileName}");
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
