using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MirageSDK.Demo.Data;
using MirageSDK.Demo.Helpers;
using TMPro;
using UnityEngine;

namespace MirageSDK.Demo
{
	public class DemoScript : MonoBehaviour
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
		
		private readonly Dictionary<HatColour, ItemSceneData> _items = new Dictionary<HatColour, ItemSceneData>();

		private void Awake()
		{
			foreach (var item in _itemsDescriptions.Descriptions)
			{
				var instantiatedGO = CreateHat(item);
				var itemButton = _inventory.AddItem(item);
				itemButton.onClick.AddListener(() => OnButtonClick(item.Address));

				_items.Add(item.Colour, new ItemSceneData
				{
					Button = itemButton,
					GameObject = instantiatedGO,
				});
			}
		}

		private void OnDestroy()
		{
			foreach (var itemData in _items.Values)
			{
				itemData.Button.onClick.RemoveAllListeners();
			}
		}

		private async void Start()
		{
			_character.SetActive(false);
			
			var hasCharacter = await LoadCharacter();
			if (!hasCharacter)
			{
				await _contractHandler.MintCharacter();
				await _contractHandler.MintItems();
			}

			await CheckCharactersEquippedHatAndDisplay();
			await GetItemTokensBalanceAndUpdateShow();

			var characterID = await _contractHandler.GetCharacterTokenId();
			var equippedHatID = await _contractHandler.GetHat();
			UpdateUILogs("characterID: " + characterID + " / HatID: " + equippedHatID);

			var isApprovedForAll = await _contractHandler.CheckIfCharacterIsApprovedForAll();
			if (!isApprovedForAll)
			{
				await _contractHandler.ApproveAllForCharacter(true);
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
			}
			else
			{
				RemoveHatVisuals();
			}
		}

		private async UniTask EquipHat(string address)
		{
			await _contractHandler.ChangeHat(address);
			await CheckCharactersEquippedHatAndDisplay();
			await GetItemTokensBalanceAndUpdateShow();
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

		private async UniTask GetItemTokensBalanceAndUpdateShow()
		{
			for (var i = 0; i < _itemsDescriptions.Descriptions.Count; i++)
			{
				var addressTokenBalance =
					await _contractHandler.GetItemBalance(_itemsDescriptions.Descriptions[i].Address);
				_inventory.ShowInventoryItem(i, addressTokenBalance > 0);
			}
		}

		private void UpdateUILogs(string log)
		{
			_text.text += "\n" + log;
			Debug.Log(log);
		}
	}
}