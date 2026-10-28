using System;
using System.Linq;
using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// Gateをまとめて評価し、決定入力の最終結果を決める。
    /// </summary>
    internal sealed class ConfirmGateChain
    {
        private IConfirmGate[] _gates = Array.Empty<IConfirmGate>();
        private ConfirmGateResult[] _ownResults = Array.Empty<ConfirmGateResult>();


        /// <summary>
        /// 対象の<see cref="GameObject" />から<see cref="IConfirmGate" />を取得し、<see cref="IConfirmGate.Stage" />の昇順に並べて格納する。
        /// 同じ<see cref="IConfirmGate.Stage" />では取得した順を保つ。
        /// </summary>
        /// <param name="owner">Gateを持つ対象の<see cref="GameObject" />。</param>
        internal void Collect(GameObject owner)
        {
            _gates = owner.GetComponents<IConfirmGate>().OrderBy(gate => gate.Stage).ToArray();
            _ownResults = new ConfirmGateResult[_gates.Length];
        }


        /// <summary>
        /// すべてのGateを評価して最終結果を決め、各Gateの内部状態を更新する。
        /// </summary>
        /// <returns>
        /// すべてのGateを通過した場合は<see cref="ConfirmGateResult.Passed" />、
        /// そうでない場合は最初に<see cref="ConfirmGateResult.Passed" />以外を返したGateの結果を返す。
        /// </returns>
        internal ConfirmGateResult Resolve()
        {
            ConfirmGateResult finalResult = ConfirmGateResult.Passed;

            // 判定：状態を変えずに、すべてのGateを評価する。
            // 最終結果は、並び順で最初にPassed以外を返したGateの結果にする。
            for (int i = 0; i < _gates.Length; i++)
            {
                _ownResults[i] = _gates[i].IsActive() ? _gates[i].Evaluate() : ConfirmGateResult.Passed;

                if (finalResult == ConfirmGateResult.Passed)
                {
                    finalResult = _ownResults[i];
                }
            }

            // 確定：最終結果が決まってから、各Gateに状態を更新させる。
            for (int i = 0; i < _gates.Length; i++)
            {
                if (_gates[i].IsActive())
                {
                    _gates[i].Commit(_ownResults[i], finalResult);
                }
            }

            return finalResult;
        }


        /// <summary>
        /// すべてのGateの内部状態を初期化する。
        /// </summary>
        internal void ResetAll()
        {
            foreach (IConfirmGate gate in _gates)
            {
                gate.ResetState();
            }
        }
    }
}
