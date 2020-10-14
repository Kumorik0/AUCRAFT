using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{

    void Start()
    {

    }

    public Item item;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("Picking up " + item.name);

            Destroy(gameObject);
        }
    }
}
