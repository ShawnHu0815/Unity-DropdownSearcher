using UnityEngine;
using ShawnHu.DropdownSearcher;

namespace ShawnHu.DropdownSearcher.Samples
{
    [CreateAssetMenu(fileName = "DemoSettings", menuName = "DropdownSearcher/Demo设置")]
    public class DropdownSearcherDemo : ScriptableObject
    {
        [Header("基础示例")]
        [Tooltip("普通的枚举下拉框")]
        public TestEnum NormalEnum;
        
        [Tooltip("支持搜索的枚举下拉框")]
        [SearchableEnum]
        public TestEnum SearchableTestEnum;

        [Header("Unity内置枚举示例")]
        [Space(10)]
        
        [Tooltip("普通的KeyCode选择")]
        public KeyCode NormalKeyCode;
        
        [Tooltip("支持搜索的KeyCode选择")]
        [SearchableEnum]
        public KeyCode SearchableKeyCode;
    }

    public enum TestEnum
    {
        [SearchText("第一个选项 First")]
        First,
        
        [SearchText("第二个选项 Second")]
        Second,
        
        [SearchText("第三个选项 Third")]
        Third,
        
        [SearchText("红色 Red")]
        Red,
        
        [SearchText("绿色 Green")]
        Green,
        
        [SearchText("蓝色 Blue")]
        Blue
    }
} 