using UnityEngine;

public class DontDestroyInstance : MonoBehaviour
{
    private static DontDestroyInstance instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Уничтожаем дубликаты
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
