using System.Collections;
using Kumiko;
using UnityEngine;
using UnityEngine.UI;


public class ButtonColorAnimation : ButtonStateAnimationBase
{
    [SerializeField] private Image _image;
    [Header("Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hoveredColor = Color.white;
    [SerializeField] private Color _focusedColor = Color.white;
    [SerializeField] private Color _pressedColor = Color.white;
    [SerializeField] private Color _disabledColor = Color.white;
    [Header("Animation")]
    [SerializeField, Min(0f)] private float _duration = 0.15f;


    private Coroutine _currentAnimation;


    // 押下の色が、最後まで再生される長さだけ保持する。
    protected override float PressedHoldDuration => _duration;


    protected override void Apply(InteractionState state, bool instant)
    {
        Color target = state switch
        {
            InteractionState.Normal => _normalColor,
            InteractionState.Hovered => _hoveredColor,
            InteractionState.Focused => _focusedColor,
            InteractionState.Pressed => _pressedColor,
            InteractionState.Disabled => _disabledColor,
            _ => _normalColor
        };

        StopAnimation();

        if (instant || _duration <= 0f)
        {
            _image.color = target;
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

    
    private IEnumerator Lerp(Color target)
    {
        Color start = _image.color;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / _duration);
            _image.color = Color.Lerp(start, target, t);

            yield return null;
        }

        _image.color = target;
        _currentAnimation = null;
    }
}