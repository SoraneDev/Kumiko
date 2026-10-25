using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// UI要素の決定入力にクールダウンの条件を付与する。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Kumiko/CooldownGate")]
    public sealed class CooldownGate : KumikoGate
    {
        [SerializeField, Min(0f), Tooltip("秒")] private float _cooldownTime = 1f;


        private float _nextAvailableTime;


        /// <inheritdoc/>
        public override ConfirmGateStage Stage => ConfirmGateStage.Restriction;


        /// <inheritdoc/>
        public override ConfirmGateResult Evaluate()
        {
            return Time.unscaledTime < _nextAvailableTime ? ConfirmGateResult.Rejected : ConfirmGateResult.Passed;
        }


        /// <inheritdoc/>
        public override void Commit(ConfirmGateResult ownResult, ConfirmGateResult finalResult)
        {
            // 決定が確定したときだけ、クールダウンを始める。
            if (finalResult != ConfirmGateResult.Passed) return;

            _nextAvailableTime = Time.unscaledTime + _cooldownTime;
        }


        /// <inheritdoc/>
        public override void ResetState()
        {
            _nextAvailableTime = 0f;
        }
    }
}
