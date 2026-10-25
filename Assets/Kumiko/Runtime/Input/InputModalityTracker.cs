using System;
using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// 入力種別の保持と固定を管理する。
    /// </summary>
    public sealed class InputModalityTracker
    {
        /// <summary>現在のインスタンス。再生を開始するたびに作り直される。</summary>
        public static InputModalityTracker Current { get; private set; } = new();


        private InputModality _detected;
        private InputModality? _locked;


        /// <summary>
        /// 入力種別が変化したときに通知される。
        /// </summary>
        /// <remarks>
        /// Lockの開始・解除でも、反映される入力種別が変わったときに通知される。
        /// 購読した時点では通知されないため、現在の入力種別はModalityで取得すること。
        /// 不要になったら購読を解除すること。
        /// </remarks>
        public event Action<InputModality> ModalityChanged;


        /// <summary>
        /// 入力種別が変化したときに通知される。
        /// 内部用。利用側の例外の影響を受けないように、先に通知される。
        /// </summary>
        /// <inheritdoc cref="InputModalityTracker.ModalityChanged"/>
        internal event Action<InputModality> InternalModalityChanged;


        /// <summary>現在の入力種別。</summary>
        public InputModality Modality { get; private set; }


        // Unityのドメインリロードを無効にしている場合、静的変数がリセットされないため、明示的にリセットする。
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Current = new InputModalityTracker();
        }


        /// <summary>
        /// 入力種別を固定する。
        /// </summary>
        /// <remarks>自動検知は裏で続くが、解除するまで反映しない。</remarks>
        public void Lock(InputModality modality)
        {
            _locked = modality;
            Refresh();
        }


        /// <summary>
        /// 固定を解除し、直近に検知した入力種別へ戻す。
        /// </summary>
        public void Unlock()
        {
            _locked = null;
            Refresh();
        }


        /// <summary>
        /// 入力種別を報告する。
        /// </summary>
        /// <param name="detected">報告する入力種別。</param>
        internal void Report(InputModality detected)
        {
            _detected = detected;
            Refresh();
        }


        private void Refresh()
        {
            InputModality next = _locked ?? _detected;
            SetModality(next);
        }


        private void SetModality(InputModality next)
        {
            if (Modality == next) return;

            Modality = next;

            InternalModalityChanged?.Invoke(Modality);
            ModalityChanged?.Invoke(Modality);
        }
    }
}
