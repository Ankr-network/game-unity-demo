using System;
using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Events;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Core.Utils;
using MirageSDK.Demo.Helpers;
using MirageSDK.Examples.ContractMessages.ERC1155;
using MirageSDK.Examples.ContractMessages.GameCharacterContract;
using MirageSDK.Examples.DTO;
using MirageSDK.Examples.WearableNFTExample;
using MirageSDK.WalletConnectSharp.Unity;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using TMPro;
using UnityEngine;

namespace MirageSDK.Demo
{
	public class DemoContractHandler : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text _text;
		private const string TransactionGasLimit = "1000000";
		private IContract _gameCharacterContract;
		private IContract _gameItemContract;

		private void Awake()
		{
			var mirageSDKWrapper = MirageSDKWrapper.GetSDKInstance(WearableNFTContractInformation.ProviderURL);
			_gameCharacterContract = mirageSDKWrapper.GetContract(
				WearableNFTContractInformation.GameCharacterContractAddress,
				WearableNFTContractInformation.GameCharacterABI);
			_gameItemContract = mirageSDKWrapper.GetContract(WearableNFTContractInformation.GameItemContractAddress,
				WearableNFTContractInformation.GameItemABI);
		}

		public async UniTask MintItems()
		{
			const string mintBatchMethodName = "mintBatch";
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var itemsToMint = new[]
			{
				ItemsContractHelper.BlueHatAddress,
				ItemsContractHelper.RedHatAddress,
				ItemsContractHelper.BlueShoesAddress,
				ItemsContractHelper.WhiteShoesAddress,
				ItemsContractHelper.RedGlassesAddress,
				ItemsContractHelper.WhiteGlassesAddress
			};
			var itemsAmounts = new[]
			{
				1, 2, 3, 4, 5, 6
			};
			var data = new byte[] { };

			var receipt = await _gameItemContract.CallMethod(mintBatchMethodName,
				new object[] { activeSessionAccount, itemsToMint, itemsAmounts, data });

			UpdateUILogs($"Game Items Minted. Receipts : {receipt}");
		}

		public async UniTask<bool> CheckIfCharacterIsApprovedForAll()
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			return await ERC721ContractFunctions.IsApprovedForAll(activeSessionAccount,
				WearableNFTContractInformation.GameCharacterContractAddress, _gameItemContract);
		}

		public async UniTask<string> ApproveAllForCharacter(bool approved)
		{
			return await ERC721ContractFunctions.SetApprovalForAll(
				WearableNFTContractInformation.GameCharacterContractAddress, approved, _gameItemContract);
		}

		public async UniTask MintCharacter()
		{
			const string safeMintMethodName = "safeMint";
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];

			var transactionHash = await _gameCharacterContract.CallMethod(safeMintMethodName,
				new object[] { activeSessionAccount });

			UpdateUILogs($"Game Character Minted. Hash : {transactionHash}");
		}

		public async UniTask<string> GetHat()
		{
			var characterID = await GetCharacterTokenId();
			var getHatMessage = new GetHatMessage
			{
				CharacterId = characterID.ToString()
			};
			var hatId = await _gameCharacterContract.GetData<GetHatMessage, BigInteger>(getHatMessage);
			var hexHatID = MirageSDKHelper.StringToBigInteger(hatId.ToString());
			UpdateUILogs($"Hat Id: {hexHatID}");

			return hexHatID;
		}

		public async UniTask ChangeHat(string hatAddress)
		{
			const string changeHatMethodName = "changeHat";
			var characterId = await GetCharacterTokenId();
			
			var evController = new EventController();
			EventControllerSubscribeToEvents(evController);

			var hasHat = await GetHasHatToken(hatAddress);

			if (!hasHat || characterId.Equals(-1))
			{
				UpdateUILogs("ERROR : CharacterID or HatID is null");
			}
			else
			{
				await _gameCharacterContract.Web3SendMethod(changeHatMethodName, new object[]
				{
					characterId,
					hatAddress
				}, evController);

				UpdateUILogs($"Change Hat transaction Complete !");
			}
			
			EventControllerUnsubscribeToEvents(evController);
		}

		private void EventControllerSubscribeToEvents(EventController evController)
		{
			evController.OnSending += HandleSending;
			evController.OnSent += HandleSent;
			evController.OnTransactionHash += HandleTransactionHash;
			evController.OnReceipt += HandleReceipt;
			evController.OnError += HandleError;
		}
		
		private void EventControllerUnsubscribeToEvents(EventController evController)
		{
			evController.OnSending -= HandleSending;
			evController.OnSent -= HandleSent;
			evController.OnTransactionHash -= HandleTransactionHash;
			evController.OnReceipt -= HandleReceipt;
			evController.OnError -= HandleError;
		}
		
		/*public void HandleReceipt(object sender, TransactionReceipt receipt)
		{
			var transferEventOutput = receipt.DecodeAllEvents<TransferEventDTO>();
			transferEventOutput[0].Event./// Get all data what you need
		}*/

		private void HandleSent(object sender, TransactionInput transaction)
		{
			UpdateUILogs($"Transaction sent");
		}

		private void HandleSending(object sender, TransactionInput transaction)
		{
			UpdateUILogs($"Transaction is sending");
		}

		private void HandleTransactionHash(object sender, string transactionHash)
		{
			UpdateUILogs($"transactionHash: {transactionHash}");
		}

		private void HandleReceipt(object sender, TransactionReceipt receipt)
		{
			UpdateUILogs("Receipt: " + receipt.Status);
		}

		private void HandleError(object sender, Exception err)
		{
			UpdateUILogs("Error: " + err.Message);
		}

		private async UniTask<bool> GetHasHatToken(string tokenAddress)
		{
			var tokenBalance = await GetBalanceERC1155(_gameItemContract, tokenAddress);

			if (tokenBalance > 0)
			{
				UpdateUILogs("You have " + tokenBalance + " hats");
				return true;
			}

			UpdateUILogs("You dont have any Hat Item");
			return false;
		}

		public async UniTask<BigInteger> GetCharacterTokenId()
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var tokenBalance = await GetCharacterBalance();

			if (tokenBalance > 0)
			{
				var tokenId =
					await ERC721ContractFunctions.TokenOfOwnerByIndex(activeSessionAccount, 0, _gameCharacterContract);

				UpdateUILogs($"GameCharacter tokenId  : {tokenId}");

				return tokenId;
			}

			UpdateUILogs("You dont own any of these tokens.");
			return -1;
		}

		private async UniTask<BigInteger> GetCharacterBalance()
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var balance = await ERC721ContractFunctions.BalanceOf(activeSessionAccount, _gameCharacterContract);

			UpdateUILogs($"Number of NFTs Owned: {balance}");
			return balance;
		}

		public async UniTask<BigInteger> GetItemBalance(string id)
		{
			var balance = await GetBalanceERC1155(_gameItemContract, id);
			return balance;
		}

		private async UniTask<BigInteger> GetBalanceERC1155(IContract contract, string id)
		{
			var activeSessionAccount = WalletConnect.ActiveSession.Accounts[0];
			var balanceOfMessage = new BalanceOfMessage
			{
				Account = activeSessionAccount,
				Id = id
			};
			var balance =
				await contract.GetData<BalanceOfMessage, BigInteger>(balanceOfMessage);

			UpdateUILogs($"Number of NFTs Owned: {balance}");
			return balance;
		}

		private void UpdateUILogs(string log)
		{
			_text.text += "\n" + log;
			Debug.Log(log);
		}
	}
}