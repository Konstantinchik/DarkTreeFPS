using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVolumeTrigger : MonoBehaviour
{
    WaterVolumePostProcessing waterPostProcessing;
    DarkTreeFPS.WeaponManager weaponManager;

    private void Start()
    {
        waterPostProcessing = GetComponent<WaterVolumePostProcessing>();
        weaponManager = FindFirstObjectByType<DarkTreeFPS.WeaponManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player enter water volume");
            DarkTreeFPS.FPSController.isSwimming = true;

            var collider = GetComponent<Collider>();
            var calc = collider.transform.position.y + collider.transform.localScale.y / 2;
            
            DarkTreeFPS.FPSController.highestTriggerEdge = calc;
            weaponManager.HideWeapon();
        }
        if(other.CompareTag("MainCamera"))
        {
            waterPostProcessing.postProcessVolume.profile = waterPostProcessing.water;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            weaponManager.UnhideWeapon();
            print("Player exit water volume");
            DarkTreeFPS.FPSController.isSwimming = false;
            DarkTreeFPS.FPSController.highestTriggerEdge = -9999;
        }
        if (other.CompareTag("MainCamera"))
        {
            waterPostProcessing.postProcessVolume.profile = waterPostProcessing.standard;
        }
    }
}
