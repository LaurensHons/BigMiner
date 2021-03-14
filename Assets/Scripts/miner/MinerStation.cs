using System;
using System.Collections;
using System.Collections.Generic;
using Grid;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class MinerStation : MonoBehaviour
{
    public GameObject minerPrefab;
    private GameObject miner;

    public GameObject BayGameObject;
    private Bay bay;

    private IMiningStrategy MiningStrategy = new RandomMiningStrategy();

    public int range = 2;
    
    void Start()
    {
        InstantiateMiner();
    }

    private void InstantiateMiner()
    {
        Vector3 pos = new Vector3(0, 1, 0);
        miner = Instantiate(minerPrefab, pos, Quaternion.identity);
        miner.gameObject.transform.SetParent(bay.gameObject.transform);
        Miner m = miner.GetComponent<Miner>();
        m.setMinerStation(this);
        bay.registerMiner(m);
    }

    public Block getNextTarget()
    {
        Block b = MiningStrategy.selectNextBlock(bay.getBlockList());
        return b;
    }

    public Grid<PathNode> getNodeGrid()
    {
        return bay.pathNodeGrid;
    }

    public Bay getBay()
    {
        return bay;
    }

    public void setBay(Bay bay)
    {
        this.bay = bay;
    }
    

    
}

public interface IMiningStrategy
{
     Block selectNextBlock(List<PathNode> blocks);
}

public class RandomMiningStrategy : IMiningStrategy
{
    public Block selectNextBlock(List<PathNode> pathNodeList)
    {
        if (pathNodeList.Count == 0)
        {
            Debug.Log("penisssssss");
            return null;
        }
        return pathNodeList[Random.Range(0, pathNodeList.Count)].structure as Block;
    }
}
