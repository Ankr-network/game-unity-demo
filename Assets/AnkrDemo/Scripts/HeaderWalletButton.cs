using System;
using AnkrSDK.Examples.UseCases.WebGlLogin;
using AnkrSDK.WebGL;
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
		private Image _markerContainer;
		
		[SerializeField]
		private Button _button;
		
		public Action<SupportedWallets> OnClickHandler;

		private void Start()
		{
			Initialize();
			SetLogouted();
		}

		public void SetLogined()
		{
			_markerContainer.gameObject.SetActive(false);
		}
		
		public void SetLogouted()
		{
			_markerContainer.gameObject.SetActive(true);
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