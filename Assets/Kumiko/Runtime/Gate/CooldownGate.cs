using UnityEngine;
using UnityEngine.Serialization;


namespace Kumiko
{
    /// <summary>
    /// UI要素の決定入力にクールダウンの条件を付与する。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Kumiko/CooldownGate")]
    public sealed class CooldownGate : KumikoGate
    {
        [FormerlySerializedAs("_cooldownTime")]
        [SerializeField, Min(0f), Tooltip("決定後、次の決定を受け付けるまでの秒数")]
        private float _cooldownDuration = 1f;


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

            _nextAvailableTime = Time.unscaledTime + _cooldownDuration;
        }


        /// <inheritdoc/>
        public override void ResetState()
        {
            _nextAvailableTime = 0f;
        }
    }
}
