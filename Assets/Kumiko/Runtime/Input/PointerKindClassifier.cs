using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;


namespace Kumiko
{
    /// <summary>
    /// <see cref="PointerEventData" />の入力をタッチとポインタに分類する。
    /// </summary>
    internal static class PointerKindClassifier
    {
        /// <summary>
        /// <see cref="PointerEventData" />から、
        /// 入力種別が<see cref="InputModality.Touch" />か<see cref="InputModality.Pointer" />かを判別する。
        /// </summary>
        /// <param name="eventData">分類する<see cref="PointerEventData" />。</param>
        /// <returns><see cref="InputModality.Touch" />または<see cref="InputModality.Pointer" />を返す。</returns>
        internal static InputModality Classify(PointerEventData eventData)
        {
            // InputSystemのUIモジュールでは、pointerIdはデバイスごとの識別子で、入力種別を表さない。
            // そのため、ExtendedPointerEventDataのpointerTypeで判定する。
            if (eventData is ExtendedPointerEventData extended)
            {
                return extended.pointerType == UIPointerType.Touch ? InputModality.Touch : InputModality.Pointer;
            }

            // InputSystemのUIモジュール以外のイベントは、入力種別を判別できないため、Pointerとして扱う。
            return InputModality.Pointer;
        }
    }
}
