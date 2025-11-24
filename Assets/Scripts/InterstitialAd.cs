using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
    private string _adUnitId;

    [SerializeField] private Button _interstitialAdButton;

    public bool isReady = false;

    public event Action OnAdClosed;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;

        if (_interstitialAdButton != null)
        {
            _interstitialAdButton.interactable = false;
            _interstitialAdButton.onClick.RemoveAllListeners();
            _interstitialAdButton.onClick.AddListener(ShowAd);
        }
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load interstitial ad before Unity ads was initialized!");
            return;
        }

        Debug.Log("Loading interstitial ad");
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        if (!isReady)
        {
            Debug.Log("Interstitial ad is not ready yet!");
            return;
        }

        Debug.Log("Showing interstitial ad...");
        if (_interstitialAdButton != null)
            _interstitialAdButton.interactable = false;

        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (!placementId.Equals(_adUnitId)) return;

        Debug.Log("Interstitial ad loaded!");
        isReady = true;

        if (_interstitialAdButton != null)
            _interstitialAdButton.interactable = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Failed to load interstitial ad: {error} - {message}");
        isReady = false;
        StartCoroutine(RetryLoad(2f));
    }

    private IEnumerator RetryLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Interstitial ad show started");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("User clicked on interstitial ad");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"Interstitial ad completed with state: {showCompletionState}");

        OnAdClosed?.Invoke();

        isReady = false;
        LoadAd();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Error showing interstitial ad: {error} - {message}");

        OnAdClosed?.Invoke();

        isReady = false;
        LoadAd();
    }

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        _interstitialAdButton = button;
        _interstitialAdButton.onClick.RemoveAllListeners();
        _interstitialAdButton.onClick.AddListener(ShowAd);
        _interstitialAdButton.interactable = isReady;
    }
}
