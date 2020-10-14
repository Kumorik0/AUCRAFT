using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;

    public Inventory inventory;

    public InventorySlot[] slots;

    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        if (Inventory.instance != null)
        {
            Debug.Log("inventory is instantiated");
        }

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    void UpdateUI()
    {
        int numberOfTypeItems = inventory.inventory.Count;

        int tempIndex = 0; 

        foreach (KeyValuePair<Item, int> itemType in inventory.inventory)
        {
            slots[tempIndex].AddItem(itemType.Key);
            tempIndex += 1;
            Debug.Log("Adding 1 Item");
        }
        for (int i = tempIndex; i < inventory.space; i++)
        {
            slots[i].ClearSlot();
            Debug.Log("Clearing Slots");
        }
    }
}
