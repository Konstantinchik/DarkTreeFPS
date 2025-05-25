using UnityEngine;
using UnityEngine.UI;

namespace DTInventory
{
    public class Tooltip : MonoBehaviour
    {
        public Text tooltip;
        public Text header;
        [HideInInspector]
        public RectTransform rectTransform;

        public static bool showTooltip = false;
        public static Item hoveredItem;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (showTooltip)
            {
                rectTransform.gameObject.SetActive(true);
                rectTransform.position = Input.mousePosition;
            }
            else
            {
                rectTransform.gameObject.SetActive(false);
            }
        }
        
        public void GenerateContent(Item item)
        {
            string tooltip;
            string header;

            if (item != null)
            {
                header = item.title;
                tooltip = item.description;
            }
            else
            {
                header = string.Empty;
                tooltip = string.Empty;
            }

            this.header.text = header;
            this.tooltip.text = tooltip;
        }
    }
}
