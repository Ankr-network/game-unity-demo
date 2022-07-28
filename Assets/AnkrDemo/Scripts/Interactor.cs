using AnkrSDK.Ads;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
	[SerializeField] private DemoAndroidAdsManager _demoAndroidAdsManager;
	[SerializeField] private Button _interactButton;
	private Chest _target;

	private void Start()
	{
		AnkrAdvertisements.AdLoaded += CallbackListenerOnAdLoaded;
		_interactButton.onClick.AddListener(OnInteractButtonClicked);
		_interactButton.interactable = false;
	}

	private void OnDestroy()
	{
		AnkrAdvertisements.AdLoaded -= CallbackListenerOnAdLoaded;
		_interactButton.onClick.RemoveAllListeners();
	}

	private void OnTriggerEnter(Collider other)
	{
		_interactButton.interactable = _demoAndroidAdsManager.isAdLoaded;
		_target = other.gameObject.GetComponent<Chest>();
	}

	private void OnTriggerExit(Collider other)
	{
		_interactButton.interactable = false;
		_target = null;
	}

	private void OnInteractButtonClicked()
	{
		_target.OnInteraction();
		_interactButton.interactable = false;
	}
	
	private async void CallbackListenerOnAdLoaded(string uuid)
	{
		await UniTask.SwitchToMainThread();
		if(_target)
		{
			_interactButton.interactable = _demoAndroidAdsManager.isAdLoaded;
		}
	}
	
}