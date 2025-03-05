using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ShawnHu.DropdownSearcher.Editor
{
    public class SearchablePopup : PopupWindowContent
    {
        private string searchText = "";
        private Vector2 scrollPosition;
        private readonly string[] displayedOptions;
        private readonly string[] searchTexts;
        private readonly int currentIndex;
        private readonly System.Action<int> onOptionSelected;
        private readonly string title;
        private List<int> filteredIndices;
        private int hoverIndex = -1;
        private float rowHeight = 20f;
        private float headerHeight = 48f;

        public SearchablePopup(string[] options, string[] searchTexts, int currentIndex, string title, System.Action<int> onOptionSelected)
        {
            this.displayedOptions = options;
            this.searchTexts = searchTexts;
            this.currentIndex = currentIndex;
            this.title = title;
            this.onOptionSelected = onOptionSelected;
            this.filteredIndices = Enumerable.Range(0, options.Length).ToList();
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, Mathf.Min(600, headerHeight + filteredIndices.Count * rowHeight));
        }

        public override void OnGUI(Rect rect)
        {
            // 标题
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            // 搜索框
            GUI.SetNextControlName("SearchField");
            string newSearch = EditorGUILayout.TextField("搜索", searchText);
            if (newSearch != searchText)
            {
                searchText = newSearch;
                UpdateFilter();
            }

            // 自动聚焦搜索框
            if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() == "")
            {
                EditorGUI.FocusTextInControl("SearchField");
            }

            // 绘制选项列表
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            for (int i = 0; i < filteredIndices.Count; i++)
            {
                int optionIndex = filteredIndices[i];
                bool isSelected = optionIndex == currentIndex;
                bool isHovered = i == hoverIndex;

                Rect itemRect = EditorGUILayout.GetControlRect(GUILayout.Height(rowHeight));
                
                if (Event.current.type == EventType.Repaint)
                {
                    if (isSelected)
                        EditorStyles.selectionRect.Draw(itemRect, false, false, true, false);
                    else if (isHovered)
                        EditorStyles.helpBox.Draw(itemRect, false, false, false, false);
                }

                if (GUI.Button(itemRect, displayedOptions[optionIndex], isSelected ? EditorStyles.boldLabel : EditorStyles.label))
                {
                    onOptionSelected?.Invoke(optionIndex);
                    editorWindow.Close();
                }

                // 处理鼠标悬停
                if (itemRect.Contains(Event.current.mousePosition))
                {
                    if (hoverIndex != i)
                    {
                        hoverIndex = i;
                        editorWindow.Repaint();
                    }
                }
            }
            
            EditorGUILayout.EndScrollView();

            // 处理键盘输入
            HandleKeyboard();
        }

        private void HandleKeyboard()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.UpArrow:
                        hoverIndex = Mathf.Max(0, hoverIndex - 1);
                        Event.current.Use();
                        editorWindow.Repaint();
                        break;

                    case KeyCode.DownArrow:
                        hoverIndex = Mathf.Min(filteredIndices.Count - 1, hoverIndex + 1);
                        Event.current.Use();
                        editorWindow.Repaint();
                        break;

                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        if (hoverIndex >= 0 && hoverIndex < filteredIndices.Count)
                        {
                            onOptionSelected?.Invoke(filteredIndices[hoverIndex]);
                            editorWindow.Close();
                        }
                        Event.current.Use();
                        break;

                    case KeyCode.Escape:
                        editorWindow.Close();
                        Event.current.Use();
                        break;
                }
            }
        }

        private void UpdateFilter()
        {
            if (string.IsNullOrEmpty(searchText))
            {
                filteredIndices = Enumerable.Range(0, displayedOptions.Length).ToList();
            }
            else
            {
                string search = searchText.ToLower();
                filteredIndices = Enumerable.Range(0, displayedOptions.Length)
                    .Where(i => displayedOptions[i].ToLower().Contains(search) || 
                               searchTexts[i].ToLower().Contains(search))
                    .ToList();
            }
            hoverIndex = filteredIndices.Count > 0 ? 0 : -1;
            editorWindow.Repaint();
        }
    }
} 