using System;
using System.Linq;
using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// 評価するGateを管理するクラス
    /// </summary>
    internal sealed class ConfirmGateChain
    {
        private IConfirmGate[] _gates = Array.Empty<IConfirmGate>();
        private ConfirmGateResult[] _ownResults = Array.Empty<ConfirmGateResult>();

        
        /// <summary>
        /// 対象のGameObjectからIConfirmGateを取得し、Stageの昇順でソートして格納する。
        /// </summary>
        /// <param name="owner">Gateを持つ対象のGameObject</param>
        internal void Collect(GameObject owner)
        {
            _gates = owner.GetComponents<IConfirmGate>().OrderBy(gate => gate.Stage).ToArray();
            _ownResults = new ConfirmGateResult[_gates.Length];
        }


        /// <summary>
        /// すべてのGateを評価して最終結果を決め、各Gateの内部状態を更新する。
        /// </summary>
        /// <returns>すべてのGateを通過した場合はPassedを返す。そうでない場合はGateが出す結果を返す。</returns>
        internal ConfirmGateResult Resolve()
        {
            var finalResult = ConfirmGateResult.Passed;

            // 判定: 状態を変えずに、すべてのGateを評価する。
            // 最終結果は、並び順で最初にPassed以外を返したGateの結果にする。
            for (int i = 0; i < _gates.Length; i++)
            {
                _ownResults[i] = _gates[i].IsActive() ? _gates[i].Evaluate() : ConfirmGateResult.Passed;

                if (finalResult == ConfirmGateResult.Passed) finalResult = _ownResults[i];
            }

            // 確定: 最終結果が決まってから、各Gateに状態を更新させる。
            for (int i = 0; i < _gates.Length; i++)
            {
                if (_gates[i].IsActive()) _gates[i].Commit(_ownResults[i], finalResult);
            }

            return finalResult;
        }


        /// <summary>
        /// すべてのGateの内部状態を初期化する。
        /// </summary>
        internal void ResetAll()
        {
            foreach (var gate in _gates) gate.ResetState();
        }
    }
}