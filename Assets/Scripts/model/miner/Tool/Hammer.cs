using System.Collections.Generic;
using UnityEngine;

public class Hammer : Tool
{

    public override List<Vector2> getSwingArea()
    {
        return new List<Vector2>()
        {
            new Vector2(1, 0)
        };
    }

    public override string getSpritePath()
    {
        return "Assets/Images/Miner.png";
    }

    public override float getBaseDamage()
    {
        return 2;
    }

    public override int getMinimumLvl()
    {
        return 0;
    }

    public override string getDecriptionText()
    {
        return "Trusty ol' pickaxe" +
               "\nMines one block in front" +
               "\nBase Damage: 1";
    }
}