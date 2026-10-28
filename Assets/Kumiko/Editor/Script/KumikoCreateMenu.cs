using UnityEditor;
using UnityEngine;


namespace Kumiko.Editor
{
    /// <summary>
    /// エディターのCreateMenuを拡張する。
    /// </summary>
    public static class KumikoCreateMenu
    {
        private const string ButtonPrefabPath = "Packages/com.sorane.kumiko/Editor/Prefabs/Button.prefab";
        private const string SliderPrefabPath = "Packages/com.sorane.kumiko/Editor/Prefabs/Slider.prefab";
        private const string TogglePrefabPath = "Packages/com.sorane.kumiko/Editor/Prefabs/Toggle.prefab";


        [MenuItem("GameObject/Kumiko/Button", isValidateFunction: false, priority: 0)]
        private static void CreateButton(MenuCommand command)
        {
            Create(command, ButtonPrefabPath);
        }


        [MenuItem("GameObject/Kumiko/Slider", isValidateFunction: false, priority: 1)]
        private static void CreateSlider(MenuCommand command)
        {
            Create(command, SliderPrefabPath);
        }


        [MenuItem("GameObject/Kumiko/Toggle", isValidateFunction: false, priority: 2)]
        private static void CreateToggle(MenuCommand command)
        {
            Create(command, TogglePrefabPath);
        }


        private static void Create(MenuCommand command, string prefabPath)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"プレハブ： {prefabPath} が見つかりません。");
                return;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            var parent = command.context as GameObject;

            // ルートで生成する場合は、シーン内のCanvasを親にする。
            if (parent == null)
            {
                Canvas canvas = Object.FindFirstObjectByType<Canvas>();

                if (canvas != null)
                {
                    parent = canvas.gameObject;
                }
            }

            GameObjectUtility.SetParentAndAlign(instance, parent);

            PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");

            Selection.activeGameObject = instance;
        }
    }
}
