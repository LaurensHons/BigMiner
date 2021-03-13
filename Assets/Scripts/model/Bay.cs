using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Blocks;
using Grid;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bay : MonoBehaviour
{
    public GameObject GameCanvas;

    private List<Miner> minerList = new List<Miner>();
    
    public int gridSize
    {
        get { return GameController.getGridSize(); }
        set { GameController.setGridSize(value);}
    }
    public float blockScale
    {
        get { return GameController.getBlockScale(); }
        set { GameController.setBlockScale(value);}
    }
    

    
    
    public Grid<PathNode> pathNodeGrid { get; private set; }
    

    void Start()
    {
        pathNodeGrid = new Grid<PathNode>(gridSize, gridSize, 1f, Vector3.zero,
            (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));

        Vector2 siloPos = new Vector2(0, 0);
        new Silo(siloPos.x, siloPos.y, getPathNode(siloPos), this);
        pathNodeGrid.GetGridObject(siloPos).SetStructure(Silo.Instance);
        
        //GenerateBay();
    }

    private void testPathfinding()
    {
        Pathfinding pathfinding = new Pathfinding(5, 5);
        Vector3 randomPos = new Vector3(Random.Range(0, 5), Random.Range(0, 5));
        Debug.Log("Pathing to : " + randomPos.ToString());
        pathfinding.GetGrid().GetXY(randomPos, out int x, out int y);
        List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
        
    }
    
    /*
    Function made to update walkable status for occupied nodes,
    seems it works fine without tho

    public void updateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (pathNodeGrid.GetGridObject(x, y).structure != null)
                    pathNodeGrid.GetGridObject(new Vector3(x, y)).SetIsWalkable(true);
            }
            
        }
    }
    
    */

    public void spawnBlockType(BlockTypes BlockType)
    {
        Debug.Log("Spawning Block:" + BlockType.ToString());
        List<PathNode> freeNodes = getFreeNodes();
        if (freeNodes.Count == 0) return;
        Vector3 pos = freeNodes[Random.Range(0, freeNodes.Count)].getPos();
        PathNode pathNode = pathNodeGrid.GetGridObject(pos);
        if (pathNode.isWalkable)
        {
            bool occupied = false;
            foreach (var miner in minerList)
            {
                if (Vector3.Distance(miner.transform.position, pos) < 1.01 ) occupied = true;
            }
            

            if (!occupied)
            {
                Block block;
                switch (BlockType)
                {
                    case BlockTypes.DirtBlock:
                    {
                        block = new DirtBlock(pos.x, pos.y, pathNode);
                        break;
                    }
                    case BlockTypes.StoneBlock:
                    {
                        block = new StoneBlock(pos.x, pos.y, pathNode);
                        break;
                    }
                    default:
                    {
                        block = new DirtBlock(pos.x, pos.y, pathNode);
                        break;
                    }
                }
                block.setParent(GameCanvas.transform);
                pathNodeGrid.GetGridObject((int) pos.x, (int) pos.y).SetStructure(block);
            }
        }
    }

    
    

    public void GenerateBay()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                int randomint = Random.Range(0, 10);
                if (randomint > 3)
                    continue;
                if (x == 0 || y == 0)
                    continue;
                Block block = new StoneBlock(x, y, pathNodeGrid.GetGridObject(x, y));
                pathNodeGrid.GetGridObject(x, y).SetStructure(block);
                //block.setParent(GameCanvas.transform);
            }
        }
    }

    void Update()
    {
        
    }

    public List<PathNode> getBlockList()
    {
        List<PathNode> returnList = new List<PathNode>();
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                PathNode b = pathNodeGrid.GetGridObject(x, y);
                if (!pathNodeGrid.GetGridObject(x, y).isWalkable)
                    returnList.Add(b);
            }
        }

        return returnList;
    }

    private List<PathNode> getFreeNodes()
    {
        List<PathNode> returnList = new List<PathNode>();
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                PathNode b = pathNodeGrid.GetGridObject(x, y);
                if (b.isWalkable)
                    returnList.Add(b);
            }
        }

        return returnList;
    }

    public PathNode getPathNode(int x, int y)
    {
        return pathNodeGrid.GetGridObject(x, y);
    }

    public PathNode getPathNode(Vector2 pos)
    {
        return pathNodeGrid.GetGridObject((int) pos.x, (int) pos.y);
    }

    public void registerMiner(Miner miner)
    {
        minerList.Add(miner);
    }
    
    

    
}
