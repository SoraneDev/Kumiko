using UnityEditor;
using UnityEngine;


namespace Kumiko.Editor
{
    public static class KumikoCreateMenu
    {
        private const string KumikoButtonPath = "Packages/com.sorane.kumiko/Editor/Prefabs/Button.prefab";
        private const string SliderPath = "Packages/com.sorane.kumiko/Editor/Prefabs/Slider.prefab";
        private const string TogglePath = "Packages/com.sorane.kumiko/Editor/Prefabs/Toggle.prefab";
    
    
        [MenuItem("GameObject/Kumiko/Button", false, 10)]
        private static void CreateButton(MenuCommand command) => Create(command, KumikoButtonPath);
    
        
        [MenuItem("GameObject/Kumiko/Slider", false, 11)]
        private static void CreateSlider(MenuCommand command) => Create(command, SliderPath);
    
        
        [MenuItem("GameObject/Kumiko/Toggle", false, 12)]
        private static void CreateToggle(MenuCommand command) => Create(command, TogglePath);
        
        
        private static void Create(MenuCommand command, string prefabPath)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"Prefab not found: {prefabPath}");
                return;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            GameObject parent = command.context as GameObject;

            // ルートで生成する場合はCanvasを検索
            if (parent == null)
            {
                var canvas = Object.FindFirstObjectByType<Canvas>();

                if (canvas != null)
                    parent = canvas.gameObject;
            }

            GameObjectUtility.SetParentAndAlign(instance, parent);

            PrefabUtility.UnpackPrefabInstance(
                instance,
                PrefabUnpackMode.Completely,
                InteractionMode.AutomatedAction
            );

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");

            Selection.activeGameObject = instance;
        }
    }
}