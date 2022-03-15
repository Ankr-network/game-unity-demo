using System.Numerics;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Implementation;
using MirageSDK.Core.Infrastructure;
using MirageSDK.Core.Utils;
using MirageSDK.Examples.ContractMessages.ERC1155;
using MirageSDK.Examples.ContractMessages.GameCharacterContract;
using MirageSDK.Examples.WearableNFTExample;
using MirageSDK.WalletConnectSharp.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoScript : MonoBehaviour
{
	private const string TransactionGasLimit = "1000000";
	private const string BlueHatAddress = "0x00010000000000000000000000000000000000000000000000000000000001";
	private const string RedHatAddress = "0x00010000000000000000000000000000000000000000000000000000000002";

	[SerializeField]
	private GameObject _character;

	[SerializeField]
	private TMP_Text _characterID;

	[SerializeField]
	private GameObject _blueHat;

	[SerializeField]
	private Button _blueHatButton;

	[SerializeField]
	private GameObject _redHat;

	[SerializeField]
	private Button _redHatButton;

	[SerializeField]
	private TMP_Text _text;

	[SerializeField]
	private Inventory _inventory;

	private readonly int _blueHatID = 0;

	private readonly int _redHatID = 1;

	private IContract _gameCharacterContract;
	private IContract _gameItemContract;


	private void Awake()
	{
		_blueHatButton.onClick.AddListener(EquipBlueHat);
		_redHatButton.onClick.AddListener(EquipRedHat);
	}

	private void Start()
	{
		var mirageSDKWrapper = MirageSDKWrapper.GetSDKInstance(WearableNFTContractInformation.ProviderURL);
		_gameCharacterContract = mirageSDKWrapper.GetContract(
			WearableNFTContractInformation.GameCharacterContractAddress,
			WearableNFTContractInformation.GameCharacterABI);
		_gameItemContract = mirageSDKWrapper.GetContract(WearableNFTContractInformation.GameItemContractAddress,
			WearableNFTContractInformation.GameItemABI);

		_character.SetActive(false);
		LoadCharacter();
		CheckCharactersEquippedHatAndDisplay();
		GetItemTokensBalanceAndUpdateShow();
	}

	private void OnDestroy()
	{
		_blueHatButton.onClick.RemoveListener(EquipBlueHat);
		_redHatButton.onClick.RemoveListener(EquipRedHat);
	}

	private void EquipBlueHat()
	{
		EquipHat("Blue");
	}

	private void EquipRedHat()
	{
		EquipHat("Red");
	}

	private async void CheckCharactersEquippedHatAndDisplay()
	{
		var equippedHat = await GetHat();
		switch (equippedHat)
		{
			case "0x10000000000000000000000000000000000000000000000000000000002":
				Debug.LogError("1");
				break;
			case "0x10000000000000000000000000000000000000000000000000000000001":
				UpdateHatVisuals("Blue");
				break;
			default:
				UpdateHatVisuals("");
				break;
		}
	}

	private async void EquipHat(string hatColour)
	{
		switch (hatColour)
		{
			case "Red":
				await ChangeHat(RedHatAddress);
				UpdateHatVisuals(hatColour);
				break;
			case "Blue":
				await ChangeHat(BlueHatAddress);
				UpdateHatVisuals(hatColour);
				break;
		}

		GetItemTokensBalanceAndUpdateShow();
	}

	private void UpdateHatVisuals(string hatColour)
	{
		switch (hatColour)
		{
			case "Red":
				_redHat.SetActive(true);
				_blueHat.SetActive(false);
				break;
			case "Blue":
				_redHat.SetActive(false);
				_blueHat.SetActive(true);
				break;
			default:
				_redHat.SetActive(false);
				_blueHat.SetActive(false);
				break;
		}
	}

	private async UniTask<string> GetHat()
	{
		var characterID = await GetCharacterTokenId();
		var getHatMessage = new GetHatMessage
		{
			CharacterId = characterID.ToString()
		};
		var hatId = await _gameCharacterContract.GetData<GetHatMessage, BigInteger>(getHatMessage);
		var hexaHatID = MirageSDKHelper.StringToBigInteger(hatId.ToString());
		UpdateUILogs($"Hat Id: {hexaHatID}");

		return hexaHatID;
	}

	private async UniTask ChangeHat(string hatAddress)
	{
		const string changeHatMethodName = "changeHat";
		var characterId = await GetCharacterTokenId();

		var hasHat = await GetHasHatToken(hatAddress);

		if (!hasHat || characterId.Equals(-1))
		{
			UpdateUILogs("ERROR : CharacterID or HatID is null");
		}
		else
		{
			var transactionHash = await _gameCharacterContract.CallMethod(changeHatMethodName, new object[]
			{
				characterId,
				hatAddress
			}, TransactionGasLimit);

			UpdateUILogs($"Hat Changed. Hash : {transactionHash}");
		}
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

	private async void LoadCharacter()
	{
		var tokenID = await GetCharacterTokenId();

		if (tokenID != -1) // if we have a character Token then i show it ingame
		{
			_characterID.text = tokenID.ToString();
			_character.SetActive(true);
		}
	}

	private async UniTask<BigInteger> GetCharacterTokenId()
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

	private async void GetItemTokensBalanceAndUpdateShow()
	{
		var redHatTokenBalance = await GetBalanceERC1155(_gameItemContract, RedHatAddress);
		var blueHatTokenBalance = await GetBalanceERC1155(_gameItemContract, BlueHatAddress);

		if (redHatTokenBalance > 0)
		{
			_inventory.ShowInventoryItem(_redHatID, true);
		}
		else
		{
			_inventory.ShowInventoryItem(_redHatID, false);
		}

		if (blueHatTokenBalance > 0)
		{
			_inventory.ShowInventoryItem(_blueHatID, true);
		}
		else
		{
			_inventory.ShowInventoryItem(_blueHatID, false);
		}
	}

	private void UpdateUILogs(string log)
	{
		_text.text += "\n" + log;
		Debug.Log(log);
	}
}