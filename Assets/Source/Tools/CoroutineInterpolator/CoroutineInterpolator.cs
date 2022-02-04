using System;
using UnityEngine;

namespace Tools
{
	public partial class CoroutineInterpolator
    {
        public static Coroutine InterpolateUnmanaged(MonoBehaviour monoBehaviour, float startValue, float completeValue, float time, Action<float> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return monoBehaviour.StartCoroutine(InterpolProcUnmanaged(time, onInterpolationComplete, interpolation, new InterpolatorFloat(startValue, completeValue, onInterpolationProc))); }

        public static Coroutine InterpolateUnmanaged(MonoBehaviour monoBehaviour, Vector2 startValue, Vector2 completeValue, float time, Action<Vector2> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return monoBehaviour.StartCoroutine(InterpolProcUnmanaged(time, onInterpolationComplete, interpolation, new InterpolatorVector2(startValue, completeValue, onInterpolationProc))); }

        public static Coroutine InterpolateUnmanaged(MonoBehaviour monoBehaviour, Vector3 startValue, Vector3 completeValue, float time, Action<Vector3> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return monoBehaviour.StartCoroutine(InterpolProcUnmanaged(time, onInterpolationComplete, interpolation, new InterpolatorVector3(startValue, completeValue, onInterpolationProc))); }

        public static Coroutine InterpolateUnmanaged(MonoBehaviour monoBehaviour, Vector4 startValue, Vector4 completeValue, float time, Action<Vector4> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return monoBehaviour.StartCoroutine(InterpolProcUnmanaged(time, onInterpolationComplete, interpolation, new InterpolatorVector4(startValue, completeValue, onInterpolationProc))); }

        public static Coroutine InterpolateUnmanaged(MonoBehaviour monoBehaviour, Color startValue, Color completeValue, float time, Action<Color> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return monoBehaviour.StartCoroutine(InterpolProcUnmanaged(time, onInterpolationComplete, interpolation, new InterpolatorColor(startValue, completeValue, onInterpolationProc))); }

        public Coroutine Interpolate(float startValue, float completeValue, float time, Action<float> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return m_MonoBehaviour.StartCoroutine(InterpolProc(time, onInterpolationComplete, interpolation, new InterpolatorFloat(startValue, completeValue, onInterpolationProc))); }

        public Coroutine Interpolate(Vector2 startValue, Vector2 completeValue, float time, Action<Vector2> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return m_MonoBehaviour.StartCoroutine(InterpolProc(time, onInterpolationComplete, interpolation, new InterpolatorVector2(startValue, completeValue, onInterpolationProc))); }

        public Coroutine Interpolate(Vector3 startValue, Vector3 completeValue, float time, Action<Vector3> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return m_MonoBehaviour.StartCoroutine(InterpolProc(time, onInterpolationComplete, interpolation, new InterpolatorVector3(startValue, completeValue, onInterpolationProc))); }

        public Coroutine Interpolate(Vector4 startValue, Vector4 completeValue, float time, Action<Vector4> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return m_MonoBehaviour.StartCoroutine(InterpolProc(time, onInterpolationComplete, interpolation, new InterpolatorVector4(startValue, completeValue, onInterpolationProc))); }

        public Coroutine Interpolate(Color startValue, Color completeValue, float time, Action<Color> onInterpolationProc, Action onInterpolationComplete = null, Interpolations.eInterpolation interpolation = Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR)
        { return m_MonoBehaviour.StartCoroutine(InterpolProc(time, onInterpolationComplete, interpolation, new InterpolatorColor(startValue, completeValue, onInterpolationProc))); }

        public static Coroutine TimerUnmanaged(MonoBehaviour monoBehaviour, float time, Action onComplete)
        { return monoBehaviour.StartCoroutine(InterpolProcUnmanaged(time, onComplete, Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR, new InterpolatorEmpty())); }

        public Coroutine Timer(float time, Action onComplete)
        { return m_MonoBehaviour.StartCoroutine(InterpolProc(time, onComplete, Interpolations.eInterpolation.INTERPOLATE_TYPE_LINEAR, new InterpolatorEmpty())); }

        public CoroutineInterpolator(MonoBehaviour monoBehaviour)
        { CoroutineInterpolator_w(monoBehaviour); }

        public void Skip()
        { Skip_w(); }

        public void ForcedComplete()
        { ForcedComplete_w(); }

        public void SetTimeScale(float timeScale)
        { SetTimeScale_w(timeScale); }

        public float GetTimeScale()
        { return GetTimeScale_w(); }

        public void SetUnscaledTime()
        { SetUnscaledTime_w(); }

        public void SetScaledTime()
        { SetScaledTime_w(); }

        public void SetFixedTime()
        { SetFixedTime_w(); }
    }
}
