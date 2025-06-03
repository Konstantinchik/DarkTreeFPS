//using DTInventory;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DTInventory
{
    public partial class SaveData : MonoBehaviour
{
    /// <summary>
    /// Method to save level state for transition persistance
    /// </summary>
    public void SaveLevelPersistence()
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
        File.WriteAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceItems", _itemsLevelData);

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
        File.WriteAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceLoot", _lootBoxData);
    }



    public void LoadLevelPersistence()
    {
        if (instance == null || loadDataTrigger)
            return;

        print("Loading persistence data");

        if (File.Exists(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceItems"))
        {
            Item[] existingItems = FindObjectsOfType<Item>();

            if (existingItems != null)
            {
                foreach (Item item in existingItems)
                {
                    Destroy(item.gameObject);
                }
            }

            LevelData itemsLevelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceItems"));

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

        if (File.Exists(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceLoot"))
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
                LootBoxData lootBoxData = JsonUtility.FromJson<LootBoxData>(File.ReadAllText(Application.dataPath + "/" + SceneManager.GetActiveScene().name + "_persistenceLoot"));

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

            string itemsLevelData = Application.dataPath + "/" + sceneName + "_persistenceItems";
            string lootBoxData = Application.dataPath + "/" + sceneName + "_persistenceLoot";

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
}
}