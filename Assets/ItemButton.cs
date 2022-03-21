using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _itemBalanceText;

	private Button _itemButton;
	private Image _itemImage;

	private void Awake()
	{
		if (!_itemBalanceText)
		{
			_itemBalanceText = GetComponentInChildren<TMP_Text>();
		}

		_itemImage = GetComponent<Image>();
		_itemButton = GetComponent<Button>();
	}

	public Button GetItemButton()
	{
		return _itemButton;
	}

	public void SetItemBalanceText(string text)
	{
		_itemBalanceText.text = text;
	}

	public void SetItemImageSprite(Sprite sprite)
	{
		_itemImage.sprite = sprite;
	}
}