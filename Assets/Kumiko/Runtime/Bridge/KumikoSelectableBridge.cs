using UnityEngine;
using UnityEngine.UI;


namespace Kumiko
{
    /// <summary>
    /// <see cref="KumikoElement" />の実効的な操作可否を<see cref="Selectable" />へ反映する。
    /// <see cref="Selectable.navigation" />と<see cref="Selectable.transition" />は使わないため、無効にする。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Selectable))]
    [AddComponentMenu("Kumiko/KumikoSelectableBridge")]
    public sealed class KumikoSelectableBridge : KumikoModule
    {
        private Selectable _targetSelectable;


        private Selectable TargetSelectable
        {
            get
            {
                if (_targetSelectable == null)
                {
                    _targetSelectable = GetComponent<Selectable>();
                }

                return _targetSelectable;
            }
        }


        protected override void Awake()
        {
            base.Awake();

            DisableSelectableSettings();
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            Element.InternalStateChanged += OnStateChanged;

            // 無効の間に変化した状態を反映する。
            ApplyState(Element.State);
        }


        protected override void OnDisable()
        {
            base.OnDisable();

            Element.InternalStateChanged -= OnStateChanged;
        }


#if UNITY_EDITOR


        protected override void Reset()
        {
            base.Reset();

            DisableSelectableSettings();
        }


#endif


        private void OnStateChanged(InteractionState state)
        {
            ApplyState(state);
        }


        private void ApplyState(InteractionState state)
        {
            TargetSelectable.interactable = state != InteractionState.Disabled;
        }


        // 選択の移動と見た目の切り替えはKumiko側で行うため、
        // SelectableのNavigationとTransitionをNoneにする。
        private void DisableSelectableSettings()
        {
            Navigation navigation = TargetSelectable.navigation;
            navigation.mode = Navigation.Mode.None;
            TargetSelectable.navigation = navigation;

            TargetSelectable.transition = Selectable.Transition.None;
        }
    }
}
