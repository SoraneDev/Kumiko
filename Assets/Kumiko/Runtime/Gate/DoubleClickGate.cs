using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// UI要素の決定入力にダブルクリックの条件を付与する。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Kumiko/DoubleClickGate")]
    public sealed class DoubleClickGate : KumikoGate
    {
        [SerializeField] [Min(0f)] [Tooltip("1回目から2回目の入力までに許容する秒数")]
        private float _doubleClickInterval = 0.4f;


        private float _firstClickTime;
        private bool _hasFirstClick;


        /// <inheritdoc/>
        public override ConfirmGateStage Stage => ConfirmGateStage.Recognition;


        /// <inheritdoc/>
        public override ConfirmGateResult Evaluate()
        {
            return IsSecondClick() ? ConfirmGateResult.Passed : ConfirmGateResult.Pending;
        }


        /// <inheritdoc/>
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


        /// <inheritdoc/>
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
