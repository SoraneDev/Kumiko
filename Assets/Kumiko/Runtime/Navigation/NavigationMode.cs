namespace Kumiko
{
    /// <summary>
    /// 遷移先が未設定、または選択できない方向で、遷移先を自動で探す範囲を表す。
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>自動で探さない。設定した遷移先へだけ選択を移す。</summary>
        Manual,

        /// <summary>上下左右のすべてで、自動で探す。</summary>
        Automatic,

        /// <summary>左右だけ、自動で探す。</summary>
        Horizontal,

        /// <summary>上下だけ、自動で探す。</summary>
        Vertical,
    }
}
