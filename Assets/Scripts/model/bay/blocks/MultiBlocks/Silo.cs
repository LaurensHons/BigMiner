using System;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class Silo : MultiBlock, IJobCallStructure
{
    public static Silo Instance { get; private set; }

    public IInventory Inventory;
    
    public IInventory InputInventory => Inventory;
    public IInventory OutputInventory  => Inventory;

    private void Start()
    {
        Instance = this;
        Inventory = new JobCallInventory(this);
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

    public override string getSpritePath()
    {
        return "Assets/Addressables/Blocks/stone.png";
    }

    public override void onClick()
    {
        GameObject.FindWithTag("UIController").GetComponent<UIController>().OpenSiloMenu();
    }

    public override Vector2 getDimensions()
    {
        return new Vector2(2, 2);
    }

    

    public void deliverJobCall(Item itemToBeDelivered, IInventory minerInventory)
    {
        Inventory.TakeItem(itemToBeDelivered, minerInventory);
        JobController.Instance.successJobCall(this, itemToBeDelivered);
    }

    public void pickUpJobCall(Item itemToBeDelivered, IInventory minerInventory)
    {
        int amount = itemToBeDelivered.getAmount();
        if (amount > minerInventory.getLeftOverInventorySpace()) amount = minerInventory.getLeftOverInventorySpace();
        Inventory.putItem(itemToBeDelivered, minerInventory, amount);
        
    }

    public void addInventoryCall(Item item)
    {
        throw new NotImplementedException();
    }
}
