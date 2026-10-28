using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// Gateの基底クラス。
    /// </summary>
    /// <remarks>無効になると、<see cref="ResetState" />で内部状態を初期化する。</remarks>
    [RequireComponent(typeof(KumikoButton))]
    public abstract class KumikoGate : KumikoModule, IConfirmGate
    {
        /// <inheritdoc />
        public abstract ConfirmGateStage Stage { get; }


        protected override void OnDisable()
        {
            base.OnDisable();

            // 再び有効になったときに、判定途中の状態を持ち越さないため、リセットする。
            ResetState();
        }


        /// <inheritdoc />
        public abstract ConfirmGateResult Evaluate();


        /// <inheritdoc />
        public abstract void Commit(ConfirmGateResult ownResult, ConfirmGateResult finalResult);


        /// <inheritdoc />
        public abstract void ResetState();
    }
}
