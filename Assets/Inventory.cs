using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _itemList;
    
    // Start is called before the first frame update
    void Start()
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
        switch (itemID)
        {
            case 0:
                _itemList[0].SetActive(show);
                break;
            case 1:
                _itemList[1].SetActive(show);
                break;
        }
    }
}
