using System;
using System.Numerics;
using AnkrSDK;
using AnkrSDK.Core.Infrastructure;
using AnkrSDK.Data.ContractMessages.ERC1155;
using AnkrSDK.GameCharacterContract;
using AnkrSDK.Provider;
using AnkrSDK.Utils;
using AnkrSDK.WearableNFTExample;
using Common.Logging.Configuration;
using Cysharp.Threading.Tasks;
using AnkrDemo.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnkrDemo
{
	public class DemoContractHandler : MonoBehaviour
	{
		private const string TransactionGasLimit = "1000000";

		[SerializeField] private TMP_Text _text;

		private IContract _gameCharacterContract;
		private IContract _gameItemContract;
		private string _activeSessionAccount;

		public Action onContractInitializedCallback;

		public void Init()
		{
			var sdkWrapper = AnkrSDKFactory.GetAnkrSDKInstance(WearableNFTContractInformation.ProviderURL);
			_gameCharacterContract = sdkWrapper.GetContract(
				WearableNFTContractInformation.GameCharacterContractAddress,
				WearableNFTContractInformation.GameCharacterABI);
			_gameItemContract = sdkWrapper.GetContract(WearableNFTContractInformation.GameItemContractAddress,
				WearableNFTContractInformation.GameItemABI);
			ActivateAsync(sdkWrapper).Forget();
		}
		
		private async UniTaskVoid ActivateAsync(IAnkrSDK ankrSDK)
		{
			var result = await ankrSDK.Eth.GetDefaultAccount();
			_activeSessionAccount = result;
			onContractInitializedCallback?.Invoke();
		}

		public async UniTask MintItems()
		{
			const string mintBatchMethodName = "mintBatch";
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
				new object[] { _activeSessionAccount, itemsToMint, itemsAmounts, data });

			UpdateUILogs($"Game Items Minted. Receipts : {receipt}");
		}

		public async UniTask<bool> CheckIfCharacterIsApprovedForAll()
		{
			return await _gameCharacterContract.IsApprovedForAll(_activeSessionAccount,
				WearableNFTContractInformation.GameCharacterContractAddress);
		}

		public async UniTask<string> ApproveAllForCharacter(bool approved)
		{
			return await _gameItemContract.SetApprovalForAll(
				WearableNFTContractInformation.GameCharacterContractAddress, approved);
		}

		public async UniTask MintCharacter()
		{
			const string safeMintMethodName = "safeMint";

			var transactionHash = await _gameCharacterContract.CallMethod(safeMintMethodName,
				new object[] { _activeSessionAccount });

			UpdateUILogs($"Game Character Minted. Hash : {transactionHash}");
		}

		public async UniTask<string> GetHat()
		{
			var characterID = await GetCharacterTokenId();
			var getHatMessage = new GetHatMessage
			{
				CharacterId = characterID
			};
			var hatId = await _gameCharacterContract.GetData<GetHatMessage, BigInteger>(getHatMessage);
			var hexHatID = AnkrSDKHelper.StringToBigInteger(hatId.ToString());
			UpdateUILogs($"Hat Id: {hexHatID}");

			return hexHatID;
		}

		public async UniTask ChangeHat(string hatAddress)
		{
			const string changeHatMethodName = "changeHat";
			var characterId = await GetCharacterTokenId();

			var evController = new LoggerEventHandler();
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

				UpdateUILogs("Change Hat transaction Complete !");
			}

			EventControllerUnsubscribeToEvents(evController);
		}

		private void EventControllerSubscribeToEvents(LoggerEventHandler evController)
		{
		}

		private void EventControllerUnsubscribeToEvents(LoggerEventHandler evController)
		{
		}
		
		public async UniTask<bool> GetHasHatToken(string tokenAddress)
		{
			var tokenBalance = await GetBalanceErc1155(_gameItemContract, tokenAddress);

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
			var tokenBalance = await GetCharacterBalance();

			if (tokenBalance > 0)
			{
				var tokenId =
					await _gameCharacterContract.TokenOfOwnerByIndex(_activeSessionAccount, 0);

				UpdateUILogs($"GameCharacter tokenId  : {tokenId}");

				return tokenId;
			}

			UpdateUILogs("You dont own any of these tokens.");
			return -1;
		}

		private async UniTask<BigInteger> GetCharacterBalance()
		{
			var balance = await _gameCharacterContract.BalanceOf(_activeSessionAccount);

			UpdateUILogs($"Number of NFTs Owned: {balance}");
			return balance;
		}

		public async UniTask<BigInteger> GetItemBalance(string id)
		{
			var balance = await GetBalanceErc1155(_gameItemContract, id);
			return balance;
		}

		private async UniTask<BigInteger> GetBalanceErc1155(IContract contract, string id)
		{
			var balanceOfMessage = new BalanceOfMessage
			{
				Account = _activeSessionAccount,
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