using UnityEngine;

public class Pickaxe : Tool
{
    public override Vector2[] getAdditionalMiningPos()
    {
        return new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
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
