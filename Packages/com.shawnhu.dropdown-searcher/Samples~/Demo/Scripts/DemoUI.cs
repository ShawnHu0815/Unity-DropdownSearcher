using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ShawnHu.DropdownSearcher.Samples
{
    public class DemoUI : MonoBehaviour
    {
        [SerializeField] private DropdownSearcherDemo demoSettings;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private void Start()
        {
            if (descriptionText != null)
            {
                descriptionText.text = "请在Inspector中查看DemoSettings资产\n" +
                                     "点击带有[SearchableEnum]特性的枚举字段\n" +
                                     "体验支持中英文搜索的下拉菜单";
            }
        }
    }
} 