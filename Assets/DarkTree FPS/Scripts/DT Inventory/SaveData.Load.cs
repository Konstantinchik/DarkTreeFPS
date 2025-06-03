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
            Debug.LogError("=====LOADING=====");

            if (weaponManager == null)
                weaponManager = FindFirstObjectByType<WeaponManager>();

            string saveDir = GetSaveDirectory();
            string sceneName = SceneManager.GetActiveScene().name;

            string playerDataPath = Path.Combine(saveDir, sceneName + "_playerData");

            if (!File.Exists(playerDataPath))
            {
                Debug.Log("No save data found");
                return;
            }

            PlayerStatsData data = JsonUtility.FromJson<PlayerStatsData>(File.ReadAllText(playerDataPath));

            var playerStats = FindFirstObjectByType<PlayerStats>();
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

            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            player.position = data.playerPosition;
            player.rotation = data.playerRotation;

            GameObject.Find("Camera Holder").transform.rotation = data.camRotation;

            // NPCs and zombies
            CharactersData charactersData = JsonUtility.FromJson<CharactersData>(
                File.ReadAllText(Path.Combine(saveDir, sceneName + "_charactersData")));

            foreach (var npc in FindObjectsOfType<NPC>())
                Destroy(npc.gameObject);

            foreach (var zombie in FindObjectsOfType<ZombieNPC>())
                Destroy(zombie.gameObject);

            for (int k = 0; k < charactersData.npcName.Length; k++)
            {
                var _npc = Instantiate(assetsDatabase.FindNPC(charactersData.npcName[k]));
                _npc.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                _npc.transform.position = charactersData.npcPos[k] + Vector3.up;
                _npc.transform.rotation = charactersData.npcRot[k];
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

            // Удаляем старые предметы и очищаем лутбоксы
            foreach (var item in FindObjectsOfType<Item>())
                Destroy(item.gameObject);

            foreach (var invItem in FindObjectsOfType<InventoryItem>())
                Destroy(invItem.gameObject);

            foreach (var lootbox in FindObjectsOfType<LootBox>())
                lootbox.lootBoxItems.Clear();

            // Инвентарь
            DTInventory inventory = Object.FindFirstObjectByType<DTInventory>();
            InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(
                File.ReadAllText(Path.Combine(saveDir, sceneName + "_inventoryData")));

            bool wasAutoEquip = inventory.autoEquipItems;
            inventory.autoEquipItems = false;

            if (inventoryData.itemNames != null)
            {
                for (int i = 0; i < inventoryData.itemNames.Length; i++)
                {
                    var prefab = assetsDatabase.FindItem(inventoryData.itemNames[i]);
                    if (prefab != null)
                    {
                        var item = Instantiate(prefab);
                        item.stackSize = inventoryData.stackSize[i];

                        inventory.AddItem(item, (int)inventoryData.itemGridPos[i].x, (int)inventoryData.itemGridPos[i].y);

                        var slot = inventory.FindSlotByIndex((int)inventoryData.itemGridPos[i].x, (int)inventoryData.itemGridPos[i].y);
                        if (slot.equipmentPanel != null)
                        {
                            slot.equipmentPanel.equipedItem = item;
                        }
                    }
                    else
                    {
                        Debug.LogAssertion("Missing item in database: " + inventoryData.itemNames[i]);
                    }
                }
            }

            if (inventoryData.activeWeaponIndex != -1)
                weaponManager.ActivateByIndexOnLoad(inventoryData.activeWeaponIndex);

            inventory.autoEquipItems = wasAutoEquip;

            // Предметы на сцене
            LevelData itemsLevelData = JsonUtility.FromJson<LevelData>(
                File.ReadAllText(Path.Combine(saveDir, sceneName + "_itemsLevelData")));

            for (int i = 0; i < itemsLevelData.itemName.Length; i++)
            {
                if (!string.IsNullOrEmpty(itemsLevelData.itemName[i]))
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
                        Debug.LogAssertion("Ошибка загрузки предмета: " + itemsLevelData.itemName[i]);
                    }
                }
            }

            // Лутбоксы
            LootBoxData lootBoxData = JsonUtility.FromJson<LootBoxData>(
                File.ReadAllText(Path.Combine(saveDir, sceneName + "_lootboxData")));

            var sceneLootBoxes = FindObjectsOfType<LootBox>();
            var boxNames = lootBoxData.lootBoxSceneNames.Split('|', System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < boxNames.Length; i++)
            {
                foreach (var lootBox in sceneLootBoxes)
                {
                    if (lootBox.name == boxNames[i])
                    {
                        var items = lootBoxData.itemNames[i].Split('|', System.StringSplitOptions.RemoveEmptyEntries);
                        var counts = lootBoxData.stackSize[i].Split('|', System.StringSplitOptions.RemoveEmptyEntries);

                        for (int j = 0; j < items.Length; j++)
                        {
                            var item = Instantiate(assetsDatabase.FindItem(items[j]));
                            if (int.TryParse(counts[j], out int count))
                            {
                                item.stackSize = count;
                            }
                            lootBox.lootBoxItems.Add(item);
                        }
                    }
                }
            }
        }
    }
}
