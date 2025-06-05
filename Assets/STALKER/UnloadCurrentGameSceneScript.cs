using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnloadCurrentGameSceneScript : MonoBehaviour
{
    int currentGameScene;
    private IEnumerator UnloadCurrentGameScene()
    {
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentGameScene);
        while (!unloadOp.isDone)
        {
            yield return null;
        }

        currentGameScene = 0;
    }
}
