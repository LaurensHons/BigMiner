using System.Collections.Generic;
using UnityEngine;

public class Silo : MultiBlock
{
    public static Silo Instance { get; private set; }

    public Inventory Inventory;

    public Silo(float x, float y, Bay bay) : base(x, y, bay)
    {
        Instance = this;
        this.bay = bay;
        Inventory = new Inventory();
    }
    

    public override bool isResource()
    {
        return false;
    }

    public override void destroy()
    {
        throw new System.NotImplementedException();
    }

    public override PathNode getInterfaceNode()
    {
        return bay.getPathNode((int) baseX, (int) baseY);
    }
    
    public override string getSpritePath()
    {
        return "Assets/Addressables/Blocks/stone.png";
    }

    public override Vector2 getDimensions()
    {
        return new Vector2(2, 1);
    }
}
