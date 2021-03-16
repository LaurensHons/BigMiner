using System;
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

    public abstract Vector2[] getAdditionalMiningPos();

    public abstract string getSpritePath();
    
    public abstract float baseDamage();
    public abstract int getMinimumLvl();

    public abstract string getDecriptionText();
}
