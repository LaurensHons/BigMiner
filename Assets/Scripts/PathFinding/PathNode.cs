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

    public bool isWalkable => Structure == null;
    public PathNode cameFromNode;

    private IStructure Structure;

    public IStructure structure
    {
        get
        {
            return Structure;
        }
    }

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
        if (structure == null) return;
        Structure = structure;

        List<PathNode> nodeList = structure.getPathNodeList();
        nodeList.Remove(this);
        foreach (var pathNode in nodeList)
        {
            pathNode.Structure = structure;
            //Debug.Log("Node: "+  pathNode.x + ", " + pathNode.y + " structure");
        }
    }

    public void MineBlock(float hit, out bool destroyed, out ItemInventory loot, out int xp)
    {
        destroyed = true;
        loot = new ItemInventory();
        xp = 0;
        if (structure != null && structure is Block)
        {
            Block b = (Block) structure;
            
            b.Mine(hit, out  destroyed);
            if (destroyed)
            {
                loot = b.getLoot();
                xp = b.getXpOnMine();
                Structure = null;
            }
        }
    }

    public bool isMineable()
    {
        if (structure == null) return false;
        return structure.isResource();
    }

    public override string ToString()
    {
        String structuretype = "";
        if (structure != null) structuretype = ", structure: " + structure.GetType();
        return "[" + x + "," + y + "]" + structuretype;
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