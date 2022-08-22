using AnkrDemo.Scripts;
using AnkrSDK.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.Examples.UseCases.WebGlLogin
{
	public class WebGLConnectionController : MonoBehaviour
	{
		[SerializeField]
		private WebGl.WebGLConnect _webGlConnect;

		[SerializeField]
		private WebGLLoginPanelController _webGlLoginManager;
		
		[SerializeField]
		private WebGLHeaderWalletsPanel _webGlLoginViewer;

		private void Awake()
		{
			_webGlConnect.OnNeedPanel += ActivatePanel;
			_webGlConnect.OnConnect += ChangeLoginPanel;
			_webGlLoginManager.NetworkChosen += OnNetworkChosen;
			_webGlLoginManager.WalletChosen += OnWalletChosen;
			_webGlLoginViewer.ConnectTo += OnConnect;
		}

		private async void Start()
		{
			var status = await _webGlConnect.GetWalletsStatus();
			_webGlLoginViewer.SetWalletsStatus(status);
		}

		private void ActivatePanel()
		{
			_webGlLoginManager.ShowPanel();
		}

		private void ChangeLoginPanel(WebGL.WebGLWrapper provider)
		{
			_webGlLoginManager.HidePanel();
		}

		private void OnNetworkChosen(NetworkName network)
		{
			_webGlConnect.SetNetwork(network);
		}
		
		private void OnWalletChosen(WebGL.SupportedWallets wallet)
		{
			_webGlConnect.SetWallet(wallet);
		}

		private void OnConnect(WebGL.SupportedWallets wallet)
		{
			_webGlConnect.ConnectTo(wallet).Forget();
		}

		private void OnDisable()
		{
			_webGlConnect.OnNeedPanel -= ActivatePanel;
			_webGlConnect.OnConnect -= ChangeLoginPanel;
			_webGlLoginManager.NetworkChosen -= OnNetworkChosen;
			_webGlLoginManager.WalletChosen -= OnWalletChosen;
		}
	}
}