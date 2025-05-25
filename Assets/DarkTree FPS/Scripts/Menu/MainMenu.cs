/// DarkTreeDevelopment (2019) DarkTree FPS v1.21
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    int demoLevelId = 1;

    public void LoadDemoLevel()
    {
        SceneManager.LoadScene(demoLevelId);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
