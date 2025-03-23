using UnityEngine;
using UnityEditor;
using System;

namespace ShawnHu.DropdownSearcher.Editor
{
    /// <summary>
    /// 可搜索枚举绘制器
    /// 实现了带搜索功能的枚举下拉菜单的自定义绘制
    /// 通过重写PropertyDrawer的OnGUI方法，替换Unity默认的枚举绘制方式
    /// </summary>
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumDrawer : PropertyDrawer
    {
        /// <summary>
        /// 绘制带搜索功能的枚举字段
        /// </summary>
        /// <param name="position">字段在Inspector中的矩形区域</param>
        /// <param name="property">序列化属性，包含枚举值</param>
        /// <param name="label">字段标签</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 检查属性类型是否为枚举，如果不是则显示错误信息
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                GUIStyle errorStyle = "CN EntryErrorIconSmall";
                Rect r = new Rect(position);
                r.width = errorStyle.fixedWidth;
                position.xMin = r.xMax;
                GUI.Label(r, "", errorStyle);
                GUI.Label(position, "SearchableEnum can only be used on enum fields.");
                return;
            }
            
            // 获取枚举类型和枚举值名称数组
            var enumType = fieldInfo.FieldType;
            var names = Enum.GetNames(enumType);
            
            // 准备搜索文本数组，用于存储每个枚举值对应的搜索文本
            var searchTexts = new string[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                // 获取枚举成员信息，提取SearchTextAttribute属性
                var memberInfo = enumType.GetMember(names[i])[0];
                var searchAttributes = memberInfo.GetCustomAttributes(typeof(SearchTextAttribute), false);
                // 如果存在SearchTextAttribute，则使用其指定的搜索文本，否则使用枚举名称本身
                searchTexts[i] = searchAttributes.Length > 0 
                    ? ((SearchTextAttribute)searchAttributes[0]).SearchText 
                    : names[i];
            }

            // 手动创建控件ID，保持标签和按钮的ID一致
            // 这样可以让它们能够在Inspector中通过键盘一起选择，类似标准下拉菜单的行为
            int id = GUIUtility.GetControlID(FocusType.Keyboard, position);
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            // 准备按钮显示的文本内容
            GUIContent buttonText;
            if (property.enumValueIndex < 0 || property.enumValueIndex >= names.Length) {
                // 枚举值索引无效时，显示空内容
                buttonText = new GUIContent();
            }
            else {
                // 显示当前选中的枚举值名称
                buttonText = new GUIContent(names[property.enumValueIndex]);
            }
            
            // 当点击下拉按钮时，显示自定义的可搜索弹出窗口
            if (DropdownButton(id, position, buttonText))
            {
                PopupWindow.Show(
                    position, 
                    new SearchablePopup(
                        names,            // 枚举名称数组
                        searchTexts,      // 搜索文本数组
                        property.enumValueIndex,  // 当前选中的枚举值索引
                        label.text,       // 标签文本
                        (index) =>        // 选择回调函数，当用户选择一个选项时被调用
                        { 
                            // 更新枚举值并应用修改
                            property.enumValueIndex = index;
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    )
                );
            }

            // 结束属性编辑
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// 自定义下拉按钮的实现
        /// 处理鼠标点击和键盘输入事件，实现与Unity标准下拉菜单相似的行为
        /// </summary>
        /// <param name="id">控件的唯一标识ID</param>
        /// <param name="position">按钮在Inspector中的矩形区域</param>
        /// <param name="content">按钮显示的内容</param>
        /// <returns>如果按钮被点击返回true，否则返回false</returns>
        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    // 处理鼠标点击事件，当左键点击按钮区域时返回true
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();  // 标记事件已被处理
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    // 处理键盘事件，当按钮获得焦点并按下回车键时返回true
                    if (GUIUtility.keyboardControl == id && current.character == '\n')
                    {
                        Event.current.Use();  // 标记事件已被处理
                        return true;
                    }
                    break;
                case EventType.Repaint:
                    // 重绘事件，使用Unity内置的popup样式绘制按钮
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }
            return false;  // 默认返回false，表示按钮未被激活
        }
    }
} 