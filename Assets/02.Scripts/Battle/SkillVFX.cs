using Core;
using UnityEngine;

namespace Battle
{
    public class SkillVFX : MonoBehaviour, IPoolable
    {
        [Header("Animation")]
        [SerializeField] private float _scaleSpeed = 5f;
        [SerializeField] private float _maxScale = 1.5f;
        [SerializeField] private float _fadeSpeed = 2f;

        [Header("Color")]
        [SerializeField] private Color _color = Color.white;

        private SpriteRenderer _spriteRenderer;
        private float _currentScale;
        private float _alpha;
        private bool _isFading;
        private float _duration;
        private float _elapsedTime;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(float duration)
        {
            _duration = duration;
            _elapsedTime = 0f;
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;

            if (!_isFading)
            {
                _currentScale += _scaleSpeed * Time.deltaTime;
                if (_currentScale >= _maxScale)
                {
                    _currentScale = _maxScale;
                    _isFading = true;
                }
            }
            else
            {
                _alpha -= _fadeSpeed * Time.deltaTime;
                if (_alpha <= 0f)
                {
                    _alpha = 0f;
                }
            }

            transform.localScale = Vector3.one * _currentScale;

            if (_spriteRenderer != null)
            {
                var color = _spriteRenderer.color;
                color.a = _alpha;
                _spriteRenderer.color = color;
            }
            
            if (_duration > 0f && _elapsedTime >= _duration)
            {
                PoolManager.Instance.Release(gameObject);
            }
        }

        public void OnGetFromPool()
        {
            _currentScale = 0f;
            _alpha = 1f;
            _isFading = false;
            _elapsedTime = 0f;

            transform.localScale = Vector3.zero;

            if (_spriteRenderer != null)
            {
                var color = _color;
                color.a = 1f;
                _spriteRenderer.color = color;
            }
        }

        public void OnReleaseToPool()
        {
            _duration = 0f;
        }
    }
}
