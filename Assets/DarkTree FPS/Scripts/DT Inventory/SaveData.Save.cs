using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using DarkTreeFPS;

// Код подготавливается для расширения (например, при будущей фильтрации по сценам).
namespace DTInventory
{
    public partial class SaveData : MonoBehaviour
    {
        public void Save()
        {
            Debug.LogError("---------SAVE---------");

            string saveDir = GetSaveDirectory();
            string sceneName = SceneManager.GetActiveScene().name;

            // Player data
            var stat = FindFirstObjectByType<PlayerStats>();
            var cameraRot = Camera.main.transform.rotation;
            var controller = FindFirstObjectByType<FPSController>();

            if (weaponManager == null)
                weaponManager = FindFirstObjectByType<WeaponManager>();

            PlayerStatsData p_data = new PlayerStatsData(
                stat.health, stat.useConsumeSystem, stat.hydration, stat.hydrationSubstractionRate,
                stat.thirstDamage, stat.hydrationTimer, stat.satiety, stat.satietySubstractionRate,
                stat.hungerDamage, stat.satietyTimer, stat.playerPosition, stat.playerRotation,
                cameraRot, controller.targetDirection, controller._mouseAbsolute, controller._smoothMouse
            );

            File.WriteAllText(Path.Combine(saveDir, sceneName + "_playerData"), JsonUtility.ToJson(p_data));

            // NPC and Zombies
            CharactersData charactersData = new CharactersData();
            var npcList = FindObjectsOfType<NPC>();

            if (npcList != null)
            {
                int npcCount = npcList.Length;
                charactersData.npcName = new string[npcCount];
                charactersData.npcPos = new Vector3[npcCount];
                charactersData.npcRot = new Quaternion[npcCount];
                charactersData.npcCurrentTarget = new Vector3[npcCount];
                charactersData.npcLookAtTarget = new Vector3[npcCount];

                for (int i = 0; i < npcCount; i++)
                {
                    charactersData.npcName[i] = npcList[i].NPCNameInDatabase;
                    charactersData.npcPos[i] = npcList[i].transform.position;
                    charactersData.npcRot[i] = npcList[i].transform.rotation;
                    charactersData.npcLookAtTarget[i] = npcList[i].lookPosition;
                }
            }

            // Zombies
            var zombies = FindObjectsOfType<ZombieNPC>();

            if (zombies != null)
            {
                int count = zombies.Length;
                charactersData.zombiePos = new Vector3[count];
                charactersData.zombieRot = new Quaternion[count];
                charactersData.zombieIsWorried = new bool[count];

                for (int z = 0; z < zombies.Length; z++)
                {
                    charactersData.zombiePos[z] = zombies[z].transform.position;
                    charactersData.zombieRot[z] = zombies[z].transform.rotation;
                    charactersData.zombieIsWorried[z] = zombies[z].isWorried;
                }
            }

            File.WriteAllText(Path.Combine(saveDir, sceneName + "_charactersData"), JsonUtility.ToJson(charactersData));

            // Inventory items
            var sceneItems = FindObjectsOfType<InventoryItem>();
            List<string> items = new List<string>();
            List<int> stacksize = new List<int>();
            List<Vector2> itemGridPos = new List<Vector2>();

            foreach (var i_item in sceneItems)
            {
                items.Add(i_item.item.title);
                stacksize.Add(i_item.item.stackSize);
                itemGridPos.Add(new Vector2(i_item.x, i_item.y));
            }

            InventoryData inventoryData = new InventoryData(
                items.ToArray(),
                stacksize.ToArray(),
                itemGridPos.ToArray(),
                weaponManager.GetActiveWeaponIndex()
            );

            File.WriteAllText(Path.Combine(saveDir, sceneName + "_inventoryData"), JsonUtility.ToJson(inventoryData));

            // Scene items
            var allSceneItems = FindObjectsOfType<Item>();
            List<Item> enabledItems = new List<Item>();

            foreach (var item in allSceneItems)
            {
                if (item.isActiveAndEnabled)
                    enabledItems.Add(item);
            }

            LevelData itemsLevelData = new LevelData
            {
                itemName = new string[enabledItems.Count],
                itemPos = new Vector3[enabledItems.Count],
                itemRot = new Quaternion[enabledItems.Count],
                itemStackSize = new int[enabledItems.Count]
            };

            for (int i = 0; i < enabledItems.Count; i++)
            {
                itemsLevelData.itemName[i] = enabledItems[i].title;
                itemsLevelData.itemPos[i] = enabledItems[i].transform.position;
                itemsLevelData.itemRot[i] = enabledItems[i].transform.rotation;
                itemsLevelData.itemStackSize[i] = enabledItems[i].stackSize;
            }

            File.WriteAllText(Path.Combine(saveDir, sceneName + "_itemsLevelData"), JsonUtility.ToJson(itemsLevelData));

            // Lootboxes
            var allSceneLootboxes = FindObjectsOfType<LootBox>();
            List<string> loot_ItemNames = new List<string>();
            List<string> loot_ItemsCount = new List<string>();
            string lootBoxSceneNames = string.Empty;

            foreach (LootBox lootBox in allSceneLootboxes)
            {
                string itemsString = "";
                string itemsStacksize = "";

                lootBoxSceneNames += lootBox.name + "|";

                foreach (Item item in lootBox.lootBoxItems)
                {
                    itemsString += item.title + "|";
                    itemsStacksize += item.stackSize + "|";
                }

                loot_ItemNames.Add(itemsString);
                loot_ItemsCount.Add(itemsStacksize);
            }

            LootBoxData lootBoxData = new LootBoxData
            {
                lootBoxSceneNames = lootBoxSceneNames,
                itemNames = loot_ItemNames.ToArray(),
                stackSize = loot_ItemsCount.ToArray()
            };

            File.WriteAllText(Path.Combine(saveDir, sceneName + "_lootboxData"), JsonUtility.ToJson(lootBoxData));
        }
    }
}
