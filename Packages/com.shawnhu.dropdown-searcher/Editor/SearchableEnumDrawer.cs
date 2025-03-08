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

            // By manually creating the control ID, we can keep the ID for the
            // label and button the same. This lets them be selected together
            // with the keyboard in the inspector, much like a normal popup.
            int id = GUIUtility.GetControlID(FocusType.Keyboard, position);
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText;
            if (property.enumValueIndex < 0 || property.enumValueIndex >= names.Length) {
                buttonText = new GUIContent();
            }
            else {
                buttonText = new GUIContent(names[property.enumValueIndex]);
            }
            
            if (DropdownButton(id, position, buttonText))
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

        private static bool DropdownButton(int id, Rect position, GUIContent content)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && current.button == 0)
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == id && current.character == '\n')
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
                case EventType.Repaint:
                    EditorStyles.popup.Draw(position, content, id, false);
                    break;
            }
            return false;
        }
    }
} 