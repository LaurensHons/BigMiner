using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool
{
    public int XP = 0;
    private int xpThreshHold = 2;

    public bool isSelected = false;

    public EventHandler ToolXpUpdate;
    public EventHandler ToolLevelUpdate;

    public float damage
    {
        get => getBaseDamage() + (getBaseDamage() * damageUpgrades * 0.1f);
    }

    private int DamageUpgrades = 0;
    public int damageUpgrades
    {
        get => DamageUpgrades;
        set
        {
            DamageUpgrades = value;
            ToolDamageUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
    public EventHandler ToolDamageUpdate;
    
    public int xp
    {
        get
        {
            return XP;
        }
        set
        {
            int previousLevel = getLevel(out double p);
            XP = value;
            ToolXpUpdate?.Invoke(this, EventArgs.Empty);
            if (getLevel(out  p) > previousLevel)
                ToolLevelUpdate?.Invoke(this, EventArgs.Empty);
        }
    }

    public int getSpeed()
    {
        return (int) Math.Round(Math.Pow(0.98f, getLevel(out double d) - 1) * (1/Time.fixedDeltaTime));
    }
    
    public int getLevel(out double percentLeft)
    {
        double level =  0.5 + Math.Sqrt(1f + 8f * (xp) / (xpThreshHold)) / 2f;
        percentLeft = level % 1;
        return (int) level;
    }

    public List<Vector2> getAdditionalMiningPos(Vector2 dir)
    {
        List<Vector2> returnList = new List<Vector2>();
        foreach (var v2 in getSwingArea())
        {
            if (v2.x > 0)
                returnList.Add(new Vector2(-dir.y * Math.Abs(v2.x), dir.x * Math.Abs(v2.x)));       //left of block
            if (v2.x < 0)
                returnList.Add(new Vector2(dir.y * Math.Abs(v2.x), -dir.x * Math.Abs(v2.x)));       //right of block
            if (v2.y > 0)
                returnList.Add(new Vector2(dir.x * Math.Abs(v2.y), dir.y * Math.Abs(v2.y)));        //behind block
        }

        return returnList;
    }

    public abstract List<Vector2> getSwingArea();
    
    /*
     *  Swingarea must be returned in a specific way
     *  new List with 0 - 3 Vector 2's  
     *
     *                          [0, How many blocks above]
     *  [-How many blocks left, 0]          BLOCK       [How many blocks right, 0]
     *                                      Miner
     * 
     */

    public Item[] getUpgradeCost()
    {
        List<Item> returnList = new List<Item>();
        foreach (var item in getBaseUpgradeCost())
        {
            item.addAmount((int) Math.Floor(item.getAmount() * 0.2 * DamageUpgrades));
            returnList.Add(item);
        }
        Debug.Log("returnList: " + returnList[0]);
        Debug.Log("baseUpgradeCost: " + getBaseUpgradeCost().Length);
        return returnList.ToArray();
    }
    
    public abstract Item[] getBaseUpgradeCost();

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj.GetType() == this.GetType())
        {
            Tool t = (Tool) obj;
            if (getBaseDamage() == t.getBaseDamage() && XP == t.XP) return true;
            else return false;
        }
        else return false;
    }

    public Sprite getSprite()
    {
        switch (getSpriteName())
        {
            default: return null;
            case("Hammer"): return ItemAssets.Instance.HammerSprite;
            case("Pickaxe"): return ItemAssets.Instance.PickaxeSprite;
        }
    }

    public abstract string getSpriteName();
    public abstract float getBaseDamage();
    public abstract string getDecriptionText();
    
}
