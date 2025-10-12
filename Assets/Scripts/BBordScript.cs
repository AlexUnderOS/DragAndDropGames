using UnityEngine;
using UnityEngine.UI;

public class BBordScript : MonoBehaviour
{
    public Text timeText;
    private float startTime;
    private float elapsedTime;
    private bool isRunning = true;

    void Start()
    {
        startTime = Time.realtimeSinceStartup;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime = Time.realtimeSinceStartup - startTime;
        timeText.text = System.TimeSpan.FromSeconds(elapsedTime).ToString("mm':'ss':'ff");
    }

    public float StopTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            timeText.text = System.TimeSpan.FromSeconds(elapsedTime).ToString("mm':'ss':'ff");
        }
        return elapsedTime;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}