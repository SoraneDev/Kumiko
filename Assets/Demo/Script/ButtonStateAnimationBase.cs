using System.Collections;
using Kumiko;
using UnityEngine;


/// <summary>
/// KumikoButtonの表示状態に合わせて見た目を変える、デモ用の基底クラス
/// 押下の見た目を、最低でもPressedHoldDuration秒は保持する。
/// </summary>
public abstract class ButtonStateAnimationBase : MonoBehaviour
{
    [SerializeField] private KumikoButton _button;


    private InteractionState _pendingState;
    private bool _hasPending;
    private float _holdUntil;
    private Coroutine _holdCoroutine;


    /// <summary>押下の見た目を、最低でも保持する秒数</summary>
    protected abstract float PressedHoldDuration { get; }


    /// <summary>状態に対応する見た目を適用する。</summary>
    /// <param name="state">適用する状態</param>
    /// <param name="instant">アニメーションせず、すぐに適用するかどうか</param>
    protected abstract void Apply(InteractionState state, bool instant);


    /// <summary>実行中のアニメーションを止める。</summary>
    protected virtual void StopAnimation()
    {
    }


    private void OnEnable()
    {
        _button.StateChanged += OnStateChanged;

        // 有効になった時点の状態へ、すぐに合わせる。
        Apply(_button.State, true);
    }


    private void OnDisable()
    {
        _button.StateChanged -= OnStateChanged;

        CancelPending();
        StopAnimation();
        _holdUntil = 0f;
    }


    private void OnStateChanged(InteractionState state)
    {
        // 無効化中に通知が来た場合は、コルーチンを開始できないため何もしない。
        // 次に有効になったとき、OnEnableで現在の状態に合わせる。
        if (!isActiveAndEnabled) return;

        // 押下の保持時間内は、最新の状態だけを覚えて、保持が終わってから適用する。
        if (state != InteractionState.Pressed && Time.unscaledTime < _holdUntil)
        {
            _pendingState = state;
            _hasPending = true;
            _holdCoroutine ??= StartCoroutine(ApplyAfterHold());
            return;
        }

        CancelPending();

        if (state == InteractionState.Pressed)
        {
            _holdUntil = Time.unscaledTime + PressedHoldDuration;
        }

        Apply(state, false);
    }


    private IEnumerator ApplyAfterHold()
    {
        while (Time.unscaledTime < _holdUntil) yield return null;

        _holdCoroutine = null;

        if (!_hasPending) yield break;

        _hasPending = false;
        Apply(_pendingState, false);
    }


    private void CancelPending()
    {
        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }

        _hasPending = false;
    }
}