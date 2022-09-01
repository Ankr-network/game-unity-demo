using System;
using AnkrSDK.Data;
using AnkrSDK.Examples.UseCases.WebGlLogin;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrDemo
{
	public class HeaderWalletButton : MonoBehaviour
	{
		[SerializeField]
		private WalletItem _walletItem;
		
		[SerializeField]
		private Image _logoContainer;
		
		[SerializeField]
		private GameObject _markerContainer;
		
		[SerializeField]
		private Button _button;

		public WalletItem WalletItem
		{
			get { return _walletItem; }
			set
			{
				_walletItem = value;
				Initialize();
			}
		}
		
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
			_logoContainer.sprite = _walletItem.Logo;
			_button.onClick.AddListener(HandleClick);
		}

		private void HandleClick()
		{
			OnClickHandler?.Invoke(_walletItem.Type);
		}
	}
}