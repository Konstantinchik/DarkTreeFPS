using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DTInventory;

namespace DarkTreeFPS
{
    public class BuildingManager : MonoBehaviour
    {
        public BuildingScriptableObjects[] buildings;

        public GameObject buttonPrefab;

        public RectTransform contentHolder;

        private DTInventory.DTInventory inventory;
        private ObjectPlacement objectPlacement;

        private void Start()
        {
            foreach (var building in buildings)
            {
                var button = Instantiate(buttonPrefab).GetComponent<Button>();
                button.onClick.AddListener(() => Build(building.buildingCostItems, building.builingCostItemsAmont, building.BuildingGameObject));
                button.GetComponentInChildren<Text>().text = building.BuildingName;
                button.GetComponent<Image>().sprite = building.BuildingIcon;
                button.gameObject.transform.SetParent(contentHolder);
            }

            inventory = FindFirstObjectByType<DTInventory.DTInventory>();
            objectPlacement = FindFirstObjectByType<ObjectPlacement>();
        }

        public void Build(GameObject[] requiredItems, int[] requiredItemsValue, GameObject buildObject)
        {
            if (DTFPSInventoryExtended.SearchItemsForBuilding(inventory, requiredItems, requiredItemsValue) != null)
            {
                var items = DTFPSInventoryExtended.SearchItemsForBuilding(inventory, requiredItems, requiredItemsValue);
                objectPlacement.itemsToRemove = items;
                objectPlacement.objectToPlace = Instantiate(buildObject);
                InventoryManager.showInventory = false;
            }
        }
    }
}
