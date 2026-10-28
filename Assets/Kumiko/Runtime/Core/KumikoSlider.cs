using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Kumiko
{
    /// <summary>
    /// UIを、範囲内の値をドラッグや方向入力で変えられるスライダーにする。
    /// </summary>
    /// <remarks>
    /// 値に合わせて、塗りとつまみのアンカーを書き換えて表示する。
    /// 塗りの<see cref="Image" />が<see cref="Image.Type.Filled" />の場合は、アンカーの代わりに<see cref="Image.fillAmount" />を書き換える。
    /// </remarks>
    [AddComponentMenu("Kumiko/KumikoSlider")]
    public sealed class KumikoSlider : KumikoElement, IInitializePotentialDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform _fillRect;
        [SerializeField] private RectTransform _handleRect;
        [SerializeField] private SlideDirection _direction = SlideDirection.LeftToRight;
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue = 1f;
        [SerializeField] private bool _wholeNumbers;
        [SerializeField] private float _value;
        [SerializeField] [Min(0f)] private float _stepSize = 0.1f;


        private DrivenRectTransformTracker _tracker;
        private Vector2 _dragOffset;


        /// <summary>
        /// 値が変化したときに通知される。
        /// </summary>
        /// <remarks>
        /// <see cref="SetValueWithoutNotify" />での変更では通知されない。
        /// 購読した時点では通知されないため、現在の値は<see cref="Value" />で取得すること。
        /// 不要になったら購読を解除すること。
        /// </remarks>
        public event Action<float> ValueChanged;


        /// <summary>現在の値。</summary>
        /// <remarks>設定した値は範囲内に収めてから反映し、変化した場合は<see cref="ValueChanged" />を通知する。</remarks>
        public float Value
        {
            get => _value;
            set => SetValue(value, notify: true);
        }


        /// <summary>最小値から最大値までを0～1で表した、現在の値。</summary>
        public float NormalizedValue
        {
            get => Mathf.Approximately(_minValue, _maxValue) ? 0f : Mathf.InverseLerp(_minValue, _maxValue, _value);
            set => Value = Mathf.Lerp(_minValue, _maxValue, value);
        }


        /// <summary>値の最小値。</summary>
        /// <remarks>変更によって現在の値が範囲外になった場合は、範囲内に収めて<see cref="ValueChanged" />を通知する。</remarks>
        public float MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                SetValue(_value, notify: true);
                UpdateVisuals();
            }
        }


        /// <summary>値の最大値。</summary>
        /// <remarks>変更によって現在の値が範囲外になった場合は、範囲内に収めて<see cref="ValueChanged" />を通知する。</remarks>
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                SetValue(_value, notify: true);
                UpdateVisuals();
            }
        }


        /// <summary>値が最小から最大へ向かう方向。</summary>
        /// <remarks>変更する場合は、<see cref="SetDirection" />を使う。</remarks>
        public SlideDirection Direction => _direction;


        // 整数のみの場合は、丸めた結果、値が変わらなくなるのを防ぐため、1以上の整数にする。
        private float StepSize => _wholeNumbers ? Mathf.Max(1f, Mathf.Round(_stepSize)) : _stepSize;


        protected override void OnEnable()
        {
            base.OnEnable();

            // Inspectorなどで範囲外の値が設定されている場合に備えて、通知せずに範囲内へ収める。
            _value = ClampValue(_value);
            UpdateVisuals();
        }


        protected override void OnDisable()
        {
            base.OnDisable();

            _tracker.Clear();
        }


#if UNITY_EDITOR


        protected override void OnValidate()
        {
            base.OnValidate();

            if (_wholeNumbers)
            {
                _minValue = Mathf.Round(_minValue);
                _maxValue = Mathf.Round(_maxValue);
            }

            _value = ClampValue(_value);

            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
            {
                return;
            }

            // OnValidate中にほかのオブジェクトを変更すると警告が出るため、次のエディター更新で反映する。
            UnityEditor.EditorApplication.delayCall -= UpdateVisualsDelayed;
            UnityEditor.EditorApplication.delayCall += UpdateVisualsDelayed;
        }


#endif


        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            // 押した直後から値をポインタに追従させるため、ドラッグ開始のしきい値を使わない。
            eventData.useDragThreshold = false;
        }


        public void OnDrag(PointerEventData eventData)
        {
            if (!CanDrag(eventData))
            {
                return;
            }

            UpdateValueFromPointer(eventData);
        }


        /// <summary>
        /// 値を変更する。<see cref="ValueChanged" />は通知しない。
        /// </summary>
        /// <param name="value">設定する値。範囲内に収めてから反映する。</param>
        public void SetValueWithoutNotify(float value)
        {
            SetValue(value, notify: false);
        }


        /// <summary>
        /// 値が最小から最大へ向かう方向を変更する。
        /// </summary>
        /// <param name="direction">変更後の方向。</param>
        /// <param name="flipsLayout">trueの場合は、方向の変化に合わせて、自身と子のレイアウトを入れ替え・反転する。</param>
        /// <remarks>
        /// レイアウトは、横と縦が入れ替わる場合は大きさや位置の横と縦を入れ替え、値の向きが逆になる場合は反転する。
        /// Inspectorで方向を変更した場合は、レイアウトも合わせて変更される。
        /// </remarks>
        public void SetDirection(SlideDirection direction, bool flipsLayout)
        {
            SlideDirection previousDirection = _direction;
            _direction = direction;

            if (flipsLayout)
            {
                ((RectTransform)transform).FlipLayout(previousDirection, direction);
            }

            UpdateVisuals();
        }


        /// <inheritdoc />
        protected internal override bool UsesMoveInput(MoveDirection direction)
        {
            return _direction.GetStepSign(direction) != 0;
        }


        /// <inheritdoc />
        protected override void OnPointerDownInput(PointerEventData eventData)
        {
            _dragOffset = Vector2.zero;

            Vector2 pressPosition = eventData.pointerPressRaycast.screenPosition;
            Camera eventCamera = eventData.pressEventCamera;

            // つまみを押した場合は値を飛ばさず、押した位置とつまみの基準点のずれを保ってドラッグさせる。
            if (_handleRect.TryGetParentArea(transform, out _)
                && _handleRect.ContainsScreenPosition(pressPosition, eventCamera))
            {
                if (_handleRect.TryGetLocalPosition(pressPosition, eventCamera, out Vector2 localPosition))
                {
                    _dragOffset = localPosition;
                }

                return;
            }

            // つまみ以外を押した場合は、押した位置へ値を移す。
            UpdateValueFromPointer(eventData);
        }


        /// <inheritdoc />
        protected override void OnMoveInput(MoveDirection direction)
        {
            Value += _direction.GetStepSign(direction) * StepSize;
        }


        private bool CanDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }


        private void UpdateValueFromPointer(PointerEventData eventData)
        {
            // ポインタ位置から値を求める範囲には、つまみの移動範囲を使い、ない場合は塗りの伸縮範囲を使う。
            if (!_handleRect.TryGetParentArea(transform, out RectTransform area)
                && !_fillRect.TryGetParentArea(transform, out area))
            {
                return;
            }

            int axis = _direction.GetAxisIndex();
            float areaLength = area.rect.size[axis];

            if (areaLength <= 0f)
            {
                return;
            }

            if (!area.TryGetLocalPosition(eventData.position, eventData.pressEventCamera, out Vector2 localPosition))
            {
                return;
            }

            // 範囲の開始端からの位置を、範囲の長さに対する割合にする。
            float ratio = Mathf.Clamp01((localPosition - area.rect.position - _dragOffset)[axis] / areaLength);
            NormalizedValue = _direction.IsReversed() ? 1f - ratio : ratio;
        }


        private void SetValue(float value, bool notify)
        {
            float clampedValue = ClampValue(value);

            if (Mathf.Approximately(_value, clampedValue))
            {
                return;
            }

            _value = clampedValue;
            UpdateVisuals();

            if (notify)
            {
                ValueChanged?.Invoke(_value);
            }
        }


        private float ClampValue(float value)
        {
            float clampedValue = Mathf.Clamp(value, _minValue, _maxValue);
            return _wholeNumbers ? Mathf.Round(clampedValue) : clampedValue;
        }


        private void UpdateVisuals()
        {
            _tracker.Clear();

            int axis = _direction.GetAxisIndex();
            float normalizedValue = NormalizedValue;

            // アンカーは座標が増える向きの割合なので、値の向きが逆の場合は反転する。
            float anchorPosition = _direction.IsReversed() ? 1f - normalizedValue : normalizedValue;

            if (_fillRect.TryGetParentArea(transform, out _))
            {
                _tracker.Add(this, _fillRect, DrivenTransformProperties.Anchors);

                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                if (_fillRect.TryGetComponent(out Image fillImage) && fillImage.type == Image.Type.Filled)
                {
                    fillImage.fillAmount = normalizedValue;
                }
                else if (_direction.IsReversed())
                {
                    // 値の向きが逆の場合は、座標の大きい側の端から伸ばす。
                    anchorMin[axis] = anchorPosition;
                }
                else
                {
                    anchorMax[axis] = anchorPosition;
                }

                _fillRect.anchorMin = anchorMin;
                _fillRect.anchorMax = anchorMax;
            }

            if (_handleRect.TryGetParentArea(transform, out _))
            {
                _tracker.Add(this, _handleRect, DrivenTransformProperties.Anchors);

                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[axis] = anchorPosition;
                anchorMax[axis] = anchorPosition;

                _handleRect.anchorMin = anchorMin;
                _handleRect.anchorMax = anchorMax;
            }
        }


#if UNITY_EDITOR


        private void UpdateVisualsDelayed()
        {
            // 呼ばれるまでの間に、破棄されている場合がある。
            if (this == null)
            {
                return;
            }

            UpdateVisuals();
        }


#endif
    }
}
