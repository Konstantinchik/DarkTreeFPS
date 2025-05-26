using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using DarkTreeFPS;

namespace DTInventory
{
    public partial class SaveData : MonoBehaviour
    {
        
        // [System.Obsolete]
        public void Save()
        {
            Debug.LogError("---------SAVE---------");

            //Player data
            var stat = FindFirstObjectByType<PlayerStats>();
            var camera_rot = Camera.main.transform.rotation;
            var controller = FindFirstObjectByType<FPSController>();

            if (weaponManager == null)
                weaponManager = FindFirstObjectByType<WeaponManager>();

            PlayerStatsData p_data = new PlayerStatsData(stat.health, stat.useConsumeSystem, stat.hydration, stat.hydrationSubstractionRate, stat.thirstDamage, stat.hydrationTimer, stat.satiety, stat.satietySubstractionRate, stat.hungerDamage, stat.satietyTimer, stat.playerPosition, stat.playerRotation, camera_rot, controller.targetDirection, controller._mouseAbsolute, controller._smoothMouse);
            string player_data = JsonUtility.ToJson(p_data);

            File.WriteAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_playerData", player_data);

            //NPC and Zombies

            CharactersData charactersData = new CharactersData();

            NPC[] npc = FindObjectsOfType<NPC>();

            if (npc != null)
            {
                charactersData.npcName = new string[npc.Length];
                charactersData.npcPos = new Vector3[npc.Length];
                charactersData.npcRot = new Quaternion[npc.Length];
                charactersData.npcCurrentTarget = new Vector3[npc.Length];
                charactersData.npcLookAtTarget = new Vector3[npc.Length];

                for (int n = 0; n < npc.Length; n++)
                {
                    charactersData.npcName[n] = npc[n].NPCNameInDatabase;
                    charactersData.npcPos[n] = npc[n].gameObject.transform.position;
                    charactersData.npcRot[n] = npc[n].gameObject.transform.rotation;

                    //if (npc[n].curretTarget != null)
                    //    itemsLevelData.npcCurrentTarget[n] = npc[n].curretTarget.position;

                    charactersData.npcLookAtTarget[n] = npc[n].lookPosition;
                }
            }

            ZombieNPC[] zombies = FindObjectsOfType<ZombieNPC>();

            if (zombies != null)
            {
                charactersData.zombiePos = new Vector3[zombies.Length];
                charactersData.zombieRot = new Quaternion[zombies.Length];
                charactersData.zombieIsWorried = new bool[zombies.Length];

                for (int z = 0; z < zombies.Length; z++)
                {
                    charactersData.zombiePos[z] = zombies[z].transform.position;
                    charactersData.zombieRot[z] = zombies[z].transform.rotation;
                    charactersData.zombieIsWorried[z] = zombies[z].GetComponent<ZombieNPC>().isWorried;
                }
            }

            string _charactersData = JsonUtility.ToJson(charactersData);
            File.WriteAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_charactersData", _charactersData);
            //Save inventory items

            var sceneItems = FindObjectsOfType<InventoryItem>();
            List<string> items = new List<string>();
            List<int> stacksize = new List<int>();
            List<Vector2> itemGridPos = new List<Vector2>();
            List<Vector2> itemRectPos = new List<Vector2>();

            foreach (var i_item in sceneItems)
            {
                items.Add(i_item.item.title);
                stacksize.Add(i_item.item.stackSize);
                itemGridPos.Add(new Vector2(i_item.x, i_item.y));
                itemRectPos.Add(i_item.GetComponent<RectTransform>().anchoredPosition);
            }

            var _i = items.ToArray();
            var _s = stacksize.ToArray();
            var _p = itemGridPos.ToArray();
            var _a = weaponManager.GetActiveWeaponIndex();

            InventoryData inventoryData = new InventoryData(_i, _s, _p, _a);
            string _inventoryData = JsonUtility.ToJson(inventoryData);
            File.WriteAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_inventoryData", _inventoryData);

            //Save scene items

            var allSceneItems = FindObjectsOfType<Item>();

            List<Item> enabledItems = new List<Item>();

            foreach (var item in allSceneItems)
            {
                if (item.isActiveAndEnabled)
                    enabledItems.Add(item);
            }

            LevelData itemsLevelData = new LevelData();

            itemsLevelData.itemName = new string[enabledItems.ToArray().Length];
            itemsLevelData.itemPos = new Vector3[enabledItems.ToArray().Length];
            itemsLevelData.itemRot = new Quaternion[enabledItems.ToArray().Length];
            itemsLevelData.itemStackSize = new int[enabledItems.ToArray().Length];

            for (int i = 0; i < enabledItems.ToArray().Length; i++)
            {
                itemsLevelData.itemName[i] = enabledItems.ToArray()[i].title;
                itemsLevelData.itemPos[i] = enabledItems.ToArray()[i].transform.position;
                itemsLevelData.itemRot[i] = enabledItems.ToArray()[i].transform.rotation;
                itemsLevelData.itemStackSize[i] = enabledItems.ToArray()[i].stackSize;
            }

            string _itemsLevelData = JsonUtility.ToJson(itemsLevelData);
            //print(_itemsLevelData);
            File.WriteAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_itemsLevelData", _itemsLevelData);

            //Save lootbox items

            var allSceneLootboxes = FindObjectsOfType<LootBox>();

            List<string> loot_ItemNames = new List<string>();
            List<string> loot_ItemsCount = new List<string>();

            string lootBoxSceneNames = string.Empty;

            foreach (LootBox lootBox in allSceneLootboxes)
            {
                string itemsString = string.Empty;
                string itemsStacksize = string.Empty;

                lootBoxSceneNames = lootBoxSceneNames + lootBox.name + "|";

                foreach (Item item in lootBox.lootBoxItems)
                {
                    itemsString = itemsString + item.title + "|";
                    itemsStacksize = itemsStacksize + item.stackSize.ToString() + "|";
                }

                loot_ItemNames.Add(itemsString);
                loot_ItemsCount.Add(itemsStacksize);
            }

            LootBoxData lootBoxData = new LootBoxData();

            lootBoxData.lootBoxSceneNames = lootBoxSceneNames;
            lootBoxData.itemNames = loot_ItemNames.ToArray();
            lootBoxData.stackSize = loot_ItemsCount.ToArray();

            string _lootBoxData = JsonUtility.ToJson(lootBoxData);
            File.WriteAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_lootboxData", _lootBoxData);

        }
    }
}