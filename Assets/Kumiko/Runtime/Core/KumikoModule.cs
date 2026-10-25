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


        protected KumikoElement Element
        {
            get
            {
                return _element ??= GetComponent<KumikoElement>();
            }
        }
    }
}
