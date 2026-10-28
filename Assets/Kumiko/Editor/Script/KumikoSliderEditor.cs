using UnityEditor;
using UnityEngine;


namespace Kumiko.Editor
{
    /// <summary>
    /// <see cref="KumikoSlider" />のInspectorを拡張する。
    /// 方向を変更したときに、Unity標準の<see cref="UnityEngine.UI.Slider" />と同じようにレイアウトも合わせて変更する。
    /// </summary>
    [CustomEditor(typeof(KumikoSlider))]
    [CanEditMultipleObjects]
    public sealed class KumikoSliderEditor : UnityEditor.Editor
    {
        private SerializedProperty _script;
        private SerializedProperty _interactable;
        private SerializedProperty _fillRect;
        private SerializedProperty _handleRect;
        private SerializedProperty _direction;
        private SerializedProperty _minValue;
        private SerializedProperty _maxValue;
        private SerializedProperty _wholeNumbers;
        private SerializedProperty _value;
        private SerializedProperty _stepSize;


        private void OnEnable()
        {
            _script = serializedObject.FindProperty("m_Script");
            _interactable = serializedObject.FindProperty("_interactable");
            _fillRect = serializedObject.FindProperty("_fillRect");
            _handleRect = serializedObject.FindProperty("_handleRect");
            _direction = serializedObject.FindProperty("_direction");
            _minValue = serializedObject.FindProperty("_minValue");
            _maxValue = serializedObject.FindProperty("_maxValue");
            _wholeNumbers = serializedObject.FindProperty("_wholeNumbers");
            _value = serializedObject.FindProperty("_value");
            _stepSize = serializedObject.FindProperty("_stepSize");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            KumikoEditorGUI.DrawScriptField(_script);
            EditorGUILayout.PropertyField(_interactable);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_fillRect);
            EditorGUILayout.PropertyField(_handleRect);

            if (_fillRect.objectReferenceValue == null && _handleRect.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(
                    "Fill RectかHandle Rectの少なくとも一方を設定してください。それぞれ親のRectTransformの範囲内で伸縮・移動します。",
                    MessageType.Info);
            }

            KumikoEditorGUI.DrawDirectionField<KumikoSlider>(
                _direction,
                (slider, direction) => slider.SetDirection(direction, flipsLayout: true));

            DrawRangeFields();
            EditorGUILayout.PropertyField(_wholeNumbers);
            EditorGUILayout.Slider(_value, _minValue.floatValue, _maxValue.floatValue);
            EditorGUILayout.PropertyField(_stepSize);

            serializedObject.ApplyModifiedProperties();
        }


        // 最小値が最大値を超えないように、範囲を描画する。超える値は反映しない。
        private void DrawRangeFields()
        {
            EditorGUI.BeginChangeCheck();
            float minValue = DrawDelayedFloatField(_minValue);

            if (EditorGUI.EndChangeCheck() && minValue <= _maxValue.floatValue)
            {
                _minValue.floatValue = minValue;
            }

            EditorGUI.BeginChangeCheck();
            float maxValue = DrawDelayedFloatField(_maxValue);

            if (EditorGUI.EndChangeCheck() && maxValue >= _minValue.floatValue)
            {
                _maxValue.floatValue = maxValue;
            }
        }


        // 入力中の値で範囲の判定をしないように、確定してから値を返す欄を描画する。
        private static float DrawDelayedFloatField(SerializedProperty property)
        {
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            var label = new GUIContent(property.displayName, property.tooltip);
            float value = EditorGUILayout.DelayedFloatField(label, property.floatValue);

            EditorGUI.showMixedValue = false;
            return value;
        }
    }
}
