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

    public override void addMaterial()
    {
        
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
