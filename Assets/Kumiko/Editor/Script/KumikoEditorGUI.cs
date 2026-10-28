using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Kumiko.Editor
{
    /// <summary>
    /// KumikoのInspectorで共通して使う描画処理を提供する。
    /// </summary>
    internal static class KumikoEditorGUI
    {
        /// <summary>
        /// 標準のInspectorと同じように、スクリプトの欄を編集できない状態で描画する。
        /// </summary>
        /// <param name="script">m_Scriptのプロパティ。</param>
        internal static void DrawScriptField(SerializedProperty script)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(script);
            }
        }


        /// <summary>
        /// 方向の欄を描画する。変更された場合は、setDirectionで各対象の方向とレイアウトを変更する。
        /// </summary>
        /// <typeparam name="T">方向を持つコンポーネントの型。</typeparam>
        /// <param name="direction">方向のプロパティ。</param>
        /// <param name="setDirection">対象の方向とレイアウトを変更する処理。</param>
        /// <remarks>レイアウトの変更は子のRectTransformにも及ぶため、階層全体をUndoに記録する。</remarks>
        internal static void DrawDirectionField<T>(SerializedProperty direction, Action<T, SlideDirection> setDirection)
            where T : Component
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(direction);

            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            var nextDirection = (SlideDirection)direction.enumValueIndex;

            foreach (Object targetObject in direction.serializedObject.targetObjects)
            {
                if (targetObject is not T target)
                {
                    continue;
                }

                Undo.RegisterFullObjectHierarchyUndo(target.gameObject, "Change Direction");
                setDirection(target, nextDirection);
                RecordPrefabInstanceModifications(target);
            }
        }


        // プレハブのインスタンスでは、スクリプトからの変更をオーバーライドとして記録しないと、再読み込みで元に戻る。
        // プレハブのインスタンスでない場合は、何もしない。
        private static void RecordPrefabInstanceModifications(Component target)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);

            foreach (RectTransform rectTransform in target.GetComponentsInChildren<RectTransform>(true))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(rectTransform);
            }
        }
    }
}
