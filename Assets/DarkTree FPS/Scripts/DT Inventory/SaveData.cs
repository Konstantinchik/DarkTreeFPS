using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using DarkTreeFPS;

/*
Скрипт предполагает, что MainMenu_P загружается один раз до этого (например, как стартовая сцена) и не выгружается.
ReloadCurrentSceneAsync корректно выполняет перезагрузку только активной сцены.
loadDataTrigger = true; вызывается после загрузки — это инициирует Load() в следующем кадре.
*/

namespace DTInventory
{
    public partial class SaveData : MonoBehaviour
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
                StartCoroutine(ReloadCurrentSceneAsync());
            }
        }

        private IEnumerator ReloadCurrentSceneAsync()
        {
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Выгружаем текущую игровую сцену
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(activeSceneIndex);
            while (!unloadOp.isDone)
                yield return null;

            // Загружаем сцену снова в режиме Additive
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(activeSceneIndex, LoadSceneMode.Additive);
            while (!loadOp.isDone)
                yield return null;

            Scene loadedScene = SceneManager.GetSceneByBuildIndex(activeSceneIndex);
            SceneManager.SetActiveScene(loadedScene);

            loadDataTrigger = true;

            // Восстанавливаем игрока, если он мёртв
            if (PlayerStats.isPlayerDead)
            {
                GameObject cameraHolder = GameObject.Find("Camera Holder");
                if (cameraHolder != null)
                {
                    Destroy(cameraHolder);
                }

                if (instance != null)
                {
                    Destroy(instance.gameObject);
                }

                instance = Instantiate(gamePrefab);
            }
        }
    }
}