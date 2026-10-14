using System.Collections;
using Kumiko;
using UnityEngine;
using UnityEngine.UI;


public class ElementAnimation : MonoBehaviour
{
    [SerializeField] private KumikoElement _element;
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
    
    
    private void OnEnable()
    {
        _element.StateChanged += Apply;

        // 有効になった時点の状態へ、すぐに合わせる。
        Apply(_element.State);
    }


    private void OnDisable()
    {
        _element.StateChanged -= Apply;
        
        StopAnimation();
    }
    
    
    private void Apply(InteractionState state)
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

        if (_duration <= 0f)
        {
            _image.color = target;
            return;
        }

        _currentAnimation = StartCoroutine(Lerp(target));
    }
    
    
    private void StopAnimation()
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
