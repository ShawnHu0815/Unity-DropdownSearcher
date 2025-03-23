using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ShawnHu.DropdownSearcher.Editor
{
    /// <summary>
    /// 可搜索的弹出窗口
    /// 实现了支持搜索功能的自定义下拉菜单内容
    /// 通过继承PopupWindowContent类，提供搜索和过滤功能
    /// </summary>
    public class SearchablePopup : PopupWindowContent
    {
        /// <summary>
        /// 搜索文本，用户在输入框中输入的内容
        /// </summary>
        private string searchText = "";
        
        /// <summary>
        /// 滚动位置，控制选项列表的滚动状态
        /// </summary>
        private Vector2 scrollPosition;
        
        /// <summary>
        /// 显示选项的文本数组，用于在UI中显示
        /// </summary>
        private readonly string[] displayedOptions;
        
        /// <summary>
        /// 搜索文本数组，存储每个选项对应的可搜索文本（包括中文）
        /// </summary>
        private readonly string[] searchTexts;
        
        /// <summary>
        /// 当前选中的选项索引
        /// </summary>
        private readonly int currentIndex;
        
        /// <summary>
        /// 选项被选中时的回调函数，参数为选中项的索引
        /// </summary>
        private readonly System.Action<int> onOptionSelected;
        
        /// <summary>
        /// 弹出窗口的标题
        /// </summary>
        private readonly string title;
        
        /// <summary>
        /// 过滤后的选项索引列表，存储符合搜索条件的选项索引
        /// </summary>
        private List<int> filteredIndices;
        
        /// <summary>
        /// 当前鼠标悬停选项的索引，用于高亮显示
        /// </summary>
        private int hoverIndex = -1;
        
        /// <summary>
        /// 每行选项的高度，单位为像素
        /// </summary>
        private float rowHeight = 20f;
        
        /// <summary>
        /// 窗口头部（包含搜索框）的高度，单位为像素
        /// </summary>
        private float headerHeight = 48f;

        /// <summary>
        /// 构造函数，初始化可搜索弹出窗口
        /// </summary>
        /// <param name="options">选项显示文本数组</param>
        /// <param name="searchTexts">选项搜索文本数组，用于搜索匹配</param>
        /// <param name="currentIndex">当前选中的索引</param>
        /// <param name="title">窗口标题</param>
        /// <param name="onOptionSelected">选项选中时的回调函数</param>
        public SearchablePopup(string[] options, string[] searchTexts, int currentIndex, string title, System.Action<int> onOptionSelected)
        {
            this.displayedOptions = options;
            this.searchTexts = searchTexts;
            this.currentIndex = currentIndex;
            this.title = title;
            this.onOptionSelected = onOptionSelected;
            // 初始化时显示所有选项
            this.filteredIndices = Enumerable.Range(0, options.Length).ToList();
        }
        
        /// <summary>
        /// 获取窗口大小
        /// 根据选项数量动态计算窗口高度，确保窗口大小适当
        /// </summary>
        /// <returns>窗口尺寸向量</returns>
        public override Vector2 GetWindowSize()
        {
            // 固定宽度
            float popupWidth = 200f;
            // 最大高度限制
            float maxHeight = 400f;
            // 根据选项数量计算高度
            float calculatedHeight = headerHeight + filteredIndices.Count * rowHeight;

            // 返回计算后的尺寸，高度不超过最大限制
            return new Vector2(popupWidth, Mathf.Min(maxHeight, calculatedHeight));
        }

        /// <summary>
        /// 绘制窗口GUI
        /// 实现搜索框、选项列表的绘制和交互逻辑
        /// </summary>
        /// <param name="rect">窗口矩形区域</param>
        public override void OnGUI(Rect rect)
        {
            // 设置搜索文本框的控件名称，用于后续获取焦点
            GUI.SetNextControlName("SearchField");
            // 获取搜索框的矩形区域
            Rect searchFieldRect = EditorGUILayout.GetControlRect();
        
            // 绘制搜索文本框，使用当前搜索文本作为内容
            string newSearch = EditorGUI.TextField(searchFieldRect, searchText);
            // 当搜索文本变化时，更新过滤结果
            if (newSearch != searchText)
            {
                searchText = newSearch;
                UpdateFilter();
            }
        
            // 自动聚焦搜索框，使用户可以直接开始输入
            if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() == "")
            {
                EditorGUI.FocusTextInControl("SearchField");
            }

            // 计算可用宽度（减去垂直滚动条宽度）
            float scrollViewWidth = rect.width - GUI.skin.verticalScrollbar.fixedWidth;

            // 开始滚动视图，显示选项列表
            scrollPosition = EditorGUILayout.BeginScrollView(
                scrollPosition, 
                GUILayout.Width(rect.width) // 滚动视图占用完整宽度
            );
        
            // 遍历过滤后的选项索引列表，绘制每个选项
            for (int i = 0; i < filteredIndices.Count; i++)
            {
                // 获取原始选项索引
                int optionIndex = filteredIndices[i];
                // 判断当前选项是否被选中（与当前值匹配）
                bool isSelected = optionIndex == currentIndex;
                // 判断当前选项是否被鼠标悬停
                bool isHovered = i == hoverIndex;
        
                // 获取选项项的矩形区域，设置高度和宽度
                Rect itemRect = EditorGUILayout.GetControlRect(
                    GUILayout.Height(rowHeight),
                    GUILayout.Width(scrollViewWidth-10) // 约束为可用宽度，留出边距
                );

                // 在重绘事件中绘制选中或悬停状态的背景
                if (Event.current.type == EventType.Repaint)
                {
                    if (isSelected)
                        // 使用选中状态的样式绘制背景
                        EditorStyles.selectionRect.Draw(itemRect, false, false, true, false);
                    else if (isHovered)
                        // 使用悬停状态的样式绘制背景
                        EditorStyles.helpBox.Draw(itemRect, false, false, false, false);
                }
        
                // 创建一个按钮，点击时触发选项选中回调
                if (GUI.Button(itemRect, displayedOptions[optionIndex], isSelected ? EditorStyles.boldLabel : EditorStyles.label))
                {
                    // 调用回调函数，传递选中项的索引
                    onOptionSelected?.Invoke(optionIndex);
                    // 关闭弹出窗口
                    editorWindow.Close();
                }
        
                // 处理鼠标悬停逻辑
                if (itemRect.Contains(Event.current.mousePosition))
                {
                    // 如果鼠标悬停位置变化，更新悬停索引并重绘窗口
                    if (hoverIndex != i)
                    {
                        hoverIndex = i;
                        editorWindow.Repaint();
                    }
                }
            }
        
            // 结束滚动视图
            EditorGUILayout.EndScrollView();
        
            // 处理键盘输入（上下键导航、回车选择、ESC关闭）
            HandleKeyboard();
        }

        /// <summary>
        /// 处理键盘输入
        /// 实现键盘导航和快捷操作功能
        /// </summary>
        private void HandleKeyboard()
        {
            // 只在键盘按下事件时处理
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.UpArrow:
                        // 向上导航，选择上一个选项
                        hoverIndex = Mathf.Max(0, hoverIndex - 1);
                        // 标记事件已处理
                        Event.current.Use();
                        // 重绘窗口以更新显示
                        editorWindow.Repaint();
                        break;

                    case KeyCode.DownArrow:
                        // 向下导航，选择下一个选项
                        hoverIndex = Mathf.Min(filteredIndices.Count - 1, hoverIndex + 1);
                        // 标记事件已处理
                        Event.current.Use();
                        // 重绘窗口以更新显示
                        editorWindow.Repaint();
                        break;

                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        // 回车键确认选择当前悬停的选项
                        if (hoverIndex >= 0 && hoverIndex < filteredIndices.Count)
                        {
                            // 调用回调函数，传递选中项的索引
                            onOptionSelected?.Invoke(filteredIndices[hoverIndex]);
                            // 关闭弹出窗口
                            editorWindow.Close();
                        }
                        // 标记事件已处理
                        Event.current.Use();
                        break;

                    case KeyCode.Escape:
                        // ESC键关闭窗口
                        editorWindow.Close();
                        // 标记事件已处理
                        Event.current.Use();
                        break;
                }
            }
        }

        /// <summary>
        /// 更新过滤结果
        /// 根据搜索文本筛选符合条件的选项
        /// </summary>
        private void UpdateFilter()
        {
            // 如果搜索文本为空，显示所有选项
            if (string.IsNullOrEmpty(searchText))
            {
                filteredIndices = Enumerable.Range(0, displayedOptions.Length).ToList();
            }
            else
            {
                // 将搜索文本转为小写，便于不区分大小写的比较
                string search = searchText.ToLower();
                // 使用LINQ查询筛选符合条件的选项
                filteredIndices = Enumerable.Range(0, displayedOptions.Length)
                    .Where(i => 
                        // 选项显示文本包含搜索文本
                        displayedOptions[i].ToLower().Contains(search) || 
                        // 或选项搜索文本（包括中文别名）包含搜索文本
                        searchTexts[i].ToLower().Contains(search))
                    .ToList();
            }
            // 更新悬停索引，默认选中第一个过滤结果
            hoverIndex = filteredIndices.Count > 0 ? 0 : -1;
            // 重绘窗口以更新显示
            editorWindow.Repaint();
        }
    }
} 