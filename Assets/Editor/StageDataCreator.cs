using UnityEngine;
using UnityEditor;
using Data;

public static class StageDataCreator
{
    [MenuItem("Tools/Create Test Stage 1-1")]
    public static void CreateTestStage()
    {
        var stage = ScriptableObject.CreateInstance<StageData>();
        stage.StageId = "1-1";
        stage.StageName = "초원 입구";
        stage.Enemies = new EnemySpawnInfo[]
        {
            new EnemySpawnInfo { Level = 1, SpawnIndex = 0 },
            new EnemySpawnInfo { Level = 1, SpawnIndex = 1 },
        };
        stage.RecommendedPower = 100;
        stage.GoldReward = 50;

        const string folderPath = "Assets/09.ScriptableObjects/Stages";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/09.ScriptableObjects", "Stages");
        }

        AssetDatabase.CreateAsset(stage, $"{folderPath}/Stage_1-1.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = stage;

        Debug.Log("Stage_1-1.asset created successfully!");
    }
}
