using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInit : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    public static bool IsInitialized { get; private set; } = false;
    public event Action OnAdsInitialized;

    private void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        _gameId = _androidGameId;
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity ads initialization complete!");
        IsInitialized = true;
        OnAdsInitialized?.Invoke();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogWarning($"Unity ads initialization failed: {error} - {message}");
        IsInitialized = false;
    }
}
