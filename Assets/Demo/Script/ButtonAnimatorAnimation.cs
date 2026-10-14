using Kumiko;
using UnityEngine;


/// <summary>
/// KumikoButtonの表示状態に合わせて、Animatorのトリガーを切り替えるデモ用のクラス
/// </summary>
public class ButtonAnimatorAnimation : ButtonStateAnimationBase
{
    [SerializeField] private Animator _animator;
    [Header("Triggers")]
    [SerializeField] private string _normalTrigger = "Normal";
    [SerializeField] private string _hoveredTrigger = "Hovered";
    [SerializeField] private string _focusedTrigger = "Focused";
    [SerializeField] private string _pressedTrigger = "Pressed";
    [SerializeField] private string _disabledTrigger = "Disabled";
    [Header("Animation")]
    [SerializeField, Min(0f), Tooltip("Pressedのアニメーションを保持する秒数")] private float _pressedHoldDuration = 0.15f;


    private int _normalHash;
    private int _hoveredHash;
    private int _focusedHash;
    private int _pressedHash;
    private int _disabledHash;


    // Pressedのクリップの長さに合わせて設定する。
    protected override float PressedHoldDuration => _pressedHoldDuration;


    private void Awake()
    {
        _normalHash = Animator.StringToHash(_normalTrigger);
        _hoveredHash = Animator.StringToHash(_hoveredTrigger);
        _focusedHash = Animator.StringToHash(_focusedTrigger);
        _pressedHash = Animator.StringToHash(_pressedTrigger);
        _disabledHash = Animator.StringToHash(_disabledTrigger);
    }


    // instantは、トリガー方式では使わない。遷移の長さは、Animatorの遷移設定に従う。
    protected override void Apply(InteractionState state, bool instant)
    {
        int trigger = state switch
        {
            InteractionState.Normal => _normalHash,
            InteractionState.Hovered => _hoveredHash,
            InteractionState.Focused => _focusedHash,
            InteractionState.Pressed => _pressedHash,
            InteractionState.Disabled => _disabledHash,
            _ => _normalHash
        };

        _animator.ResetTrigger(_normalHash);
        _animator.ResetTrigger(_hoveredHash);
        _animator.ResetTrigger(_focusedHash);
        _animator.ResetTrigger(_pressedHash);
        _animator.ResetTrigger(_disabledHash);

        _animator.SetTrigger(trigger);
    }
}