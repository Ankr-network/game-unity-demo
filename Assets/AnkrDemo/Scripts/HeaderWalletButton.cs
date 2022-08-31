using System;
using AnkrSDK.Data;
using AnkrSDK.Examples.UseCases.WebGlLogin;
using UnityEngine;
using UnityEngine.UI;

namespace Demo.Scripts
{
	public class HeaderWalletButton : MonoBehaviour
	{
		public WalletItem WalletItem;
		
		[SerializeField]
		private Image _logoContainer;
		
		[SerializeField]
		private GameObject _markerContainer;
		
		[SerializeField]
		private Button _button;
		
		public event Action<Wallet> OnClickHandler;

		private void Start()
		{
			Initialize();
			SetLoginState(false);
		}
		
		private void OnDisable()
		{
			_button.onClick.RemoveListener(HandleClick);
		}

		public void SetLoginState(bool isLoggedIn)
		{
			_markerContainer.SetActive(isLoggedIn);
		}

		private void Initialize()
		{
			_logoContainer.sprite = WalletItem.Logo;
			_button.onClick.AddListener(HandleClick);
		}

		private void HandleClick()
		{
			OnClickHandler?.Invoke(WalletItem.Type);
		}
	}
}