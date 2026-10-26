using UnityEngine.EventSystems;


namespace Kumiko
{
    /// <summary>
    /// SlideDirectionの拡張メソッドを提供する。
    /// </summary>
    internal static class SlideDirectionExtensions
    {
        /// <summary>
        /// 値が変化する軸を、Vector2の要素番号で取得する。
        /// </summary>
        /// <param name="direction">対象の方向。</param>
        /// <returns>横方向なら0、縦方向なら1を返す。</returns>
        internal static int GetAxisIndex(this SlideDirection direction)
        {
            return direction is SlideDirection.LeftToRight or SlideDirection.RightToLeft ? 0 : 1;
        }


        /// <summary>
        /// 座標が増える向きと、値が増える向きが逆かどうかを取得する。
        /// </summary>
        /// <param name="direction">対象の方向。</param>
        /// <returns>逆の場合はtrue、同じ場合はfalseを返す。</returns>
        internal static bool IsReversed(this SlideDirection direction)
        {
            return direction is SlideDirection.RightToLeft or SlideDirection.TopToBottom;
        }


        /// <summary>
        /// 方向入力によって、値が変化する向きを取得する。
        /// </summary>
        /// <param name="direction">対象の方向。</param>
        /// <param name="moveDirection">方向入力の向き。</param>
        /// <returns>値が増える場合は1、減る場合は-1、値が変化する軸と異なる場合は0を返す。</returns>
        internal static int GetStepSign(this SlideDirection direction, MoveDirection moveDirection)
        {
            bool isHorizontal = direction.GetAxisIndex() == 0;

            // 座標が増える向きを正として求めてから、値の向きに合わせる。
            int sign = moveDirection switch
            {
                MoveDirection.Right when isHorizontal => 1,
                MoveDirection.Left when isHorizontal => -1,
                MoveDirection.Up when !isHorizontal => 1,
                MoveDirection.Down when !isHorizontal => -1,
                _ => 0,
            };

            return direction.IsReversed() ? -sign : sign;
        }
    }
}
