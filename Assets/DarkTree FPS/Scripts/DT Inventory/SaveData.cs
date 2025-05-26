using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using DarkTreeFPS;

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
    }
}