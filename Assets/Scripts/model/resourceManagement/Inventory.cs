
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class Inventory 
{
    private HashSet<Item> ItemInventory;

    private int inventoryWeight = 0;
    public Action InventoryUpdate;

    private int maxInventoryWeight;
    
    public Inventory(int maxInventoryWeight = Int32.MaxValue)
    {
        this.maxInventoryWeight = maxInventoryWeight;
        ItemInventory = new HashSet<Item>(new ItemComparator());
    }

    private void addInventoryWeight(int amount)
    {
        inventoryWeight += amount;
        InventoryUpdate?.Invoke();
    }

    private Item TryGetItem(Item item)
    {
        foreach (var ItemInInventory in ItemInventory)
        {
            if (ItemInInventory.GetType() == item.GetType())
                return ItemInInventory;
        }
        return null;
    }

    private void AddToInventoryList(Item item, int amount)
    {
        Item itemInInventory = TryGetItem(item);
        
        if (itemInInventory != null)
            itemInInventory.addAmount(amount);
        else
        {
            ItemInventory.Add(Item.CreateItem(item, amount));
            //Debug.Log("Added new item, amount: " + amount);
        }
        addInventoryWeight(amount);

    }

    private void RemoveItemFromInventoryList(Item item, int amount)
    {
        Item itemInInventory = TryGetItem(item);
        if (itemInInventory == null) throw new InventoryException("Item not found");
        
        itemInInventory.addAmount(-amount);
        addInventoryWeight(-amount);
    }
    
    public void AddItem(Item addItem, int? amount = null)
    {
        if (addItem == null) throw new NullReferenceException();
        amount ??= addItem.getAmount();
        
        int leftOverSpace = maxInventoryWeight - inventoryWeight;
        if (leftOverSpace <= 0) throw new InventoryException("Full");
        
        if (amount > leftOverSpace) amount = leftOverSpace;
        
        //Debug.Log("Adding: " + addItem.GetType() + ", amount " + amount);
        AddToInventoryList(addItem, (int) amount);
    }

    public void RemoveItem(Item removeItem, int? amount = null)
    {
        if (removeItem == null) throw new NullReferenceException();
        Item itemInInv = TryGetItem(removeItem);
        if (itemInInv == null) throw new InventoryException("Item not found: equal=" + removeItem.GetType());
        amount ??= itemInInv.getAmount();

        if (amount > itemInInv.getAmount()) throw new InventoryException("Too much");
        RemoveItemFromInventoryList(itemInInv, (int) amount);
    }

    public void TakeItem(Item item, Inventory inventoryToSubtract, int? amount = null)
    {
        Item itemToTake = TryGetItem(item);
        if (itemToTake == null) throw new InventoryException("Item not found");
        //Debug.Log(itemToTake.GetType() + ", amount: " + amount);
        if (itemToTake == null) throw new Exception("Item to be taken not found");

        AddItem(itemToTake, amount);
        inventoryToSubtract.RemoveItem(itemToTake, amount);
    }
    
    public void DepositInventory(Inventory receivingInventory)
    {
        foreach (var item in ItemInventory)
        {
            receivingInventory.AddItem(item, null);
            RemoveItem(item, null);
        }
    }

    public List<Item> getInventory()
    {
        return ItemInventory.ToList();
    }

    public bool isFull()
    {
        return maxInventoryWeight - inventoryWeight <= 0;
    }
    public int getInventoryWeight()
    {
        return inventoryWeight;
    }

    public int getMaxInventoryweight()
    {
        return maxInventoryWeight;
    }

    public override string ToString()
    {
        String outstring = "Inventory, weight: " + getInventoryWeight();
        foreach (var item in ItemInventory)
        {
            outstring += "\nItem " + item.GetType() + ", amount: " + item.getAmount();
        }

        return outstring;
    }
}

public class InventoryException : Exception
{
    public InventoryException() { }
    public InventoryException(string message) : base(message) { }
    public InventoryException(string message, Exception inner) : base(message, inner) { }
}


