using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnkrSDK.Data;
using AnkrSDK.Utils;
using AnkrSDK.WebGL;
using AnkrSDK.WebGL.DTO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AnkrSDK.WebGl
{
	public class WebGLConnect : MonoBehaviour
	{
		[SerializeField] private SupportedWallets _defaultWallet = SupportedWallets.None;
		[SerializeField] private NetworkName _defaultNetwork = NetworkName.Rinkeby;
		[SerializeField] private bool _connectOnAwake;
		[SerializeField] private bool _connectOnStart = true;

		private UniTaskCompletionSource<SupportedWallets> _walletCompletionSource;
		public WebGLWrapper Session { get; private set; }
		public Action OnNeedPanel;
		public Action<WebGLWrapper> OnConnect;

	#if UNITY_WEBGL
		private async void Awake()
		{
			if (_connectOnAwake)
			{
				await Initialize();
			}
		}

		private async void Start()
		{
			if (_connectOnStart)
			{
				await Initialize();
			}
		}
	#endif

		private async Task Initialize()
		{
			DontDestroyOnLoad(this);
			Session = new WebGLWrapper();
			await Connect();
		}

		private async Task<SupportedWallets> GetLoginedWallet()
		{
			var status = await GetWalletsStatus();
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

		private async UniTask Connect()
		{
			var wallet = await GetLoginedWallet();
			Debug.Log(wallet);
			if (wallet == SupportedWallets.None)
			{
				OnNeedPanel?.Invoke();
				_walletCompletionSource = new UniTaskCompletionSource<SupportedWallets>();
				wallet = await _walletCompletionSource.Task;
			}

			await Session.ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_defaultNetwork));
			OnConnect?.Invoke(Session);
		}

		public async UniTask ConnectTo(SupportedWallets wallet)
		{
			await Session.ConnectTo(wallet, EthereumNetworks.GetNetworkByName(_defaultNetwork));
		}

		public UniTask<WalletsStatus> GetWalletsStatus()
		{
			return Session.GetWalletsStatus();
		}

		public void SetWallet(SupportedWallets wallet)
		{
			_walletCompletionSource.TrySetResult(wallet);
		}

		public void SetNetwork(NetworkName network)
		{
			_defaultNetwork = network;
		}
	}
}