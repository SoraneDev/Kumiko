namespace Kumiko
{
    /// <summary>
    /// Gateの判定結果を表す。
    /// </summary>
    public enum ConfirmGateResult
    {
        /// <summary>通過させる。</summary>
        Passed,

        /// <summary>判定途中（ダブルクリックの1回目など）。拒否ではないので、拒否の通知は出さない。</summary>
        Pending,

        /// <summary>拒否する。拒否の演出を出してよい。</summary>
        Rejected,
    }
}
