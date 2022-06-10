using UnityEngine;
using AnkrSDK.Ads.UI;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.Provider;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
public class DemoAndroidAdsManager : MonoBehaviour
{
		[SerializeField] private Button _initializeButton;
		[SerializeField] private Button _loadButton;
		[SerializeField] private Button _viewButton;
		[SerializeField] private TMP_Text _logs;

		private IEthHandler _eth;

		private void Awake()
		{
			var ankrSDK = AnkrSDKFactory.GetAnkrSDKInstance(ERC20ContractInformation.HttpProviderURL);
			_eth = ankrSDK.Eth;
		}
		
		private void OnDestroy()
		{
			_initializeButton.onClick.RemoveAllListeners();
			_viewButton.onClick.RemoveAllListeners();
			_loadButton.onClick.RemoveAllListeners();
			UnsubscribeToCallbackListenerEvents();
		}

		private void OnEnable()
		{
			_initializeButton.interactable = true;
			_viewButton.interactable = false;
			_loadButton.interactable = false;
			
			_initializeButton.onClick.AddListener(OnInitializeButtonClick);
			_loadButton.onClick.AddListener(OnLoadButtonClick);
			_viewButton.onClick.AddListener(OnViewButtonClick);
		}
		
		private void ActivateNextButton(int buttonToActivate)
		{
			switch (buttonToActivate)
			{
				case 0:
					_initializeButton.interactable = true;
					_viewButton.interactable = false;
					_loadButton.interactable = false;
					break;
				case 1:
					_initializeButton.interactable = false;
					_viewButton.interactable = false;
					_loadButton.interactable = true;
					break;
				case 2:
					_initializeButton.interactable = false;
					_viewButton.interactable = true;
					_loadButton.interactable = false;
					break;
				default: Debug.LogError("WrongButtonNb");
					break;
			}
		}
		
		private void SubscribeToCallbackListenerEvents()
		{
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdClicked += CallbackListenerOnAdClicked;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdClosed += CallbackListenerOnAdClosed;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdFinished += CallbackListenerOnAdFinished;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdLoaded += CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdOpened += CallbackListenerOnAdOpened;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdFailedToLoad += CallbackListenerOnAdFailedToLoad;
		}

		private void UpdateUILogs(string log)
		{
			_logs.text += "\n" + log;
			Debug.Log(log);
		}

		private void CallbackListenerOnAdClicked()
		{
			UpdateUILogs("CallbackListenerOnAdClicked");
			Debug.LogWarning("CallbackListenerOnAdClicked");
		}
		
		private void CallbackListenerOnAdClosed()
		{
			UpdateUILogs("CallbackListenerOnAdClosed");
			Debug.LogWarning("CallbackListenerOnAdClosed");
		}
		
		private void CallbackListenerOnAdFinished()
		{
			UpdateUILogs("CallbackListenerOnAdFinished");
			Debug.LogWarning("CallbackListenerOnAdFinished");
		}
		
		private void CallbackListenerOnAdLoaded()
		{
			UpdateUILogs("CallbackListenerOnAdLoaded");
			Debug.LogWarning("CallbackListenerOnAdLoaded");
		}
		
		private void CallbackListenerOnAdOpened()
		{
			UpdateUILogs("CallbackListenerOnAdOpened");
			Debug.LogWarning("CallbackListenerOnAdOpened");
		}
		
		private void CallbackListenerOnAdFailedToLoad()
		{
			UpdateUILogs("CallbackListenerOnAdFailedToLoad");
			Debug.LogWarning("CallbackListenerOnAdFailedToLoad");
		}

		private void UnsubscribeToCallbackListenerEvents()
		{
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdClicked -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdClosed -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdFinished -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdLoaded -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdOpened -= CallbackListenerOnAdLoaded;
			AnkrAds.Ads.AnkrAdsNativeAndroid.callbackListener.OnAdFailedToLoad -= CallbackListenerOnAdLoaded;
		}

		private void OnInitializeButtonClick()
		{
			const string walletAddress = "This is ankr mobile address";
			const string appId = "1c562170-9ee5-4157-a5f8-e99c32e73cb0";
			AnkrAds.Ads.AnkrAdsNativeAndroid.Initialize(appId, walletAddress);
			UnsubscribeToCallbackListenerEvents();
			SubscribeToCallbackListenerEvents();
			ActivateNextButton(1);
		}

		private void OnLoadButtonClick()
		{
			const string unitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
			AnkrAds.Ads.AnkrAdsNativeAndroid.LoadAd(unitId);
			ActivateNextButton(2);
		}
		private void OnViewButtonClick()
		{
			const string unitId = "d396af2c-aa3a-44da-ba17-68dbb7a8daa1";
			AnkrAds.Ads.AnkrAdsNativeAndroid.ShowAd(unitId);
			ActivateNextButton(3);
		}
}
