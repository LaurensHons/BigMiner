using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory
{
    public Dictionary<ItemType, int> Inventory = new Dictionary<ItemType, int>();
    private int maxCapacity = Int32.MaxValue;
    private int usedCapacity = 0;
    
    public ItemInventory(int maxCapacity = Int32.MaxValue)
    {
        this.maxCapacity = maxCapacity;
        foreach (var itemtype in Enum.GetNames(typeof(ItemType)))
        {
            ItemType.TryParse(itemtype, out ItemType itemType);
            Inventory.Add(itemType, 0);
        }
    }

    public bool safePushInventory(ItemInventory itemInventory)
    {
        bool succes = true;
        foreach (var itemtype in Inventory)
        {
            int spaceleft = itemInventory.maxCapacity - itemInventory.usedCapacity;
            float toomuch = (itemtype.Value * getItemTypeSize(itemtype.Key)) % spaceleft;
            float itemSizesGoingOut = itemtype.Value * getItemTypeSize(itemtype.Key) - toomuch;
            float itemsGoingOut = itemSizesGoingOut / itemtype.Value;
            
            itemInventory.Inventory[itemtype.Key] += (int) Math.Floor(itemsGoingOut);
            itemInventory.usedCapacity += (int) itemsGoingOut;
            
            if (itemsGoingOut != itemtype.Value)
            {
                Debug.Log("youre a dumbass");
                return false;
            }
        }

        return true;
    }
    
    public void PullInventory(ItemInventory itemInventory) 
    {
        foreach (var itemtype in itemInventory.Inventory)
        {
            int spaceleft = maxCapacity - usedCapacity;
            float toomuch = (itemtype.Value * getItemTypeSize(itemtype.Key)) % spaceleft;
            float itemSizesGoingOut = itemtype.Value * getItemTypeSize(itemtype.Key) - toomuch;
            float itemsGoingOut = itemSizesGoingOut / itemtype.Value;
            Inventory[itemtype.Key] += (int) Math.Floor(itemsGoingOut);
        }
    }

    public bool addToInventory(ItemType itemType, int amount)
    {
        if (getItemTypeSize(itemType) * amount > maxCapacity - usedCapacity) return false;
        Inventory[itemType] += amount;
        return true;
    }
    
    public void removeFromInventory(ItemType itemType, int amount)
    {
        Inventory[itemType] -= amount;
    }

    private float getItemTypeSize(ItemType itemType)
    {
        switch (itemType)
        {
            case(ItemType.DirtBlockItem): return 1;
            
            default: return 1;
        }
    }

    public int getUsedCapacity()
    {
        return usedCapacity;
    }

    public int getMaxCapacity()
    {
        return maxCapacity;
    }

}

public enum ItemType{
    DirtBlockItem
}

