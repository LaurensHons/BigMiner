using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class DirtBlock : Block
{
    public DirtBlock(float x, float y) : base(x, y) { }

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
}
