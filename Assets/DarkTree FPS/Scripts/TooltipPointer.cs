using UnityEngine;
using UnityEngine.EventSystems;

namespace DTInventory
{
    public class TooltipPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        Item item;

        void OnEnable()
        {
            item = GetComponent<Item>();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            Tooltip.showTooltip = true;
            Tooltip.hoveredItem = item;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            Tooltip.showTooltip = false;
        }
    }
}