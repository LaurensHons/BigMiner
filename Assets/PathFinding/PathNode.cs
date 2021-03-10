/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Grid;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class PathNode {

    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public PathNode cameFromNode;

    public Block block;

    public PathNode(Grid<PathNode> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetBlock(Block block)
    {
        this.block = block;
        SetIsWalkable(false);
    }

    public void MineBlock(int hit, out bool returnDestroyed)
    {
        bool destroyed = true;
        if (block != null)
        {
            block.Mine(hit, out  destroyed);
            if (destroyed)
            {
                block = null;
                SetIsWalkable(true);
            }
        }
        returnDestroyed = destroyed;
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
}