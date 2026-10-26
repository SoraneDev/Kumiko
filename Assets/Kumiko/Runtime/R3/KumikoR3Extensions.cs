using System;
using R3;


namespace Kumiko.R3
{
    /// <summary>
    /// Kumikoのイベント購読をR3で行うための拡張メソッドを提供する。
    /// </summary>
    public static class KumikoR3Extensions
    {
        /// <summary>
        /// 表示状態の変化をObservableとして取得する。
        /// </summary>
        /// <remarks>購読した時点で、現在の状態を流す。</remarks>
        public static Observable<InteractionState> StateAsObservable(this KumikoElement element)
        {
            return Observable.Create<InteractionState>(observer =>
            {
                observer.OnNext(element.State);

                Action<InteractionState> handler = observer.OnNext;
                element.StateChanged += handler;

                return Disposable.Create(() => element.StateChanged -= handler);
            });
        }


        /// <summary>
        /// 決定入力がGateを通過したことを、Observableとして取得する。
        /// </summary>
        [Obsolete]
        public static Observable<Unit> OnConfirmAsObservable(this KumikoButton button)
        {
            return Observable.Create<Unit>(observer =>
            {
                Action handler = () => observer.OnNext(Unit.Default);
                button.Confirmed += handler;

                return Disposable.Create(() => button.Confirmed -= handler);
            });
        }


        /// <summary>
        /// 決定入力がGateを通過したことを、Observableとして取得する。
        /// </summary>
        public static Observable<Unit> ConfirmAsObservable(this KumikoButton button)
        {
            return Observable.Create<Unit>(observer =>
            {
                Action handler = () => observer.OnNext(Unit.Default);
                button.Confirmed += handler;

                return Disposable.Create(() => button.Confirmed -= handler);
            });
        }


        /// <summary>
        /// 決定入力がGateに拒否されたことを、Observableとして取得する。
        /// </summary>
        [Obsolete]
        public static Observable<Unit> OnConfirmRejectedAsObservable(this KumikoButton button)
        {
            return Observable.Create<Unit>(observer =>
            {
                Action handler = () => observer.OnNext(Unit.Default);
                button.ConfirmRejected += handler;

                return Disposable.Create(() => button.ConfirmRejected -= handler);
            });
        }


        /// <summary>
        /// 決定入力がGateに拒否されたことを、Observableとして取得する。
        /// </summary>
        public static Observable<Unit> ConfirmRejectedAsObservable(this KumikoButton button)
        {
            return Observable.Create<Unit>(observer =>
            {
                Action handler = () => observer.OnNext(Unit.Default);
                button.ConfirmRejected += handler;

                return Disposable.Create(() => button.ConfirmRejected -= handler);
            });
        }


        /// <summary>
        /// トグルのオンとオフの切り替わりをObservableとして取得する。
        /// </summary>
        /// <remarks>購読した時点で、現在の状態を流す。</remarks>
        public static Observable<bool> IsOnAsObservable(this KumikoToggle toggle)
        {
            return Observable.Create<bool>(observer =>
            {
                observer.OnNext(toggle.IsOn);

                Action<bool> handler = observer.OnNext;
                toggle.IsOnChanged += handler;

                return Disposable.Create(() => toggle.IsOnChanged -= handler);
            });
        }


        /// <summary>
        /// スライダーの値の変化をObservableとして取得する。
        /// </summary>
        /// <remarks>購読した時点で、現在の値を流す。</remarks>
        public static Observable<float> ValueAsObservable(this KumikoSlider slider)
        {
            return Observable.Create<float>(observer =>
            {
                observer.OnNext(slider.Value);

                Action<float> handler = observer.OnNext;
                slider.ValueChanged += handler;

                return Disposable.Create(() => slider.ValueChanged -= handler);
            });
        }


        /// <summary>
        /// 入力種別の変化をObservableとして取得する。
        /// </summary>
        /// <remarks>購読した時点で、現在の入力種別を流す。</remarks>
        public static Observable<InputModality> ModalityAsObservable(this InputModalityTracker tracker)
        {
            return Observable.Create<InputModality>(observer =>
            {
                observer.OnNext(tracker.Modality);

                Action<InputModality> handler = observer.OnNext;
                tracker.ModalityChanged += handler;

                return Disposable.Create(() => tracker.ModalityChanged -= handler);
            });
        }
    }
}
