namespace Kumiko
{
    /// <summary>
    /// 表示するインタラクションの状態を表す。
    /// 常に1つだけが成立する。
    /// </summary>
    public enum InteractionState
    {
        Normal,

        /// <summary>マウスなどのポインタが重なっている。入力種別がPointerのときだけ表示する。</summary>
        Hovered,

        /// <summary>選択されている。入力種別がNavigationのときだけ表示する。</summary>
        Focused,

        /// <summary>押されている。Submit入力では、押下と解除が続けて通知される。</summary>
        Pressed,

        /// <summary>操作できない。ほかの状態より優先する。</summary>
        Disabled,
    }
}
