using UnityEngine;
using UnityEditor;
using System;

namespace ShawnHu.DropdownSearcher.Editor
{
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public class SearchableEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                var enumType = fieldInfo.FieldType;
                var names = Enum.GetNames(enumType);
                
                // 准备搜索文本
                var searchTexts = new string[names.Length];
                for (int i = 0; i < names.Length; i++)
                {
                    var memberInfo = enumType.GetMember(names[i])[0];
                    var searchAttributes = memberInfo.GetCustomAttributes(typeof(SearchTextAttribute), false);
                    searchTexts[i] = searchAttributes.Length > 0 
                        ? ((SearchTextAttribute)searchAttributes[0]).SearchText 
                        : names[i];
                }

                EditorGUI.BeginProperty(position, label, property);

                if (GUI.Button(position, new GUIContent($"{label.text}: {names[property.enumValueIndex]}")))
                {
                    PopupWindow.Show(
                        position, 
                        new SearchablePopup(
                            names,
                            searchTexts,
                            property.enumValueIndex,
                            label.text,
                            (index) => 
                            {
                                property.enumValueIndex = index;
                                property.serializedObject.ApplyModifiedProperties();
                            }
                        )
                    );
                }

                EditorGUI.EndProperty();
            }
        }
    }
} 