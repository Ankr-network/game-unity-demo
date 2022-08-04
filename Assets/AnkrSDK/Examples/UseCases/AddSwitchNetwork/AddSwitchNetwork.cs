using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data;
using AnkrSDK.Examples.ERC20Example;
using AnkrSDK.Provider;
using AnkrSDK.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrSDK.UseCases.AddSwitchNetwork
{
	public class AddSwitchNetwork : UseCase
	{
		[SerializeField]
		private Button _avaxButton;

		[SerializeField]
		private Button _avaxTestButton;
		
		private IAnkrSDK _ankrSDKWrapper;
		
		public override void ActivateUseCase()
		{
			base.ActivateUseCase();
			_ankrSDKWrapper = AnkrSDKFactory.GetAnkrSDKInstance(ERC20ContractInformation.HttpProviderURL);
		}

		private void Awake()
		{
			_avaxButton.onClick.AddListener(OpenAddSwitchAvalanche);
			_avaxTestButton.onClick.AddListener(OpenAddSwitchAvalancheTestnet);
		}

		private void OnDestroy()
		{
			_avaxButton.onClick.RemoveListener(OpenAddSwitchAvalanche);
			_avaxTestButton.onClick.RemoveListener(OpenAddSwitchAvalancheTestnet);
		}

		private void OpenAddSwitchAvalanche()
		{
			var network = EthereumNetworks.GetNetworkByName(NetworkName.Avalanche);
			_ankrSDKWrapper.NetworkHelper.AddAndSwitchNetwork(network);
		}

		private void OpenAddSwitchAvalancheTestnet()
		{
			var network = EthereumNetworks.GetNetworkByName(NetworkName.Avalanche_TestNet);
			_ankrSDKWrapper.NetworkHelper.AddAndSwitchNetwork(network);
		}
	}
}