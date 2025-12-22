using System.Collections.Generic;
using Battle;
using Core;
using Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillTestWindow : EditorWindow
{
    private Vector2 _scrollPosition;

    [Header("Unit Spawn")]
    private UnitData _allyUnitData;
    private UnitData _enemyUnitData;
    private int _allyLevel = 1;
    private int _enemyLevel = 1;
    private int _allySpawnIndex = 0;
    private int _enemySpawnIndex = 0;

    [Header("Skill Test")]
    private SkillData _testSkillData;
    private int _selectedUnitIndex = 0;

    private bool _showAllyFoldout = true;
    private bool _showEnemyFoldout = true;
    private bool _showSkillFoldout = true;

    [MenuItem("Tools/Skill Test Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<SkillTestWindow>("Skill Test");
        window.minSize = new Vector2(350, 500);
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        Repaint();
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        DrawHeader();
        DrawPlayModeCheck();

        if (Application.isPlaying)
        {
            DrawSpawnSection();
            DrawSkillTestSection();
            DrawBattleControlSection();
            DrawUnitStatusSection();
        }

        EditorGUILayout.EndScrollView();

        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(5);
        GUILayout.Label("Skill Test Window", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
    }

    private void DrawPlayModeCheck()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox(
                "Play Mode에서만 동작합니다.\nSkillTestScene 또는 BattleScene에서 실행해주세요.",
                MessageType.Warning);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Open BattleScene"))
            {
                EditorSceneManager.OpenScene("Assets/01.Scenes/BattleScene.unity");
            }

            if (GUILayout.Button("Open/Create SkillTestScene"))
            {
                OpenOrCreateTestScene();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Create SkillTestScene (Force New)"))
            {
                CreateTestScene();
            }

            return;
        }

        if (BattleManager.Instance == null)
        {
            EditorGUILayout.HelpBox("BattleManager가 씬에 없습니다.", MessageType.Error);
            return;
        }
    }

    private void OpenOrCreateTestScene()
    {
        const string scenePath = "Assets/01.Scenes/SkillTestScene.unity";

        if (System.IO.File.Exists(scenePath))
        {
            EditorSceneManager.OpenScene(scenePath);
        }
        else
        {
            CreateTestScene();
        }
    }

    private void CreateTestScene()
    {
        const string scenePath = "Assets/01.Scenes/SkillTestScene.unity";

        // Create new scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Add Camera
        var cameraGo = new GameObject("Main Camera");
        var camera = cameraGo.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5;
        cameraGo.transform.position = new Vector3(0, 0, -10);
        cameraGo.tag = "MainCamera";

        // Add Light (2D)
        var lightGo = new GameObject("Global Light 2D");
        var light2D = lightGo.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
        light2D.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Global;

        // Add Managers
        CreateManagerFromPrefab("Assets/03.Prefabs/Manager/GameManager.prefab");
        CreateManagerFromPrefab("Assets/03.Prefabs/Manager/PartyManager.prefab");
        CreateManagerFromPrefab("Assets/03.Prefabs/Manager/GameDataManager.prefab");
        CreateManagerFromPrefab("Assets/03.Prefabs/Manager/PlayerResourceManager.prefab");
        CreateManagerFromPrefab("Assets/03.Prefabs/Manager/Battle/BattleManager.prefab");
        CreateManagerFromPrefab("Assets/03.Prefabs/Manager/Battle/SpawnManager.prefab");
        CreateManagerFromPrefab("Assets/03.Prefabs/Manager/Battle/PoolManager.prefab");
        CreateManagerFromPrefab("Assets/03.Prefabs/Manager/Battle/ProjectileManager.prefab");

        // Create spawn points
        var spawnPointsParent = new GameObject("SpawnPoints");

        // Ally spawn points (left side)
        for (int i = 0; i < 4; i++)
        {
            var spawnGo = new GameObject($"AllySpawn_{i}");
            spawnGo.transform.parent = spawnPointsParent.transform;
            spawnGo.transform.position = new Vector3(-4f, 2f - i * 1.5f, 0);
            var spawnPoint = spawnGo.AddComponent<SpawnPoint>();

            // Set fields via SerializedObject
            var so = new SerializedObject(spawnPoint);
            so.FindProperty("_team").enumValueIndex = 0; // Team.Ally
            so.FindProperty("_index").intValue = i;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Enemy spawn points (right side)
        for (int i = 0; i < 4; i++)
        {
            var spawnGo = new GameObject($"EnemySpawn_{i}");
            spawnGo.transform.parent = spawnPointsParent.transform;
            spawnGo.transform.position = new Vector3(4f, 2f - i * 1.5f, 0);
            var spawnPoint = spawnGo.AddComponent<SpawnPoint>();

            var so = new SerializedObject(spawnPoint);
            so.FindProperty("_team").enumValueIndex = 1; // Team.Enemy
            so.FindProperty("_index").intValue = i;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Connect SpawnManager to spawn points
        var spawnManager = Object.FindFirstObjectByType<SpawnManager>();
        if (spawnManager != null)
        {
            var spawnManagerSO = new SerializedObject(spawnManager);
            var allySpawns = spawnManagerSO.FindProperty("_allySpawns");
            var enemySpawns = spawnManagerSO.FindProperty("_enemySpawns");

            for (int i = 0; i < 4; i++)
            {
                var allySpawn = spawnPointsParent.transform.Find($"AllySpawn_{i}")?.GetComponent<SpawnPoint>();
                var enemySpawn = spawnPointsParent.transform.Find($"EnemySpawn_{i}")?.GetComponent<SpawnPoint>();

                if (allySpawn != null)
                    allySpawns.GetArrayElementAtIndex(i).objectReferenceValue = allySpawn;
                if (enemySpawn != null)
                    enemySpawns.GetArrayElementAtIndex(i).objectReferenceValue = enemySpawn;
            }

            spawnManagerSO.ApplyModifiedPropertiesWithoutUndo();
        }

        // Connect BattleManager to SpawnManager
        var battleManager = Object.FindFirstObjectByType<BattleManager>();
        if (battleManager != null && spawnManager != null)
        {
            var battleManagerSO = new SerializedObject(battleManager);
            battleManagerSO.FindProperty("_spawnManager").objectReferenceValue = spawnManager;
            battleManagerSO.ApplyModifiedPropertiesWithoutUndo();
        }

        // Add DamagePopupSpawner
        var damagePopupSpawnerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/UI/DamagePopupSpawner.prefab");
        if (damagePopupSpawnerPrefab != null)
        {
            PrefabUtility.InstantiatePrefab(damagePopupSpawnerPrefab);
        }
        else
        {
            var damageSpawnerGo = new GameObject("DamagePopupSpawner");
            damageSpawnerGo.AddComponent<DamagePopupSpawner>();
        }

        // Save scene
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log($"[SkillTestWindow] Created SkillTestScene at {scenePath}");
    }

    private GameObject CreateManagerFromPrefab(string prefabPath)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            return PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        }

        Debug.LogWarning($"[SkillTestWindow] Prefab not found: {prefabPath}");
        return null;
    }

    private void DrawSpawnSection()
    {
        EditorGUILayout.Space();
        _showAllyFoldout = EditorGUILayout.Foldout(_showAllyFoldout, "Ally Spawn", true, EditorStyles.foldoutHeader);

        if (_showAllyFoldout)
        {
            EditorGUI.indentLevel++;
            _allyUnitData = (UnitData)EditorGUILayout.ObjectField("Unit Data", _allyUnitData, typeof(UnitData), false);
            _allyLevel = EditorGUILayout.IntSlider("Level", _allyLevel, 1, 10);
            _allySpawnIndex = EditorGUILayout.IntSlider("Spawn Index", _allySpawnIndex, 0, 3);

            GUI.enabled = _allyUnitData != null;
            if (GUILayout.Button("Spawn Ally"))
            {
                SpawnTestUnit(_allyUnitData, _allyLevel, Team.Ally, _allySpawnIndex);
            }
            GUI.enabled = true;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        _showEnemyFoldout = EditorGUILayout.Foldout(_showEnemyFoldout, "Enemy Spawn", true, EditorStyles.foldoutHeader);

        if (_showEnemyFoldout)
        {
            EditorGUI.indentLevel++;
            _enemyUnitData = (UnitData)EditorGUILayout.ObjectField("Unit Data", _enemyUnitData, typeof(UnitData), false);
            _enemyLevel = EditorGUILayout.IntSlider("Level", _enemyLevel, 1, 10);
            _enemySpawnIndex = EditorGUILayout.IntSlider("Spawn Index", _enemySpawnIndex, 0, 3);

            GUI.enabled = _enemyUnitData != null;
            if (GUILayout.Button("Spawn Enemy"))
            {
                SpawnTestUnit(_enemyUnitData, _enemyLevel, Team.Enemy, _enemySpawnIndex);
            }
            GUI.enabled = true;
            EditorGUI.indentLevel--;
        }
    }

    private void DrawSkillTestSection()
    {
        EditorGUILayout.Space();
        _showSkillFoldout = EditorGUILayout.Foldout(_showSkillFoldout, "Skill Test", true, EditorStyles.foldoutHeader);

        if (!_showSkillFoldout) return;

        EditorGUI.indentLevel++;

        var allies = BattleManager.Instance?.GetAllies();
        if (allies == null || allies.Count == 0)
        {
            EditorGUILayout.HelpBox("아군 유닛을 먼저 스폰해주세요.", MessageType.Info);
            EditorGUI.indentLevel--;
            return;
        }

        // Unit selector
        string[] unitNames = new string[allies.Count];
        for (int i = 0; i < allies.Count; i++)
        {
            var unit = allies[i];
            unitNames[i] = unit != null ? $"{i}: {unit.name}" : $"{i}: null";
        }

        _selectedUnitIndex = Mathf.Clamp(_selectedUnitIndex, 0, allies.Count - 1);
        _selectedUnitIndex = EditorGUILayout.Popup("Target Unit", _selectedUnitIndex, unitNames);

        var selectedUnit = allies[_selectedUnitIndex];
        if (selectedUnit == null)
        {
            EditorGUILayout.HelpBox("선택된 유닛이 없습니다.", MessageType.Warning);
            EditorGUI.indentLevel--;
            return;
        }

        EditorGUILayout.Space(5);

        // Display unit's skills
        EditorGUILayout.LabelField("Unit Skills:", EditorStyles.boldLabel);
        if (selectedUnit.Skills.Count == 0)
        {
            EditorGUILayout.LabelField("  (no skills)");
        }
        else
        {
            foreach (var skill in selectedUnit.Skills)
            {
                EditorGUILayout.LabelField($"  • {skill.SkillName} ({skill.EffectType})");
            }
        }

        EditorGUILayout.Space(5);

        // Manual skill execution
        _testSkillData = (SkillData)EditorGUILayout.ObjectField("Override Skill", _testSkillData, typeof(SkillData), false);

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Trigger Unit Skills"))
        {
            TriggerUnitSkills(selectedUnit);
        }

        GUI.enabled = _testSkillData != null;
        if (GUILayout.Button("Execute Override Skill"))
        {
            ExecuteSkillOnUnit(selectedUnit, _testSkillData);
        }
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        // Damage/Heal test
        EditorGUILayout.LabelField("Quick Actions:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Damage -50"))
        {
            selectedUnit.TakeDamage(50);
        }
        if (GUILayout.Button("Heal +50"))
        {
            selectedUnit.Heal(50);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    private void DrawBattleControlSection()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Battle Control", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Start Fighting"))
        {
            StartTestFighting();
        }

        if (GUILayout.Button("Stop/Pause"))
        {
            StopTestFighting();
        }

        if (GUILayout.Button("Clear All"))
        {
            BattleManager.Instance?.ClearAllUnits();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        if (BattleManager.Instance != null)
        {
            var phaseColor = BattleManager.Instance.Phase switch
            {
                BattlePhase.Ready => Color.yellow,
                BattlePhase.Fighting => Color.green,
                BattlePhase.Ended => Color.gray,
                _ => Color.white
            };

            var prevColor = GUI.color;
            GUI.color = phaseColor;
            EditorGUILayout.LabelField($"Phase: {BattleManager.Instance.Phase}", EditorStyles.boldLabel);
            GUI.color = prevColor;

            EditorGUILayout.LabelField($"Result: {BattleManager.Instance.BattleResult}");
        }
    }

    private void DrawUnitStatusSection()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Unit Status", EditorStyles.boldLabel);

        var allies = BattleManager.Instance?.GetAllies();
        var enemies = BattleManager.Instance?.GetEnemies();

        EditorGUILayout.LabelField($"Allies: {allies?.Count ?? 0}  |  Enemies: {enemies?.Count ?? 0}");

        EditorGUILayout.Space(5);

        // Allies
        if (allies != null && allies.Count > 0)
        {
            EditorGUILayout.LabelField("Allies:", EditorStyles.miniBoldLabel);
            foreach (var unit in allies)
            {
                DrawUnitStatus(unit);
            }
        }

        EditorGUILayout.Space(3);

        // Enemies
        if (enemies != null && enemies.Count > 0)
        {
            EditorGUILayout.LabelField("Enemies:", EditorStyles.miniBoldLabel);
            foreach (var unit in enemies)
            {
                DrawUnitStatus(unit);
            }
        }
    }

    private void DrawUnitStatus(BattleUnit unit)
    {
        if (unit == null) return;

        EditorGUILayout.BeginHorizontal();

        var stateColor = unit.State switch
        {
            UnitState.Idle => Color.white,
            UnitState.Move => Color.cyan,
            UnitState.Attack => Color.yellow,
            UnitState.Dead => Color.gray,
            _ => Color.white
        };

        var prevColor = GUI.contentColor;
        GUI.contentColor = stateColor;

        float healthRatio = unit.MaxHealth > 0 ? unit.Health / unit.MaxHealth : 0;
        string healthBar = new string('█', (int)(healthRatio * 10)) + new string('░', 10 - (int)(healthRatio * 10));

        EditorGUILayout.LabelField($"  {unit.name}", GUILayout.Width(120));
        EditorGUILayout.LabelField($"[{healthBar}] {unit.Health:F0}/{unit.MaxHealth:F0}", GUILayout.Width(180));
        EditorGUILayout.LabelField($"{unit.State}", GUILayout.Width(60));

        GUI.contentColor = prevColor;

        EditorGUILayout.EndHorizontal();

        // Show skill cooldowns
        if (unit.SkillInstances.Count > 0)
        {
            foreach (var skillInst in unit.SkillInstances)
            {
                var cd = skillInst.CurrentCooldown;
                var maxCd = skillInst.Data.Cooldown;
                string cdText = cd > 0 ? $"CD: {cd:F1}s" : "Ready";
                EditorGUILayout.LabelField($"      ↳ {skillInst.Data.SkillName}: {cdText}");
            }
        }
    }

    private void SpawnTestUnit(UnitData unitData, int level, Team team, int spawnIndex)
    {
        if (BattleManager.Instance == null)
        {
            Debug.LogError("[SkillTestWindow] BattleManager not found");
            return;
        }

        var unit = BattleManager.Instance.SpawnUnit(unitData, level, team, spawnIndex);
        if (unit != null)
        {
            Debug.Log($"[SkillTestWindow] Spawned {unitData.UnitName} (Lv.{level}) as {team} at index {spawnIndex}");
        }
    }

    private void TriggerUnitSkills(BattleUnit unit)
    {
        if (unit == null || unit.SkillInstances.Count == 0)
        {
            Debug.LogWarning("[SkillTestWindow] No skills to trigger");
            return;
        }

        // Reset all cooldowns to trigger skills
        foreach (var skillInst in unit.SkillInstances)
        {
            // Force skill activation by using reflection or direct call
            var skillData = skillInst.Data;
            var primaryTarget = SkillTargetSelector.SelectTarget(unit, skillData);

            if (primaryTarget == null)
            {
                Debug.Log($"[SkillTestWindow] {skillData.SkillName}: No valid target found");
                continue;
            }

            bool targetEnemies = IsEnemyTargetingEffect(skillData.EffectType);
            var effectTargets = SkillTargetSelector.FindEffectTargets(unit, primaryTarget, skillData, targetEnemies);

            Debug.Log($"[SkillTestWindow] Executing {skillData.SkillName} on {effectTargets.Count} target(s)");
            SkillEffectExecutor.Execute(unit, primaryTarget, skillData, effectTargets);
        }
    }

    private void ExecuteSkillOnUnit(BattleUnit unit, SkillData skillData)
    {
        if (unit == null || skillData == null)
        {
            Debug.LogWarning("[SkillTestWindow] Unit or SkillData is null");
            return;
        }

        var primaryTarget = SkillTargetSelector.SelectTarget(unit, skillData);

        if (primaryTarget == null)
        {
            Debug.Log($"[SkillTestWindow] {skillData.SkillName}: No valid target found");
            return;
        }

        bool targetEnemies = IsEnemyTargetingEffect(skillData.EffectType);
        var effectTargets = SkillTargetSelector.FindEffectTargets(unit, primaryTarget, skillData, targetEnemies);

        Debug.Log($"[SkillTestWindow] Executing override skill {skillData.SkillName} on {effectTargets.Count} target(s)");
        SkillEffectExecutor.Execute(unit, primaryTarget, skillData, effectTargets);
    }

    private bool IsEnemyTargetingEffect(EffectType effectType)
    {
        return effectType switch
        {
            EffectType.Damage => true,
            EffectType.DamageAoE => true,
            EffectType.DebuffAtk => true,
            EffectType.Stun => true,
            EffectType.None => true,
            EffectType.Heal => false,
            EffectType.HealAoE => false,
            EffectType.BuffAtk => false,
            EffectType.Shield => false,
            _ => true
        };
    }

    private void StartTestFighting()
    {
        if (BattleManager.Instance == null) return;

        // Use reflection to set phase to Fighting
        var phaseField = typeof(BattleManager).GetField("_phase",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (phaseField != null)
        {
            phaseField.SetValue(BattleManager.Instance, BattlePhase.Fighting);
            Debug.Log("[SkillTestWindow] Battle started (test mode)");

            // Update targets for all units
            foreach (var unit in BattleManager.Instance.GetAllUnits())
            {
                unit.UpdateTarget();
            }

            // BattleStart 스킬 발동
            foreach (var unit in BattleManager.Instance.GetAllUnits())
            {
                unit.TriggerBattleStartSkills();
            }
        }
    }

    private void StopTestFighting()
    {
        if (BattleManager.Instance == null) return;

        var phaseField = typeof(BattleManager).GetField("_phase",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (phaseField != null)
        {
            phaseField.SetValue(BattleManager.Instance, BattlePhase.Ready);
            Debug.Log("[SkillTestWindow] Battle stopped (test mode)");
        }
    }
}
