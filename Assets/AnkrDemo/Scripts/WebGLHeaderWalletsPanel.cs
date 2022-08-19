using System;
using System.Collections.Generic;
using AnkrSDK.WebGL;
using Demo.Scripts;
using UnityEngine;

namespace AnkrDemo.Scripts
{
	public class WebGLHeaderWalletsPanel : MonoBehaviour
	{
		[SerializeField] private HeaderWalletButton[] _buttons;

		public Action<SupportedWallets> ConnectTo;

		public void SetWalletsStatus(Dictionary<string, bool> status)
		{
			foreach (KeyValuePair<string, bool> valuePair in status)
			{
				foreach (var button in _buttons)
				{
					var walletType = SupportedWallets.None;
					if (SupportedWallets.TryParse(valuePair.Key, out walletType) &&
					    button.WalletItem.Type == walletType && valuePair.Value)
					{
						button.SetLogined();
					}
				}
			}
		}

		private void SetUpButtons()
		{
			foreach (var button in _buttons)
			{
				button.OnClickHandler += OnWalletClick;
			}
		}

		private void DisableButtons()
		{
			foreach (var button in _buttons)
			{
				button.OnClickHandler -= OnWalletClick;
			}
		}

		private void OnWalletClick(SupportedWallets wallet)
		{
			ConnectTo?.Invoke(wallet);
		}
	}
}