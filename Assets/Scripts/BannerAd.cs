using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    string _adUnitId;
    [SerializeField] Button _bannerButton;
    public bool isBannerVisible = false;
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;
        Advertisement.Banner.SetPosition(_bannerPosition);
    }

    public void LoadBanner()
    {
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
        _bannerButton.interactable = true;
    }
    private void OnBannerError(string message)
    {
        Debug.LogWarning($"Banner ad failed to load: {message}");
        LoadBanner();
    }

    public void ShowBannerAd()
    {
        if (isBannerVisible)
        {
            HideBannerAd();
        }
        else
        {
            BannerOptions options = new BannerOptions
            {
                showCallback = OnBannerShown,
                hideCallback = OnBannerHidded,
                clickCallback = OnBannerClicked
            };

            Advertisement.Banner.Show(_adUnitId, options);
        }
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    private void OnBannerShown()
    {
        Debug.Log("Banner ad is now visible.");
        isBannerVisible = true;
    }
    private void OnBannerHidded()
    {
        Debug.Log("Banner ad is now hidden.");
        isBannerVisible = false;
    }
    private void OnBannerClicked()
    {
        Debug.Log("Banner ad was clicked.");
    }

    public void SetButton(Button button) 
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowBannerAd);
        _bannerButton = button;
        _bannerButton.interactable = false;
    }
}
