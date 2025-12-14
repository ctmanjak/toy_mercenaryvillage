using Battle;
using Core;
using Data;
using UnityEditor;
using UnityEngine;

public class BattleTestWindow : EditorWindow
{
    private StageData _stageData;

    [MenuItem("Tools/Battle Test")]
    public static void ShowWindow()
    {
        GetWindow<BattleTestWindow>("Battle Test");
    }

    private void OnGUI()
    {
        GUILayout.Label("Battle Test", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _stageData = (StageData)EditorGUILayout.ObjectField("Stage Data", _stageData, typeof(StageData), false);

        EditorGUILayout.Space();

        GUI.enabled = Application.isPlaying && _stageData != null;

        if (GUILayout.Button("Start Battle"))
        {
            if (BattleManager.Instance == null)
            {
                Debug.LogError("[BattleTestWindow] BattleManager not found");
                return;
            }

            if (PartyManager.Instance == null)
            {
                Debug.LogError("[BattleTestWindow] PartyManager not found");
                return;
            }

            if (!PartyManager.Instance.CanStartBattle())
            {
                Debug.LogWarning("[BattleTestWindow] Party is empty!");
                return;
            }

            BattleManager.Instance.StartBattle(_stageData);
        }

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Clear Units"))
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.ClearAllUnits();
            }
        }

        GUI.enabled = true;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Play Mode에서만 동작합니다.\nPartyManager의 파티를 사용합니다.", MessageType.Info);

        if (Application.isPlaying)
        {
            EditorGUILayout.Space();
            GUILayout.Label("Status", EditorStyles.boldLabel);

            if (BattleManager.Instance != null)
            {
                EditorGUILayout.LabelField("Phase", BattleManager.Instance.Phase.ToString());
                EditorGUILayout.LabelField("Result", BattleManager.Instance.BattleResult.ToString());
                EditorGUILayout.LabelField("Allies", BattleManager.Instance.GetAllies().Count.ToString());
                EditorGUILayout.LabelField("Enemies", BattleManager.Instance.GetEnemies().Count.ToString());
            }

            if (PartyManager.Instance != null)
            {
                EditorGUILayout.Space();
                GUILayout.Label("Party", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Party Count", PartyManager.Instance.GetPartyCount().ToString());

                var party = PartyManager.Instance.GetParty();
                for (int i = 0; i < party.Length; i++)
                {
                    var name = party[i]?.DisplayName ?? "(empty)";
                    EditorGUILayout.LabelField($"Slot {i}", name);
                }
            }

            Repaint();
        }
    }
}
