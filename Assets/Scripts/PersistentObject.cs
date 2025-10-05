using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    private static PersistentObject instance;
    private bool isQuitting = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (!isQuitting && instance == this)
        {
            instance = null;
        }
    }
}