using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MirageSDK.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoScript : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _text;
	[SerializeField]
	private List<ItemScriptableObject> _items;
	
	[SerializeField]
	private List<Button> _inventoryButtons;
	[SerializeField]
	private GameObject _inventoryButtonRoot;

	[SerializeField]
	private GameObject _character;
	[SerializeField]
	private GameObject _characterHead;
	[SerializeField]
	private TMP_Text _characterID;

	[SerializeField]
	private Inventory _inventory;
	
	private DemoContractHandler _contractHandler;

	private void Awake()
	{
		foreach (var item in _items)
		{
			var itemButton = item._button;
			itemButton = Instantiate(itemButton, _inventoryButtonRoot.transform, true);
			itemButton.onClick.AddListener(()=>OnButtonClick(item));
			itemButton.GetComponent<Image>().sprite = item._icon;
			_inventoryButtons.Add(itemButton);
			_inventory._itemList.Add(itemButton.gameObject);
			
			item._button = itemButton;
		}
	}

	private void OnButtonClick(ItemScriptableObject item)
	{
		OnButtonClickAsync(item).Forget();
	}

	private async UniTask OnButtonClickAsync(ItemScriptableObject item)
	{
		await EquipHat(item);
	}

	private async void Start()
	{
		_contractHandler = GetComponent<DemoContractHandler>();
		_character.SetActive(false);
		var hasCharacter = await LoadCharacter();
		if (!hasCharacter)
		{
			await _contractHandler.MintCharacter();
			await _contractHandler.MintItems();
		}
		LoadHats();
		await CheckCharactersEquippedHatAndDisplay();
		await GetItemTokensBalanceAndUpdateShow();

		var characterID = await _contractHandler.GetCharacterTokenId();
		var equippedHatID = await _contractHandler.GetHat();
		UpdateUILogs("characterID: "+characterID+" / HatID: "+ equippedHatID);

		var isApprovedForAll = await _contractHandler.CheckIfCharacterIsApprovedForAll();
		if (!isApprovedForAll)
		{
			await _contractHandler.ApproveAllForCharacter(true);
		}
	}

	private void LoadHats()
	{
		foreach (var item in _items)
		{
			var itemsGameObject = item._gameObject;
			itemsGameObject = Instantiate(itemsGameObject, _characterHead.transform, false);
			itemsGameObject.SetActive(false);
			
			item._gameObject = itemsGameObject;
		}
	}

	private void OnDestroy()
	{
		foreach (var button in _inventoryButtons)
		{
			button.onClick.RemoveAllListeners();
		}
	}
	
	private async Task CheckCharactersEquippedHatAndDisplay()
	{
		var equippedHat = await _contractHandler.GetHat();
		Debug.LogWarning(equippedHat);
		switch (equippedHat)
		{
			case "0x10000000000000000000000000000000000000000000000000000000002":
				UpdateHatVisuals(HatColour.Red);
				break;
			case "0x10000000000000000000000000000000000000000000000000000000001":
				UpdateHatVisuals(HatColour.Blue);
				break;
			default:
				Debug.LogWarning(equippedHat);
				RemoveHatVisuals();
				break;
		}
	}

	private async Task EquipHat(ItemScriptableObject item)
	{
		await _contractHandler.ChangeHat(item._address);
		await CheckCharactersEquippedHatAndDisplay();
		//UpdateHatVisuals(item._colour);
		await GetItemTokensBalanceAndUpdateShow();
	}

	private void UpdateHatVisuals(HatColour hatColour)
	{
		foreach (var item in _items)
		{
			var isRightColour = hatColour == item._colour;
			item._gameObject.SetActive(isRightColour);
		}
	}
	
	private void RemoveHatVisuals()
	{
		foreach (var item in _items)
		{
			item._gameObject.SetActive(false);
		}
	}

	private async Task<bool> LoadCharacter()
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

	private async Task GetItemTokensBalanceAndUpdateShow()
	{
		for(var i = 0; i < _items.Count; i++)
		{
			var addressTokenBalance = await _contractHandler.GetItemBalance(_items[i]._address);
			_inventory.ShowInventoryItem(i, addressTokenBalance > 0);
		}
	}
	
	private void UpdateUILogs(string log)
	{
		_text.text += "\n" + log;
		Debug.Log(log);
	}
}