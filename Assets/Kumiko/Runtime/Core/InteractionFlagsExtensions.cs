namespace Kumiko
{
    /// <summary>
    /// InteractionFlagsの拡張メソッドを提供する静的クラス
    /// </summary>
    internal static class InteractionFlagsExtensions
    {
        internal static InteractionFlags Add(this InteractionFlags flags, InteractionFlags flag) => flags | flag;

        internal static InteractionFlags Remove(this InteractionFlags flags, InteractionFlags flag) => flags & ~flag;

        internal static InteractionFlags Set(this InteractionFlags flags, InteractionFlags flag, bool isOn) =>
            isOn ? flags | flag : flags & ~flag;

        internal static bool Has(this InteractionFlags flags, InteractionFlags flag) => (flags & flag) != 0;
    }
}