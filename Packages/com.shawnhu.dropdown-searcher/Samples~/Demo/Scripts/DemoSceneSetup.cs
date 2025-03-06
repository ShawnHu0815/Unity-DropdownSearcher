using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ShawnHu.DropdownSearcher.Samples
{
    public class DemoSceneSetup
    {
        [MenuItem("DropdownSearcher/Setup Demo Scene")]
        public static void SetupDemoScene()
        {
            // 创建新场景
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            
            // 创建Canvas
            var canvas = new GameObject("Demo Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvasComponent = canvas.GetComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // 创建说明文本
            var textGo = new GameObject("Description Text", typeof(TextMeshProUGUI));
            textGo.transform.SetParent(canvas.transform, false);
            var text = textGo.GetComponent<TextMeshProUGUI>();
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            text.rectTransform.anchoredPosition = new Vector2(0, 0);
            text.rectTransform.sizeDelta = new Vector2(800, 600);
            
            // 创建DemoUI
            var demoUI = new GameObject("Demo UI", typeof(DemoUI));
            var demoUIComponent = demoUI.GetComponent<DemoUI>();
            demoUIComponent.descriptionText = text;
            
            // 创建DemoSettings资产
            var demoSettings = ScriptableObject.CreateInstance<DropdownSearcherDemo>();
            AssetDatabase.CreateAsset(demoSettings, "Assets/Samples/Dropdown Searcher/0.1.0/Demo/DemoSettings.asset");
            demoUIComponent.demoSettings = demoSettings;
            
            // 保存场景
            EditorSceneManager.SaveScene(scene, "Assets/Samples/Dropdown Searcher/0.1.0/Demo/Scenes/DemoScene.unity");
            
            Debug.Log("Demo场景设置完成！");
        }
    }
} 