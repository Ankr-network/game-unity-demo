using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public List<GameObject> _itemList;

	// Start is called before the first frame update
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

	public void ShowInventoryItem(int itemID, bool show)
	{
		_itemList[itemID].SetActive(show);
	}
}