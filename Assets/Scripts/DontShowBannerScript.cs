using UnityEngine;
using UnityEngine.Advertisements;

public class DontShowBannerScript : MonoBehaviour
{
    private void Start()
    {
        HideBanner();
    }

    private void HideBanner()
    {
        if (Advertisement.isInitialized && Advertisement.Banner.isLoaded)
        {
            Advertisement.Banner.Hide();
        }
    }
}
