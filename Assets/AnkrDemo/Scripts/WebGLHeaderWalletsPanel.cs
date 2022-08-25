using System;
using System.Collections.Generic;
using AnkrSDK.Examples.UseCases.WebGlLogin;
using AnkrSDK.WebGL;
using Demo.Scripts;
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
		public Action<SupportedWallets> ConnectTo;

		public void SetWalletsStatus(Dictionary<string, bool> status)
		{
			foreach (var wallet in _wallets)
			{
				var button = Instantiate(_buttonPrefab);
				button.transform.parent = _panel.transform;
				var buttonScript = button.GetComponent<HeaderWalletButton>();
				buttonScript.WalletItem = wallet;
				buttonScript.OnClickHandler += OnWalletClick;
				_buttons.Add(buttonScript);
					
				var walletType = SupportedWallets.None;
				if (status[wallet.Type.ToString()])
				{
					buttonScript.SetLogined();
				}
			}
		}

		private void OnWalletClick(SupportedWallets wallet)
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