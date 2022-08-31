using System;
using System.Collections.Generic;
using AnkrSDK.Data;
using AnkrSDK.Examples.UseCases.WebGlLogin;
using Demo.Scripts;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrDemo.Scripts
{
	public class WebGLHeaderWalletsPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private GameObject _buttonPrefab;
		[SerializeField]
		private WalletItem[] _wallets;
		private List<HeaderWalletButton> _buttons;
		public Action<Wallet> ConnectTo;

		private void Start()
		{
			_buttons = new List<HeaderWalletButton>();
			CreateButtons();
		}

		private void CreateButtons()
		{
			foreach (var wallet in _wallets)
			{
				var button = Instantiate(_buttonPrefab);
				button.transform.SetParent(_panel.transform, false);
				var buttonScript = button.GetComponent<HeaderWalletButton>();
				buttonScript.WalletItem = wallet;
				buttonScript.OnClickHandler += OnWalletClick;
				_buttons.Add(buttonScript);
			}
		}

		public void SetWalletsStatus(WalletsStatus status)
		{
			JsonConvert.SerializeObject(status);
			foreach (var buttonScript in _buttons)
			{	
				var walletType = buttonScript.WalletItem.Type;
				if (status.ContainsKey(walletType) && status[walletType])
				{
					buttonScript.SetLogined();
				}
				else
				{
					buttonScript.SetLogouted();
				}
			}
		}

		private void OnWalletClick(Wallet wallet)
		{
			ConnectTo?.Invoke(wallet);
		}

		private void OnDisable()
		{
			foreach (var button in _buttons)
			{
				button.OnClickHandler -= OnWalletClick;
			}
		}
	}
}