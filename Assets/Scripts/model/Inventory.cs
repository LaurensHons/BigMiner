using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class ItemInventory
{
    public List<Item> Inventory = new List<Item>();
    private int maxCapacity = Int32.MaxValue;
    private int usedCapacity = 0;
    
    public ItemInventory(int maxCapacity = Int32.MaxValue)
    {
        this.maxCapacity = maxCapacity;
    }

    public void PushInventory(ItemInventory receivingItemInventory)
    {
       foreach (var Item in Inventory)
       {
           receivingItemInventory.addAllItemToInventory(Item, out int acutalAmount);
           usedCapacity -= acutalAmount;
       }
    }
    
    public void PullInventory(ItemInventory sendingItemInventory) 
    {
        foreach (var Item in sendingItemInventory.Inventory)
        {
            addAllItemToInventory(Item, out int actualAmount);
            sendingItemInventory.usedCapacity -= actualAmount;
        }
    }

    public bool safeAddItemToInventory(Item item, int amount)
    {
        if (usedCapacity + item.getAmount() > maxCapacity)
            return false;
        bool succes = addItemToInventory(item, amount, out int acutalAmount);
        if (succes && amount != acutalAmount) throw new Exception("This should not happen");
        return succes;
    }

    public bool addItemToInventory(Item item, int amount, out int actualAmount)
    {
       
        actualAmount  = amount;
        if (usedCapacity >= maxCapacity)
        {
            actualAmount = 0;
            return false;
        }
        
        if (usedCapacity + item.getAmount() > maxCapacity)
            actualAmount = maxCapacity - usedCapacity;
        Item localItem = getItem(item.GetType());
        if (localItem == null)
        {
            Item emptyItem = (Item) Activator.CreateInstance(item.GetType(), 0);
            emptyItem.AddItem(item, actualAmount);
            Inventory.Add(emptyItem);
            usedCapacity += actualAmount;
            Debug.Log("added new type, amount: " + actualAmount);
            return true;
        }
        
        return getItem(item.GetType()).AddItem(item, actualAmount);
    }
    
    public void addAllItemToInventory(Item item, out int actualAmount)
    {
        addItemToInventory(item, item.getAmount(), out actualAmount);
    }
    
    
    public Item getItem(Type itemType)
    {
        foreach (var item in Inventory)
        {
            if (item.GetType().Equals(itemType.GetType()))
                return item;
        }
        return null;
    }
    

    public float getUsedCapacity()
    {
        return usedCapacity;
    }

    public float getMaxCapacity()
    {
        return maxCapacity;
    }

    public bool isFull()
    {
        return usedCapacity >= maxCapacity;
    }

    
}

