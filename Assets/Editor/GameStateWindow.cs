using Core;
using Data;
using UnityEditor;
using UnityEngine;

public class GameStateWindow : EditorWindow
{
    private Vector2 _scrollPosition;
    private bool _showMercenaryList = true;
    private bool _showPartySlots = true;

    [MenuItem("Tools/Game State")]
    public static void ShowWindow()
    {
        GetWindow<GameStateWindow>("Game State");
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Play Mode에서 실행해야 데이터를 확인할 수 있습니다.", MessageType.Info);
            EditorGUILayout.EndScrollView();
            return;
        }

        DrawPlayerResourceSection();
        EditorGUILayout.Space(10);
        DrawPartySection();

        EditorGUILayout.EndScrollView();
        Repaint();
    }

    private void DrawPlayerResourceSection()
    {
        GUILayout.Label("Player Resources", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if (PlayerResourceManager.Instance == null)
        {
            EditorGUILayout.HelpBox("PlayerResourceManager가 없습니다.", MessageType.Warning);
            return;
        }

        var prm = PlayerResourceManager.Instance;

        // Gold
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Gold", EditorStyles.boldLabel, GUILayout.Width(80));
        EditorGUILayout.LabelField(prm.Gold.ToString("N0"), GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        // Mercenary Count
        EditorGUILayout.LabelField($"보유 용병: {prm.MercenaryCount}명");

        EditorGUILayout.Space(5);

        // Mercenary List Foldout
        _showMercenaryList = EditorGUILayout.Foldout(_showMercenaryList, "용병 목록", true);

        if (_showMercenaryList)
        {
            EditorGUI.indentLevel++;
            var mercenaries = prm.GetAllMercenaries();

            if (mercenaries.Count == 0)
            {
                EditorGUILayout.LabelField("(용병 없음)");
            }
            else
            {
                foreach (var merc in mercenaries)
                {
                    DrawMercenaryInfo(merc);
                }
            }
            EditorGUI.indentLevel--;
        }
    }

    private void DrawPartySection()
    {
        GUILayout.Label("Party", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if (PartyManager.Instance == null)
        {
            EditorGUILayout.HelpBox("PartyManager가 없습니다.", MessageType.Warning);
            return;
        }

        var pm = PartyManager.Instance;

        EditorGUILayout.LabelField($"파티 인원: {pm.GetPartyCount()} / {pm.PartySize}");
        EditorGUILayout.Space(5);

        _showPartySlots = EditorGUILayout.Foldout(_showPartySlots, "파티 슬롯", true);

        if (_showPartySlots)
        {
            EditorGUI.indentLevel++;
            var party = pm.GetParty();

            for (int i = 0; i < party.Length; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"슬롯 {i + 1}", EditorStyles.boldLabel);

                if (party[i] != null)
                {
                    DrawMercenaryInfo(party[i]);
                }
                else
                {
                    EditorGUILayout.LabelField("(비어있음)");
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }
            EditorGUI.indentLevel--;
        }
    }

    private void DrawMercenaryInfo(MercenaryData merc)
    {
        if (merc == null) return;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Name & Level
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(merc.DisplayName, EditorStyles.boldLabel, GUILayout.Width(120));
        EditorGUILayout.LabelField($"Lv.{merc.Level}", GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        // ID (shortened)
        var shortId = merc.Id.Length > 8 ? merc.Id.Substring(0, 8) + "..." : merc.Id;
        EditorGUILayout.LabelField($"ID: {shortId}", EditorStyles.miniLabel);

        // Stats
        if (merc.UnitData != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"HP: {merc.GetCurrentHP():F0}", GUILayout.Width(80));
            EditorGUILayout.LabelField($"ATK: {merc.GetCurrentAttackDamage():F0}", GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField($"타입: {merc.UnitData.Role}", EditorStyles.miniLabel);
        }
        else
        {
            EditorGUILayout.LabelField("(UnitData 없음)", EditorStyles.miniLabel);
        }

        EditorGUILayout.EndVertical();
    }
}
