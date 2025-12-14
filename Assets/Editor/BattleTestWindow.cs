using System.Collections.Generic;
using Battle;
using Data;
using UnityEditor;
using UnityEngine;

public class BattleTestWindow : EditorWindow
{
    private StageData _stageData;
    private List<UnitData> _allyParty = new();
    private int _allyLevel = 1;

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
        GUILayout.Label("Ally Party", EditorStyles.boldLabel);

        _allyLevel = EditorGUILayout.IntSlider("Ally Level", _allyLevel, 1, 10);

        for (int i = 0; i < _allyParty.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _allyParty[i] = (UnitData)EditorGUILayout.ObjectField($"Slot {i}", _allyParty[i], typeof(UnitData), false);
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                _allyParty.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (_allyParty.Count < 4 && GUILayout.Button("+ Add Ally"))
        {
            _allyParty.Add(null);
        }

        EditorGUILayout.Space();

        bool hasAllies = _allyParty.Exists(u => u != null);
        GUI.enabled = Application.isPlaying && _stageData != null && hasAllies;

        if (GUILayout.Button("Start Battle"))
        {
            if (BattleManager.Instance != null)
            {
                var validAllies = _allyParty.FindAll(u => u != null);
                BattleManager.Instance.StartBattle(_stageData, validAllies, _allyLevel);
            }
            else
            {
                Debug.LogError("[BattleTestWindow] BattleManager not found");
            }
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
        EditorGUILayout.HelpBox("Play Mode에서만 동작합니다.", MessageType.Info);

        if (Application.isPlaying && BattleManager.Instance != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Phase", BattleManager.Instance.Phase.ToString());
            EditorGUILayout.LabelField("Result", BattleManager.Instance.BattleResult.ToString());
            EditorGUILayout.LabelField("Allies", BattleManager.Instance.GetAllies().Count.ToString());
            EditorGUILayout.LabelField("Enemies", BattleManager.Instance.GetEnemies().Count.ToString());

            Repaint();
        }
    }
}
