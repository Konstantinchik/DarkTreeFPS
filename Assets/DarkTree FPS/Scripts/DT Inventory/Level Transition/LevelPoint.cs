﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTInventory
{
    public class LevelPoint : MonoBehaviour
    {
        public Transform myTransform;
        public bool loadPersistentItemsOnSceneStart = false;
        public bool movePlayerHereOnSceneEnter = true;

        private void Start()
        {
            myTransform = transform;

            if(movePlayerHereOnSceneEnter)
            GameObject.FindGameObjectWithTag("Player").transform.position = myTransform.position;

            FindFirstObjectByType<DTInventory>().levelPoint = transform;

            if (loadPersistentItemsOnSceneStart && !SaveData.loadDataTrigger && SaveData.instance != null)
            {
                print("Loading scene persistence");
                    FindFirstObjectByType<SaveData>().LoadLevelPersistence();
            }
            else
            {
                print("Scene persistence wasn't loaded due to condition mismatch");
            }
            
        }
    }
}