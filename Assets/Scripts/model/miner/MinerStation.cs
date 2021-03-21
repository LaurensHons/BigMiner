using System;
using System.Collections;
using System.Collections.Generic;
using Grid;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class MinerStation : MultiBlock
{
    public Miner Miner { get; private set; }
    
    private IMiningStrategy miningStrategy = new RandomMiningStrategy();

    private MinerStationClickController ClickController;

    public MinerStation(float x, float y, Bay bay) : base(x, y, bay)
    {
        InstantiateMiner();

        BoxCollider2D bc = BlockObject.AddComponent<BoxCollider2D>();
        bc.size = Vector2.one * (1/GameController.getBlockScale());
        ClickController = BlockObject.AddComponent<MinerStationClickController>();
        ClickController.minerstation = this;
    }
    
    private void InstantiateMiner()
    {
        Vector2 pos = new Vector2(0, 1);
        Miner = new Miner(pos, this);
        bay.registerMiner(Miner);
    }

    public Block getNextTarget()
    {
        String s = "";
        foreach (var pathNode in bay.getBlockList())
        {
            s += pathNode.ToString() + "; ";
        }
        
        Debug.Log(s);
        
        if (Miner == null) throw new Exception("No Miner Found");
        switch (Miner.miningStrategy)
        {
            case MiningStrategy.Random:
                return new RandomMiningStrategy().selectNextBlock(bay.getBlockList(), Miner.getTransform().position);
            case MiningStrategy.Closest:
                return new ClosestMiningStrategy().selectNextBlock(bay.getBlockList(), Miner.getTransform().position);
            case MiningStrategy.MinValue:
                return new LowestMiningStrategy().selectNextBlock(bay.getBlockList(), Miner.getTransform().position); 
            case MiningStrategy.MaxValue:
                return new HighestMiningStrategy().selectNextBlock(bay.getBlockList(), Miner.getTransform().position);
        }

        throw new Exception("Very weird");
        return null;
    }

    public Grid<PathNode> getNodeGrid()
    {
        return bay.pathNodeGrid;
    }

    public Bay getBay()
    {
        return bay;
    }

    public override string getSpritePath()
    {
        return "Assets/Addressables/Blocks/shardRock.png";
    }

    public override Vector2 getDimensions()
    {
        return new Vector2(2, 1);
    }

    public override bool isResource()
    {
        return false;
    }

    public override void destroy()
    {
        throw new NotImplementedException();
    }

    public override PathNode getInterfaceNode()
    {
        return bay.getPathNode((int) baseX, (int) baseY);
    }
}

public enum MiningStrategy
{
    Random,
    Closest,
    MinValue,
    MaxValue,
}

public interface IMiningStrategy
{
     Block selectNextBlock(List<PathNode> blocks, Vector2 minerPos);
}

public class RandomMiningStrategy : IMiningStrategy
{
    public Block selectNextBlock(List<PathNode> pathNodeList, Vector2 minerPos)
    {
        if (pathNodeList.Count == 0)
        {
            return null;
        }
        return pathNodeList[Random.Range(0, pathNodeList.Count)].structure as Block;
    }
}

public class ClosestMiningStrategy : IMiningStrategy
{
    public Block selectNextBlock(List<PathNode> pathNodeList, Vector2 minerPos)
    {
        if (pathNodeList.Count == 0)
        {
            return null;
        }

        PathNode lowestPathnode = pathNodeList[0];
        foreach (var pathNode in pathNodeList)
        {
            if (!pathNode.isMineable())
                continue;
            if (Vector2.Distance(lowestPathnode.getPos(), minerPos) > Vector2.Distance(pathNode.getPos(), minerPos))
                lowestPathnode = pathNode;
        }

        return lowestPathnode.structure as Block;
    }
}

public class LowestMiningStrategy : IMiningStrategy
{
    public Block selectNextBlock(List<PathNode> pathNodeList, Vector2 minerPos)
    {
        if (pathNodeList.Count == 0)
        {
            return null;
        }
        
        PathNode lowestPathnode = pathNodeList[0];
        foreach (var pathNode in pathNodeList)
        {
            if (!pathNode.isMineable())
                continue;
            if (((Block) lowestPathnode.structure).getMaxHealth() > ((Block) pathNode.structure).getMaxHealth() )
                lowestPathnode = pathNode;
        }

        return lowestPathnode.structure as Block;
    }
}

public class HighestMiningStrategy : IMiningStrategy
{
    public Block selectNextBlock(List<PathNode> pathNodeList, Vector2 minerPos)
    {
        if (pathNodeList.Count == 0)
        {
            return null;
        }

        PathNode highestPathnode = pathNodeList[0];
        foreach (var pathNode in pathNodeList)
        {
            if (!pathNode.isMineable())
                continue;
            if (((Block) highestPathnode.structure).getMaxHealth() < ((Block) pathNode.structure).getMaxHealth())
                highestPathnode = pathNode;
        }

        return highestPathnode.structure as Block;
    }
}