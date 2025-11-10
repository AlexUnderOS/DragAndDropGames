using System.Threading.Tasks;
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
        PlayerPrefs.SetString("NextSceneAfterAd", targetScene);
        PlayerPrefs.Save();
        SceneManager.LoadScene("AdScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
