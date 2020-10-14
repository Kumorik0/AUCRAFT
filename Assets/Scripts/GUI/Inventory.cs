using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    #region Singleton

    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }

    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 32;

    public IDictionary<Item, int> inventory = new Dictionary<Item, int>();

    public void Add(Item item)
    {
        Debug.Log("not null");
        onItemChangedCallback.Invoke();
        int testAvailableSpace = 0;
        bool isInInventory = false;

        foreach (KeyValuePair<Item, int> entry in inventory)
        {
            testAvailableSpace += entry.Value;
            Debug.Log("poo");
        }

        if (testAvailableSpace >= space)
        {
            Debug.Log("Not enough room.");
        }

        // Check if item is in the dictionary
        if (inventory.ContainsKey(item))
        {
            inventory[item] = inventory[item] + 1;
            Debug.Log("Contains Key Item");
        }
        else
        {
            inventory.Add(item, 1);
            Debug.Log("Adding New Item");

        }
    }
        // TODO: incorporate using/dropping items
    public void Remove(Item item)
    {
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
}
