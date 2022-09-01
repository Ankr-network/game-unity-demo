using System.Collections.Generic;
using System.Numerics;
using AnkrDemo.Data;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace AnkrDemo
{
	public class Inventory : MonoBehaviour
	{
		[SerializeField] private GameObject _inventoryButtonRoot;
		[SerializeField] private GameObject _buttonPrefab;
		[SerializeField] private GameObject _uiGameObject;
		[SerializeField] private Button _showUIButton;
		[SerializeField] private Button _closeUIButton;

		private readonly List<ItemButton> _itemList = new List<ItemButton>();

		private void Start()
		{
			HideInventoryUI();
			_showUIButton.onClick.AddListener(OnShowUIButtonClicked);
			_closeUIButton.onClick.AddListener(OnCloseUIButtonClicked);
		}

		private void OnDestroy()
		{
			_showUIButton.onClick.RemoveListener(OnShowUIButtonClicked);
			_closeUIButton.onClick.RemoveListener(OnCloseUIButtonClicked);
		}

		private void OnShowUIButtonClicked()
		{
			_uiGameObject.SetActive(true);
			_showUIButton.interactable = false;
		}
		
		private void OnCloseUIButtonClicked()
		{
			_uiGameObject.SetActive(false);
			_showUIButton.interactable = true;
		}

		private void HideInventoryUI()
		{
			_uiGameObject.SetActive(false);
		}

		public Button AddItem(ItemDescription item)
		{
			var itemButtonGO = Instantiate(_buttonPrefab, _inventoryButtonRoot.transform, true);
			itemButtonGO.transform.localScale = Vector3.one;
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