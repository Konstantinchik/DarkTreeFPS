using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTInventory;

namespace DarkTreeFPS
{
    public class ObjectPlacement : MonoBehaviour
    {
        private ObjectPlaceUtility utility;
        [HideInInspector]
        public GameObject objectToPlace;

        public float maxPlacementDistance = 5f;

        private float rotationAmount;
        public float rotationAddAngle = 10f;

        [HideInInspector]
        public List<InventoryItem> itemsToRemove;

        public Material std_mat;
        public Material canBuild;
        public Material cantBuild;

        private DTInventory.DTInventory inventory;
        private WeaponManager weaponManager;

        private void Start()
        {
            inventory = FindFirstObjectByType<DTInventory.DTInventory>();
            weaponManager = FindFirstObjectByType<WeaponManager>();
        }

        void Update()
        {
            if (objectToPlace != null)
            {
                weaponManager.HideAll();

                if(std_mat == null)
                    std_mat = objectToPlace.GetComponentInChildren<MeshRenderer>().material;

                if (utility == null)
                    utility = objectToPlace.GetComponent<ObjectPlaceUtility>();

                if (utility.canPlaceObject)
                {
                    foreach (var obj in objectToPlace.GetComponentsInChildren<Transform>())
                    {
                        if (obj.gameObject.GetComponent<MeshRenderer>())
                            obj.gameObject.GetComponent<MeshRenderer>().material = canBuild;
                    }
                }
                else
                {
                    foreach (var obj in objectToPlace.GetComponentsInChildren<Transform>())
                    {
                        if (obj.gameObject.GetComponent<MeshRenderer>())
                            obj.gameObject.GetComponent<MeshRenderer>().material = cantBuild;
                    }
                }

                RaycastHit hit;

                if(Input.GetKeyDown(KeyCode.Mouse1))
                {
                    objectToPlace.gameObject.GetComponentInChildren<MeshRenderer>().material = std_mat;
                    
                    utility = null;
                    std_mat = null;

                    Destroy(objectToPlace);

                    objectToPlace = null;

                    return;
                }

                if (Physics.Raycast(transform.position, transform.forward, out hit, 5f))
                {
                    if (utility.canPlaceObject)
                        if (Input.GetMouseButton(0))
                        {
                            objectToPlace.gameObject.GetComponentInChildren<MeshRenderer>().material = std_mat;

                            objectToPlace.transform.position = hit.point;

                            utility = null;
                            objectToPlace = null;
                            std_mat = null;

                            
                            foreach (var item in itemsToRemove)
                            {
                                inventory.RemoveItem(item);
                            }

                            return;
                        }

                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    rotationAmount += rotationAddAngle;
                }

                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    rotationAmount -= rotationAddAngle;
                }
                
                objectToPlace.transform.position = hit.point + new Vector3(0, 0.1f, 0);

                var modRotation = new Quaternion(Quaternion.FromToRotation(objectToPlace.transform.up, hit.normal).x,
                                                    0,
                                                    Quaternion.FromToRotation(objectToPlace.transform.up, hit.normal).z,
                                                    Quaternion.FromToRotation(objectToPlace.transform.up, hit.normal).w) * Quaternion.Euler(0, rotationAmount, 0);

                objectToPlace.transform.rotation = Quaternion.Lerp(objectToPlace.transform.rotation, modRotation, 5 * Time.deltaTime);
            }
        }
    }
}
