using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : Tool
{

    public override List<Vector2> getSwingArea()
    {
        return new List<Vector2>();
    }

    public override string getSpriteName()
    {
        return "Pickaxe";
    }

    public override float getBaseDamage()
    {
        return 1;
    }

    public override string getDecriptionText()
    {
        return "Trusty ol' pickaxe" +
               "\nMines one block in front" +
               "\nBase Damage: 1";
    }

    public override Item[] getBaseUpgradeCost()
    {
        return new Item[]
        {
            new DirtBlockItem(10)
        };
    }
}
