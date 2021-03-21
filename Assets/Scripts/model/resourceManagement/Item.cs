using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class Item
{
    private int amount = 0;
    private Sprite sprite;

    public Item(int amount)
    {
        if (amount >= 0) this.amount = amount;
        AsyncOperationHandle<Sprite> ItemSpriteHandler = Addressables.LoadAssetAsync<Sprite>(getSpritePath());
        ItemSpriteHandler.Completed += LoadItemSpriteWhenReady; 
    }

    public static Item CreateItem(Item item, int amount)
    {
        return (Item) Activator.CreateInstance(item.GetType(), amount);
    }

    public void addAmount(int amount)
    {
        this.amount += amount;
    }

    public void setAmount(int amount)
    {
        this.amount = amount;
    }

   
    public int getAmount() { return amount; }

    public string getName() { return this.GetType().ToString(); }
    
    public Sprite getSprite()
    {
        return sprite;
    }

    private void LoadItemSpriteWhenReady(AsyncOperationHandle<Sprite> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite itemSprite = obj.Result;
            
            if (itemSprite == null) throw new Exception("No item sprite found");
            sprite = itemSprite;
        }
        else throw new Exception("Loading item sprite failed");
    }

    public abstract string getSpritePath();
}

public class ItemComparator : IEqualityComparer<Item>
{
    public bool Equals(Item? x, Item? y)
    {
        if (x == null || y == null) return false;
        return x.GetType() == y.GetType();
    }

    public int GetHashCode(Item obj)
    {
        return 0;
    }
}

public class DirtBlockItem : Item
{
    public DirtBlockItem(int amount) : base(amount) { }

    public override string getSpritePath()
    {
        return "Assets/Addressables/Blocks/DirtBlock.png";
    }
}

public class StoneBlockItem : Item
{
    public StoneBlockItem(int amount) : base(amount) { }

    public override string getSpritePath()
    {
        return "Assets/Addressables/Blocks/stoneW all.png";
    }
}