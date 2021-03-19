using System.Collections.Generic;

public abstract class MinerUpgrade
{
    public bool isBought = false;

    public void BuyUpgrade()
    {
        if (true)  //Inventory cost implementation
        {
            isBought = true;
        }
    }
    
    public abstract UpgradeType getUpgradeType();

    public abstract List<Item> getCost();

    public abstract int getMinimumLvl();

    public abstract float getUpgradeImpact();

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
