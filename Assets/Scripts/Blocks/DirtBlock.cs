using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class DirtBlock : Block
{
    public DirtBlock(float x, float y, PathNode pathNode) : base(x, y, pathNode) { }

    public override int getMaxHealth()
    {
        return 3;
    }

    public override ItemInventory getLoot()
    {
        ItemInventory iteminv = new ItemInventory();
        iteminv.addItemToInventory(new DirtBlockItem(1), 1, out int actualAmount);
        return iteminv;
    }

    public override int getXpOnMine()
    {
        return 1;
    }

    public override string getSpritePath()
    {
        return "Assets/Images/DirtBlock.png";
    }

    public override int getSearchCost()
    {
        return (int) BlockTypeSearchCost.DirtBlock;
    }
}
