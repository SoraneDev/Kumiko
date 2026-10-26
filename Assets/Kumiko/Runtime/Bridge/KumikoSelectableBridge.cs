using UnityEngine;
using UnityEngine.UI;


namespace Kumiko
{
    /// <summary>
    /// KumikoElementの実効的な操作可否をSelectableへ反映する。
    /// SelectableのNavigationとTransitionは使わないため、無効にする。
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
                return _targetSelectable ??= GetComponent<Selectable>();
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


#if UNITY_EDITOR

        protected override void Reset()
        {
            base.Reset();

            DisableSelectableSettings();
        }

#endif
    }
}
