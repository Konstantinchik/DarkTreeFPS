using UnityEngine;
using UnityEngine.UI;

public class AnimatedCursor : MonoBehaviour
{
    public string resourceFolder = "CursorFrames"; // путь в Resources/
    public float frameRate = 10f;                  // кадров в секунду

    private Texture2D[] cursorTextures;
    private int currentFrame;
    private float timer;

    void Start()
    {
        cursorTextures = Resources.LoadAll<Texture2D>(resourceFolder);
        if (cursorTextures == null || cursorTextures.Length == 0)
        {
            Debug.LogError($"Не удалось загрузить текстуры из Resources/{resourceFolder}");
            enabled = false;
            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursorTextures[0], Vector2.zero, CursorMode.Auto);

        currentFrame = 0;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame = (currentFrame + 1) % cursorTextures.Length;
            Cursor.SetCursor(cursorTextures[currentFrame], Vector2.zero, CursorMode.Auto);
        }
    }
}
