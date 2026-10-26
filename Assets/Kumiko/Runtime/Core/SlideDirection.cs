namespace Kumiko
{
    /// <summary>
    /// 値が最小から最大へ向かう方向を表す。
    /// </summary>
    public enum SlideDirection
    {
        /// <summary>左端が最小、右端が最大。</summary>
        LeftToRight,

        /// <summary>右端が最小、左端が最大。</summary>
        RightToLeft,

        /// <summary>下端が最小、上端が最大。</summary>
        BottomToTop,

        /// <summary>上端が最小、下端が最大。</summary>
        TopToBottom,
    }
}
