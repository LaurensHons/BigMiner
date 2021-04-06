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

    public override Item[] getBaseUpgradeCost()
    {
        return new Item[]
        {
            new DirtBlockItem(10),
            new StoneBlockItem(10)
        };
    }

    public override string getSpriteName()
    {
        return "Hammer";
    }

    public override float getBaseDamage()
    {
        return 2;
    }

    public override string getDecriptionText()
    {
        return "Its Hammer Time" +
               "\nMines block in front and right" +
               "\nBase Damage: 2";
    }
}