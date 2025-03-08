using UnityEngine;
using ShawnHu.DropdownSearcher;

namespace ShawnHu.DropdownSearcher.Samples
{
    [CreateAssetMenu(fileName = "DropdownSearcherDemo", menuName = "DropdownSearcher/Demo")]
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
        [SearchText("第一个选项 First Option")]
        FirstOption,
        
        [SearchText("第二个选项 Second Option")]
        SecondOption,
        
        [SearchText("第三个选项 Third Option")]
        ThirdOption,
        
        [SearchText("红色 Red")]
        Red,
        
        [SearchText("绿色 Green")]
        Green,
        
        [SearchText("蓝色 Blue")]
        Blue,
        
        [SearchText("黄色 Yellow")]
        Yellow,
        
        [SearchText("紫色 Purple")]
        Purple,
        
        [SearchText("橙色 Orange")]
        Orange,
        
        [SearchText("黑色 Black")]
        Black,
        
        [SearchText("白色 White")]
        White,
        
        [SearchText("灰色 Gray")]
        Gray,
        
        [SearchText("粉色 Pink")]
        Pink,
        
        [SearchText("棕色 Brown")]
        Brown,
        
        [SearchText("金色 Gold")]
        Gold,
        
        [SearchText("银色 Silver")]
        Silver,
        
        [SearchText("透明 Transparent")]
        Transparent,
        
        [SearchText("彩虹 Rainbow")]
        Rainbow,
        
        [SearchText("多彩 Multicolor")]
        Multicolor,
        
        [SearchText("荧光 Fluorescent")]
        Fluorescent,
        
        [SearchText("金属 Metallic")]
        Metallic,
        
        [SearchText("哑光 Matte")]
        Matte
        
    }

} 