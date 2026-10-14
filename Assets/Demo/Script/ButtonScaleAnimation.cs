using System.Collections;
using Kumiko;
using UnityEngine;


/// <summary>
/// KumikoButtonの表示状態に合わせて、スケールを変えるデモ用のクラス
/// </summary>
public class ButtonScaleAnimation : ButtonStateAnimationBase
{
    [SerializeField] private RectTransform _target;
    [Header("Scales")]
    [SerializeField] private Vector3 _normalScale = Vector3.one;
    [SerializeField] private Vector3 _hoveredScale = Vector3.one * 1.1f;
    [SerializeField] private Vector3 _focusedScale = Vector3.one * 1.1f;
    [SerializeField] private Vector3 _pressedScale = Vector3.one * 0.95f;
    [SerializeField] private Vector3 _disabledScale = Vector3.one;
    [Header("Animation")]
    [SerializeField, Min(0f)] private float _duration = 0.15f;


    private Coroutine _currentAnimation;


    // 押下のスケールが、最後まで再生される長さだけ保持する。
    protected override float PressedHoldDuration => _duration;


    protected override void Apply(InteractionState state, bool instant)
    {
        Vector3 target = state switch
        {
            InteractionState.Normal => _normalScale,
            InteractionState.Hovered => _hoveredScale,
            InteractionState.Focused => _focusedScale,
            InteractionState.Pressed => _pressedScale,
            InteractionState.Disabled => _disabledScale,
            _ => _normalScale
        };

        StopAnimation();

        if (instant || _duration <= 0f)
        {
            _target.localScale = target;
            return;
        }

        _currentAnimation = StartCoroutine(Lerp(target));
    }


    protected override void StopAnimation()
    {
        if (_currentAnimation == null) return;

        StopCoroutine(_currentAnimation);
        _currentAnimation = null;
    }


    private IEnumerator Lerp(Vector3 target)
    {
        Vector3 start = _target.localScale;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / _duration);
            _target.localScale = Vector3.Lerp(start, target, t);

            yield return null;
        }

        _target.localScale = target;
        _currentAnimation = null;
    }
}