using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    string _adUnitId;

    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    bool isLoaded = false;
    bool isVisible = false;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;
        Advertisement.Banner.SetPosition(_bannerPosition);
    }

    private void Start()
    {
        StartCoroutine(WaitForAdsAndLoad());
    }

    private IEnumerator WaitForAdsAndLoad()
    {
        while (!Advertisement.isInitialized)
        {
            Debug.Log("[BannerAd] Waiting for Unity Ads initialization...");
            yield return null;
        }

        LoadBanner();
    }

    public void LoadBanner()
    {
        Debug.Log($"[BannerAd] LoadBanner called. Advertisement.isInitialized={Advertisement.isInitialized}");

        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load banner ad before ads was initialized!");
            return;
        }

        Debug.Log("Loading banner ad...");
        BannerLoadOptions options = new BannerLoadOptions()
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, options);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner ad loaded successfully.");
        isLoaded = true;

        ShowBanner();
    }

    private void OnBannerError(string message)
    {
        Debug.LogWarning($"Banner ad failed to load: {message}");
        isLoaded = false;

        StartCoroutine(RetryLoad(5f));
    }

    private IEnumerator RetryLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadBanner();
    }

    public void ShowBanner()
    {
        if (!isLoaded)
        {
            Debug.LogWarning("Trying to show banner, but it is not loaded yet.");
            return;
        }

        BannerOptions options = new BannerOptions
        {
            showCallback = OnBannerShown,
            hideCallback = OnBannerHidden,
            clickCallback = OnBannerClicked
        };

        Advertisement.Banner.Show(_adUnitId, options);
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    private void OnBannerShown()
    {
        Debug.Log("Banner ad is now visible.");
        isVisible = true;
    }

    private void OnBannerHidden()
    {
        Debug.Log("Banner ad is now hidden.");
        isVisible = false;
    }

    private void OnBannerClicked()
    {
        Debug.Log("Banner ad was clicked.");
    }
}
