using System.Collections.Generic;
using AnkrSDK.UseCases;
using Cysharp.Threading.Tasks;
using AnkrDemo;
using AnkrDemo.Data;
using AnkrDemo.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrAnkrDemo
{
	public class DemoScript : UseCase
	{
		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private TMP_Text _characterID;

		[SerializeField]
		private GameObject _character;

		[SerializeField]
		private GameObject _characterHead;

		[SerializeField]
		private ItemDescriptionsScriptableObject _itemsDescriptions;

		[SerializeField]
		private Inventory _inventory;

		[SerializeField]
		private DemoContractHandler _contractHandler;

		[SerializeField] private Button _mintCharacterButton;
		[SerializeField] private Button _mintHatButton;
		[SerializeField] private Button _approveCharacterButton;
		[SerializeField] private Button _loadCharacterButton;

		private readonly Dictionary<HatColour, ItemSceneData> _items = new Dictionary<HatColour, ItemSceneData>();

		private void Awake()
		{
			_mintCharacterButton.onClick.AddListener(MintCharacterCall);
			_mintHatButton.onClick.AddListener(MintItemsCall);
			_approveCharacterButton.onClick.AddListener(ApproveCharacterCall);
			_loadCharacterButton.onClick.AddListener(OnLoadButtonClickedCall);

			foreach (var item in _itemsDescriptions.Descriptions)
			{
				var instantiatedGO = CreateHat(item);
				var itemButton = _inventory.AddItem(item);
				itemButton.onClick.AddListener(() => OnButtonClick(item.Address));

				_items.Add(item.Colour, new ItemSceneData
				{
					Button = itemButton,
					GameObject = instantiatedGO
				});
			}
		}

		private void OnDestroy()
		{
			_mintCharacterButton.onClick.RemoveListener(MintCharacterCall);
			_mintHatButton.onClick.RemoveListener(MintItemsCall);
			_approveCharacterButton.onClick.RemoveListener(ApproveCharacterCall);
			_loadCharacterButton.onClick.RemoveListener(OnLoadButtonClickedCall);

			foreach (var itemData in _items.Values)
			{
				itemData.Button.onClick.RemoveAllListeners();
			}
		}

		public override void ActivateUseCase()
		{
			base.ActivateUseCase();
			Init();
		}

		private void Init()
		{
			_contractHandler.Init();
			_character.SetActive(false);
		}

		private async UniTask CheckIfHasCharacterOrMint()
		{
			var hasCharacter = await LoadCharacter();
			if (!hasCharacter)
			{
				UpdateUILogs("You do not have a CharacterNFT");
			}
		}

		private GameObject CreateHat(ItemDescription item)
		{
			var prefab = item.GameObjectPrefab;
			var instantiatedGO = Instantiate(prefab, _characterHead.transform, false);
			instantiatedGO.SetActive(false);
			return instantiatedGO;
		}

		private void OnButtonClick(string address)
		{
			OnButtonClickAsync(address).Forget();
		}

		private async UniTask OnButtonClickAsync(string address)
		{
			await EquipHat(address);
		}

		private async UniTask CheckCharactersEquippedHatAndDisplay()
		{
			var equippedHat = await _contractHandler.GetHat();
			if (equippedHat.TryConvertToHatColour(out var hatColour))
			{
				UpdateHatVisuals(hatColour);
				UpdateUILogs("Your character has a "+hatColour+" hat equipped");
			}
			else
			{
				RemoveHatVisuals();
				UpdateUILogs("Your character doesnt have a hat equipped");
			}
		}

		private async UniTask EquipHat(string address)
		{
			if (await _contractHandler.GetHasHatToken(address))
			{
				_inventory.EnableItemButtons(false);
				await _contractHandler.ChangeHat(address);
				_inventory.EnableItemButtons(true);
			}

			await CheckCharactersEquippedHatAndDisplay();
			await GetItemTokensBalanceAndUpdateInventory();
		}

		private void UpdateHatVisuals(HatColour hatColour)
		{
			foreach (var item in _items)
			{
				var isRightColour = hatColour == item.Key;
				item.Value.GameObject.SetActive(isRightColour);
			}
		}

		private void RemoveHatVisuals()
		{
			foreach (var item in _items.Values)
			{
				item.GameObject.SetActive(false);
			}
		}

		private async UniTask<bool> LoadCharacter()
		{
			var tokenID = await _contractHandler.GetCharacterTokenId();

			if (tokenID == -1)
			{
				return false;
			}

			_characterID.text = tokenID.ToString();
			_character.SetActive(true);

			return true;
		}

		private async UniTask GetItemTokensBalanceAndUpdateInventory()
		{
			for (var i = 0; i < _itemsDescriptions.Descriptions.Count; i++)
			{
				var addressTokenBalance =
					await _contractHandler.GetItemBalance(_itemsDescriptions.Descriptions[i].Address);
				_inventory.ShowInventoryItem(i, addressTokenBalance > 0, addressTokenBalance);
			}
		}

		private async void MintItemsCall()
		{
			await _contractHandler.MintItems();
		}

		private async void MintCharacterCall()
		{
			await _contractHandler.MintCharacter();
		}

		private async void ApproveCharacterCall()
		{
			await _contractHandler.ApproveAllForCharacter(true);
		}

		private async void OnLoadButtonClickedCall()
		{
			await CheckIfHasCharacterOrMint();
			await CheckCharactersEquippedHatAndDisplay();
			await GetItemTokensBalanceAndUpdateInventory();

			var characterID = await _contractHandler.GetCharacterTokenId();
			var equippedHatID = await _contractHandler.GetHat();
			UpdateUILogs("CharacterID: " + characterID + " / Equipped HatID: " + equippedHatID);
		}

		private void UpdateUILogs(string log)
		{
			_text.text += "\n" + log;
			Debug.Log(log);
		}
	}
}