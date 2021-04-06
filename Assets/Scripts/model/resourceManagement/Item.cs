using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public abstract class Item
{
    private int amount = 0;
    public Action ItemUpdate;
    private static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    private Sprite sprite;

    public Item(int amount)
    {
        if (amount >= 0)
            this.amount = amount;
    }

    public static Item CreateItem(Item item, int amount)
    {
        return (Item) Activator.CreateInstance(item.GetType(), amount);
    }

    public void addAmount(int amount)
    {
        this.amount += amount;
        ItemUpdate?.Invoke();
    }

    public void setAmount(int amount)
    {
        this.amount = amount;
        ItemUpdate?.Invoke();
    }

    public static Item operator +(Item a, Item b)
    {
        if (a.GetType() != b.GetType()) throw new ArgumentException("Cannot add 2 items of different type");
        a.addAmount(b.amount);
        return a;
    }

    public static Item operator *(Item a, int b)
    {
        a.amount *= b;
        return a;
    }


    public int getAmount() { return amount; }

    public string getName() { return this.GetType().ToString(); }
    

    public Sprite GetSprite()
    {
        switch (getSpriteName())
        {
            default: return null;
            case "DirtBlock":  return ItemAssets.Instance.DirtBlockSprite;
            case "StoneBlock": return ItemAssets.Instance.StoneBlockSprite;
        }
    }
    
    public abstract string getSpriteName();
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

    public override string getSpriteName()
    {
        return "DirtBlock";
    }
}

public class StoneBlockItem : Item
{
    public StoneBlockItem(int amount) : base(amount) { }

    public override string getSpriteName()
    {
        return "StoneBlock";
    }
}