using System;
using Data;
using DG.Tweening;
using UnityEngine;

namespace Battle
{
    public static class LeapAnimator
    {
        private struct LeapContext
        {
            public Transform Root;
            public Transform VisualRoot;
            public Transform Shadow;
            public LeapSettings Settings;
            public UnitAnimator UnitAnimator;

            public Vector3 StartPos;
            public Vector3 EndPos;
            public Vector3 Direction;

            public Vector3 OriginalSpriteLocalPos;
            public Vector3 OriginalScale;
            public Vector3 OriginalShadowScale;

            public float Duration;
            public float JumpHeight;
        }

        public static void Play(
            Transform root,
            Transform visualRoot,
            Transform shadow,
            Vector3 targetPos,
            float stopDistance,
            float duration,
            float jumpHeight,
            LeapSettings settings,
            UnitAnimator unitAnimator = null,
            Action onComplete = null)
        {
            if (settings == null)
                settings = LeapSettings.Default;

            var ctx = CreateContext(root, visualRoot, shadow, targetPos, stopDistance, duration, jumpHeight, settings, unitAnimator);
            
            ctx.UnitAnimator?.SetJumping(true);

            var sequence = BuildSequence(ctx);

            sequence.OnComplete(() =>
            {
                ResetVisuals(ctx);
                onComplete?.Invoke();
            });
        }

        private static LeapContext CreateContext(
            Transform root,
            Transform visualRoot,
            Transform shadow,
            Vector3 targetPos,
            float stopDistance,
            float duration,
            float jumpHeight,
            LeapSettings settings,
            UnitAnimator unitAnimator)
        {
            Vector3 startPos = root.position;
            Vector3 direction = (targetPos - startPos).normalized;
            Vector3 endPos = targetPos - direction * stopDistance;

            return new LeapContext
            {
                Root = root,
                VisualRoot = visualRoot ?? root,
                Shadow = shadow,
                Settings = settings,
                UnitAnimator = unitAnimator,
                StartPos = startPos,
                EndPos = endPos,
                Direction = direction,
                OriginalSpriteLocalPos = (visualRoot ?? root).localPosition,
                OriginalScale = (visualRoot ?? root).localScale,
                OriginalShadowScale = shadow != null ? shadow.localScale : Vector3.one,
                Duration = duration,
                JumpHeight = jumpHeight
            };
        }

        private static Sequence BuildSequence(LeapContext ctx)
        {
            var s = ctx.Settings;
            var seq = DOTween.Sequence();
            
            float remainingRatio = (1f - s.HangTimeRatio) * 0.5f;
            var timing = new Timing
            {
                Anticipation = s.AnticipationTime,
                Launch = ctx.Duration * remainingRatio,
                Hang = ctx.Duration * s.HangTimeRatio,
                Descent = ctx.Duration * remainingRatio,
                Landing = s.LandingTime
            };
            
            Vector3 anticipationPos = ctx.StartPos - ctx.Direction * s.BackDistance;
            Vector3 midPos = Vector3.Lerp(ctx.StartPos, ctx.EndPos, 0.5f);
            Vector3 hangPos = Vector3.Lerp(ctx.StartPos, ctx.EndPos, 0.6f);
            
            Vector3 squashScale = MultiplyScale(ctx.OriginalScale, s.SquashScale);
            Vector3 peakScale = ctx.OriginalScale * s.PeakScaleMultiplier;
            Vector3 landSquashScale = MultiplyScale(ctx.OriginalScale, s.LandSquashScale);
            
            Vector3 peakOffset = new Vector3(0f, s.PeakHeightOffset * ctx.JumpHeight, 0f);
            
            AppendAnticipation(seq, ctx, anticipationPos, squashScale, timing.Anticipation);
            
            AppendLaunch(seq, ctx, midPos, peakScale, peakOffset, timing.Launch);
            
            AppendHang(seq, ctx, hangPos, timing.Hang);
            
            AppendDescent(seq, ctx, timing.Descent);

            seq.AppendCallback(() => ctx.UnitAnimator?.SetJumping(false));

            AppendLanding(seq, ctx, landSquashScale, timing.Landing);

            return seq;
        }

        private static void AppendAnticipation(Sequence seq, LeapContext ctx, Vector3 pos, Vector3 scale, float time)
        {
            var curve = ctx.Settings.AnticipationCurve;
            seq.Append(ctx.Root.DOMove(pos, time).SetEase(curve));
            seq.Join(ctx.VisualRoot.DOScale(scale, time).SetEase(curve));
        }

        private static void AppendLaunch(Sequence seq, LeapContext ctx, Vector3 groundPos, Vector3 peakScale, Vector3 peakOffset, float time)
        {
            var curve = ctx.Settings.LaunchCurve;

            seq.Append(ctx.Root.DOMove(groundPos, time).SetEase(curve));
            seq.Join(ctx.VisualRoot.DOLocalMove(ctx.OriginalSpriteLocalPos + peakOffset, time).SetEase(curve));
            seq.Join(ctx.VisualRoot.DOScale(peakScale, time).SetEase(curve));

            if (ctx.Shadow != null)
            {
                seq.Join(ctx.Shadow.DOScale(ctx.OriginalShadowScale * ctx.Settings.ShadowPeakScale, time).SetEase(curve));
            }
        }

        private static void AppendHang(Sequence seq, LeapContext ctx, Vector3 hangPos, float time)
        {
            seq.Append(ctx.Root.DOMove(hangPos, time).SetEase(Ease.Linear));
        }

        private static void AppendDescent(Sequence seq, LeapContext ctx, float time)
        {
            var curve = ctx.Settings.DescentCurve;

            seq.Append(ctx.Root.DOMove(ctx.EndPos, time).SetEase(curve));
            seq.Join(ctx.VisualRoot.DOLocalMove(ctx.OriginalSpriteLocalPos, time).SetEase(curve));
            seq.Join(ctx.VisualRoot.DOScale(ctx.OriginalScale, time).SetEase(curve));

            if (ctx.Shadow != null)
            {
                seq.Join(ctx.Shadow.DOScale(ctx.OriginalShadowScale, time).SetEase(curve));
            }
        }

        private static void AppendLanding(Sequence seq, LeapContext ctx, Vector3 landSquashScale, float time)
        {
            seq.Append(ctx.VisualRoot.DOScale(landSquashScale, time * 0.4f).SetEase(ctx.Settings.LandSquashCurve));
            seq.Append(ctx.VisualRoot.DOScale(ctx.OriginalScale, time * 0.6f).SetEase(ctx.Settings.LandRecoveryCurve));
        }

        private static void ResetVisuals(LeapContext ctx)
        {
            ctx.VisualRoot.localScale = ctx.OriginalScale;
            ctx.VisualRoot.localPosition = ctx.OriginalSpriteLocalPos;
            if (ctx.Shadow != null)
                ctx.Shadow.localScale = ctx.OriginalShadowScale;
        }

        private static Vector3 MultiplyScale(Vector3 original, Vector2 multiplier)
        {
            return new Vector3(original.x * multiplier.x, original.y * multiplier.y, original.z);
        }

        private struct Timing
        {
            public float Anticipation;
            public float Launch;
            public float Hang;
            public float Descent;
            public float Landing;
        }
    }
}
