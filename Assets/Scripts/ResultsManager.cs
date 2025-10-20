using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ResultsManager : MonoBehaviour
{
    public ObjectScript objectScript;
    public BBordScript timerScript;
    public GameObject resultsObject;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI topScoresText;
    private bool levelCompleted = false;
    private const int TopCount = 5;
    private const string TopScoresKey = "TopScores";

    private FlyingObjectSpawnScript spawnScript;
    private List<FlyingObjectsControllerScript> flyingObjects = new List<FlyingObjectsControllerScript>();

    void Start()
    {
        spawnScript = FindAnyObjectByType<FlyingObjectSpawnScript>();
    }

    void Update()
    {
        if (levelCompleted || objectScript == null || resultsObject == null || timerScript == null) return;
        if (ObjectScript.drag) return;
        if (objectScript.placedCount == objectScript.vehicles.Length)
        {
            StartCoroutine(ActivateResultsWithDelay(0.1f));
        }
    }

    private IEnumerator ActivateResultsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (objectScript.placedCount == objectScript.vehicles.Length)
        {
            float finalTime = timerScript.StopTimer();
            if (currentTimeText != null)
            {
                currentTimeText.text = "Jūsu laiks: " + System.TimeSpan.FromSeconds(finalTime).ToString("mm':'ss':'fff");
            }
            UpdateTopScores(finalTime);
            DisplayStars(finalTime);
            DisplayTopScores();
            
            FreezeGameTime();
            
            resultsObject.SetActive(true);
            levelCompleted = true;
        }
    }

    private void FreezeGameTime()
    {
        if (spawnScript != null)
        {
            spawnScript.StopAllCoroutines();
            CancelInvokeOnSpawnScript();
        }

        StopAllFlyingObjects();

        Time.timeScale = 0f;
    }

    private void CancelInvokeOnSpawnScript()
    {
        if (spawnScript != null)
        {
            CancelInvoke("SpawnCloud");
            CancelInvoke("SpawnObject");
        }
    }

    private void StopAllFlyingObjects()
    {
        flyingObjects.Clear();
        FlyingObjectsControllerScript[] allFlyingObjects = Object.FindObjectsByType<FlyingObjectsControllerScript>(FindObjectsSortMode.None);
        flyingObjects.AddRange(allFlyingObjects);

        foreach (FlyingObjectsControllerScript obj in flyingObjects)
        {
            if (obj != null)
            {
                obj.StopAllMovement();
            }
        }
    }

    public void ResumeGameTime()
    {
        Time.timeScale = 1f;
        
        foreach (FlyingObjectsControllerScript obj in flyingObjects)
        {
            if (obj != null)
            {
                obj.ResumeMovement();
            }
        }
        flyingObjects.Clear();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
    }

    private void UpdateTopScores(float newTime)
    {
        SortedSet<float> topScores = new SortedSet<float>();
        for (int i = 0; i < TopCount; i++)
        {
            float score = PlayerPrefs.GetFloat(TopScoresKey + i, 3599.999f);
            topScores.Add(score);
        }
        topScores.Add(newTime);
        if (topScores.Count > TopCount)
        {
            topScores.Remove(topScores.Max);
        }
        float[] scoresArray = topScores.ToArray();
        for (int i = 0; i < TopCount; i++)
        {
            if (i < scoresArray.Length)
            {
                PlayerPrefs.SetFloat(TopScoresKey + i, scoresArray[i]);
            }
            else
            {
                PlayerPrefs.SetFloat(TopScoresKey + i, 3599.999f);
            }
        }
        PlayerPrefs.Save();
    }

    private void DisplayTopScores()
    {
        if (topScoresText == null) return;
        System.Text.StringBuilder sb = new System.Text.StringBuilder("Top-5:\n");
        for (int i = 0; i < TopCount; i++)
        {
            float score = PlayerPrefs.GetFloat(TopScoresKey + i, 3599.999f);
            string timeStr = (score >= 3599.999f) ? "00:00:00" : System.TimeSpan.FromSeconds(score).ToString("mm':'ss':'ff");
            sb.AppendLine($"{i + 1} - {timeStr}");
        }
        topScoresText.text = sb.ToString();
    }

    public GameObject[] stars;
    public float timeForThreeStars;
    public float timeForTwoStars;  
    public float timeForOneStar;
    
    public void DisplayStars(float finalTime)
    {
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }

        if (finalTime <= timeForThreeStars)
        {
            stars[0].SetActive(true);
            stars[1].SetActive(true);
            stars[2].SetActive(true);
        }
        else if (finalTime <= timeForTwoStars)
        {
            stars[0].SetActive(true);
            stars[1].SetActive(true);
        }
        else if (finalTime <= timeForOneStar)
        {
            stars[0].SetActive(true);
        }
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}