using UnityEngine;


namespace Kumiko
{
    /// <summary>
    /// RectTransformの拡張メソッドを提供する。
    /// </summary>
    internal static class RectTransformExtensions
    {
        /// <summary>
        /// 対象を動かしたり伸縮させたりする範囲として、対象の親のRectTransformを取得する。
        /// </summary>
        /// <param name="target">動かす対象。</param>
        /// <param name="owner">対象を動かすコンポーネントのTransform。</param>
        /// <param name="area">対象の親のRectTransform。</param>
        /// <returns>取得できた場合はtrue、対象が未設定・ownerと同じ・親がRectTransformでない場合はfalseを返す。</returns>
        /// <remarks>ownerと同じ対象は、自身のレイアウトを書き換えてしまうため扱わない。</remarks>
        internal static bool TryGetParentArea(this RectTransform target, Transform owner, out RectTransform area)
        {
            area = null;

            if (target == null || target == owner)
            {
                return false;
            }

            area = target.parent as RectTransform;
            return area != null;
        }


        /// <summary>
        /// スクリーン座標を、対象のローカル座標に変換する。
        /// </summary>
        /// <param name="rect">変換先のRectTransform。</param>
        /// <param name="screenPosition">変換するスクリーン座標。</param>
        /// <param name="eventCamera">イベントを受けたカメラ。Screen Space - Overlayの場合はnull。</param>
        /// <param name="localPosition">変換したローカル座標。</param>
        /// <returns>変換できた場合はtrue、できなかった場合はfalseを返す。</returns>
        internal static bool TryGetLocalPosition(
            this RectTransform rect,
            Vector2 screenPosition,
            Camera eventCamera,
            out Vector2 localPosition
        )
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPosition, eventCamera,
                out localPosition);
        }


        /// <summary>
        /// スクリーン座標が、対象の範囲内にあるかどうかを取得する。
        /// </summary>
        /// <param name="rect">判定するRectTransform。</param>
        /// <param name="screenPosition">判定するスクリーン座標。</param>
        /// <param name="eventCamera">イベントを受けたカメラ。Screen Space - Overlayの場合はnull。</param>
        /// <returns>範囲内の場合はtrue、範囲外の場合はfalseを返す。</returns>
        internal static bool ContainsScreenPosition(this RectTransform rect, Vector2 screenPosition, Camera eventCamera)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rect, screenPosition, eventCamera);
        }


        /// <summary>
        /// 方向の変化に合わせて、対象と子のレイアウトを入れ替え・反転する。
        /// </summary>
        /// <param name="rect">レイアウトを変更するRectTransform。</param>
        /// <param name="previousDirection">変更前の方向。</param>
        /// <param name="direction">変更後の方向。</param>
        /// <remarks>
        /// 横と縦が入れ替わる場合は、大きさ・アンカー・位置の横と縦を入れ替える。
        /// 値の向きが逆になる場合は、変更後の軸でレイアウトを反転する。
        /// 対象のアンカーが1点の場合は、置いた位置を保ったままピボットを中心に入れ替える。
        /// 親に合わせて伸縮している場合は、対象のアンカーも入れ替える。
        /// </remarks>
        internal static void FlipLayout(this RectTransform rect, SlideDirection previousDirection,
            SlideDirection direction)
        {
            // 伸縮するアンカーのまま大きさだけを入れ替えると、幅や高さが0になって表示されなくなるため、
            // アンカーが1点の場合だけ、アンカーと位置を保つ。
            bool keepsPosition = rect.anchorMin == rect.anchorMax;

            if (previousDirection.GetAxisIndex() != direction.GetAxisIndex())
            {
                RectTransformUtility.FlipLayoutAxes(rect, keepsPosition, recursive: true);
            }

            if (previousDirection.IsReversed() != direction.IsReversed())
            {
                int axis = direction.GetAxisIndex();
                RectTransformUtility.FlipLayoutOnAxis(rect, axis, keepsPosition, recursive: true);
            }
        }
    }
}
