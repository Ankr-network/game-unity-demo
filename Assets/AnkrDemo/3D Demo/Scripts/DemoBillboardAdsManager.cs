using System.Collections.Generic;
using AnkrSDK.Ads;
using AnkrSDK.Ads.UI;
using AnkrSDK.UseCases.Ads;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DemoBillboardAdsManager : MonoBehaviour
{
    [SerializeField] private List<AnkrBannerAdSprite> _worldSpaceAdsList;
    private int _adCount = 0;
    
    private void Start()
    {
        InitializeAds();
    }

    private void OnDestroy()
    {
        UnsubscribeToCallbackListenerEvents();
    }

    private void InitializeAds()
    {
        const string walletAddress = "This is ankr mobile address";
        UnsubscribeToCallbackListenerEvents();
        SubscribeToCallbackListenerEvents();

        AnkrAdvertisements.Initialize(AdsBackendInformation.TestAppId, walletAddress);
    }
    
    private void SubscribeToCallbackListenerEvents()
    {
        AnkrAdvertisements.AdInitialized += CallbackListenerOnAdInitialized;
        AnkrAdvertisements.AdFailedToLoad += CallbackListenerOnAdFailedToLoad;
        AnkrAdvertisements.AdTextureReceived += CallbackListenerOnAdTextureReceived;
        AnkrAdvertisements.Error += CallbackListenerOnError;
    }

    private void UnsubscribeToCallbackListenerEvents()
    {
        AnkrAdvertisements.AdInitialized -= CallbackListenerOnAdInitialized;
        AnkrAdvertisements.AdFailedToLoad -= CallbackListenerOnAdFailedToLoad;
        AnkrAdvertisements.AdTextureReceived -= CallbackListenerOnAdTextureReceived;
        AnkrAdvertisements.Error -= CallbackListenerOnError;
    }
    
    private async void CallbackListenerOnAdInitialized()
    {
        await UniTask.SwitchToMainThread();
        AnkrAdvertisements.LoadAdTexture(AdsBackendInformation.BannerAdTestUnitId);
        AnkrAdvertisements.LoadAd(AdsBackendInformation.FullscreenAdTestUnitId);
    }
    
    private async void CallbackListenerOnAdFailedToLoad(string uuid)
    {
        await UniTask.SwitchToMainThread();
    }
    
    private async void CallbackListenerOnAdTextureReceived(string unitID, byte[] adTextureData)
    {
        await UniTask.SwitchToMainThread();
        
        DownloadAds(adTextureData);
    }

    private async void CallbackListenerOnError(string errorMessage)
    {
        await UniTask.SwitchToMainThread();
        Debug.Log("Error : " + errorMessage);
    }
    
    private void DownloadAds(byte[] adTextureData)
    {
        var texture = new Texture2D(2, 2);
        texture.LoadImage(adTextureData);
        foreach (var ankrBannerAdSprite in _worldSpaceAdsList)
        {
            ankrBannerAdSprite.SetupAd(texture);
            ankrBannerAdSprite.TryShow();
        }
    }
}
