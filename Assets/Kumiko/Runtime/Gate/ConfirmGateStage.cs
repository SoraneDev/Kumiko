namespace Kumiko
{
    /// <summary>
    /// Gateの評価段階を表す。値が小さい段階の結果を最終結果として優先する。
    /// </summary>
    public enum ConfirmGateStage
    {
        /// <summary>入力の形を判定する（ダブルクリックなど）。</summary>
        Recognition,

        /// <summary>通過の頻度や回数を制限する（クールダウンなど）。</summary>
        Restriction,
    }
}
