using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrDemo.Scripts;
using AnkrSDK.Data;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.Examples.UseCases.WebGlLogin
{
	public class WebGLConnectionController : MonoBehaviour
	{
#if UNITY_WEBGL
		[SerializeField] private WebGl.WebGLConnect _webGlConnect;

		[SerializeField] private WebGLLoginPanelController _webGlLoginManager;

		[SerializeField] private WebGLHeaderWalletsPanel _webGlLoginViewer;

		private Dictionary<string, bool> _walletsStatus;
		private TaskCompletionSource<WalletsStatus> _completionSource;

		private void Awake()
		{
			_completionSource = new TaskCompletionSource<WalletsStatus>();

			_webGlConnect.OnNeedPanel += ActivatePanel;
			_webGlConnect.OnConnect += ChangePanels;
			_webGlLoginManager.NetworkChosen += OnNetworkChosen;
			_webGlLoginManager.WalletChosen += OnWalletChosen;
			_webGlLoginViewer.ConnectTo += OnConnect;
		}

		private async void Start()
		{
			var _walletsStatus = await _webGlConnect.GetWalletsStatus();
			_completionSource.SetResult(_walletsStatus);
			_webGlLoginViewer.SetWalletsStatus(_walletsStatus);
		}

		private void ActivatePanel()
		{
			var walletStatusTask = _completionSource.Task;
			walletStatusTask.ContinueWith(HandleWalletStatus);
		}
		
		private void HandleWalletStatus(Task<WalletsStatus> answer)
		{
			var status = answer.Result;
			Debug.Log(JsonConvert.SerializeObject(status));
			var loginedWallet = GetLoginedWallet(status);
			if (loginedWallet != Wallet.None)
			{
				_webGlConnect.SetWallet(loginedWallet);
			}
			else
			{
				_webGlLoginManager.ShowPanel();
			}
		}

		private async UniTask UpdateWalletsStatus()
		{
			var _walletsStatus = await _webGlConnect.GetWalletsStatus();
			_webGlLoginViewer.SetWalletsStatus(_walletsStatus);
		}

		private void ChangePanels(WebGL.WebGLWrapper provider)
		{
			UpdateWalletsStatus().Forget();
			_webGlLoginManager.HidePanel();
		}

		private void OnNetworkChosen(NetworkName network)
		{
			_webGlConnect.SetNetwork(network);
		}

		private void OnWalletChosen(Wallet wallet)
		{
			_webGlConnect.SetWallet(wallet);
		}

		private void OnConnect(Wallet wallet)
		{
			_webGlConnect.Connect(wallet).Forget();
		}

		private Wallet GetLoginedWallet(WalletsStatus status)
		{
			var defaultWallet = Wallet.None;
			var preferableWallet = Wallet.Metamask;

			foreach (KeyValuePair<Wallet, bool> valuePair in status)
			{
				if (valuePair.Key == preferableWallet && valuePair.Value)
				{
					defaultWallet = preferableWallet;
					break;
				}

				if (valuePair.Value)
				{
					defaultWallet = valuePair.Key;
				}
			}

			return defaultWallet;
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			return _webGlConnect.Session.GetWalletsStatus();
		}

		private void OnDisable()
		{
			_webGlConnect.OnNeedPanel -= ActivatePanel;
			_webGlConnect.OnConnect -= ChangePanels;
			_webGlLoginManager.NetworkChosen -= OnNetworkChosen;
			_webGlLoginManager.WalletChosen -= OnWalletChosen;
			_webGlLoginViewer.ConnectTo -= OnConnect;
		}
#endif
	}
}