using UnityEngine;
using UnityEngine.EventSystems;


namespace Kumiko
{
    /// <summary>
    /// UI要素にアタッチされるモジュールの基底クラス。
    /// </summary>
    [RequireComponent(typeof(KumikoElement))]
    public abstract class KumikoModule : UIBehaviour
    {
        private KumikoElement _element;


        /// <summary>同じ<see cref="GameObject" />にアタッチされている<see cref="KumikoElement" />。</summary>
        protected KumikoElement Element
        {
            get
            {
                // Unityのオブジェクトは、??=では破棄されたかどうかを判定できないため、== nullで判定する。
                if (_element == null)
                {
                    _element = GetComponent<KumikoElement>();
                }

                return _element;
            }
        }
    }
}
