using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrDemo.Scripts;
using AnkrSDK.Data;
using AnkrSDK.WebGL;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace AnkrSDK.Examples.UseCases.WebGlLogin
{
	public class WebGLConnectionController : MonoBehaviour
	{
#if UNITY_WEBGL
		[SerializeField]
		private WebGl.WebGLConnect _webGlConnect;

		[SerializeField]
		private WebGLLoginPanelController _webGlLoginManager;
		
		[SerializeField]
		private WebGLHeaderWalletsPanel _webGlLoginViewer;

		private Dictionary<string, bool> _walletsStatus;
		private TaskCompletionSource<Dictionary<string, bool>> _completionSource;

		private void Awake()
		{
			_completionSource = new TaskCompletionSource<Dictionary<string, bool>>();
			
			_webGlConnect.OnNeedPanel += ActivatePanel;
			_webGlConnect.OnConnect += ChangeLoginPanel;
			_webGlLoginManager.NetworkChosen += OnNetworkChosen;
			_webGlLoginManager.WalletChosen += OnWalletChosen;
			_webGlLoginViewer.ConnectTo += OnConnect;
		}

		private async void Start()
		{
			var _walletsStatus = await _webGlConnect.GetWalletsStatus();
			_completionSource.TrySetResult(_walletsStatus);
			_webGlLoginViewer.SetWalletsStatus(_walletsStatus);
		}

		private void ActivatePanel()
		{
			var task = _completionSource.Task;
			task.ContinueWith(answer =>
			{
				var status = answer.Result;
				Debug.Log(JsonConvert.SerializeObject(status));
				var loginedWallet = GetLoginedWallet(status);
				if (loginedWallet != SupportedWallets.None)
				{
					_webGlConnect.SetWallet(loginedWallet);
				}
				else
				{
					_webGlLoginManager.ShowPanel();
				}
			});
		}

		private void ChangeLoginPanel(WebGL.WebGLWrapper provider)
		{
			_webGlLoginManager.HidePanel();
		}

		private void OnNetworkChosen(NetworkName network)
		{
			_webGlConnect.SetNetwork(network);
		}
		
		private void OnWalletChosen(SupportedWallets wallet)
		{
			_webGlConnect.SetWallet(wallet);
		}

		private void OnConnect(SupportedWallets wallet)
		{
//			_webGlConnect.Session.ConnectTo(wallet).Forget();
		}
		
		private SupportedWallets GetLoginedWallet(Dictionary<string, bool> status)
		{
			var defaultWallet = SupportedWallets.None;
			var preferableWallet = SupportedWallets.Metamask;
			
			foreach (KeyValuePair<string, bool> valuePair in status)
			{
				var walletType = SupportedWallets.None;
				if (SupportedWallets.TryParse(valuePair.Key, out walletType))
				{
					if (walletType == preferableWallet && valuePair.Value)
					{
						defaultWallet = preferableWallet;
						break;
					}
					if (valuePair.Value)
					{
						defaultWallet = walletType;
					}
				}
			}

			return defaultWallet;
		}
		
		public UniTask<Dictionary<string, bool>> GetWalletsStatus()
		{
			return _webGlConnect.Session.GetWalletsStatus();
		}

		private void OnDisable()
		{
			_webGlConnect.OnNeedPanel -= ActivatePanel;
			_webGlConnect.OnConnect -= ChangeLoginPanel;
			_webGlLoginManager.NetworkChosen -= OnNetworkChosen;
			_webGlLoginManager.WalletChosen -= OnWalletChosen;
			_webGlLoginViewer.ConnectTo -= OnConnect;
		}
#endif
	}
}