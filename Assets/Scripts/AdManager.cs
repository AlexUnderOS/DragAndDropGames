using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public AdsInit adsInitializer;
    public InterstitialAd interstitialAd;
    [SerializeField] bool turnOffInterstitialAd = false;

    public RewardedAds rewardedAds;
    [SerializeField] bool turnOffRewardedAds = false;

    public BannerAd bannerAd;
    [SerializeField] bool turnOffBannerAd = false;

    public static AdManager Instance { get; private set; }

    private void Awake()
    {
        if (adsInitializer == null)
            adsInitializer = FindFirstObjectByType<AdsInit>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (adsInitializer != null)
        {
            adsInitializer.OnAdsInitialized += HandleAdsInitialized;

            if (AdsInit.IsInitialized)
            {
                HandleAdsInitialized();
            }
        }
        else
        {
            Debug.LogWarning("AdsInit not found in scene!");
        }
    }

    private void HandleAdsInitialized()
    {
        Debug.Log("AdManager: HandleAdsInitialized");

        if (!turnOffInterstitialAd && interstitialAd != null)
        {
            interstitialAd.LoadAd();
        }

        if (!turnOffRewardedAds && rewardedAds != null)
        {
            rewardedAds.LoadAd();
        }
        if (!turnOffBannerAd && bannerAd != null)
        {
            bannerAd.LoadBanner();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (adsInitializer != null)
            adsInitializer.OnAdsInitialized -= HandleAdsInitialized;
    }

    private bool firstSceneLoad = false;

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    rewardedAds = FindFirstObjectByType<RewardedAds>();
    if (rewardedAds == null)
    {
        Debug.Log($"[AdManager] No RewardedAds on scene '{scene.name}'");
    }
    else
    {
        var rewardedButtonObj = FindGameObjectWithTagSafe("RewardedButton");
        if (rewardedButtonObj == null)
        {
            Debug.Log($"[AdManager] No object with tag 'RewardedButton' on scene '{scene.name}'");
        }
        else
        {
            var rewardedAdButton = rewardedButtonObj.GetComponent<Button>();
            if (rewardedAdButton == null)
            {
                Debug.Log("[AdManager] Object with tag 'RewardedButton' has no Button component!");
            }
            else
            {
                rewardedAds.SetButton(rewardedAdButton);
                Debug.Log("[AdManager] Rewarded button successfully hooked.");
            }
        }

        if (AdsInit.IsInitialized && !turnOffRewardedAds)
        {
            rewardedAds.LoadAd();
        }
    }

    if (bannerAd == null)
        bannerAd = FindFirstObjectByType<BannerAd>();

    if (interstitialAd == null)
        interstitialAd = FindFirstObjectByType<InterstitialAd>();

    var interstitialButtonObj = FindGameObjectWithTagSafe("InterstitialAdButton");
    if (interstitialAd != null && interstitialButtonObj != null)
    {
        var interstitialButton = interstitialButtonObj.GetComponent<Button>();
        if (interstitialButton != null)
            interstitialAd.SetButton(interstitialButton);
    }

    if (!firstSceneLoad)
    {
        firstSceneLoad = true;
        Debug.Log("First time scene loaded – не показываем рекламу.");
        return;
    }

    Debug.Log("Scene loaded (no auto ad here, используем ручной вызов при переходе).");
}
    public void ShowInterstitialAndThen(string nextScene)
    {
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogWarning("ShowInterstitialAndThen: nextScene is null or empty!");
            return;
        }

        if (turnOffInterstitialAd || interstitialAd == null)
        {
            Debug.Log("Interstitial is off or missing, loading scene without ad.");
            SceneManager.LoadScene(nextScene);
            return;
        }

        void OnClosed()
        {
            interstitialAd.OnAdClosed -= OnClosed;
            SceneManager.LoadScene(nextScene);
        }

        if (interstitialAd.isReady)
        {
            Debug.Log("ShowInterstitialAndThen: ad is ready, showing.");
            interstitialAd.OnAdClosed += OnClosed;
            interstitialAd.ShowAd();
        }
        else
        {
            Debug.Log("ShowInterstitialAndThen: ad NOT ready, loading scene without ad.");
            SceneManager.LoadScene(nextScene);
        }
    }

    public static GameObject FindGameObjectWithTagSafe(string tag)
    {
        try
        {
            return GameObject.FindGameObjectWithTag(tag);
        }
        catch
        {
            return null;
        }
    }
}
