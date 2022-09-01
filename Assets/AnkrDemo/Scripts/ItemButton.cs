using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnkrDemo
{
	public class ItemButton : MonoBehaviour
	{
		[SerializeField] private TMP_Text _itemBalanceText;
		[SerializeField] private Button _itemButton;
		[SerializeField] private Image _itemImage;

		public Button Button => _itemButton;

		public void SetItemBalanceText(string text)
		{
			_itemBalanceText.text = text;
		}

		public void SetItemImageSprite(Sprite sprite)
		{
			_itemImage.sprite = sprite;
		}
	}
}