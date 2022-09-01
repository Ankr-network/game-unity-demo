using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrAnkrDemo;
using AnkrSDK.Data;
using AnkrSDK.Tools;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.Examples.UseCases.WebGlLogin
{
	public class WebGLConnectionController : MonoBehaviour
	{
#if UNITY_WEBGL
		[SerializeField] private WebGl.WebGLConnect _webGlConnect;

		[SerializeField] private WebGLLoginPanelController _webGlLoginManager;

		[SerializeField] private WebGLHeaderWalletsPanel _webGlLoginViewer;
		
		[CustomDropdown(typeof (SupportedWallets), "GetWebGLWallets")]
		private Wallet _preferableWallet = Wallet.Metamask;

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

		private void Start()
		{
			Initialize().Forget();
		}

		private async UniTaskVoid Initialize()
		{
			var _walletsStatus = await _webGlConnect.GetWalletsStatus();
			_completionSource.SetResult(_walletsStatus);
			_webGlLoginViewer.SetWalletsStatus(_walletsStatus);
		}

		private void ActivatePanel()
		{
			HandleWalletStatus().Forget();
		}
		
		private async UniTaskVoid HandleWalletStatus()
		{
			var walletStatus = await _completionSource.Task;
			var loginedWallet = GetLoggedInWallet(walletStatus);
			if (loginedWallet != Wallet.None)
			{
				_webGlConnect.SetWallet(loginedWallet);
			}
			else
			{
				_webGlLoginManager.ShowPanel();
			}
		}

		private async UniTaskVoid UpdateWalletsStatus()
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

		private Wallet GetLoggedInWallet(WalletsStatus status)
		{
			if (status.ContainsKey(_preferableWallet) && status[_preferableWallet])
			{
				return _preferableWallet;
			}

			return GetFirstOrDefaultLoggedIn(status);
		}

		private Wallet GetFirstOrDefaultLoggedIn(WalletsStatus status)
		{
			var defaultWallet = Wallet.None;
			
			foreach (KeyValuePair<Wallet, bool> valuePair in status)
			{
				if (valuePair.Value)
				{
					defaultWallet = valuePair.Key;
					break;
				}
			}

			return defaultWallet;
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			return _webGlConnect.SessionWrapper.GetWalletsStatus();
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