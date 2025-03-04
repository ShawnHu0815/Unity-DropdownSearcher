using System;

namespace ShawnHu.DropdownSearcher
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SearchTextAttribute : Attribute
    {
        public string SearchText { get; private set; }

        public SearchTextAttribute(string searchText)
        {
            SearchText = searchText;
        }
    }
} 