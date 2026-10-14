using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// UI要素の決定入力にダブルクリックの条件を付与するクラス
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Kumiko/DoubleClickGate")]
    public sealed class DoubleClickGate : KumikoGate
    {
        [SerializeField, Min(0f), Tooltip("秒")] private float _doubleClickInterval = 0.4f;
        
        
        private float _firstClickTime;
        private bool _hasFirstClick;
        

        /// <inheritdoc cref="KumikoGate.Stage"/>
        public override ConfirmGateStage Stage => ConfirmGateStage.Recognition;
        
        
        /// <inheritdoc cref="KumikoGate.Evaluate"/>
        public override ConfirmGateResult Evaluate()
        {
            return IsSecondClick() ? ConfirmGateResult.Passed : ConfirmGateResult.Pending;
        }
        
        
        /// <inheritdoc cref="KumikoGate.Commit"/>
        public override void Commit(ConfirmGateResult ownResult, ConfirmGateResult finalResult)
        {
            if (finalResult == ConfirmGateResult.Passed)
            {
                // 決定が確定したので、1回目の記録を消費する。
                _hasFirstClick = false;
                return;
            }
            
            // 判定途中のときは、今回の入力を1回目として記録する。
            // 2回目でも、ほかのGateに拒否されたときは、記録を変えない。
            if (ownResult == ConfirmGateResult.Pending)
            {
                _firstClickTime = Time.unscaledTime;
                _hasFirstClick = true;
            }
        }


        /// <inheritdoc cref="KumikoGate.ResetState"/>
        public override void ResetState()
        {
            _hasFirstClick = false;
        }
        
        
        private bool IsSecondClick()
        {
            return _hasFirstClick && Time.unscaledTime - _firstClickTime < _doubleClickInterval;
        }
    }
}