namespace Kumiko
{
    /// <summary>
    /// InteractionFlagsの拡張メソッドを提供する。
    /// </summary>
    internal static class InteractionFlagsExtensions
    {
        internal static InteractionFlags Add(this InteractionFlags flags, InteractionFlags flag)
        {
            return flags | flag;
        }


        internal static InteractionFlags Remove(this InteractionFlags flags, InteractionFlags flag)
        {
            return flags & ~flag;
        }


        internal static InteractionFlags Set(this InteractionFlags flags, InteractionFlags flag, bool isOn)
        {
            return isOn ? flags | flag : flags & ~flag;
        }


        internal static bool HasAny(this InteractionFlags flags, InteractionFlags flag)
        {
            return (flags & flag) != 0;
        }
    }
}
