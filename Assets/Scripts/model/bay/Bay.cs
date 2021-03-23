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
    private List<Miner> minerList = new List<Miner>();
    private List<IStructure> structures = new List<IStructure>();

    public EventHandler UpdateObservers;
    public void FixedUpdate()
    {
        UpdateObservers?.Invoke(this, EventArgs.Empty);
    }

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
        pathNodeGrid = new Grid<PathNode>(gridSize, gridSize * 2, 1f, Vector3.zero,
            (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));

        Vector2 siloPos = new Vector2(0, 0);
        new Silo(siloPos.x, siloPos.y, this);
        addStructureToGrid(Silo.Instance);

        Vector2 minerStationPos = new Vector2(2, 0);
        MinerStation minerStation = new MinerStation(minerStationPos.x, minerStationPos.y, this);
        addStructureToGrid(minerStation);

        /*
        Vector2 secondminerStationPos = new Vector2(0, 4);
        MinerStation secondminerStation = new MinerStation(secondminerStationPos.x, secondminerStationPos.y, this);
        pathNodeGrid.GetGridObject(secondminerStationPos).SetStructure(secondminerStation);
        */

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
        //Debug.Log("Spawning Block:" + BlockType.ToString());
        List<PathNode> freeNodes = getFreeNodes();
        if (freeNodes.Count == 0) return;
        Vector3 pos = freeNodes[Random.Range(0, freeNodes.Count)].getPos();
        PathNode pathNode = pathNodeGrid.GetGridObject(pos);
        if (pathNode.isWalkable && pathNode.structure == null)
        {
            bool occupied = false;
            foreach (var miner in minerList)
            {
                if (Vector3.Distance(miner.getTransform().position, pos) < 1.01) occupied = true;
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
                block.setParent(transform);
                structures.Add(pathNode.structure);
            }
        }
    }

    public bool canPlaceStructure(MultiBlock structure, Vector2 pos)
    {
        float width = structure.getDimensions().x, height = structure.getDimensions().y;

        List<PathNode> pathNodes = new List<PathNode>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0 ; y < height; y++)
            {
                pathNodes.Add(getPathNode((int) pos.x + x, (int) pos.y + y));
            }
        }
        
        
        foreach (var pathNode in pathNodes)
        {
            if (pathNode != null &&pathNode.structure != null && pathNode.structure != structure) return false;
        }

        return true;
    }
    
    public void addStructureToGrid(MultiBlock structure)
    {
        
        if (!canPlaceStructure(structure, structure.getPos()))
        {
            Debug.Log("Cannot add structure, pathNode is occupied");
            return;
        }
        foreach (var pathNode in structure.getPathNodeList())
        {
            pathNode.SetStructure(structure);
            structures.Add(structure);
            Debug.Log("Setting Structure: " + pathNode.x + ", " + pathNode.y);
        }
    }

    public void updateStructure(MultiBlock structure)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                PathNode b = pathNodeGrid.GetGridObject(x, y);
                if (b.structure == structure)
                    b.removeStructure();
            }
        }
        addStructureToGrid(structure);
    }

    public void removeBlock(Block b)
    {
        if (structures.Contains(b))
            structures.Remove(b);
        else throw new Exception("Could not find block to remove");
    }

    public List<PathNode> getBlockList()
    {
        List<PathNode> returnList = new List<PathNode>();
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                PathNode b = pathNodeGrid.GetGridObject(x, y);
                if (!b.isWalkable && b.structure.isResource())
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
            for (int y = gridSize / 2; y < gridSize * 2; y++)
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
        UpdateObservers += miner.FixedUpdate;
    }

    public List<IStructure> getStructureList()
    {
        return structures;
    }

    
}
