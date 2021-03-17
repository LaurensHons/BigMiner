using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : Tool
{

    public override List<Vector2> getSwingArea()
    {
        return new List<Vector2>();
    }

    public override string getSpritePath()
    {
        throw new System.NotImplementedException();
    }

    public override float baseDamage()
    {
        return 1;
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
