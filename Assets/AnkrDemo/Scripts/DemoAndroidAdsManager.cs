using AnkrSDK.Ads;
using UnityEngine;
using AnkrSDK.UseCases.Ads;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
public class DemoAndroidAdsManager : MonoBehaviour
{
		[SerializeField] private Button _viewButton;
		[SerializeField] private TMP_Text _logs;

		public bool isAdLoaded;

		private void Start()
		{
			UnsubscribeToCallbackListenerEvents();
			SubscribeToCallbackListenerEvents();
		}

		private void OnDestroy()
		{
			_viewButton.onClick.RemoveAllListeners();
			UnsubscribeToCallbackListenerEvents();
		}

		private void OnEnable()
		{
			_viewButton.interactable = false;
			_viewButton.onClick.AddListener(OnViewButtonClick);
		}

		private void SubscribeToCallbackListenerEvents()
		{
			AnkrAdvertisements.AdInitialized += CallbackListenerOnAdInitialized;
			AnkrAdvertisements.AdClicked += CallbackListenerOnAdClicked;
			AnkrAdvertisements.AdClosed += CallbackListenerOnAdClosed;
			AnkrAdvertisements.AdFinished += CallbackListenerOnAdFinished;
			AnkrAdvertisements.AdLoaded += CallbackListenerOnAdLoaded;
			AnkrAdvertisements.AdOpened += CallbackListenerOnAdOpened;
			AnkrAdvertisements.AdRewarded += CallbackListenerOnAdRewarded;
			AnkrAdvertisements.AdFailedToLoad += CallbackListenerOnAdFailedToLoad;
			AnkrAdvertisements.Error += CallbackListenerOnError;
		}

		private void UpdateUILogs(string log)
		{
			_logs.text += "\n" + log;
			Debug.Log(log);
		}

		private async void CallbackListenerOnAdInitialized()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdInitialized");
		}

		private async void CallbackListenerOnAdLoaded(string uuid)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdLoaded");
			
			if (uuid == AdsBackendInformation.FullscreenAdTestUnitId)
			{
				isAdLoaded = true;
				_viewButton.interactable = true;
			}
		}

		private async void CallbackListenerOnAdClicked(string uuid)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdClicked");
		}

		private async void CallbackListenerOnAdClosed()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdClosed");
			_viewButton.interactable = false;
			isAdLoaded = false;
			AnkrAdvertisements.LoadAd(AdsBackendInformation.FullscreenAdTestUnitId);
			
		}

		private async void CallbackListenerOnAdFinished()
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("CallbackListenerOnAdFinished");
			_viewButton.interactable = false;
			isAdLoaded = false;
			AnkrAdvertisements.LoadAd(AdsBackendInformation.FullscreenAdTestUnitId);
			
		}

		private async void CallbackListenerOnAdRewarded(string uuid)
		{
			await UniTask.SwitchToMainThread();
			_viewButton.interactable = false;
			isAdLoaded = false;
			AnkrAdvertisements.LoadAd(AdsBackendInformation.FullscreenAdTestUnitId);
			
		}

		private async void CallbackListenerOnAdOpened()
		{
			await UniTask.SwitchToMainThread();
		}

		private async void CallbackListenerOnAdFailedToLoad(string uuid)
		{
			await UniTask.SwitchToMainThread();
		}

		private async void CallbackListenerOnError(string errorMessage)
		{
			await UniTask.SwitchToMainThread();
			UpdateUILogs("Error : " + errorMessage);
		}

		private void UnsubscribeToCallbackListenerEvents()
		{
			AnkrAdvertisements.AdInitialized -= CallbackListenerOnAdInitialized;
			AnkrAdvertisements.AdClicked -= CallbackListenerOnAdClicked;
			AnkrAdvertisements.AdClosed -= CallbackListenerOnAdClosed;
			AnkrAdvertisements.AdFinished -= CallbackListenerOnAdFinished;
			AnkrAdvertisements.AdLoaded -= CallbackListenerOnAdLoaded;
			AnkrAdvertisements.AdOpened -= CallbackListenerOnAdOpened;
			AnkrAdvertisements.AdRewarded -= CallbackListenerOnAdRewarded;
			AnkrAdvertisements.AdFailedToLoad -= CallbackListenerOnAdFailedToLoad;
			AnkrAdvertisements.Error -= CallbackListenerOnError;
		}

		private void OnViewButtonClick()
		{
			_viewButton.interactable = false;
			ShowFullscreenAd();
		}

		public void ShowFullscreenAd()
		{
			if (isAdLoaded)
			{
				AnkrAdvertisements.ShowAd(AdsBackendInformation.FullscreenAdTestUnitId);
			}
			else
			{
				Debug.LogError("No Ads loaded");
			}
		}
}
