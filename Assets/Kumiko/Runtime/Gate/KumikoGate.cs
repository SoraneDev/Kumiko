using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// Gateモジュールの基底クラス。
    /// </summary>
    [RequireComponent(typeof(KumikoButton))]
    public abstract class KumikoGate : KumikoModule, IConfirmGate
    {
        protected override void OnDisable()
        {
            base.OnDisable();

            // 再び有効になったときに、判定途中の状態を持ち越さないためリセットする。
            ResetState();
        }


        /// <inheritdoc/>
        public abstract ConfirmGateStage Stage { get; }


        /// <inheritdoc/>
        public abstract ConfirmGateResult Evaluate();


        /// <inheritdoc/>
        public abstract void Commit(ConfirmGateResult ownResult, ConfirmGateResult finalResult);


        /// <inheritdoc/>
        public abstract void ResetState();
    }
}
