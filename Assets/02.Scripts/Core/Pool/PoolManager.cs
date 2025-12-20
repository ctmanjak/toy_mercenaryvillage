using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Core
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        [SerializeField] private List<PoolConfig> _preregisteredPools = new();

        private readonly Dictionary<GameObject, IObjectPool<GameObject>> _pools = new();
        private readonly Dictionary<GameObject, PoolConfig> _configs = new();
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();

        private void Awake()
        {
            Instance = this;
            InitializePreregisteredPools();
        }

        private void InitializePreregisteredPools()
        {
            foreach (var config in _preregisteredPools)
            {
                if (config.prefab == null) continue;

                _configs[config.prefab] = config;
                var pool = CreatePool(config.prefab, config.initialSize, config.maxSize);
                _pools[config.prefab] = pool;

                var instances = new List<GameObject>();
                for (int i = 0; i < config.initialSize; i++)
                {
                    instances.Add(pool.Get());
                }
                foreach (var instance in instances)
                {
                    pool.Release(instance);
                }
            }
        }

        public T Get<T>(T prefab) where T : Component
        {
            var go = GetGameObject(prefab.gameObject);
            return go.GetComponent<T>();
        }

        public GameObject Get(GameObject prefab)
        {
            return GetGameObject(prefab);
        }

        public void Release(GameObject instance)
        {
            if (_instanceToPrefab.TryGetValue(instance, out var prefab))
            {
                _pools[prefab].Release(instance);
            }
            else
            {
                Debug.LogWarning($"[PoolManager] Unknown instance: {instance.name}");
                Destroy(instance);
            }
        }

        public void Release<T>(T instance) where T : Component
        {
            Release(instance.gameObject);
        }

        private GameObject GetGameObject(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = CreatePool(prefab, 10, 100);
                _pools[prefab] = pool;
            }
            return pool.Get();
        }

        private IObjectPool<GameObject> CreatePool(GameObject prefab, int defaultCapacity, int maxSize)
        {
            return new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var instance = Instantiate(prefab, transform);
                    _instanceToPrefab[instance] = prefab;
                    return instance;
                },
                actionOnGet: obj =>
                {
                    obj.SetActive(true);
                    if (obj.TryGetComponent<IPoolable>(out var poolable))
                        poolable.OnGetFromPool();
                },
                actionOnRelease: obj =>
                {
                    if (obj.TryGetComponent<IPoolable>(out var poolable))
                        poolable.OnReleaseToPool();
                    obj.SetActive(false);
                },
                actionOnDestroy: obj =>
                {
                    _instanceToPrefab.Remove(obj);
                    Destroy(obj);
                },
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }
    }
}