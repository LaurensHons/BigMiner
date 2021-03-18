using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public abstract class Item
{
    private int amount = 0;

    public Item(int amount)
    {
        if (amount >= 0) this.amount = amount;
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
        throw new System.NotImplementedException();
    }
}

public class StoneBlockItem : Item
{
    public StoneBlockItem(int amount) : base(amount) { }

    public override string getSpritePath()
    {
        throw new System.NotImplementedException();
    }
}