namespace Kumiko
{
    /// <summary>
    /// <see cref="InteractionFlags" />と<see cref="InputModality" />に基づいて、<see cref="InteractionState" />を決定する。
    /// </summary>
    internal static class InteractionStateResolver
    {
        /// <summary>
        /// 表示する<see cref="InteractionState" />を決定する。
        /// </summary>
        /// <param name="flags">インタラクションの状態フラグ。</param>
        /// <param name="modality">入力種別。</param>
        /// <returns>表示する<see cref="InteractionState" />を返す。</returns>
        /// <remarks>
        /// 優先順位は<see cref="InteractionState.Disabled" /> → <see cref="InteractionState.Pressed" /> → 入力種別ごとの状態。
        /// <see cref="InputModality.Navigation" />では<see cref="InteractionState.Focused" />、
        /// <see cref="InputModality.Pointer" />では<see cref="InteractionState.Hovered" />だけを表示し、
        /// <see cref="InputModality.Touch" />では押下以外の状態を表示しない。
        /// </remarks>
        internal static InteractionState Resolve(InteractionFlags flags, InputModality modality)
        {
            if (flags.HasAny(InteractionFlags.Disabled))
            {
                return InteractionState.Disabled;
            }

            if (flags.HasAny(InteractionFlags.Pressed))
            {
                return InteractionState.Pressed;
            }

            switch (modality)
            {
                case InputModality.Navigation:
                    return flags.HasAny(InteractionFlags.Focused) ? InteractionState.Focused : InteractionState.Normal;
                case InputModality.Pointer:
                    return flags.HasAny(InteractionFlags.Hovered) ? InteractionState.Hovered : InteractionState.Normal;
                case InputModality.Touch:
                    return InteractionState.Normal;
                default:
                    return InteractionState.Normal;
            }
        }
    }
}
