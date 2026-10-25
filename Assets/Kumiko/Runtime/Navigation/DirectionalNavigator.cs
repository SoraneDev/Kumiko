using UnityEngine;
using UnityEngine.EventSystems;


namespace Kumiko
{
    /// <summary>
    /// UIの方向入力を受けたとき、設定した要素へ選択を移す。
    /// </summary>
    /// <remarks>遷移先が未設定、非アクティブ、または操作不可のときは選択を移さない。</remarks>
    [DisallowMultipleComponent]
    [AddComponentMenu("Kumiko/DirectionalNavigator")]
    public sealed class DirectionalNavigator : KumikoModule, IMoveHandler
    {
        [SerializeField] private KumikoElement _selectOnUp;
        [SerializeField] private KumikoElement _selectOnDown;
        [SerializeField] private KumikoElement _selectOnLeft;
        [SerializeField] private KumikoElement _selectOnRight;


        public void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Up:
                    Select(eventData, _selectOnUp);
                    break;
                case MoveDirection.Down:
                    Select(eventData, _selectOnDown);
                    break;
                case MoveDirection.Left:
                    Select(eventData, _selectOnLeft);
                    break;
                case MoveDirection.Right:
                    Select(eventData, _selectOnRight);
                    break;
            }
        }


        private static void Select(AxisEventData eventData, KumikoElement element)
        {
            if (element == null || !element.IsActive() || !element.IsInteractable()) return;

            eventData.selectedObject = element.gameObject;
        }
    }
}
