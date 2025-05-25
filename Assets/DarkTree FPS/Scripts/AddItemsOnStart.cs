using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTreeFPS;
using DTInventory;

public class AddItemsOnStart : MonoBehaviour
{
    public List<Item> items;

    bool spawned = false;

    void Update()
    {
        if (items == null || spawned)
        {
            return;
        }

        var Inventory = FindFirstObjectByType<DTInventory.DTInventory>();

        foreach (var item in items)
        {
            var _instance = Instantiate(item);
            Inventory.AddItem(_instance);
        }

        spawned = true;
    }
}