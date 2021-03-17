using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool
{
    public int xp = 0;
    private int xpThreshHold = 20;

    public int damage = 1;
    
    public bool isUnlocked => getMinimumLvl() <= getLevel(out double d);

    public int getSpeed()
    {
        return (int) Math.Round(Math.Pow(0.98f, getLevel(out double d)) * (1/Time.fixedDeltaTime));
    }
    
    public int getLevel(out double percentLeft)
    {
        double level =  0.5 + Math.Sqrt(1 + 8 * (xp) / (xpThreshHold)) / 2;
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
     *                       [0, How many blocks above]
     *  [-How many blocks left]          BLOCK       [How many blocks right, 0]
     *                                   Miner
     * 
     */
    
    
    public abstract string getSpritePath();
    
    public abstract float baseDamage();
    public abstract int getMinimumLvl();

    public abstract string getDecriptionText();
}
