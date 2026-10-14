namespace Kumiko
{
    /// <summary>
    /// InteractionFlagsとInputModalityに基づいて、InteractionStateを決定するための静的クラス
    /// </summary>
    internal static class InteractionStateResolver
    {
        /// <summary>
        /// 指定されたInteractionFlagsとInputModalityに基づいて、InteractionStateを決定する。
        /// </summary>
        /// <param name="flags">インタラクションの状態フラグ</param>
        /// <param name="modality">入力種別</param>
        /// <returns>算出されたInteractionState</returns>
        /// <remarks>
        /// 優先順位はDisabled → Pressed → 入力種別ごとの状態。
        /// NavigationではFocused、PointerではHoveredだけを表示し、Touchでは押下以外の状態を表示しない。
        /// </remarks>
        internal static InteractionState Resolve(InteractionFlags flags, InputModality modality)
        {
            if (flags.Has(InteractionFlags.Disabled)) return InteractionState.Disabled;
            if (flags.Has(InteractionFlags.Pressed)) return InteractionState.Pressed;
            
            switch (modality)
            {
                case InputModality.Navigation:
                    return flags.Has(InteractionFlags.Focused) ? InteractionState.Focused : InteractionState.Normal;
                case InputModality.Pointer:
                    return flags.Has(InteractionFlags.Hovered) ? InteractionState.Hovered : InteractionState.Normal;
                case InputModality.Touch:
                    return InteractionState.Normal;
                default:
                    return InteractionState.Normal;
            }
        }
    }
}