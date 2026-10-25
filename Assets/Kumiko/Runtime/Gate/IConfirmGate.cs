namespace Kumiko
{
    /// <summary>
    /// 決定入力に通過条件を付与するモジュールが実装する。
    /// </summary>
    public interface IConfirmGate
    {
        /// <summary>最終結果に採用する優先順位。</summary>
        /// <remarks>複数のGateがPassed以外を返したとき、段階の小さいGate（同じ段階ではInspectorで上のGate）の結果を採用する。</remarks>
        ConfirmGateStage Stage { get; }


        /// <summary>
        /// Gateがアクティブかどうかを取得する。
        /// </summary>
        /// <returns>アクティブ状態を返す。</returns>
        bool IsActive();


        /// <summary>
        /// 決定入力を通過させてよいかを評価する。
        /// </summary>
        /// <remarks>内部状態を変更しない。同じ状態では、何度呼んでも同じ結果を返す。</remarks>
        /// <returns>通過可能ならPassed、判定途中ならPending、通過拒否ならRejectedを返す。</returns>
        ConfirmGateResult Evaluate();


        /// <summary>
        /// 最終結果が決まった後に呼ばれ、内部状態を更新する。
        /// </summary>
        /// <param name="ownResult">自身のEvaluateの結果。</param>
        /// <param name="finalResult">すべてのGateを合わせた最終結果。</param>
        void Commit(ConfirmGateResult ownResult, ConfirmGateResult finalResult);


        /// <summary>
        /// 内部状態を初期化する。
        /// </summary>
        void ResetState();
    }
}
