using UnityEngine;
using UnityEngine.SceneManagement;

public class AdsSceneController : MonoBehaviour
{
    private void Start()
    {
        if (AdManager.Instance != null && AdManager.Instance.interstitialAd != null)
        {
            AdManager.Instance.interstitialAd.OnAdClosed += OnAdClosed;

            AdManager.Instance.interstitialAd.ShowAd();
        }
        else
        {
            Debug.LogWarning("AdManager or interstitialAd is missing! Loading next scene anyway...");
            LoadNextScene();
        }
    }

    private void OnAdClosed()
    {
        if (AdManager.Instance != null && AdManager.Instance.interstitialAd != null)
            AdManager.Instance.interstitialAd.OnAdClosed -= OnAdClosed;

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        string nextScene = PlayerPrefs.GetString("NextSceneAfterAd", string.Empty);
        PlayerPrefs.DeleteKey("NextSceneAfterAd");

        if (!string.IsNullOrEmpty(nextScene))
        {
            Debug.Log($"Loading next scene after ad: {nextScene}");
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("No next scene name found in PlayerPrefs!");
        }
    }
}
