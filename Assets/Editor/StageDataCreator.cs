using UnityEngine;
using UnityEditor;
using Data;

public static class StageDataCreator
{
    private static readonly (string id, string name, int power, int gold, int enemyCount)[] GrasslandStages =
    {
        ("1-1", "초원 입구", 100, 50, 2),
        ("1-2", "초원 외곽", 120, 60, 3),
        ("1-3", "초원 중심지", 150, 80, 3),
        ("1-4", "초원 깊은 곳", 180, 100, 4),
        ("1-5", "초원 끝자락", 220, 130, 5),
    };

    [MenuItem("Tools/Create Test Stage 1-1")]
    public static void CreateTestStage()
    {
        CreateStage("1-1", "초원 입구", 100, 50, 2);
    }

    [MenuItem("Tools/Create All Grassland Stages (1-1 ~ 1-5)")]
    public static void CreateAllGrasslandStages()
    {
        const string folderPath = "Assets/09.ScriptableObjects/Stages";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/09.ScriptableObjects", "Stages");
        }

        foreach (var (id, name, power, gold, enemyCount) in GrasslandStages)
        {
            CreateStage(id, name, power, gold, enemyCount);
        }

        Debug.Log("All Grassland stages (1-1 ~ 1-5) created successfully!");
    }

    private static void CreateStage(string id, string name, int power, int gold, int enemyCount)
    {
        const string folderPath = "Assets/09.ScriptableObjects/Stages";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/09.ScriptableObjects", "Stages");
        }

        string assetPath = $"{folderPath}/Stage_{id}.asset";
        if (AssetDatabase.LoadAssetAtPath<StageData>(assetPath) != null)
        {
            Debug.Log($"Stage_{id}.asset already exists, skipping...");
            return;
        }

        var stage = ScriptableObject.CreateInstance<StageData>();
        stage.StageId = id;
        stage.StageName = name;
        stage.RecommendedPower = power;
        stage.GoldReward = gold;

        var enemies = new EnemySpawnInfo[enemyCount];
        for (int i = 0; i < enemyCount; i++)
        {
            enemies[i] = new EnemySpawnInfo { Level = 1, SpawnIndex = i };
        }
        stage.Enemies = enemies;

        AssetDatabase.CreateAsset(stage, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = stage;

        Debug.Log($"Stage_{id}.asset created successfully!");
    }
}
