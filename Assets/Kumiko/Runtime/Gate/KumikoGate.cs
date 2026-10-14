using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// Gateモジュールの基底クラス
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
        
        
        /// <inheritdoc cref="IConfirmGate.Stage"/>
        public abstract ConfirmGateStage Stage { get; }


        /// <inheritdoc cref="IConfirmGate.Evaluate"/>
        public abstract ConfirmGateResult Evaluate();
        
        
        /// <inheritdoc cref="IConfirmGate.Commit"/>
        public abstract void Commit(ConfirmGateResult ownResult, ConfirmGateResult finalResult);
        
        
        /// <inheritdoc cref="IConfirmGate.ResetState"/>
        public abstract void ResetState();
    }
}