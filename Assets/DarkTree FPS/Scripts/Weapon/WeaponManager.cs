using DTInventory;
using System.Collections.Generic;
/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;

namespace DarkTreeFPS
{
    public class WeaponManager : MonoBehaviour
    {
        //A public list which get all aviliable weapons on Start() and operate with them
        public List<Weapon> weapons;
        
        public List<EquipmentPanel> equipmentPanel;
        
        [HideInInspector]
        public GameObject scopeImage;
        
        [HideInInspector]
        public Animator weaponHolderAnimator;

        [HideInInspector]
        public GameObject tempGameobject;

        private Transform swayTransform;

        private DTInventory.DTInventory inventory;

        public GameObject scopeUI;

        //[HideInInspector]
        public Weapon activeWeapon;

        private Weapon weaponToUnhide;

        private SoundManager soundManager;
        
        public string grenadeItemName = "Grenade";
        
        private static WeaponManager weaponManager;

        private void Awake()
        {
            //Set static refence to weapon manager
            weaponManager = this;

            swayTransform = FindFirstObjectByType<Sway>().GetComponent<Transform>();
            soundManager = FindFirstObjectByType<SoundManager>();

            foreach (Weapon weapon in swayTransform.GetComponentsInChildren<Weapon>(true))
            {
                weapons.Add(weapon);
            }

            inventory = FindFirstObjectByType<DTInventory.DTInventory>();
        }

        private void Update()
        {
            if (!PlayerStats.isPlayerDead)
                SlotInput();

            if (activeWeapon != null && activeWeapon.weaponName == "Grenade" && inventory.CheckIfItemExist("Grenade") == false)
            {
                weaponHolderAnimator.Play("Hide");
                Invoke("HideWeapon", 0.5f);
            }
            
        }

        public void EquipmentPanelDisableRemovedWeapon(Item item)
        {
            if (item == null)
            {
                return;
            }

            if (activeWeapon == null)
            {
                return;
            }

            foreach (var weapon in weapons)
            {
                if (activeWeapon != null)
                {
                    if (activeWeapon.weaponName == item.title)
                    {
                        HideWeapon();
                    }
                }
            }
        }
        
        public int GetActiveWeaponIndex()
        {
            if (activeWeapon == null)
                return -1;

            foreach(var p in equipmentPanel)
            {
                if(activeWeapon.currentItem == p.equipedItem)
                {
                    return equipmentPanel.IndexOf(p);
                }
            }

            return -1;
        }

        public void AutoEquip(Item item)
        {
            if (activeWeapon == null)
            {
                foreach (var weapon in weapons)
                {
                    if (weapon.weaponName == item.title)
                    {
                        soundManager.WeaponPicking(false);
                        weapon.currentItem = item;
                        weapon.gameObject.SetActive(true);
                        activeWeapon = weapon;
                        activeWeapon.currentItem = item;
                        weaponHolderAnimator.Play("Unhide");
                    }
                }
            }
        }

        public void MobileSlotInput(int equipmentPanelIndexInPanels)
        {
            if (equipmentPanel != null && !ChangingWeapon)
            {
                if (activeWeapon != null && activeWeapon.name == equipmentPanel[equipmentPanelIndexInPanels].equipedItem.title)
                {
                    return;
                }

                ChangingWeapon = true;
                weaponHolderAnimator.Play("Hide");
                Invoke("HideWeapon", 0.5f);

                foreach (var weapon in weapons)
                {
                    if (weapon.weaponName == equipmentPanel[equipmentPanelIndexInPanels].equipedItem.title)
                    {
                        weaponToUnhide = weapon;
                        weaponToUnhide.currentItem = equipmentPanel[equipmentPanelIndexInPanels].equipedItem;
                    }
                }

                Invoke("UnhideWeapon", 0.6f);
            }
        }

        public void ActivateByIndexOnLoad(int equipmentPanelIndexInPanels)
        {
            if (equipmentPanel[equipmentPanelIndexInPanels] != null)
            {
                foreach (var weapon in weapons)
                {
                    if (weapon.weaponName == equipmentPanel[equipmentPanelIndexInPanels].equipedItem.title)
                    {
                        weaponToUnhide = weapon;
                        weaponToUnhide.currentItem = equipmentPanel[equipmentPanelIndexInPanels].equipedItem;
                        print("weapon to unhide on load :" + weapon.weaponName);
                    }
                }

                UnhideWeapon();
            }
        }

        public void SlotInput()
        {
            if (Input.GetKeyDown(KeyCode.G) && inventory.CheckIfItemExist("Grenade"))
            {
                ChangingWeapon = true;
                weaponHolderAnimator.Play("Hide");
                //Invoke("HideWeapon", 0.5f);
                HideWeapon();

                foreach (var weapon in weapons)
                {
                    if (weapon.weaponName == grenadeItemName)
                    {
                        weaponToUnhide = weapon;
                    }
                }

                //Invoke("UnhideWeapon", 1f);
                UnhideWeapon();
            }

            if (equipmentPanel != null && !ChangingWeapon)
            {
                for (int i = 0; i < equipmentPanel.Count; i++)
                {
                    if (Input.GetKeyDown(equipmentPanel[i].activateKey) && equipmentPanel[i].equipedItem != null)
                    {
                        if (activeWeapon != null && activeWeapon.currentItem == equipmentPanel[i].equipedItem)
                        {
                            return;
                        }
                        
                        //CancelInvoke("UnhideDelay");

                        ChangingWeapon = true;
                        weaponHolderAnimator.Play("Hide");
                        //Invoke("HideWeapon", 0.5f);
                        HideWeapon();

                        foreach (var weapon in weapons)
                        {
                            if (weapon.weaponName == equipmentPanel[i].equipedItem.title)
                            {
                                weaponToUnhide = weapon;
                                weaponToUnhide.currentItem = equipmentPanel[i].equipedItem;
                            }
                        }

                        //Invoke("UnhideWeapon", 0.6f);
                        UnhideWeapon();
                    }
                }
            }
        }
        

        public Weapon GetWeapon(Item item)
        {
            foreach (Weapon weapon in weapons)
            {
                if (weapon.weaponName == item.title)
                {
                    return weapon;
                }
            }
            return null;
        }

        public Weapon GetWeapon(string name)
        {
            foreach (Weapon weapon in weapons)
            {
                if (weapon.weaponName == name)
                {
                    return weapon;
                }
            }
            return null;
        }

        public void HideWeaponOnDeath()
        {
            weaponHolderAnimator.SetLayerWeight(1, 0);
            weaponHolderAnimator.Play("Hide");
        }

        public void UnhideWeaponOnRespawn()
        {
            weaponHolderAnimator.SetLayerWeight(1, 1);
            weaponHolderAnimator.Play("Hide");
        }

        private bool ChangingWeapon;

        public void UneqipWeapon(Item item)
        {
            foreach (var weapon in weapons)
            {
                if (item == null)
                    print("Unequip weapon: items you try to unequip is null");

                if (item.title == weapon.weaponName && weapon.gameObject.activeInHierarchy)
                {
                    weaponHolderAnimator.Play("Hide");
                    Invoke("HideWeapon", 0.5f);
                }
            }
        }

        public void SetChangingWeaponFalse()
        {
            ChangingWeapon = false;
        }

        public void HideWeapon()
        {
            if (activeWeapon != null)
            {
                activeWeapon.gameObject.SetActive(false);
                activeWeapon = null;
                soundManager.WeaponPicking(true);
            }
        }

        public void UnhideWeapon()
        {
            if (weaponToUnhide != null)
            {
                activeWeapon = weaponToUnhide;
                weaponToUnhide.gameObject.SetActive(true);
            }

            soundManager.WeaponPicking(false);
            weaponHolderAnimator.Play("Unhide");
            weaponToUnhide = null;
            Invoke("SetChangingWeaponFalse", 0.5f);
        }
        
        public void HideAll()
        {
            print("Hide weapon works");

            if (activeWeapon != null)
            {
                activeWeapon.gameObject.SetActive(false);

                weaponHolderAnimator.Play("Hide");
            }
        }
    }
}
