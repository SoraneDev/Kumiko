using System;
using UnityEngine;
using UnityEngine.UI;


namespace Kumiko
{
    /// <summary>
    /// UIを、決定入力でオンとオフを切り替えるトグルにする。
    /// </summary>
    /// <remarks>
    /// オンのときだけ表示するGraphicを設定すると、オンとオフに合わせてGraphicを有効・無効にする。
    /// 切り替えの演出を自分で作る場合は、設定せずにIsOnChangedを購読する。
    /// </remarks>
    [AddComponentMenu("Kumiko/KumikoToggle")]
    public sealed class KumikoToggle : KumikoElement
    {
        [SerializeField] private bool _isOn;
        [SerializeField] private Graphic _isOnGraphic;


        /// <summary>
        /// オンとオフが切り替わったときに通知される。
        /// </summary>
        /// <remarks>
        /// SetIsOnWithoutNotifyでの変更では通知されない。
        /// 購読した時点では通知されないため、現在の状態はIsOnで取得すること。
        /// 不要になったら購読を解除すること。
        /// </remarks>
        public event Action<bool> IsOnChanged;


        /// <summary>オンかどうか。</summary>
        /// <remarks>設定した値が現在と異なる場合は、IsOnChangedを通知する。</remarks>
        public bool IsOn
        {
            get => _isOn;
            set => SetIsOn(value, true);
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            // 無効の間にGraphicが変更されていても、現在の状態に合わせる。
            ApplyIsOnGraphic();
        }


#if UNITY_EDITOR


        protected override void OnValidate()
        {
            base.OnValidate();

            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
            {
                return;
            }

            // OnValidate中にほかのコンポーネントを変更すると警告が出るため、次のエディター更新で反映する。
            UnityEditor.EditorApplication.delayCall -= ApplyIsOnGraphicDelayed;
            UnityEditor.EditorApplication.delayCall += ApplyIsOnGraphicDelayed;
        }


#endif


        /// <summary>
        /// オンとオフを切り替える。IsOnChangedは通知しない。
        /// </summary>
        /// <param name="isOn">オンにする場合はtrue。</param>
        public void SetIsOnWithoutNotify(bool isOn)
        {
            SetIsOn(isOn, false);
        }


        /// <inheritdoc/>
        protected override void OnConfirmInput()
        {
            IsOn = !_isOn;
        }


        private void SetIsOn(bool isOn, bool notify)
        {
            if (_isOn == isOn)
            {
                return;
            }

            _isOn = isOn;
            ApplyIsOnGraphic();

            if (notify)
            {
                IsOnChanged?.Invoke(_isOn);
            }
        }


        private void ApplyIsOnGraphic()
        {
            if (_isOnGraphic == null)
            {
                return;
            }

            _isOnGraphic.enabled = _isOn;
        }


#if UNITY_EDITOR


        private void ApplyIsOnGraphicDelayed()
        {
            // 呼ばれるまでの間に、破棄されている場合がある。
            if (this == null)
            {
                return;
            }

            ApplyIsOnGraphic();
        }


#endif
    }
}
