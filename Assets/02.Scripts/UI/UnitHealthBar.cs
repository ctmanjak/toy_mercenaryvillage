using Battle;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitHealthBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _fillImage;

        [Header("Settings")]
        [SerializeField] private Vector3 _offset = new Vector3(0, 1.5f, 0);

        [Header("Team Appearance")]
        [SerializeField] private Sprite _allyFillSprite;
        [SerializeField] private Sprite _enemyFillSprite;
        [SerializeField] private Color _allyColor = Color.blue;
        [SerializeField] private Color _enemyColor = Color.red;

        private BattleUnit _unit;
        private Transform _unitTransform;
        private Camera _camera;

        public void Initialize(BattleUnit unit)
        {
            _unit = unit;
            _unitTransform = unit.transform;
            _camera = Camera.main;

            SetTeamAppearance(unit.Team);
            UpdateHealthBar();
        }

        private void LateUpdate()
        {
            if (_unit == null) return;

            FollowUnit();
            FaceCamera();
        }

        private void FollowUnit()
        {
            transform.position = _unitTransform.position + _offset;
        }

        private void FaceCamera()
        {
            if (_camera != null)
            {
                transform.forward = _camera.transform.forward;
            }
        }

        private void SetTeamAppearance(Team team)
        {
            if (_fillImage == null) return;

            bool isAlly = team == Team.Ally;

            Sprite sprite = isAlly ? _allyFillSprite : _enemyFillSprite;
            if (sprite != null)
            {
                _fillImage.sprite = sprite;
            }

            _fillImage.color = isAlly ? _allyColor : _enemyColor;
        }

        private const float MIN_VISIBLE_VALUE = 0.188f;

        public void UpdateHealthBar()
        {
            if (_unit == null || _slider == null) return;

            float ratio = _unit.HealthRatio;

            if (ratio > 0f && ratio < MIN_VISIBLE_VALUE)
            {
                ratio = MIN_VISIBLE_VALUE;
            }

            _slider.value = ratio;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
