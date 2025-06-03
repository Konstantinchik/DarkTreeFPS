using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DTInventory
{
    public partial class SaveData : MonoBehaviour
    {
        private static string GetSaveDirectory()
        {
            Debug.Log("Вызван GetSaveDirectory()");

            string savePath;

#if UNITY_EDITOR
            // В редакторе: на уровне с Assets
            savePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "SaveGames");
#else
    // В билде: на уровне с .exe
    savePath = Path.Combine(Application.dataPath, "..", "SaveGames");
    savePath = Path.GetFullPath(savePath); // нормализуем
#endif

            try
            {
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                    Debug.Log("Создана папка для сохранений: " + savePath);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Ошибка при создании папки сохранений: " + ex.Message);
            }

            return savePath;
        }

        public void SaveLevelPersistence()
        {
            string saveDir = GetSaveDirectory();

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

            string _itemsLevelData = JsonUtility.ToJson(itemsLevelData);
            File.WriteAllText(Path.Combine(saveDir, SceneManager.GetActiveScene().name + "_persistenceItems"), _itemsLevelData);

            var allSceneLootboxes = FindObjectsOfType<LootBox>();
            List<string> loot_ItemNames = new List<string>();
            List<string> loot_ItemsCount = new List<string>();
            string lootBoxSceneNames = string.Empty;

            foreach (LootBox lootBox in allSceneLootboxes)
            {
                string itemsString = string.Empty;
                string itemsStacksize = string.Empty;
                lootBoxSceneNames += lootBox.name + "|";

                foreach (Item item in lootBox.lootBoxItems)
                {
                    itemsString += item.title + "|";
                    itemsStacksize += item.stackSize.ToString() + "|";
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

            

            string jsonLoot = JsonUtility.ToJson(lootBoxData);
            File.WriteAllText(Path.Combine(saveDir, SceneManager.GetActiveScene().name + "_persistenceLoot"), jsonLoot);
        }

        public void LoadLevelPersistence()
        {
            if (instance == null || loadDataTrigger)
                return;

            Debug.Log("Loading persistence data");
            string saveDir = GetSaveDirectory();

            string itemPath = Path.Combine(saveDir, SceneManager.GetActiveScene().name + "_persistenceItems");
            if (File.Exists(itemPath))
            {
                foreach (Item item in FindObjectsOfType<Item>())
                {
                    Destroy(item.gameObject);
                }

                LevelData itemsLevelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(itemPath));

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
                            Debug.LogAssertion("Item from save not found: " + itemsLevelData.itemName[i]);
                        }
                    }
                }
            }

            string lootPath = Path.Combine(saveDir, SceneManager.GetActiveScene().name + "_persistenceLoot");
            if (File.Exists(lootPath))
            {
                var sceneLootBoxes = FindObjectsOfType<LootBox>();

                foreach (var lootbox in sceneLootBoxes)
                {
                    lootbox.lootBoxItems = null;
                }

                LootBoxData lootBoxData = JsonUtility.FromJson<LootBoxData>(File.ReadAllText(lootPath));

                for (int i = 0; i < sceneLootBoxes.Length; i++)
                {
                    var lootbox = sceneLootBoxes[i];

                    char[] separator = new char[] { '|' };
                    string[] itemsTitles = lootBoxData.itemNames[i].Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                    string[] itemStackSizes = lootBoxData.stackSize[i].Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

                    List<int> itemStackSizesInt = new List<int>();
                    foreach (string sizeStr in itemStackSizes)
                    {
                        int.TryParse(sizeStr, out int result);
                        itemStackSizesInt.Add(result);
                    }

                    lootbox.lootBoxItems = new List<Item>();
                    for (int j = 0; j < itemsTitles.Length; j++)
                    {
                        var asset = assetsDatabase.FindItem(itemsTitles[j]);
                        if (asset != null)
                        {
                            var item = Instantiate(asset);
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
            string saveDir = GetSaveDirectory();

            string[] sceneNames = new string[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                sceneNames[i] = Path.GetFileNameWithoutExtension(path);
            }

            foreach (string scene in sceneNames)
            {
                string itemsPath = Path.Combine(saveDir, scene + "_persistenceItems");
                string lootPath = Path.Combine(saveDir, scene + "_persistenceLoot");

                try
                {
                    if (File.Exists(itemsPath))
                        File.Delete(itemsPath);
                }
                catch
                {
                    Debug.Log($"Failed to delete item data for scene {scene}");
                }

                try
                {
                    if (File.Exists(lootPath))
                        File.Delete(lootPath);
                }
                catch
                {
                    Debug.Log($"Failed to delete loot data for scene {scene}");
                }
            }

            Debug.Log("Persistence for all levels was removed");
        }
    }
}
