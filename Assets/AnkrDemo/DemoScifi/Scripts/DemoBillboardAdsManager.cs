using System.Collections;
using System.Collections.Generic;
using AnkrSDK.Ads.UI;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.Provider;
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
    
    private void InitializeAds()
    {
        const string walletAddress = "This is ankr mobile address";
        UnsubscribeToCallbackListenerEvents();
        SubscribeToCallbackListenerEvents();

        AnkrAds.Ads.AnkrAds.Initialize(AdsBackendInformation.TestAppId, walletAddress, RuntimePlatform.Android);
    }
    
    private void SubscribeToCallbackListenerEvents()
    {
        AnkrAds.Ads.AnkrAds.OnAdInitialized += CallbackListenerOnAdInitialized;
        AnkrAds.Ads.AnkrAds.OnAdFailedToLoad += CallbackListenerOnAdFailedToLoad;
        AnkrAds.Ads.AnkrAds.OnAdTextureReceived += CallbackListenerOnAdTextureReceived;
        AnkrAds.Ads.AnkrAds.OnError += CallbackListenerOnError;
    }

    private void UnsubscribeToCallbackListenerEvents()
    {
        AnkrAds.Ads.AnkrAds.OnAdInitialized -= CallbackListenerOnAdInitialized;
        AnkrAds.Ads.AnkrAds.OnAdFailedToLoad -= CallbackListenerOnAdFailedToLoad;
        AnkrAds.Ads.AnkrAds.OnAdTextureReceived -= CallbackListenerOnAdTextureReceived;
        AnkrAds.Ads.AnkrAds.OnError -= CallbackListenerOnError;
    }
    
    private async void CallbackListenerOnAdInitialized()
    {
        await UniTask.SwitchToMainThread();
        Debug.Log("CallbackListenerOnAdInitialized");
        AnkrAds.Ads.AnkrAds.LoadAd(AdsBackendInformation.FullscreenAdTestUnitId);
    }
    
    private async void CallbackListenerOnAdFailedToLoad(string uuid)
    {
        await UniTask.SwitchToMainThread();
        Debug.Log("CallbackListenerOnAdFailedToLoad");
    }
    
    private async void CallbackListenerOnAdTextureReceived(string unitID, byte[] adTextureData)
    {
        await UniTask.SwitchToMainThread();
        Debug.Log("CallbackListenerOnAdTextureReceived");
        
        DownloadAds(adTextureData);
        
        if(_adCount<_worldSpaceAdsList.Count)
        {
            AnkrAds.Ads.AnkrAds.LoadAdTexture(AdsBackendInformation.BannerAdTestUnitId);
        }
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
        _worldSpaceAdsList[_adCount].SetupAd(texture);
        _worldSpaceAdsList[_adCount].TryShow();
        _adCount++;
    }
}
