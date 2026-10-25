namespace Kumiko
{
    /// <summary>
    /// Gateの判定結果を表す。
    /// </summary>
    public enum ConfirmGateResult
    {
        /// <summary>通過させる。</summary>
        Passed,

        /// <summary>判定途中（ダブルクリックの1回目など）。</summary>
        Pending,

        /// <summary>拒否する。</summary>
        Rejected,
    }
}
