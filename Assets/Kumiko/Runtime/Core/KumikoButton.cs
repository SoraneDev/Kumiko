using System;
using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// UIをインタラクション可能なボタンにする。
    /// Gateを同じGameObjectにアタッチすると、ダブルクリックやクールダウンなどの通過条件を決定入力に付与できる。
    /// </summary>
    /// <remarks>Gateは、Awakeの時点でアタッチされているものだけが対象になる。</remarks>
    [AddComponentMenu("Kumiko/KumikoButton")]
    public sealed class KumikoButton : KumikoElement
    {
        private readonly ConfirmGateChain _confirmGateChain = new();


        /// <summary>
        /// 決定入力がGateを通過したときに通知される。
        /// </summary>
        /// <remarks>クリック・タップ・Submitのいずれも、このイベントで通知される。</remarks>
        public event Action Confirmed;


        /// <summary>
        /// 決定入力がGateに拒否されたときに通知される。
        /// </summary>
        /// <remarks>判定途中のときは通知されない。拒否の演出（効果音など）に使う。</remarks>
        public event Action ConfirmRejected;


        protected override void Awake()
        {
            base.Awake();

            _confirmGateChain.Collect(gameObject);
        }


        protected override void OnDisable()
        {
            base.OnDisable();

            // 再び有効になったときに、判定途中の状態を持ち越さないため、すべてリセットする。
            _confirmGateChain.ResetAll();
        }


        /// <inheritdoc/>
        protected override void OnConfirmInput()
        {
            Confirm();
        }


        private void Confirm()
        {
            switch (_confirmGateChain.Resolve())
            {
                case ConfirmGateResult.Passed:
                    Confirmed?.Invoke();
                    break;
                case ConfirmGateResult.Rejected:
                    ConfirmRejected?.Invoke();
                    break;
                case ConfirmGateResult.Pending:
                    // Pendingは入力拒否ではなく判定途中のため通知しない。
                    break;
            }
        }
    }
}
