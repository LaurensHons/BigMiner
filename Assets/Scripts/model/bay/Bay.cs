using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Blocks;
using Grid;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bay : MonoBehaviour
{
    private List<Miner> minerList = new List<Miner>();
    private List<IStructure> structures = new List<IStructure>();
    private List<Processor> processors = new List<Processor>();

    public GameObject DirtBlockPrefab;
    public GameObject StoneBlockPrefab;


    public GameObject SiloPrefab;
    public GameObject MinerStationPrefab;
    public GameObject SawPrefab;
    
    

    public EventHandler UpdateObservers;
    public void FixedUpdate()
    {
        UpdateObservers?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 gridSize
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
        pathNodeGrid = new Grid<PathNode>((int) gridSize.x, (int) gridSize.y, 1f, Vector3.zero,
            (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));

        Vector2 siloPos = new Vector2(0, 0);
        
        GameObject siloGameObject = Instantiate(SiloPrefab);
        IStructure silo = siloGameObject.GetComponent<Silo>();
        silo.InstantiateBlock(siloPos.x, siloPos.y);
        addStructureToGrid(silo);

        new JobController();       // its yellow but leave it, its fine

        Vector2 SawPos = new Vector2(0, 2);
        GameObject sawGameObject = Instantiate(SawPrefab);
        Saw saw = sawGameObject.GetComponent<Saw>();
        saw.InstantiateBlock(SawPos.x, SawPos.y, 2, Processor.Tier.Bronze);
        addProcessorToGrid(saw);
        
        
        
        
        Vector2 minerStationPos = new Vector2(2, 0);
        GameObject minerStationGameObject = Instantiate(MinerStationPrefab);
        MinerStation minerStation = minerStationGameObject.GetComponent<MinerStation>();
        minerStation.InstantiateBlock(minerStationPos.x, minerStationPos.y);
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

    public void clearBay()
    {
        foreach (var structure in structures.ToArray())
        {
            if (structure.isResource())
            {
                structure.destroy();
                removeBlock(structure as Block);
            }
        }
    }

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
                if (Vector3.Distance(miner.transform.position, pos) < 1.01) occupied = true;
            }
            

            if (!occupied)
            {
                Block block;
                switch (BlockType)
                {
                    case BlockTypes.DirtBlock:
                    {
                        GameObject blockGameObject = Instantiate(DirtBlockPrefab);
                        block = blockGameObject.GetComponent<DirtBlock>();
                        block.InstantiateBlock(pos.x, pos.y);
                        break;
                    }
                    case BlockTypes.StoneBlock:
                    {
                        GameObject blockGameObject = Instantiate(StoneBlockPrefab);
                        block = blockGameObject.GetComponent<StoneBlock>();
                        block.InstantiateBlock(pos.x, pos.y);
                        break;
                    }
                    default: return;
                }
                addStructureToGrid(block);
                block.setParent(transform);
                structures.Add(pathNode.structure);
            }
        }
    }

    public bool canPlaceStructure(IStructure structure, Vector2 pos)
    {
        if (structure.getPathNodeList().Count <= 1)
        {
            PathNode pathNode = structure.getPathNodeList()[0];
            if (pathNode.structure != null && pathNode.structure != structure) return false;
            return true;
        }

        MultiBlock multiBlock = (MultiBlock) structure;
        
        float width = multiBlock.getDimensions().x, height = multiBlock.getDimensions().y;

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

    public void addProcessorToGrid(Processor processor)
    {
        processors.Add(processor);
        addStructureToGrid(processor);
    }
    
    public void addStructureToGrid(IStructure structure)
    {
        
        if (!canPlaceStructure(structure, structure.getPathNodeList()[0].getPos()))
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
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
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
        if (!structures.Remove(b)) 
            Debug.LogError("Could not find block to remove");
    }

    public List<PathNode> getBlockList()
    {
        List<PathNode> returnList = new List<PathNode>();

        foreach (var structure in structures)
        {
           if (structure.isResource() && !structure.isDestroyed()) returnList.AddRange(structure.getPathNodeList());
        }

        return returnList;
    }

    private List<PathNode> getFreeNodes()
    {
        List<PathNode> returnList = new List<PathNode>();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = (int) (gridSize.y / 2f); y < gridSize.y; y++)
            {
                PathNode b = pathNodeGrid.GetGridObject(x, y);
                if (b.isWalkable)
                    returnList.Add(b);
            }
        }

        return returnList;
    }

    public List<Processor> getProcessors()
    {
        return processors;
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

    public List<IStructure> getStructureList()
    {
        return structures;
    }
}
