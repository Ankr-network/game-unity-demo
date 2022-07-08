using System;
using System.Collections;
using System.Collections.Generic;
using AnkrSDK.UseCases.Ads;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private DemoAndroidAdsManager _demoAndroidAdsManager;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _insideGfx;
    
    public void OnInteraction()
    {
        _demoAndroidAdsManager.ShowFullscreenAd();
    }

    private void Start()
    {
        _insideGfx.SetActive(false);
        AnkrAds.Ads.AnkrAds.OnAdRewarded += OnAdRewarded;
    }

    private void OnDestroy()
    {
        AnkrAds.Ads.AnkrAds.OnAdRewarded -= OnAdRewarded;
    }

    private async void OnAdRewarded(string uuid)
    {
        await UniTask.SwitchToMainThread();
        
        if (uuid == AdsBackendInformation.FullscreenAdTestUnitId)
        {
            _insideGfx.SetActive(true);
            _animator.SetTrigger("OpenChest");
        }
    }
}
