using System;
using System.Collections.Generic;
using System.IO;
using Core;
using UnityEngine;

namespace Data
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private static string SavePath =>
            Path.Combine(Application.persistentDataPath, "save.json");

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadGame();
        }

        public void LoadGame()
        {
            var data = LoadFromFile();

            if (PlayerResourceManager.Instance != null)
            {
                var mercenaries = RestoreMercenaries(data?.Mercenaries);
                PlayerResourceManager.Instance.ApplyLoadedData(data?.Gold, mercenaries);
            }

            if (PartyManager.Instance != null)
            {
                PartyManager.Instance.ApplyLoadedParty(data?.PartyIds);
            }

            Debug.Log("[SaveManager] 게임 로드 완료");
        }

        public void SaveGame()
        {
            var data = new SaveData();

            if (PlayerResourceManager.Instance != null)
            {
                data.Gold = PlayerResourceManager.Instance.Gold;
                data.Mercenaries = PlayerResourceManager.Instance.GetMercenariesForSave();
            }

            if (PartyManager.Instance != null)
            {
                data.PartyIds = PartyManager.Instance.GetPartyIdsForSave();
            }

            SaveToFile(data);
        }

        #region Mercenary Restoration

        private List<MercenaryData> RestoreMercenaries(List<MercenarySaveData> saves)
        {
            if (saves == null || saves.Count == 0)
                return null;

            var result = new List<MercenaryData>();

            foreach (var save in saves)
            {
                var merc = save.ToMercenaryData();
                RestoreUnitDataReference(merc);
                result.Add(merc);
            }

            return result;
        }

        private void RestoreUnitDataReference(MercenaryData merc)
        {
            if (string.IsNullOrEmpty(merc.UnitDataId)) return;

            var unitData = GameDataManager.Instance?.GetUnitById(merc.UnitDataId);
            if (unitData != null)
            {
                merc.RestoreUnitData(unitData);
            }
            else
            {
                Debug.LogWarning($"[SaveManager] UnitData not found for ID: {merc.UnitDataId}");
            }
        }

        #endregion

        #region File I/O

        private SaveData LoadFromFile()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[SaveManager] 저장 파일 없음, 새 게임 시작");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                var data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("[SaveManager] 파일 로드 성공");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 로드 실패 (파일 손상?): {e.Message}");
                return null;
            }
        }

        private void SaveToFile(SaveData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
                Debug.Log($"[SaveManager] 저장 완료: {SavePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 저장 실패: {e.Message}");
            }
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("[SaveManager] 저장 파일 삭제됨");
            }
        }

        public bool HasSaveFile()
        {
            return File.Exists(SavePath);
        }

        #endregion
    }
}
