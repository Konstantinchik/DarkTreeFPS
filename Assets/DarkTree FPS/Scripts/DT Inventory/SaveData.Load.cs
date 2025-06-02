using DarkTreeFPS;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DTInventory
{
    public partial class SaveData : MonoBehaviour
    {

        public void Load()
        {
            print("Load started");

            if (weaponManager == null)
                weaponManager = FindFirstObjectByType<WeaponManager>();

            //Player data

            if (JsonUtility.FromJson<PlayerStatsData>(File.ReadAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_playerData")) == null)
            {
                print("No save data found data found");
                return;
            }

            PlayerStatsData data = JsonUtility.FromJson<PlayerStatsData>(File.ReadAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_playerData"));

            //AudioListener.volume = 0;

            //Player stats
            var playerStats = Object.FindFirstObjectByType<PlayerStats>();
            playerStats.health = data.health;
            playerStats.useConsumeSystem = data.useConsumeSystem;
            playerStats.hydration = data.hydratation;
            playerStats.hydrationSubstractionRate = data.hydratationSubstractionRate;
            playerStats.thirstDamage = data.thirstDamage;
            playerStats.hydrationTimer = data.hydratationTimer;
            playerStats.satiety = data.satiety;
            playerStats.satietySubstractionRate = data.satietySubstractionRate;
            playerStats.hungerDamage = data.hungerDamage;
            playerStats.satietyTimer = data.satietyTimer;

            var controller = Object.FindFirstObjectByType<FPSController>();

            controller.targetDirection = data.targetDirection;
            controller._mouseAbsolute = data.mouseAbsolute;
            controller._smoothMouse = data.smoothMouse;

            Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            player.position = data.playerPosition;
            player.rotation = data.playerRotation;

            Transform cameraHolder = GameObject.Find("Camera Holder").GetComponent<Transform>();
            cameraHolder.rotation = data.camRotation;

            //NPC and Zombies

            CharactersData charactersData = JsonUtility.FromJson<CharactersData>(File.ReadAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_charactersData"));

            var npcToDestroy = FindObjectsOfType<NPC>();
            var zombiesToDestroy = FindObjectsOfType<ZombieNPC>();

            foreach (var npc in npcToDestroy)
            {
                Destroy(npc.gameObject);
            }

            foreach (var zombie in zombiesToDestroy)
            {
                Destroy(zombie.gameObject);
            }

            for (int k = 0; k < charactersData.npcName.Length; k++)
            {
                var _npc = Instantiate(assetsDatabase.FindNPC(charactersData.npcName[k]));
                _npc.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                _npc.transform.position = charactersData.npcPos[k] + Vector3.up;
                _npc.transform.rotation = charactersData.npcRot[k];
                //_npc.GetComponent<NPC>().curretTarget.position = itemsLevelData.npcCurrentTarget[k];
                _npc.GetComponent<NPC>().lookPosition = charactersData.npcLookAtTarget[k];

                _npc.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            }

            for (int z = 0; z < charactersData.zombiePos.Length; z++)
            {
                var _zombie = Instantiate(assetsDatabase.ReturnZombie());
                _zombie.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                _zombie.transform.position = charactersData.zombiePos[z];
                _zombie.transform.rotation = charactersData.zombieRot[z];
                _zombie.GetComponent<ZombieNPC>().isWorried = charactersData.zombieIsWorried[z];
                _zombie.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            }

            //Items

            var itemsToDestroy = FindObjectsOfType<Item>();
            var invItemsToDestroy = FindObjectsOfType<InventoryItem>();

            var sceneLootBoxes = FindObjectsOfType<LootBox>();

            foreach (var item in itemsToDestroy)
            {
                Destroy(item.gameObject);
            }

            foreach (var invItem in invItemsToDestroy)
            {
                Destroy(invItem.gameObject);
            }

            foreach (var lootbox in sceneLootBoxes)
            {
                lootbox.lootBoxItems.Clear();
            }

            //Inventory
            DTInventory inventory = Object.FindFirstObjectByType<DTInventory>();

            InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(File.ReadAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_inventoryData"));

            var inventoryItems = inventoryData.itemNames;
            var stackSize = inventoryData.stackSize;
            var itemPos = inventoryData.itemGridPos;

            bool isAutoEquipEnabled = inventory.autoEquipItems;

            inventory.autoEquipItems = false;

            if (inventoryItems != null)
            {
                for (int i = 0; i < inventoryItems.Length; i++)
                {
                    var findItem = assetsDatabase.FindItem(inventoryItems[i]);

                    if (findItem != null)
                    {
                        var item = Instantiate(findItem);

                        item.stackSize = stackSize[i];

                        inventory.AddItem(item, (int)itemPos[i].x, (int)itemPos[i].y);

                        if (inventory.FindSlotByIndex((int)itemPos[i].x, (int)itemPos[i].y).equipmentPanel != null)
                        {
                            inventory.FindSlotByIndex((int)itemPos[i].x, (int)itemPos[i].y).equipmentPanel.equipedItem = item;
                        }
                    }
                    else
                    {
                        Debug.LogAssertion("Missing item. Check if it exists in the ItemsDatabase inspector");
                    }
                }
            }

            if (inventoryData.activeWeaponIndex != -1)
                weaponManager.ActivateByIndexOnLoad(inventoryData.activeWeaponIndex);

            inventory.autoEquipItems = isAutoEquipEnabled;

            LevelData itemsLevelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_itemsLevelData"));

            for (int i = 0; i < itemsLevelData.itemName.Length; i++)
            {
                if (itemsLevelData.itemName[i] != null)
                {
                    try
                    {
                        var item = Instantiate(assetsDatabase.FindItem(itemsLevelData.itemName[i]));
                        item.transform.position = itemsLevelData.itemPos[i];
                        item.transform.rotation = itemsLevelData.itemRot[i];
                        item.stackSize = itemsLevelData.itemStackSize[i];
                    }
                    catch
                    {
                        Debug.LogAssertion("Item you try to restore from save: " + itemsLevelData.itemName[i] + " is null or not exist in database");
                    }
                }
            }

            LootBoxData lootBoxData = JsonUtility.FromJson<LootBoxData>(File.ReadAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_lootboxData"));

            string lootBoxSceneNames = lootBoxData.lootBoxSceneNames;
            char[] separator = new char[] { '|' };
            string[] lootBoxSceneNamesArray = lootBoxData.lootBoxSceneNames.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lootBoxSceneNamesArray.Length; i++)
            {
                foreach (LootBox lootBox in sceneLootBoxes)
                {
                    if (lootBox.name == lootBoxSceneNamesArray[i])
                    {
                        string[] itemsTitles = lootBoxData.itemNames[i].Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        string[] itemStackSizes = lootBoxData.stackSize[i].Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

                        List<int> itemStackSizesInt = new List<int>();

                        foreach (string s in itemStackSizes)
                        {
                            int integer = -1;
                            int.TryParse(s, out integer);
                            itemStackSizesInt.Add(integer);
                        }

                        for (int j = 0; j < itemsTitles.Length; j++)
                        {
                            var item = Instantiate(assetsDatabase.FindItem(itemsTitles[j]));
                            item.stackSize = itemStackSizesInt[j];

                            lootBox.lootBoxItems.Add(item);
                        }

                    }
                }
            }
        }
    }
}