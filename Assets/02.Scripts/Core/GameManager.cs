using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public StageData CurrentStage { get; private set; }

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

        public void GoToTown()
        {
            SceneManager.LoadScene("TownScene");
        }

        public void GoToDungeonSelect()
        {
            SceneManager.LoadScene("DungeonSelectScene");
        }

        public void StartBattle(StageData stage)
        {
            CurrentStage = stage;
            SceneManager.LoadScene("BattleScene");
        }
    }
}
