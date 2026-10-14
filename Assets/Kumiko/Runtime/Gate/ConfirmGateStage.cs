namespace Kumiko
{
    /// <summary>
    /// 最終結果に採用する優先順位を表す列挙型
    /// </summary>
    public enum ConfirmGateStage
    {
        /// <summary>入力の形を判定する（ダブルクリックなど）。</summary>
        Recognition,

        /// <summary>通過の頻度や回数を制限する（クールダウンなど）。</summary>
        Restriction,
    }
}