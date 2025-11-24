using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void LoadWithAd(string targetScene)
    {
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowInterstitialAndThen(targetScene);
        }
        else
        {
            Debug.LogWarning("AdManager.Instance is null, loading scene without ad.");
            SceneManager.LoadScene(targetScene);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
