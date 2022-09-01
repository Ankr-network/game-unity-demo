using System.Collections.Generic;
using UnityEngine;

namespace AnkrDemo.Data
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemScriptableObject", order = 1)]
	public class ItemDescriptionsScriptableObject : ScriptableObject
	{
		[SerializeField] private List<ItemDescription> _descriptions;

		public List<ItemDescription> Descriptions => _descriptions;
	}
}
