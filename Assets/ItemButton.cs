using TMPro;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
	public TMP_Text _itemBalanceText;

	private void Start()
	{
		if (!_itemBalanceText)
		{
			_itemBalanceText = GetComponentInChildren<TMP_Text>();
		}
	}
}