using System;
using AnkrSDK.Data;
using AnkrSDK.Examples.UseCases.WebGlLogin;
using AnkrSDK.WebGL.DTO;
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
		
		public Action<Wallet> OnClickHandler;

		private void Start()
		{
			Initialize();
			SetLogouted();
		}

		public void SetLogined()
		{
			_markerContainer.SetActive(true);
		}
		
		public void SetLogouted()
		{
			_markerContainer.SetActive(false);
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

		private void OnDisable()
		{
			_button.onClick.RemoveListener(HandleClick);
		}
	}
}