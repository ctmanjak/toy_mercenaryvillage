using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialSize = 10;
        public int maxSize = 100;
    }
}