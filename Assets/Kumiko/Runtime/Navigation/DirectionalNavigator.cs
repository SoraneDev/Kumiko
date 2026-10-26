using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Kumiko
{
    /// <summary>
    /// UIの方向入力を受けたとき、その方向にある要素へ選択を移す。
    /// </summary>
    /// <remarks>
    /// 遷移先を設定した方向では、その要素を優先する。
    /// 遷移先が未設定、または選択できない方向では、Modeに応じて遷移先を自動で探す。
    /// 自動で探す対象は、DirectionalNavigatorを持ち、同じルートのCanvasにある、選択できる要素だけ。
    /// 要素がUsesMoveInputでtrueを返す方向（スライダーの値の変更など）では、選択を移さない。
    /// </remarks>
    [DisallowMultipleComponent]
    [AddComponentMenu("Kumiko/DirectionalNavigator")]
    public sealed class DirectionalNavigator : KumikoModule, IMoveHandler
    {
        [SerializeField, Tooltip("遷移先が未設定、または選択できない方向で、遷移先を自動で探す範囲")]
        private NavigationMode _mode = NavigationMode.Manual;

        [SerializeField, Tooltip("HorizontalかVerticalのとき、端から反対側の端へ回り込むかどうか")]
        private bool _wrapAround;

        [SerializeField] private KumikoElement _selectOnUp;
        [SerializeField] private KumikoElement _selectOnDown;
        [SerializeField] private KumikoElement _selectOnLeft;
        [SerializeField] private KumikoElement _selectOnRight;


        // 遷移先を自動で探すときの候補として、有効なDirectionalNavigatorを登録する。
        private static readonly List<DirectionalNavigator> s_activeNavigators = new();


        // Unityのドメインリロードを無効にしている場合、静的変数がリセットされないため、明示的にリセットする。
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_activeNavigators.Clear();
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            s_activeNavigators.Add(this);
        }


        protected override void OnDisable()
        {
            base.OnDisable();

            s_activeNavigators.Remove(this);
        }


        public void OnMove(AxisEventData eventData)
        {
            // 要素自身が操作に使う方向では、値の変更と選択の移動が同時に起きないように選択を移さない。
            if (Element.UsesMoveInput(eventData.moveDir)) return;

            KumikoElement target = FindTarget(eventData.moveDir);

            if (target == null) return;

            eventData.selectedObject = target.gameObject;
        }


        /// <summary>
        /// 指定した方向の入力で、選択を移す先の要素を探す。
        /// </summary>
        /// <param name="direction">方向入力の向き。</param>
        /// <returns>選択を移す先の要素。見つからない場合はnullを返す。</returns>
        /// <remarks>設定した遷移先が選択できる場合はそれを返し、そうでない場合はModeに応じて自動で探す。</remarks>
        public KumikoElement FindTarget(MoveDirection direction)
        {
            KumikoElement manualTarget = GetManualTarget(direction);

            if (CanSelect(manualTarget)) return manualTarget;

            return IsAutomaticDirection(direction) ? FindAutomaticTarget(direction) : null;
        }


        private KumikoElement GetManualTarget(MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Up => _selectOnUp,
                MoveDirection.Down => _selectOnDown,
                MoveDirection.Left => _selectOnLeft,
                MoveDirection.Right => _selectOnRight,
                _ => null,
            };
        }


        private bool IsAutomaticDirection(MoveDirection direction)
        {
            bool isHorizontal = direction is MoveDirection.Left or MoveDirection.Right;
            bool isVertical = direction is MoveDirection.Up or MoveDirection.Down;

            return _mode switch
            {
                NavigationMode.Automatic => isHorizontal || isVertical,
                NavigationMode.Horizontal => isHorizontal,
                NavigationMode.Vertical => isVertical,
                _ => false,
            };
        }


        private KumikoElement FindAutomaticTarget(MoveDirection direction)
        {
            Vector2 localDirection = ToLocalVector(direction);
            Vector3 worldDirection = transform.rotation * localDirection;
            Canvas rootCanvas = GetRootCanvas(this);

            // 自身の中心ではなく、進む向きの辺から測る。大きさの違う要素どうしでも、間の距離で比べるため。
            var rectTransform = (RectTransform)transform;
            Vector3 origin = rectTransform.TransformPoint(GetEdgeCenter(rectTransform, localDirection));

            KumikoElement target = FindNearestInDirection(origin, worldDirection, rootCanvas, out float minDistanceAlong);

            // 前方に見つからず、後ろに要素がある場合だけ回り込む。
            bool wrapsAround = _wrapAround && (_mode is NavigationMode.Horizontal or NavigationMode.Vertical);

            if (target != null || !wrapsAround || minDistanceAlong >= 0f) return target;

            // 自身が反対側の端のさらに外にあるとして、同じ向きに探し直す。
            // 起点を、後ろで最も遠い要素の位置から、自身の長さ1つ分だけ外側に置く。
            Vector3 backEdge = rectTransform.TransformPoint(GetEdgeCenter(rectTransform, -localDirection));
            float ownLength = Vector3.Dot(worldDirection, origin - backEdge);
            Vector3 wrappedOrigin = origin + worldDirection * (minDistanceAlong - ownLength);

            return FindNearestInDirection(wrappedOrigin, worldDirection, rootCanvas, out _);
        }


        // Unity標準のSelectableと同じ評価方法で、originからdirectionの向きにある最も近い要素を探す。
        // minDistanceAlongには、候補の向きに沿った距離の最小値を返す。負の場合は、後ろに要素がある。
        private KumikoElement FindNearestInDirection(
            Vector3 origin,
            Vector3 direction,
            Canvas rootCanvas,
            out float minDistanceAlong
        )
        {
            KumikoElement nearestTarget = null;
            float bestScore = float.NegativeInfinity;
            minDistanceAlong = 0f;

            foreach (DirectionalNavigator candidate in s_activeNavigators)
            {
                if (!IsCandidate(candidate, rootCanvas)) continue;

                var candidateRect = (RectTransform)candidate.transform;
                Vector3 toCandidate = candidateRect.TransformPoint(candidateRect.rect.center) - origin;

                // 向きに沿って、どれだけ先にあるか。
                float distanceAlong = Vector3.Dot(direction, toCandidate);
                minDistanceAlong = Mathf.Min(minDistanceAlong, distanceAlong);

                // 後ろや真横にある要素は対象外。距離が0の要素を除くことで、下の割り算も安全になる。
                if (distanceAlong <= 0f) continue;

                // 近いほど、また向きからのずれが小さいほど高く評価する。
                // この値は「cos(ずれの角度) / 距離」と等しく、ずれと距離を1つの値で比べられる。
                float score = distanceAlong / toCandidate.sqrMagnitude;

                if (score > bestScore)
                {
                    bestScore = score;
                    nearestTarget = candidate.Element;
                }
            }

            return nearestTarget;
        }


        private bool IsCandidate(DirectionalNavigator candidate, Canvas rootCanvas)
        {
            if (candidate == this || !CanSelect(candidate.Element)) return false;

            // 別のCanvasの要素とは座標を正しく比べられないため、同じルートのCanvasにある要素だけを対象にする。
            return GetRootCanvas(candidate) == rootCanvas;
        }


        private static bool CanSelect(KumikoElement element)
        {
            return element != null && element.IsActive() && element.IsInteractable();
        }


        private static Vector2 ToLocalVector(MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Up => Vector2.up,
                MoveDirection.Down => Vector2.down,
                MoveDirection.Left => Vector2.left,
                MoveDirection.Right => Vector2.right,
                _ => Vector2.zero,
            };
        }


        // 矩形の中心から指定した向きにある辺の中点を、ローカル座標で求める。
        private static Vector2 GetEdgeCenter(RectTransform rectTransform, Vector2 direction)
        {
            Rect rect = rectTransform.rect;
            return rect.center + Vector2.Scale(rect.size, direction * 0.5f);
        }


        private static Canvas GetRootCanvas(Component component)
        {
            Canvas canvas = component.GetComponentInParent<Canvas>();
            return canvas != null ? canvas.rootCanvas : null;
        }
    }
}
