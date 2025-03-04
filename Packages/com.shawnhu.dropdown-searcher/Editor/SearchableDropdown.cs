using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ShawnHu.DropdownSearcher.Editor
{
    public class SearchableDropdown : EditorWindow
    {
        private string searchText = "";
        private Vector2 scrollPosition;
        private List<SearchableOption> options;
        private System.Action<int> onOptionSelected;
        private int selectedIndex;
        private string windowTitle;

        public class SearchableOption
        {
            public string DisplayName { get; set; }
            public string SearchText { get; set; }
            public int Index { get; set; }
        }

        public static void Show(Rect position, string[] options, string[] searchTexts, int selectedIndex, string title, System.Action<int> onOptionSelected)
        {
            var window = CreateInstance<SearchableDropdown>();
            window.Init(options, searchTexts, selectedIndex, title, onOptionSelected);
            window.ShowAsDropDown(position, new Vector2(position.width, 200));
        }

        private void Init(string[] optionNames, string[] searchTexts, int currentIndex, string windowTitle, System.Action<int> callback)
        {
            options = new List<SearchableOption>();
            for (int i = 0; i < optionNames.Length; i++)
            {
                options.Add(new SearchableOption
                {
                    DisplayName = optionNames[i],
                    SearchText = searchTexts[i],
                    Index = i
                });
            }
            
            selectedIndex = currentIndex;
            onOptionSelected = callback;
            this.windowTitle = windowTitle;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(windowTitle, EditorStyles.boldLabel);
            
            GUI.SetNextControlName("SearchField");
            searchText = EditorGUILayout.TextField("搜索", searchText);
            
            if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() == "")
            {
                EditorGUI.FocusTextInControl("SearchField");
            }

            var filteredOptions = options.Where(option =>
                string.IsNullOrEmpty(searchText) ||
                option.DisplayName.ToLower().Contains(searchText.ToLower()) ||
                option.SearchText.ToLower().Contains(searchText.ToLower())
            ).ToList();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            for (int i = 0; i < filteredOptions.Count; i++)
            {
                var option = filteredOptions[i];
                bool isSelected = option.Index == selectedIndex;
                
                if (GUILayout.Button(option.DisplayName, isSelected ? EditorStyles.selectionRect : EditorStyles.label))
                {
                    onOptionSelected?.Invoke(option.Index);
                    Close();
                }
            }
            
            EditorGUILayout.EndScrollView();

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Close();
            }
        }
    }
} 