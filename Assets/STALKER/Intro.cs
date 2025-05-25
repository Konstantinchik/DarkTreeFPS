using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    AsyncOperation asyncOperation;
    [SerializeField] int sceneID;
    
    void Start()
    {
        sceneID = 1;
        StartCoroutine(LoadLevel());

    }

    void Update()
    {
        
    }

    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(3f);
        asyncOperation = SceneManager.LoadSceneAsync(sceneID);
        while (!asyncOperation.isDone)
        {
            // отображаем прогресс
            yield return 0;
        }

    }
}
