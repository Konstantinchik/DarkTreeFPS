/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkTreeFPS
{
    public class MobileInputRig : MonoBehaviour
    {
        WeaponManager manager;
        
        public bool setFire = false;

        private void Start()
        {
            manager = FindFirstObjectByType<WeaponManager>();
        }

        private void Update()
        {
            if (setFire)
                Fire();
        }
        
        public void SetFire(bool state)
        {
            setFire = state;
        }

        public void Fire()
        {
                manager.activeWeapon.Fire();
        }

        public void Aim()
        {
            if (manager.activeWeapon != null)
            {
                manager.activeWeapon.AimMobile();
            }
        }

        public void Reload()
        {
            if (manager.activeWeapon != null)
            {
                manager.activeWeapon.ReloadBegin();
            }
        }
    }
}