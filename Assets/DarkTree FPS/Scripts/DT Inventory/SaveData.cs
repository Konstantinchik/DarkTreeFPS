using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using DarkTreeFPS;

namespace DTInventory
{
    public class SaveData : MonoBehaviour
    {
        public AssetsDatabase assetsDatabase;

        public KeyCode saveKeyCode = KeyCode.F5;
        public KeyCode loadKeyCode = KeyCode.F9;

        public static bool loadDataTrigger = false;

        public static GameObject instance;
        public static SaveData saveInstance;

        private WeaponManager weaponManager;

        public GameObject gamePrefab;

        private void Start()
        {
            if (saveInstance == null)
            {
                saveInstance = this;
            }
            else
                Destroy(this.gameObject);
        }

        private void Update()
        {
            if (loadDataTrigger)
            {
                print("Attemp to load player save");
                Load();

                CoverList.sceneActiveNPC = new List<NPC>();
                CoverList.sceneActiveNPC.Clear();

                loadDataTrigger = false;
            }

            if (Input.GetKeyDown(saveKeyCode))
            {
                Save();
            }

            if (Input.GetKeyDown(loadKeyCode))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                loadDataTrigger = true;

                if (PlayerStats.isPlayerDead)
                {
                    Destroy(GameObject.Find("Camera Holder"));  
                    Destroy(instance.gameObject);
                    instance = Instantiate(gamePrefab); 
                }
            }
        }

        /// <summary>
        /// Method to save level state for transition persistance
        /// </summary>
        public void SaveLevelPeristence()
        {
            var allSceneItems = FindObjectsOfType<Item>();

            //Save scene items

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
            File.WriteAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceItems", _itemsLevelData);

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
            File.WriteAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceLoot", _lootBoxData);
        }

        // [System.Obsolete]
        public void Save()
        {
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

        public void LoadLevelPersistence()
        {
            if (instance == null || loadDataTrigger)
                return;
         
            print("Loading persistence data");

            if (File.Exists(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceItems"))
            {
                Item[] existingItems = FindObjectsOfType<Item>();

                if (existingItems != null)
                {
                    foreach (Item item in existingItems)
                    {
                        Destroy(item.gameObject);
                    }
                }

                LevelData itemsLevelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceItems"));

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
            }

            if (File.Exists(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceLoot"))
            {
                var sceneLootBoxes = FindObjectsOfType<LootBox>();

                if (sceneLootBoxes != null)
                {
                    foreach (var lootbox in sceneLootBoxes)
                    {
                        lootbox.lootBoxItems = null;
                    }
                }

                for (int i = 0; i < sceneLootBoxes.Length; i++)
                {
                    LootBoxData lootBoxData = JsonUtility.FromJson<LootBoxData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceLoot"));

                    var lootbox = sceneLootBoxes[i];

                    char[] separator = new char[] { '|' };

                    string[] itemsTitles = lootBoxData.itemNames[i].Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

                    //foreach (string t in itemsTitles)
                    //    print(t);

                    string[] itemStackSizes = lootBoxData.stackSize[i].Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

                    //foreach (string jk in itemStackSizes)
                    //    print(jk);

                    List<int> itemStackSizesInt = new List<int>();

                    foreach (string itemStackSizeString in itemStackSizes)
                    {
                        int resultInt = -1;

                        int.TryParse(itemStackSizeString, out resultInt);

                        itemStackSizesInt.Add(resultInt);
                    }

                    print(itemsTitles.Length);

                    lootbox.lootBoxItems = new List<Item>();

                    for (int j = 0; j < itemsTitles.Length; j++)
                    {
                        if (assetsDatabase.FindItem(itemsTitles[j]) != null)
                        {
                            var item = Instantiate(assetsDatabase.FindItem(itemsTitles[j]));

                            //print("Cycle pass - " + j + ". Spawn item " + item.title);

                            item.gameObject.SetActive(false);

                            if (itemStackSizesInt[j] > -1)
                                item.stackSize = itemStackSizesInt[j];

                            lootbox.lootBoxItems.Add(item);
                        }
                    }
                }
            }
        }

        public void ClearScenePersistence()
        {
            string sceneName = string.Empty;
            
            string[] sceneNamesInBuild = new string[SceneManager.sceneCountInBuildSettings];

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string pathToScene = SceneUtility.GetScenePathByBuildIndex(i);
                string _sceneName = Path.GetFileNameWithoutExtension(pathToScene);

                sceneNamesInBuild[i] = _sceneName;

            }

            foreach (var _sceneName in sceneNamesInBuild)
            {
                sceneName = _sceneName;

                string itemsLevelData = Application.persistentDataPath + "/" + sceneName + "_persistenceItems";
                string lootBoxData = Application.persistentDataPath + "/" + sceneName + "_persistenceLoot";

                try
                {
                    File.Delete(itemsLevelData);
                }
                catch
                {
                    Debug.Log("Attemp to clear persistence for scene " + sceneName + " is failed. Probably, scene persistent data not exist");
                }
                try
                {
                    File.Delete(lootBoxData);
                }
                catch
                {
                    Debug.Log("Attemp to clear persistence for scene " + sceneName + " is failed. Probably, scene persistent data not exist");
                }
            }

            print("Persistence for all levels in build was removed");
        }

        public void Load()
        {
            print("Load started");

            if (weaponManager == null)
                weaponManager = FindFirstObjectByType<WeaponManager>();

            //Player data

            if (JsonUtility.FromJson<PlayerStatsData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_playerData")) == null)
            {
                print("No save data found data found");
                return;
            }

            PlayerStatsData data = JsonUtility.FromJson<PlayerStatsData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_playerData"));

            //AudioListener.volume = 0;

            //Player stats
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

            Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            player.position = data.playerPosition;
            player.rotation = data.playerRotation;

            Transform cameraHolder = GameObject.Find("Camera Holder").GetComponent<Transform>();
            cameraHolder.rotation = data.camRotation;

            //NPC and Zombies
            
            CharactersData charactersData = JsonUtility.FromJson<CharactersData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_charactersData"));
            
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
                _npc.transform.position = charactersData.npcPos[k]+Vector3.up;
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

            InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_inventoryData"));

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
            
            LevelData itemsLevelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_itemsLevelData"));

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

            LootBoxData lootBoxData = JsonUtility.FromJson<LootBoxData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_lootboxData"));
            
            string lootBoxSceneNames = lootBoxData.lootBoxSceneNames;

            char[] separator = new char[] { '|' };

            string[] lootBoxSceneNamesArray = lootBoxData.lootBoxSceneNames.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
            

            for(int i = 0; i < lootBoxSceneNamesArray.Length; i++)
            {
                foreach(LootBox lootBox in sceneLootBoxes)
                {
                    if(lootBox.name == lootBoxSceneNamesArray[i])
                    {
                        string[] itemsTitles = lootBoxData.itemNames[i].Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        string[] itemStackSizes = lootBoxData.stackSize[i].Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                    
                        List<int> itemStackSizesInt = new List<int>();

                        foreach(string s in itemStackSizes)
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

    public class InventoryData
    {
        public string[] itemNames;
        public int[] stackSize;
        public Vector2[] itemGridPos;
        public int activeWeaponIndex;

        public InventoryData(string[] itemNames, int[] stackSize, Vector2[] itemGridPos, int activeWeaponIndex)
        {
            this.itemNames = itemNames;
            this.stackSize = stackSize;
            this.itemGridPos = itemGridPos;
            this.activeWeaponIndex = activeWeaponIndex;
        }
    }

    public class CharactersData
    {
        //NPC

        public string[] npcName;
        public Vector3[] npcPos;
        public Quaternion[] npcRot;
        public Vector3[] npcCurrentTarget;
        public Vector3[] npcLookAtTarget;

        //Zombies

        public Vector3[] zombiePos;
        public Quaternion[] zombieRot;
        public bool[] zombieIsWorried;
    }

    public class LevelData
    {
        public Vector3[] itemPos;
        public Quaternion[] itemRot;
        public string[] itemName;
        public int[] itemStackSize;
    }

    public class LootBoxData
    {
        public string lootBoxSceneNames;
        public string[] itemNames;
        public string[] stackSize;
    }

    public class PlayerStatsData
    {
        public int health;
        public bool useConsumeSystem;
        public int hydratation;
        public float hydratationSubstractionRate;
        public int thirstDamage;
        public float hydratationTimer;
        public int satiety;
        public float satietySubstractionRate;
        public int hungerDamage;
        public float satietyTimer;

        public Vector3 playerPosition;
        public Quaternion playerRotation;

        public Quaternion camRotation;

        public Vector2 targetDirection;
        public Vector2 mouseAbsolute;
        public Vector2 smoothMouse;

        public PlayerStatsData(int health, bool useConsumeSystem, int hydratation, float hydratationSubstractionRate, int thirstDamage,
                               float hydratationTimer, int satiety, float satietySubstractionRate, int hungerDamage, float satietyTimer,
                               Vector3 playerPosition, Quaternion playerRotation, Quaternion camRotation, Vector2 targetDirection, Vector2 mouseAbsolute, Vector2 smoothMouse)
        {
            this.health = health;
            this.useConsumeSystem = useConsumeSystem;
            this.hydratation = hydratation;
            this.hydratationSubstractionRate = hydratationSubstractionRate;
            this.thirstDamage = thirstDamage;
            this.hydratationTimer = hydratationTimer;
            this.satiety = satiety;
            this.satietySubstractionRate = satietySubstractionRate;
            this.hungerDamage = hungerDamage;
            this.satietyTimer = satietyTimer;
            this.playerPosition = playerPosition;
            this.playerRotation = playerRotation;
            this.camRotation = camRotation;
            this.targetDirection = targetDirection;
            this.mouseAbsolute = mouseAbsolute;
            this.smoothMouse = smoothMouse;
        }
    }

}