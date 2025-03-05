using UnityEngine;

namespace ShawnHu.DropdownSearcher
{
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