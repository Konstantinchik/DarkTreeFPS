using System.Collections.Generic;
using UnityEngine;

namespace DTInventory
{
    public static class DTFPSInventoryExtended 
    {
        public static List<InventoryItem> ammoItems;
        
        public static List<InventoryItem> SearchItemWithCount(DTInventory inventory, GameObject itemToFind, int itemsValue)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            foreach (var i in inventory.inventoryItems)
            {
                if (i.item.id == itemToFind.GetComponent<Item>().id)
                    items.Add(i);

                if (items.Count == itemsValue)
                    return items;
            }

            return null;
        }

        public static List<InventoryItem> SearchItemsForBuilding(DTInventory inventory, GameObject[] itemsToFind, int[] itemsValue)
        {
            int status = 0;

            for (int i = 0; i < itemsValue.Length; i++)
            {
                if (SearchItemWithCount(inventory, itemsToFind[i], itemsValue[i]) != null)
                    status++;
            }
            
            if (status == itemsToFind.Length)
            {
                List<InventoryItem> items = new List<InventoryItem>();

                for (int i = 0; i < itemsValue.Length; i++)
                {
                    items.AddRange(SearchItemWithCount(inventory, itemsToFind[i], itemsValue[i]));
                }

                return items;
            }
            else
                return null;
        }

        public static void UseGrenade(DTInventory inventory)
        {
            InventoryItem UIItem = null;

            foreach (var item in inventory.inventoryItems)
            {
                if (item.item.title == "Grenade")
                {
                    UIItem = item;
                    break;
                }
            }

            if (UIItem == null)
                return;

            // If not stackable
            if (!UIItem.item.stackable || UIItem.item.stackSize <= 1)
            {
                UIItem.item.onUseEvent.Invoke();
                inventory.RemoveItem(UIItem);
            }
            // If stackable
            else
            {
                UIItem.item.onUseEvent.Invoke();
                UIItem.item.stackSize -= 1;
            }
        }
    }
}

