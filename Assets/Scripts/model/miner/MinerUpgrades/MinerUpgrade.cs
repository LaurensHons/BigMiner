using System;
using System.Collections.Generic;

public abstract class MinerUpgrade
{
    public EventHandler UpgradeUpdate;
    
    private int amount = 0;
    private int maxAmount;

    public void BuyUpgrade()
    {
        if (amount >= getMaxAmount()) return;
        if (Silo.Instance.buyItem(getUpgradeCost()))  
        {
            amount++;
            UpgradeUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public Item[] getUpgradeCost()
    {
        List<Item> returnList = new List<Item>();
        foreach (var item in getBaseUpgradeCost())
        {
            item.addAmount((int) Math.Floor(item.getAmount() * 0.2 * amount));
            returnList.Add(item);
        }
        return returnList.ToArray();
    }


    public int getAmount() { return amount; }
    public static int CompareByMinimumLvl(MinerUpgrade upgrade1, MinerUpgrade upgrade2)
    {
        return upgrade1.getMinimumLvl() - upgrade2.getMinimumLvl();
    }

    public abstract UpgradeType getUpgradeType();
    public abstract operatorType getOperatorType();
    public abstract Item[] getBaseUpgradeCost();
    public abstract int getMinimumLvl();
    public abstract float getUpgradeImpact();
    public abstract int getMaxAmount();
    public abstract string getName();
    public abstract string getDescription();
    public abstract string getSpritePath();
}

public enum UpgradeType
{
    Speed,
    Damage,
    Battery,
    InventorySize,
    MiningStrategy,
}

public enum operatorType
{
    Percent,
    Flat,
}
