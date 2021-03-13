/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */


using System;
using System.Collections.Generic;
using Grid;
using UnityEngine;

public class PathNode {

    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable
    {
        get
        {
            return structure == null;
        }
    }
    public PathNode cameFromNode;

    public IStructure structure;

    public PathNode(Grid<PathNode> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }
    

    public void SetStructure(IStructure structure)
    {
        this.structure = structure;
        List<PathNode> nodeList = structure.getPathNodeList();
        nodeList.Remove(this);
        foreach (var pathNode in nodeList)
        {
            pathNode.structure = structure;
        }
    }

    public void MineBlock(int hit, out bool returnDestroyed, out ItemInventory loot)
    {
        bool destroyed = true;
        loot = new ItemInventory();
        if (structure != null && structure is Block)
        {
            Block b = (Block) structure;
            
            b.Mine(hit, out  destroyed);
            if (destroyed)
            {
                structure = null;
                
                loot.addToInventory(ItemType.DirtBlockItem, 1);
            }
        }
        returnDestroyed = destroyed;
    }

    public bool isMineable()
    {
        return structure.isResource();
    }

    public override string ToString() {
        return x + "," + y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        PathNode pathNode = (PathNode) obj;
        if (pathNode.x == x && pathNode.y == y) return true;
        else return false;
        
    }

    public Vector3 getPos()
    {
        return new Vector3(x, y, 0);
    }
}