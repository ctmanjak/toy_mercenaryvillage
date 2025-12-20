using Core;
using UnityEngine;

namespace Battle
{
    public class DamagePopupSpawner : MonoBehaviour
    {
        public static DamagePopupSpawner Instance { get; private set; }

        [SerializeField] private DamagePopup _prefab;
        [SerializeField] private float _randomOffsetX = 0.2f;
        [SerializeField] private float _minRandomOffsetY = 0.3f;
        [SerializeField] private float _maxRandomOffsetY = 1f;

        private void Awake()
        {
            Instance = this;
        }

        public void Show(Vector3 position, int damage, bool isAllyDamage)
        {
            var popup = PoolManager.Instance.Get(_prefab);
            popup.transform.position = position + GetRandomOffset();
            popup.Setup(damage, isAllyDamage);
        }

        private Vector3 GetRandomOffset()
        {
            return new Vector3(
                Random.Range(-_randomOffsetX, _randomOffsetX),
                Random.Range(_minRandomOffsetY, _maxRandomOffsetY),
                0f
            );
        }
    }
}