using AnkrSDK.Data;
using UnityEngine;

namespace AnkrSDK.Examples.UseCases.WebGlLogin
{
#if UNITY_WEBGL
	public class WebGLConnectionController : MonoBehaviour
	{
		[SerializeField]
		private WebGl.WebGLConnect _webGlConnect;

		[SerializeField]
		private WebGLLoginPanelController _webGlLoginManager;

		private void Awake()
		{
			_webGlConnect.OnNeedPanel += ActivatePanel;
			_webGlConnect.OnConnect += ChangeLoginPanel;
			_webGlLoginManager.NetworkChosen += OnNetworkChosen;
			_webGlLoginManager.WalletChosen += OnWalletChosen;
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

		private void OnDisable()
		{
			_webGlConnect.OnNeedPanel -= ActivatePanel;
			_webGlConnect.OnConnect -= ChangeLoginPanel;
			_webGlLoginManager.NetworkChosen -= OnNetworkChosen;
			_webGlLoginManager.WalletChosen -= OnWalletChosen;
		}
	}
#endif
}