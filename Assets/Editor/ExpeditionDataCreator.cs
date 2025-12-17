using UnityEngine;
using UnityEditor;
using Data;
using System.Collections.Generic;

public static class ExpeditionDataCreator
{
    [MenuItem("Tools/Create Expeditions/Create Prairie Expedition")]
    public static void CreatePrairieExpedition()
    {
        CreateExpedition(
            "prairie",
            "초원",
            "넓은 초원에서 시작하는 첫 번째 원정입니다.",
            150,
            100,
            150,
            null,
            new[] { "1-1", "1-2", "1-3", "1-4", "1-5" }
        );
    }

    [MenuItem("Tools/Create Expeditions/Create Forest Expedition")]
    public static void CreateForestExpedition()
    {
        var prairie = AssetDatabase.LoadAssetAtPath<ExpeditionData>(
            "Assets/09.ScriptableObjects/Expeditions/Expedition_Prairie.asset");

        CreateExpedition(
            "forest",
            "숲",
            "울창한 숲 속에서 펼쳐지는 두 번째 원정입니다.",
            280,
            120,
            200,
            prairie,
            new[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-5" }
        );
    }

    [MenuItem("Tools/Create Expeditions/Create All Expeditions")]
    public static void CreateAllExpeditions()
    {
        CreatePrairieExpedition();
        CreateForestExpedition();
        Debug.Log("All expeditions created successfully!");
    }

    private static void CreateExpedition(
        string id, 
        string name, 
        string description,
        int recommendedPower, 
        int completionBonus, 
        int firstClearBonus,
        ExpeditionData unlockRequirement,
        string[] stageIds)
    {
        const string folderPath = "Assets/09.ScriptableObjects/Expeditions";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/09.ScriptableObjects", "Expeditions");
        }

        string assetPath = $"{folderPath}/Expedition_{char.ToUpper(id[0])}{id.Substring(1)}.asset";
        if (AssetDatabase.LoadAssetAtPath<ExpeditionData>(assetPath) != null)
        {
            Debug.Log($"Expedition_{id}.asset already exists, skipping...");
            return;
        }

        var expedition = ScriptableObject.CreateInstance<ExpeditionData>();
        expedition.ExpeditionId = id;
        expedition.ExpeditionName = name;
        expedition.Description = description;
        expedition.RecommendedPower = recommendedPower;
        expedition.CompletionBonus = completionBonus;
        expedition.FirstClearBonus = firstClearBonus;
        expedition.UnlockRequirement = unlockRequirement;

        expedition.Battles = new List<StageData>();
        foreach (var stageId in stageIds)
        {
            var stage = AssetDatabase.LoadAssetAtPath<StageData>(
                $"Assets/09.ScriptableObjects/Stages/Stage_{stageId}.asset");
            if (stage != null)
            {
                expedition.Battles.Add(stage);
            }
            else
            {
                Debug.LogWarning($"Stage_{stageId}.asset not found!");
            }
        }

        AssetDatabase.CreateAsset(expedition, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = expedition;

        Debug.Log($"Expedition_{id}.asset created successfully!");
    }
}
