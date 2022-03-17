using System.Collections.Generic;
using MirageSDK.Demo.Data;
using UnityEngine;
using UnityEngine.UI;

namespace MirageSDK.Demo
{
	public class Inventory : MonoBehaviour
	{
		[SerializeField] private GameObject _inventoryButtonRoot;
		[SerializeField] private GameObject _buttonPrefab;

		private readonly List<GameObject> _itemList = new List<GameObject>();

		private void Start()
		{
			HideInventoryItems();
		}

		private void HideInventoryItems()
		{
			foreach (var item in _itemList)
			{
				item.SetActive(false);
			}
		}

		public Button AddItem(ItemDescription item)
		{
			var itemButtonGO = Instantiate(_buttonPrefab, _inventoryButtonRoot.transform, true);
			itemButtonGO.GetComponent<Image>().sprite = item.Icon;
			var itemButton = itemButtonGO.GetComponent<Button>();
			_itemList.Add(itemButtonGO);

			return itemButton;
		}

		public void ShowInventoryItem(int itemID, bool show)
		{
			_itemList[itemID].SetActive(show);
		}
	}
}