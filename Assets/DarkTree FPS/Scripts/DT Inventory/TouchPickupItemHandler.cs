using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DTInventory
{
    public class TouchPickupInventoryItem : MonoBehaviour, IPointerClickHandler
    {
        PickupItem pickupItem;
        DTInventory inventory;

        private void OnEnable()
        {
            pickupItem = FindFirstObjectByType<PickupItem>();
            inventory = FindFirstObjectByType<DTInventory>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (pickupItem.interactionType != InteractionType.clickToPickup)
                return;

            if (eventData.hovered[0].gameObject.GetComponent<Item>() != null)
            {
                inventory.AddItem(eventData.hovered[0].gameObject.GetComponent<Item>());
            }
        }
    }
}