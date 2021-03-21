using System;
using System.Collections.Generic;
using UnityEngine;

public class Silo : MultiBlock
{
    public static Silo Instance { get; private set; }

    public Inventory Inventory;

    public Silo(float x, float y, Bay bay) : base(x, y, bay)
    {
        Instance = this;
        Inventory = new Inventory();
        Inventory.AddItem(new DirtBlockItem(50));
    }

    public bool buyItem(Item[] items)
    {
        Debug.Log("Trying to buy something with cost: " + items);
        List<Item> inventoryItems = new List<Item>();
        foreach (var item in items)
        {
            var inventoryItem = Inventory.TryGetItem(item);
            if (inventoryItem != null && inventoryItem.getAmount() >= item.getAmount())
            {
                inventoryItems.Add(inventoryItem);
            }
            else
            {
                Debug.Log("Not enought Items to buy, Cancelling");
                return false;
            }
        }

        if (items.Length != inventoryItems.Count) throw new Exception("This error should not happen");

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            inventoryItems[i] .addAmount(-items[i].getAmount());
        }
        Debug.Log("Successfully bought something!");
        return true;
    }

    public override bool isResource()
    {
        return false;
    }

    public override void destroy()
    {
        throw new System.NotImplementedException();
    }

    public override PathNode getInterfaceNode()
    {
        return bay.getPathNode((int) baseX, (int) baseY);
    }
    
    public override string getSpritePath()
    {
        return "Assets/Addressables/Blocks/stone.png";
    }

    public override Vector2 getDimensions()
    {
        return new Vector2(2, 1);
    }
}
