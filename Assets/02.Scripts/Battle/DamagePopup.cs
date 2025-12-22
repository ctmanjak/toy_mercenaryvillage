using System.Collections;
using Core;
using TMPro;
using UnityEngine;

namespace Battle
{
    public class DamagePopup : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshProUGUI _damageText;
        [SerializeField] private TextMeshProUGUI[] _texts;
        [SerializeField] private float _duration = 0.6f;
        [SerializeField] private float _floatDistance = 1f;
        [SerializeField] private AnimationCurve _moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        [Header("Colors")]
        [SerializeField] private Color _allyDamageColor = Color.red;
        [SerializeField] private Color _enemyDamageColor = Color.white;
        [SerializeField] private Color _healColor = Color.green;

        private Vector3 _startPos;
        private Color[] _startColors;

        public void Setup(int damage, bool isAllyDamage)
        {
            SetupInternal(damage.ToString(), isAllyDamage ? _allyDamageColor : _enemyDamageColor);
        }

        public void SetupHeal(int healAmount)
        {
            SetupInternal($"+{healAmount}", _healColor);
        }

        private void SetupInternal(string text, Color color)
        {
            _damageText.color = color;
            _startPos = transform.position;

            if (_startColors == null || _startColors.Length != _texts.Length)
            {
                _startColors = new Color[_texts.Length];
            }

            for (int i = 0; i < _texts.Length; i++)
            {
                _texts[i].text = text;
                _startColors[i] = _texts[i].color;
            }

            StartCoroutine(AnimateAndReturn());
        }

        private IEnumerator AnimateAndReturn()
        {
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _duration;

                float moveT = _moveCurve.Evaluate(t);
                float fadeT = _fadeCurve.Evaluate(t);

                transform.position = _startPos + Vector3.up * (_floatDistance * moveT);

                for (int i = 0; i < _texts.Length; i++)
                {
                    var c = _startColors[i];
                    _texts[i].color = new Color(c.r, c.g, c.b, fadeT);
                }

                yield return null;
            }

            PoolManager.Instance.Release(this);
        }

        public void OnGetFromPool() { }

        public void OnReleaseToPool()
        {
            StopAllCoroutines();
        }
    }
}