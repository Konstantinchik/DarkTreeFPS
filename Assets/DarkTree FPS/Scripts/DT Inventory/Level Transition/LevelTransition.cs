using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DTInventory
{
    public class LevelTransition : MonoBehaviour
    {
        public int sceneId;

        private void OnTriggerEnter(Collider other)
        {
            FindFirstObjectByType<SaveData>().SaveLevelPersistence();
            SceneManager.LoadScene(sceneId);
        }
    }
}