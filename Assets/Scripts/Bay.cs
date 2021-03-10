using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Grid;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bay : MonoBehaviour, BayTickObserver
{
    public static int gridSize = 5;

    public int GridSize
    {
        get => gridSize;
        set => gridSize = value;
    }

    

    public float blockScale = .19f;

    public float beltSpeed = 0.5f;
    
    public GameObject blockPrefab;
    
    public Grid<PathNode> pathNodeGrid { get; private set; }
    

    void Start()
    {
        pathNodeGrid = new Grid<PathNode>(gridSize, gridSize, 1f, Vector3.zero,
            (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
        updateGrid();


        GenerateBay();

        
    }

    private void testPathfinding()
    {
        Pathfinding pathfinding = new Pathfinding(5, 5);
        Vector3 randomPos = new Vector3(Random.Range(0, 5), Random.Range(0, 5));
        Debug.Log("Pathing to : " + randomPos.ToString());
        pathfinding.GetGrid().GetXY(randomPos, out int x, out int y);
        List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
        
    }

    public void updateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (pathNodeGrid.GetGridObject(x, y).block != null)
                    pathNodeGrid.GetGridObject(new Vector3(x, y)).SetIsWalkable(true);
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
                if (randomint > 1)
                    continue;
                if (x == 0 || y == 0)
                    continue;
                Block block = new Block(x, y, blockPrefab);
                pathNodeGrid.GetGridObject(x, y).SetBlock(block);
            }
        }
    }

    void Update()
    {
        
    }
    
    public void bayTick()
    {
        updateGrid();
    }

    public List<PathNode> getBlockList()
    {
        List<PathNode> returnList = new List<PathNode>();
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                PathNode b = pathNodeGrid.GetGridObject(x, y);
                if (pathNodeGrid.GetGridObject(x, y).block != null)
                    returnList.Add(b);
            }
        }

        return returnList;
    }

    public void DeleteBlock(int x, int y)
    {
        PathNode endNode = pathNodeGrid.GetGridObject(x, y);
        endNode.DeleteBlock();
    }

    public void DeleteBlock(Block block)
    {
        PathNode endNode = pathNodeGrid.GetGridObject(block.transform.position);
        endNode.DeleteBlock();
    }
    

    
}
