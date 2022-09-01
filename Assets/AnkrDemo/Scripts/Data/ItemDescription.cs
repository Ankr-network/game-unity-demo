using System;
using UnityEngine;

namespace AnkrDemo.Data
{
	[Serializable]
	public class ItemDescription
	{
		[SerializeField] private Sprite _icon;
		[SerializeField] private GameObject _gameObjectPrefab;
		[SerializeField] private HatColour _colour;
		[SerializeField] private string _address;

		public Sprite Icon => _icon;

		public GameObject GameObjectPrefab => _gameObjectPrefab;

		public HatColour Colour => _colour;

		public string Address => _address;
	}
}