using System.Collections;
using System.Collections.Generic;
using AnkrSDK.Ads.UI;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.Provider;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DemoBillboardAdsManager : MonoBehaviour
{
    [SerializeField] private List<AnkrBannerAdSprite> _worldSpaceAdsList;
    
    private IEthHandler _eth;
    
    private void Start()
    {
        var ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(ERC20ContractInformation.HttpProviderURL);
        _eth = ankrSDK.Eth;
        DownloadAd().Forget();
    }
    
    private async UniTaskVoid DownloadAd()
    {
        var defaultAccount = await _eth.GetDefaultAccount();
        var testAppId = "e8d0f552-22a5-482c-a149-2d51bace6ccb";
        var testUnitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
        
        foreach (AnkrBannerAdSprite ankrBannerAdSprite in _worldSpaceAdsList)
        {
            var requestResult = await AnkrAds.Ads.AnkrAds.DownloadAdData(testAppId, testUnitId, defaultAccount);
            
            if (requestResult != null)
            {
                await ankrBannerAdSprite.SetupAd(requestResult);
                ankrBannerAdSprite.TryShow();
            }
        }
    }
}
