using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DarkTreeFPS;

namespace DTInventory {

    [System.Serializable]
    public class OnItemEquip : UnityEvent { }
    [System.Serializable]
    public class OnItemRemove : UnityEvent { }

    public class EquipmentPanel : MonoBehaviour
    {
        /// <summary>
        /// An item that panel stores
        /// </summary>
        public Item equipedItem;
        
        /// <summary>
        /// We call update only if item in not the same as before. Made for optimization
        /// </summary>
        [HideInInspector]
        public Item lastItem;

        [HideInInspector]
        public GridSlot mainSlot;
        
        [HideInInspector]
        public int width, height;

        [SerializeField]
        public OnItemEquip OnItemEquip = new OnItemEquip();
        [SerializeField]
        public OnItemRemove OnItemRemove = new OnItemRemove();

        /// <summary>
        /// Item type allowed for this slot
        /// </summary>
        public string allowedItemType;

        [Header("Using ids ignore allowedItemType. Only specified id items will be equiped")]
        public int[] allowedIds;

        /// <summary>
        /// Activate key - DT FPS key to turn on/off weapons
        /// </summary>
        public KeyCode activateKey;
        
        public void Local_EquipmentPanelDisableRemovedWeapon()
        {
            if(lastItem != null)
            FindFirstObjectByType<WeaponManager>().EquipmentPanelDisableRemovedWeapon(lastItem);
        }

        private void Update()
        {
            if(equipedItem != null && lastItem == null)
            {
                lastItem = equipedItem;
                OnItemEquip.Invoke();
            }

            if(equipedItem == null && lastItem != null)
            {
                Local_EquipmentPanelDisableRemovedWeapon();
                OnItemRemove.Invoke();
                lastItem = null;
            }
        }
    }
}