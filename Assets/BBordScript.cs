using UnityEngine;
using UnityEngine.UI;

public class BBordScript : MonoBehaviour
{
    public Text timeText;

    void Update()
    {
        timeText.text = System.TimeSpan.FromSeconds(Time.realtimeSinceStartup).ToString("mm':'ss':'fff");
    }
}
