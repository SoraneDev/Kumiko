using System;


namespace Kumiko
{
    /// <summary>
    /// 内部で保持するインタラクションの状態の組み合わせを、ビットフラグで表す列挙型。
    /// 複数が同時に成立する。
    /// </summary>
    [Flags]
    internal enum InteractionFlags
    {
        None     = 0,
        
        /// <summary>入力由来のフラグ</summary>
        Hovered  = 1 << 0,
        
        /// <summary>入力由来のフラグ</summary>
        Focused  = 1 << 1,
        
        /// <summary>入力由来のフラグ</summary>
        Pressed  = 1 << 2,
        
        /// <summary>操作可否由来のフラグ</summary>
        Disabled = 1 << 3,
    }
}