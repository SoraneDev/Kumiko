using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;


namespace Kumiko
{
    /// <summary>
    /// PointerEventDataの入力をタッチとポインタに分類する。
    /// </summary>
    internal static class PointerKindClassifier
    {
        /// <summary>
        /// PointerEventDataの入力をタッチとポインタに分類する。
        /// </summary>
        /// <param name="eventData">分類したいPointerEventData。</param>
        /// <returns>入力種別を返す。</returns>
        internal static InputModality Classify(PointerEventData eventData)
        {
            // InputSystemのUIモジュールでは、pointerIdはデバイスごとの識別子で、入力種別を表さない。
            // そのため、ExtendedPointerEventDataのpointerTypeで判定する。
            if (eventData is ExtendedPointerEventData extended)
            {
                return extended.pointerType == UIPointerType.Touch ? InputModality.Touch : InputModality.Pointer;
            }

            // InputSystemのUIモジュール以外のイベントは、入力種別を判別できないためPointerとして扱う。
            return InputModality.Pointer;
        }
    }
}
