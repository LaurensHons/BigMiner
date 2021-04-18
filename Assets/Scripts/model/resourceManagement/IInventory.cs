
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public interface IInventory
{
    /// <summary>
    /// Add Item from nowhere to Inventory
    /// </summary>
    /// <param name="addItem"></param>
    /// Item to be added
    /// <param name="amount"></param>
    /// Amount of items to be added
    /// Default is all the items in <paramref name="addItem"/>
    /// <exception cref="InventoryException">Thrown if amount is less then 0 or exceeds amount of items in <paramref name="addItem"/></exception>
    public void AddItem(Item addItem, int? amount = null);
    
    /// <summary>
    /// Remove item from Inventory
    /// </summary>
    /// <param name="removeItem"></param>
    /// Item to be removed
    /// <param name="amount"></param>
    /// Amount of items to be removed
    /// Default is the amount of items in <paramref name="removeItem"/>
    /// /// <exception cref="InventoryException">Thrown if amount is less then 0 or exceeds amount of items in <paramref name="removeItem"/></exception>
    public void RemoveItem(Item removeItem, int? amount = null);
    
    /// <summary>
    /// Put Item in Inventory from other Inventory
    /// </summary>
    /// <param name="item"></param>
    /// Item to be moved to <paramref name="inventoryToAdd"></paramref>
    /// <param name="inventoryToAdd"></param>
    /// Inventory to add the Item
    /// <param name="amount"></param>
    /// Amount of items to be added
    /// Default is all the items in <paramref name="item"/>
    /// <exception cref="InventoryException">Thrown if amount is less then 0 or exceeds amount of items in <paramref name="item"/></exception>
    public void putItem(Item item, IInventory inventoryToAdd, int? amount = null);
    
    /// <summary>
    /// Take Item in inventory from other Inventory
    /// </summary>
    /// <param name="item"></param>
    /// Item to be taken from <paramref name="inventoryToSubtract"></paramref>
    /// <param name="inventoryToSubtract"></param>
    /// Inventory to take the Item from 
    /// <param name="amount"></param>
    /// Amount of items to be removed
    /// Default is the amount of items in <paramref name="item"/>
    /// /// <exception cref="InventoryException">Thrown if amount is less then 0 or exceeds amount of items in <paramref name="item"/></exception>
    public void TakeItem(Item item, IInventory inventoryToSubtract, int? amount = null);
    
    /// <summary>
    /// Looks if the Inventory contains an Item with the same class
    /// </summary>
    /// <param name="item"></param>
    /// Item class to be searched by
    /// <returns>Item (or null) in Inventory</returns>
    public Item TryGetItem(Item item);
    
    /// <summary>
    /// Returns an Action event that triggers on Inventory changes
    /// </summary>
    public Action InventoryChanged { get; set; }
    
    /// <summary>
    /// Dump the entire inventory in other inventory
    /// This is meant for miners when returning to silo
    /// </summary>
    /// <param name="receivingInventory"></param>
    public void DepositInventory(IInventory receivingInventory);
    
    /// <summary>
    /// Return List of Items
    /// </summary>
    public List<Item> getInventory();
    
    /// <summary>
    /// Checks if the Inventory is full
    /// </summary>
    public bool isFull();
    
    /// <summary>
    /// Checks if there are 0 items in the Inventory
    /// </summary>
    public bool isEmpty();
    
    /// <summary>
    /// Return amount of items in Inventory
    /// </summary>
    public int getInventoryWeight();
    
    /// <summary>
    /// Sets the max amount of items in the inventory
    /// </summary>
    public void setMaxInventoryWeight(int amount);
    
    /// <summary>
    /// Returns the maximum amount of items int the Inventory
    /// </summary>
    public int getMaxInventoryweight();

    /// <summary>
    /// Returns maxInventoryWeight - inventoryWeight
    /// </summary>
    public int getLeftOverInventorySpace();
}

public class ItemInventory : Inventory
{
    
}

public class JobCallInventory : Inventory
{
    private IJobCallStructure structure;
    public JobCallInventory(IJobCallStructure structure)
    {
        this.structure = structure;
    }
    public override void putItem(Item item, IInventory inventoryToAdd, int? amount = null)
    {
        base.putItem(item, inventoryToAdd, amount);
        JobController.Instance.successJobCall(structure, Item.CreateItem(item, amount.Value));
    }

    public override void TakeItem(Item item, IInventory inventoryToSubtract, int? amount = null)
    { 
        base.TakeItem(item, inventoryToSubtract, amount);
        JobController.Instance.MarkItemsUnderway(structure, item);
    }

    public override void DepositInventory(IInventory receivingInventory) => 
        throw new InventoryException("This operation is not permitted on JobCallInventories, You must deposit the items individually");
}



public abstract class Inventory : IInventory 
{
    private HashSet<Item> ItemInventory;

    private int inventoryWeight = 0;
    public Action InventoryChanged { get; set; }

    private int maxInventoryWeight;
    
    public Inventory(int maxInventoryWeight = Int32.MaxValue)
    {
        this.maxInventoryWeight = maxInventoryWeight;
        ItemInventory = new HashSet<Item>(new ItemComparator());
    }

    private void addInventoryWeight(int amount)
    {
        inventoryWeight += amount;
        if (inventoryWeight > maxInventoryWeight) throw new Exception("Miner Capacity Full");
        InventoryChanged?.Invoke();
    }

    public void setMaxInventoryWeight(int amount)
    {
        this.maxInventoryWeight = amount;
    }

    public Item TryGetItem(Item item)
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
        {
            itemInInventory.addAmount(amount);
        }
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
        Debug.Log("Taking item from inventory: amount = " + amount);
        itemInInventory.addAmount(-amount);
        //if (itemInInventory.getAmount() == 0) ItemInventory.Remove(itemInInventory);
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

    public virtual void TakeItem(Item item, IInventory inventoryToSubtract, int? amount = null)
    {
        Item itemToTake = inventoryToSubtract.TryGetItem(item);
        if (itemToTake == null) throw new InventoryException("Item to take not found in other inventory");
        AddItem(itemToTake, amount);
        inventoryToSubtract.RemoveItem(itemToTake, amount);
    }

    public virtual void putItem(Item item, IInventory inventoryToAdd, int? amount = null)
    {
        Item itemToPut = TryGetItem(item);
        if (itemToPut == null) throw new InventoryException("Item to put not found in own inventory");
        inventoryToAdd.AddItem(itemToPut, amount);
        RemoveItem(itemToPut, amount);
    }
    
    public virtual void DepositInventory(IInventory receivingInventory)
    {
        foreach (var item in ItemInventory)
        {
            receivingInventory.AddItem(item, null);
            RemoveItem(item, null);
        }
    }

    public  List<Item> getInventory()
    {
        return ItemInventory.ToList();
    }

    public bool isFull()
    {
        return maxInventoryWeight - inventoryWeight <= 0;
    }
    
    public bool isEmpty()
    {
        return inventoryWeight == 0;
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


    public int getLeftOverInventorySpace()
    {
        return maxInventoryWeight - inventoryWeight;
    }
}

public class InventoryException : Exception
{
    public InventoryException() { }
    public InventoryException(string message) : base(message) { }
    public InventoryException(string message, Exception inner) : base(message, inner) { }
}


