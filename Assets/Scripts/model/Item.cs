using System;

public class Item
{
    private int amount;

    public Item(int amount)
    {
        if (amount >= 0) this.amount = amount;
    }

    public int getAmount() { return amount; }

    public string getName() { return this.GetType().ToString(); }

    public bool AddItem(Item itemToSubtract, int amount)
    {
        if (!itemToSubtract.GetType().Equals(this.GetType())) return false;
        if (itemToSubtract.amount < amount) amount = itemToSubtract.amount;
        this.amount += amount;
        itemToSubtract.amount -= amount;
        return true;
    }

    public void AddAllItems(Item itemToSubstract)
    {
        this.amount += itemToSubstract.amount;
        itemToSubstract.amount = 0;
    }

    public string getSpritePath()
    {
        throw new NotImplementedException();
    }
}

public class DirtBlockItem : Item
{
    public DirtBlockItem(int amount) : base(amount) { }

    public string getSpritePath()
    {
        throw new System.NotImplementedException();
    }
}
