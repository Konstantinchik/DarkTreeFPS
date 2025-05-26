using UnityEngine;
using UnityEngine.UI;

public class AnimatedCursor : MonoBehaviour
{
    [SerializeField]
    private Sprite[] cursorFrames;
    public float frameRate = 10f;

    private Image cursorImage;
    private int currentFrame;
    private float timer;

    private Vector2 offset = new Vector2(10, -10); // Смещение вправо и вниз

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined; // Или Locked, если нужен захват мыши
    }

    void Start()
    {
        Cursor.visible = false;
        cursorImage = GetComponent<Image>();
        if (cursorFrames.Length > 0)
            cursorImage.sprite = cursorFrames[0];
    }

    void Update()
    {

        transform.position = Input.mousePosition + (Vector3)offset; 

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {

            currentFrame = (currentFrame + 1) % cursorFrames.Length;
            if (currentFrame >= 7) currentFrame = 0;
            cursorImage.sprite = cursorFrames[currentFrame];
            timer = 0f;
        }
    }
}
