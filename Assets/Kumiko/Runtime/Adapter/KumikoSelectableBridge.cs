using UnityEngine;
using UnityEngine.UI;


namespace Kumiko
{
    /// <summary>
    /// SelectableとKumikoElementの操作可否を同期させるためのクラス
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
            
            TargetSelectable.interactable = Element.Interactable;
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
        
        
        private void OnStateChanged(InteractionState state) => ApplyState(state);
        
        
        private void ApplyState(InteractionState state)
        {
            TargetSelectable.interactable = state != InteractionState.Disabled;
        }
    
    
        // 本ライブラリではSelectableのNavigation機能とTransition機能は
        // 使用しない設計のため機能をNoneにする。
        private void DisableSelectableSettings()
        {
            var navigation = TargetSelectable.navigation;
            navigation.mode = Navigation.Mode.None;
            TargetSelectable.navigation = navigation;

            TargetSelectable.transition = Selectable.Transition.None;
        }

    
#if UNITY_EDITOR

        protected override void Reset()
        {
            DisableSelectableSettings();
        }
        
#endif
    }
}
