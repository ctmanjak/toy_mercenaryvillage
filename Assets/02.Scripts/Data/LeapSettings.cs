using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "LeapSettings_", menuName = "Game/Leap Settings")]
    public class LeapSettings : ScriptableObject
    {
        [Header("타이밍")]
        [Tooltip("준비 동작 시간 (초)")]
        public float AnticipationTime = 0.12f;

        [Tooltip("체공 시간 비율 (duration 대비)")]
        [Range(0f, 0.3f)]
        public float HangTimeRatio = 0.1f;

        [Tooltip("착지 임팩트 시간 (초)")]
        public float LandingTime = 0.15f;

        [Header("준비 동작")]
        [Tooltip("뒤로 물러나는 거리")]
        public float BackDistance = 0.3f;

        [Tooltip("웅크리기 스케일 (X: 넓어짐, Y: 낮아짐)")]
        public Vector2 SquashScale = new Vector2(1.2f, 0.7f);

        [Header("도약")]
        [Tooltip("늘어나기 스케일 (X: 좁아짐, Y: 길어짐)")]
        public Vector2 StretchScale = new Vector2(0.8f, 1.3f);

        [Header("탑다운 높이 표현")]
        [Tooltip("정점에서 스프라이트의 Y 오프셋 (화면상 위로 이동)")]
        public float PeakHeightOffset = 0.5f;

        [Tooltip("정점에서 스케일 배율 (원근감 - 가까워 보이는 효과)")]
        public float PeakScaleMultiplier = 1.15f;

        [Tooltip("그림자 정점 스케일 (멀어지면 작아짐)")]
        public float ShadowPeakScale = 0.7f;

        [Header("착지")]
        [Tooltip("착지 시 찌그러짐 스케일")]
        public Vector2 LandSquashScale = new Vector2(1.3f, 0.6f);

        [Header("이징 커브")]
        [Tooltip("준비 동작 커브")]
        public AnimationCurve AnticipationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Tooltip("도약 상승 커브")]
        public AnimationCurve LaunchCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Tooltip("하강 커브")]
        public AnimationCurve DescentCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Tooltip("착지 찌그러짐 커브")]
        public AnimationCurve LandSquashCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Tooltip("착지 복귀 커브 (탄성)")]
        public AnimationCurve LandRecoveryCurve = CreateOvershootCurve();
        
        public static LeapSettings Default
        {
            get
            {
                var settings = CreateInstance<LeapSettings>();
                settings.InitializeDefaultCurves();
                return settings;
            }
        }

        private void OnEnable()
        {
            if (AnticipationCurve == null || AnticipationCurve.keys.Length == 0)
                InitializeDefaultCurves();
        }

        private void InitializeDefaultCurves()
        {
            AnticipationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            LaunchCurve = CreateEaseOutCurve();
            DescentCurve = CreateEaseInCurve();
            LandSquashCurve = CreateEaseOutCurve();
            LandRecoveryCurve = CreateOvershootCurve();
        }

        private static AnimationCurve CreateEaseOutCurve()
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 2f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        private static AnimationCurve CreateEaseInCurve()
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(1f, 1f, 2f, 0f));
            return curve;
        }

        private static AnimationCurve CreateOvershootCurve()
        {
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 3f));
            curve.AddKey(new Keyframe(0.7f, 1.1f, 0f, 0f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }
    }
}
