using System;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public AdsInit adsInit;
    public InterstitialAd interstitialAd;
    [SerializeField] bool turnOffInterstitialAd = false;
    private bool firstAdShown = false;

    // . . . . . . .

    public static AdManager Instance { get; private set; }
    void Awake()
    {
        if (adsInit == null)
            adsInit = FindFirstObjectByType<AdsInit>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        adsInit.OnAdsInitialized += handleAdsInitialized;
    }

    private void handleAdsInitialized()
    {
        if (!turnOffInterstitialAd)
        {
            interstitialAd.OnInterstitialAdReady += HandleInterstitialReady;
            interstitialAd.LoadAd();
        }
    }

    void HandleInterstitialReady()
    {
        if (!firstAdShown)
        {
            Debug.Log("Showing first time interstitial ad automatically!");
            interstitialAd.ShowAd();
            firstAdShown = true;
        } 
        else
        {
            Debug.Log("Nex interstitial ad is ready for manual show!");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private bool firstSceneLoad = false;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (interstitialAd == null)
            interstitialAd = FindFirstObjectByType<InterstitialAd>();

        Button interstitialButton = GameObject.FindGameObjectWithTag("InterstitialAdButton").GetComponent<Button>();
    
        if (interstitialAd != null && interstitialButton != null)
        {
            interstitialAd.SetButton(interstitialButton);
        }

        if (!firstSceneLoad)
        {
            firstSceneLoad = true;
            Debug.Log("First time scene loaded!");
            return;
        }

        Debug.Log("Scene loaded!");
        handleAdsInitialized();
    } 
}
