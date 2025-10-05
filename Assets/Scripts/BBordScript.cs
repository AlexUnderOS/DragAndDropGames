using UnityEngine;
using UnityEngine.UI;

public class BBordScript : MonoBehaviour
{
    public Text timeText;
    private float startTime;
    private float elapsedTime; // Храним последнее значение для остановки
    private bool isRunning = true; // Флаг для остановки

    void Start()
    {
        startTime = Time.realtimeSinceStartup;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (!isRunning) return; // Не обновляем, если остановлен

        elapsedTime = Time.realtimeSinceStartup - startTime;
        timeText.text = System.TimeSpan.FromSeconds(elapsedTime).ToString("mm':'ss':'fff");
    }

    // Метод для остановки таймера и получения финального времени (в секундах)
    public float StopTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            timeText.text = System.TimeSpan.FromSeconds(elapsedTime).ToString("mm':'ss':'fff"); // Замораживаем текст
        }
        return elapsedTime;
    }

    // Метод для получения текущего времени без остановки
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}