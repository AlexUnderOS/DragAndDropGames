using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    string _adUnitId;

    [SerializeField] Button _bannerButton;
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    bool isLoaded = false;
    bool isVisible = false;
    bool pendingShow = false;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;
        Advertisement.Banner.SetPosition(_bannerPosition);

        if (_bannerButton != null)
        {
            _bannerButton.onClick.RemoveAllListeners();
            _bannerButton.onClick.AddListener(OnButtonClick);
            _bannerButton.interactable = true;
        }
    }

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        _bannerButton = button;

        _bannerButton.onClick.RemoveAllListeners();
        _bannerButton.onClick.AddListener(OnButtonClick);
        _bannerButton.interactable = true;

        Debug.Log("[BannerAd] Banner button hooked: " + _bannerButton.name);
    }

    private void OnButtonClick()
    {
        Debug.Log($"[BannerAd] Button clicked. isLoaded={isLoaded}, isVisible={isVisible}");

        if (isVisible)
        {
            HideBanner();
            return;
        }

        if (!isLoaded)
        {
            Debug.Log("[BannerAd] Banner not loaded yet, loading and will show when ready.");
            pendingShow = true;
            LoadBanner();
            return;
        }

        ShowBanner();
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

        if (pendingShow)
        {
            pendingShow = false;
            ShowBanner();
        }
    }

    private void OnBannerError(string message)
    {
        Debug.LogWarning($"Banner ad failed to load: {message}");
        isLoaded = false;
        pendingShow = false;
    }

    private void ShowBanner()
    {
        BannerOptions options = new BannerOptions
        {
            showCallback = OnBannerShown,
            hideCallback = OnBannerHidden,
            clickCallback = OnBannerClicked
        };

        Advertisement.Banner.Show(_adUnitId, options);
    }

    private void HideBanner()
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
