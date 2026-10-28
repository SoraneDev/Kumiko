namespace Kumiko
{
    /// <summary>
    /// 表示するインタラクションの状態を表す。
    /// 常に1つだけが成立する。
    /// </summary>
    public enum InteractionState
    {
        /// <summary>ほかのどの状態も成立していない。</summary>
        Normal,

        /// <summary>マウスなどのポインタが重なっている。入力種別が<see cref="InputModality.Pointer" />のときだけ表示する。</summary>
        Hovered,

        /// <summary>選択されている。入力種別が<see cref="InputModality.Navigation" />のときだけ表示する。</summary>
        Focused,

        /// <summary>押されている。</summary>
        Pressed,

        /// <summary>操作できない。ほかの状態より優先する。</summary>
        Disabled,
    }
}
