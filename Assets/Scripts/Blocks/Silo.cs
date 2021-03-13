using System.Collections.Generic;
using UnityEngine;

public class Silo : MultiBlock
{
    public static Silo Instance { get; private set; }

    public ItemInventory Inventory;
    
    private Bay bay;
    
    public Silo(float x, float y, PathNode pathNode, Bay bay) : base(x, y, pathNode)
    {
        Instance = this;
        this.bay = bay;
        Inventory = new ItemInventory();
    }

    public override void setPathNode(PathNode pathNode)
    {
        throw new System.NotImplementedException();
    }

    public override bool isResource()
    {
        return false;
    }

    public override void destroy()
    {
        throw new System.NotImplementedException();
    }

    public override List<PathNode> getPathNodeList()
    {
        return new List<PathNode>
        {
            bay.getPathNode((int) baseX, (int) baseY),
            bay.getPathNode((int) baseX + 1, (int) baseY)
        };
    }

    public override string getSpritePath()
    {
        return "Assets/Rounded Blocks/stone.png";
    }

    public override Vector2 getDimensions()
    {
        return new Vector2(2, 1);
    }
}
