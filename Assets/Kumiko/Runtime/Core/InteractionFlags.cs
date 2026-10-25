using System;


namespace Kumiko
{
    /// <summary>
    /// 内部で保持するインタラクションの状態の組み合わせを、ビットフラグで表す。
    /// 複数が同時に成立する。
    /// </summary>
    [Flags]
    internal enum InteractionFlags
    {
        None = 0,

        /// <summary>ポインタが重なっている。入力由来。</summary>
        Hovered = 1 << 0,

        /// <summary>選択されている。入力由来。</summary>
        Focused = 1 << 1,

        /// <summary>押されている。入力由来。</summary>
        Pressed = 1 << 2,

        /// <summary>実効的に操作できない。操作可否由来。</summary>
        Disabled = 1 << 3,
    }
}
