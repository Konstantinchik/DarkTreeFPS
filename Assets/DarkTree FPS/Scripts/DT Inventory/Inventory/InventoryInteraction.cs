/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;
using UnityEngine.UI;

namespace DTInventory
{
    public class InventoryInteraction : MonoBehaviour
    {
        [HideInInspector]
        public InventoryItem InventoryItem;
        private DTInventory inventory;
        
        private void Start()
        {
            inventory = FindFirstObjectByType<DTInventory>();
        }

        public void RemoveItem()
        {
            inventory.DropItem(InventoryItem);
            this.gameObject.SetActive(false);
        }

        public void Useitem()
        {
            inventory.UseItem(InventoryItem, false);
            gameObject.SetActive(false);
        }

        public void UnstackItem()
        {
            inventory.SubstractStack(InventoryItem);
            gameObject.SetActive(false);
        }
    }
}
