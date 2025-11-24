using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    string _adUnitId;

    [SerializeField] Button _rewardedAdButton;
    public FlyingObjectManager flyingObjectManager;

    private bool isLoaded = false;

    public void Awake()
    {
        _adUnitId = _androidAdUnitId;

        if (flyingObjectManager == null)
            flyingObjectManager = FindFirstObjectByType<FlyingObjectManager>();

        if (_rewardedAdButton != null)
            _rewardedAdButton.interactable = false;
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load rewarded ad before Unity ads was initialized.");
            return;
        }

        Debug.Log("Loading rewarded ad");
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (!placementId.Equals(_adUnitId))
            return;

        Debug.Log("Rewarded ad loaded!");
        isLoaded = true;

        if (_rewardedAdButton != null)
        {
            _rewardedAdButton.interactable = true;
        }
        else
        {
            Debug.LogWarning("RewardedAds: _rewardedAdButton is null on ad loaded – button will be activated later when SetButton is called.");
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Failed to load rewarded ad! {error} - {message}");
        isLoaded = false;
        StartCoroutine(WaitAndLoad(5f));
    }

    public IEnumerator WaitAndLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadAd();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Failed to show rewarded ad! {error} - {message}");
        isLoaded = false;
        StartCoroutine(WaitAndLoad(5f));
        Time.timeScale = 1f;
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Time.timeScale = 0f;
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("User clicked on rewarded ad");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"Rewarded ad completed! State: {showCompletionState}");

        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            if (HanoiGameManager.Instance != null)
            {
                HanoiGameManager.Instance.RemoveOneDiskReward();
            }
            else if (flyingObjectManager != null)
            {
            }

            StartCoroutine(SlowMoReward(3f, 0.2f));
        }
        else
        {
            Time.timeScale = 1f;
        }

        if (_rewardedAdButton != null)
            _rewardedAdButton.interactable = false;

        isLoaded = false;
        StartCoroutine(WaitAndLoad(10f));
    }

    private IEnumerator SlowMoReward(float duration, float targetScale)
    {
        Time.timeScale = targetScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }

public void SetButton(Button button)
{
    if (button == null)
        return;

    _rewardedAdButton = button;

    _rewardedAdButton.onClick.RemoveAllListeners();
    _rewardedAdButton.onClick.AddListener(ShowAd);

    _rewardedAdButton.interactable =
        isLoaded &&
        HanoiGameManager.Instance != null &&
        HanoiGameManager.Instance.gameRunning;
}

    public void ShowAd()
    {
        if (!isLoaded)
        {
            Debug.Log("Rewarded ad is not loaded yet, cannot show.");
            return;
        }

        if (_rewardedAdButton != null)
            _rewardedAdButton.interactable = false;

        isLoaded = false;
        Advertisement.Show(_adUnitId, this);
    }
}
