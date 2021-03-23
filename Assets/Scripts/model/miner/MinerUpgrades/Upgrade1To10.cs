public class Upgrade1 : MinerUpgrade
{
    public Upgrade1(Miner owner) : base(owner) { }
    public override UpgradeType getUpgradeType()
    {
        return UpgradeType.Damage;
    }

    public override operatorType getOperatorType()
    {
        return operatorType.Percent;
    }

    public override Item[] getBaseUpgradeCost()
    {
        return new Item[]
        {
            new StoneBlockItem(3)
        };
    }

    public override int getMinimumLvl()
    {
        return 2;
    }

    public override float getUpgradeImpact()
    {
        return 0.20f;
    }

    public override int getMaxAmount()
    {
        return 5;
    }

    public override string getName()
    {
        return "Stone Pickaxes";
    }

    public override string getDescription()
    {
        return "These pickaxes will be much better then these old dusty ones. Damage +20%";
    }

    public override string getSpritePath()
    {
        return "Assets/Addressables/Items/iceCave.png";
    }

    
}

public class Upgrade2 : MinerUpgrade
{
    public Upgrade2(Miner owner) : base(owner) { }
    public override UpgradeType getUpgradeType()
    {
        return UpgradeType.Speed;
    }

    public override operatorType getOperatorType()
    {
        return operatorType.Flat;
    }

    public override Item[] getBaseUpgradeCost()
    {
        return new Item[]
        {
            new DirtBlockItem(30),
            new StoneBlockItem(20)
        };
    }

    public override int getMinimumLvl()
    {
        return 3;
    }

    public override float getUpgradeImpact()
    {
        return 0.1f;
    }

    public override int getMaxAmount()
    {
        return 6;
    }

    public override string getName()
    {
        return "Better Boots";
    }

    public override string getDescription()
    {
        return "These boots were made for walkin', cuz thats just what they do. Speed +0.1 m/sec";
    }

    public override string getSpritePath()
    {
        return "Assets/Addressables/Items/iceCave.png";
    }

}