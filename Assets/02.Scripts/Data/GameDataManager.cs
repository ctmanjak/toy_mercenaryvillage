using UnityEngine;

namespace Data
{
    public class GameDataManager : MonoBehaviour
    {
        public static GameDataManager Instance { get; private set; }

        [Header("Databases")]
        [SerializeField] private UnitDatabase _unitDatabase;

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

        public UnitData GetUnitById(string unitId)
        {
            return _unitDatabase?.GetById(unitId);
        }

        public UnitDatabase UnitDatabase => _unitDatabase;
    }
}
