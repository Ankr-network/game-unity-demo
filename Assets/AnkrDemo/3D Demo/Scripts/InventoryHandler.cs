using System.Collections.Generic;
using System.Numerics;
using AnkrDemo.Data;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrDemo
{
	public class InventoryHandler : MonoBehaviour
	{
		[SerializeField] private GameObject _inventoryButtonRoot;
		[SerializeField] private GameObject _buttonPrefab;

		private readonly List<ItemButton> _itemList = new List<ItemButton>();

		private void Start()
		{
			HideInventoryItems();
		}

		private void HideInventoryItems()
		{
			foreach (var item in _itemList)
			{
				item.gameObject.SetActive(false);
			}
		}

		public Button AddItem(ItemDescription item)
		{
			var itemButtonGO = Instantiate(_buttonPrefab, _inventoryButtonRoot.transform, true);
			var itemButtonScript = itemButtonGO.GetComponent<ItemButton>();
			itemButtonScript.SetItemImageSprite(item.Icon);

			_itemList.Add(itemButtonScript);

			return itemButtonScript.Button;
		}

		public void ShowInventoryItem(int itemID, bool shouldShowItem, BigInteger balanceOfItem)
		{
			_itemList[itemID].gameObject.SetActive(shouldShowItem);

			if (shouldShowItem)
			{
				UpdateInventoryItemUIBalance(_itemList[itemID], balanceOfItem);
			}
		}

		public void EnableItemButtons(bool enable)
		{
			foreach (var itemButton in _itemList)
			{
				itemButton.GetComponent<Button>().interactable = enable;
			}
		}

		private void UpdateInventoryItemUIBalance(ItemButton itemButton, BigInteger balanceOfItem)
		{
			itemButton.SetItemBalanceText("X" + balanceOfItem);
		}
	}
}