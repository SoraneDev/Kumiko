using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Kumiko
{
    /// <summary>
    /// UIをインタラクション可能にする。
    /// 継承してボタンなどの機能を追加できる。
    /// また、モジュールを同じGameObjectにアタッチして機能を拡張できる。
    /// </summary>
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Kumiko/KumikoElement")]
    public class KumikoElement : UIBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        ISelectHandler, IDeselectHandler, ISubmitHandler, ICancelHandler, IMoveHandler
    {
        [SerializeField] private bool _interactable = true;


        private const InteractionFlags InputFlags =
            InteractionFlags.Hovered | InteractionFlags.Focused | InteractionFlags.Pressed;


        private readonly List<CanvasGroup> _canvasGroupBuffer = new();
        private bool _groupsAllowInteraction = true;
        private InteractionFlags _flags = InteractionFlags.None;


        /// <summary>
        /// 表示状態が変化したときに通知される。
        /// </summary>
        /// <remarks>
        /// 実効的な操作可能状態、ポインタ・選択の入力、入力種別の切り替えで変化する。
        /// Submit時は、押下と解除が続けて通知される。
        /// 購読した時点では通知されないため、現在の状態はStateで取得すること。
        /// 不要になったら購読を解除すること。
        /// </remarks>
        public event Action<InteractionState> StateChanged;


        /// <summary>
        /// 表示状態が変化したときに通知される。
        /// 内部用。利用側の例外の影響を受けないように、先に通知される。
        /// </summary>
        /// <inheritdoc cref="StateChanged"/>
        internal event Action<InteractionState> InternalStateChanged;


        /// <summary>現在のUI要素の表示状態。</summary>
        public InteractionState State { get; private set; }


        /// <summary>自身の操作可能状態。CanvasGroupの状態は含まない。</summary>
        /// <remarks>実効的な操作可能状態はIsInteractableで確認すること。</remarks>
        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                SyncDisabledFlag();
                DeselectIfDisabled();
            }
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            _groupsAllowInteraction = CanvasGroupsAllowInteraction();

            // 選択されたままだった場合は、フォーカスを復元する。
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                _flags = _flags.Add(InteractionFlags.Focused);
            }

            SyncDisabledFlag();

            InputModalityTracker.Current.InternalModalityChanged += OnModalityChanged;
        }


        protected override void OnDisable()
        {
            base.OnDisable();

            InputModalityTracker.Current.InternalModalityChanged -= OnModalityChanged;

            // 入力由来のフラグだけ解除する。Disabledは操作可否から決まるので維持する。
            _flags = _flags.Remove(InputFlags);
            RefreshState();
        }


        protected override void OnCanvasGroupChanged()
        {
            base.OnCanvasGroupChanged();

            _groupsAllowInteraction = CanvasGroupsAllowInteraction();
            SyncDisabledFlag();
            DeselectIfDisabled();
        }


        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            // 親が変わると、影響を受けるCanvasGroupも変わるため再評価する。
            OnCanvasGroupChanged();
        }


        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                return;
            }

            // アプリケーションのフォーカスを失うとPointerUpが呼ばれないため、押下を解除する。
            _flags = _flags.Remove(InteractionFlags.Pressed);
            RefreshState();
        }


#if UNITY_EDITOR


        protected override void OnValidate()
        {
            base.OnValidate();

            if (!Application.isPlaying)
            {
                return;
            }

            SyncDisabledFlag();
            DeselectIfDisabled();
        }


#endif


        public void OnPointerEnter(PointerEventData eventData)
        {
            // ポインタが動いていないのにUIが動いて発火した場合は、ユーザーの操作ではないため、入力種別を報告しない。
            if (eventData.delta.sqrMagnitude > 0f)
            {
                InputModalityTracker.Current.Report(PointerKindClassifier.Classify(eventData));
            }

            _flags = _flags.Add(InteractionFlags.Hovered);
            RefreshState();
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            _flags = _flags.Remove(InteractionFlags.Hovered);
            RefreshState();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            InputModalityTracker.Current.Report(PointerKindClassifier.Classify(eventData));

            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            // クリックまたはタップした要素へ選択を移す。
            // 後でゲームパッドに切り替えたときに、最後に触れた要素から操作を始められるようにするため。
            if (IsInteractable()
                && EventSystem.current != null
                && EventSystem.current.currentSelectedGameObject != gameObject)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }

            _flags = _flags.Add(InteractionFlags.Pressed);
            RefreshState();

            if (!IsInteractable())
            {
                return;
            }

            OnPointerDownInput(eventData);
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            _flags = _flags.Remove(InteractionFlags.Pressed);
            RefreshState();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (!IsInteractable())
            {
                return;
            }

            OnConfirmInput();
        }


        public void OnSelect(BaseEventData eventData)
        {
            _flags = _flags.Add(InteractionFlags.Focused);
            RefreshState();
        }


        public void OnDeselect(BaseEventData eventData)
        {
            _flags = _flags.Remove(InteractionFlags.Focused);
            RefreshState();
        }


        public void OnSubmit(BaseEventData eventData)
        {
            InputModalityTracker.Current.Report(InputModality.Navigation);

            // Submit入力には押している期間がないため、押下と解除を続けて通知する。
            // 押下の見た目をどれだけ保持するかは購読側に委ねる。
            try
            {
                _flags = _flags.Add(InteractionFlags.Pressed);
                RefreshState();
            }
            finally
            {
                // 購読側の例外で、押下が残らないようにする。
                _flags = _flags.Remove(InteractionFlags.Pressed);
                RefreshState();
            }

            if (!IsInteractable())
            {
                return;
            }

            OnConfirmInput();
        }


        public void OnCancel(BaseEventData eventData)
        {
            InputModalityTracker.Current.Report(InputModality.Navigation);
        }


        public void OnMove(AxisEventData eventData)
        {
            InputModalityTracker.Current.Report(InputModality.Navigation);

            if (!IsInteractable())
            {
                return;
            }

            if (!UsesMoveInput(eventData.moveDir))
            {
                return;
            }

            OnMoveInput(eventData.moveDir);
        }


        /// <summary>
        /// 自身とCanvasGroupによる操作可否を考慮した、実効的な操作可能状態を取得する。
        /// </summary>
        /// <returns>操作可能な場合はtrue、操作不可能な場合はfalseを返す。</returns>
        public bool IsInteractable()
        {
            return _groupsAllowInteraction && _interactable;
        }


        /// <summary>
        /// 方向入力を、要素自身の操作に使うかどうかを取得する。
        /// 派生クラスで上書きして、値の変更でtrueを返す。基底は常にfalseを返す。
        /// </summary>
        /// <param name="direction">方向入力の向き。</param>
        /// <returns>自身の操作に使う場合はtrue、使わない場合はfalseを返す。</returns>
        /// <remarks>trueを返した方向の入力を受けると、OnMoveInputが呼ばれる。</remarks>
        protected internal virtual bool UsesMoveInput(MoveDirection direction)
        {
            return false;
        }


        /// <summary>
        /// 決定入力（クリック・タップ・Submit）を、操作可能な状態で受けたときに呼ばれる。
        /// 派生クラスで上書きして、決定時の処理を実装する。基底は何もしない。
        /// </summary>
        protected virtual void OnConfirmInput()
        {
        }


        /// <summary>
        /// ポインタの押下（マウスの左ボタン・タッチなど）を、操作可能な状態で受けたときに呼ばれる。
        /// 派生クラスで上書きして、押した位置に応じた処理を実装する。基底は何もしない。
        /// </summary>
        /// <param name="eventData">押下のイベント情報。</param>
        /// <remarks>呼ばれる時点で、表示状態はPressedになっている。</remarks>
        protected virtual void OnPointerDownInput(PointerEventData eventData)
        {
        }


        /// <summary>
        /// UsesMoveInputでtrueを返した方向入力を、操作可能な状態で受けたときに呼ばれる。
        /// 派生クラスで上書きして、方向入力による処理を実装する。基底は何もしない。
        /// </summary>
        /// <param name="direction">方向入力の向き。</param>
        protected virtual void OnMoveInput(MoveDirection direction)
        {
        }


        private void OnModalityChanged(InputModality _)
        {
            try
            {
                RefreshState();
            }
            catch (Exception exception)
            {
                // 利用側の購読者の例外で、ほかの要素への通知が止まらないようにする。
                Debug.LogException(exception);
            }
        }


        private void RefreshState()
        {
            // 現在の入力種別と状態フラグから、表示する状態を取得し通知する。
            InputModality modality = InputModalityTracker.Current.Modality;
            InteractionState nextState = InteractionStateResolver.Resolve(_flags, modality);

            SetState(nextState);
        }


        private void SetState(InteractionState next)
        {
            if (State == next)
            {
                return;
            }

            State = next;

            InternalStateChanged?.Invoke(State);
            StateChanged?.Invoke(State);
        }


        private void SyncDisabledFlag()
        {
            _flags = _flags.Set(InteractionFlags.Disabled, !IsInteractable());
            RefreshState();
        }


        private void DeselectIfDisabled()
        {
            // 選択中の要素が操作不可能になったら、選択を外す。
            if (IsInteractable())
            {
                return;
            }

            if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject != gameObject)
            {
                return;
            }

            EventSystem.current.SetSelectedGameObject(null);
        }


        private bool CanvasGroupsAllowInteraction()
        {
            Transform current = transform;

            // 自身から親方向に向かって探索し、CanvasGroupが操作を許可しているかを返す。
            while (current != null)
            {
                current.GetComponents(_canvasGroupBuffer);
                foreach (CanvasGroup canvasGroup in _canvasGroupBuffer)
                {
                    if (canvasGroup.enabled && !canvasGroup.interactable)
                    {
                        return false;
                    }

                    // ignoreParentGroupsが有効なCanvasGroupで探索を止める。
                    // ignoreParentGroupsはenabledの状態に関わらずチェックする。
                    // Unity標準のSelectableと同じ判定方法にしている。
                    if (canvasGroup.ignoreParentGroups)
                    {
                        return true;
                    }
                }

                current = current.parent;
            }

            return true;
        }
    }
}
