using System;
using System.Collections.Generic;
using AnkrSDK.Data;
using AnkrSDK.Examples.UseCases.WebGlLogin;
using AnkrDemo;
using UnityEngine;

namespace AnkrAnkrDemo
{
	public class WebGLHeaderWalletsPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject _panel;
		[SerializeField]
		private HeaderWalletButton _buttonPrefab;
		[SerializeField]
		private WalletItem[] _wallets;
		private List<HeaderWalletButton> _buttons = new List<HeaderWalletButton>();
		public event Action<Wallet> ConnectTo;

		private void Start()
		{
			CreateButtons();
		}

		private void CreateButtons()
		{
			foreach (var wallet in _wallets)
			{
				var buttonScript = Instantiate(_buttonPrefab, _panel.transform);
				buttonScript.WalletItem = wallet;
				buttonScript.OnClickHandler += OnWalletClicked;
				_buttons.Add(buttonScript);
			}
		}

		public void SetWalletsStatus(WalletsStatus status)
		{
			foreach (var buttonScript in _buttons)
			{	
				var walletType = buttonScript.WalletItem.Type;
				buttonScript.SetLoginState(status.ContainsKey(walletType) && status[walletType]);
			}
		}

		private void OnWalletClicked(Wallet wallet)
		{
			ConnectTo?.Invoke(wallet);
		}

		private void OnDisable()
		{
			foreach (var button in _buttons)
			{
				button.OnClickHandler -= OnWalletClicked;
			}
		}
	}
}