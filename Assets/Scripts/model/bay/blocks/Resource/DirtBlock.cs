using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class DirtBlock : Block 
{
    public override int getMaxHealth()
    {
        return 3;
    }

    public override Inventory getLoot()
    {
        Inventory iteminv = new Inventory();
        iteminv.AddItem(new DirtBlockItem(1), null);
        return iteminv;
    }

    public override int getXpOnMine()
    {
        return 1;
    }

    public override string getSpritePath()
    {
        return "Assets/Addressables/Blocks/DirtBlock.png";
    }

    public override int getSearchCost()
    {
        return (int) BlockTypeSearchCost.DirtBlock;
    }
}
