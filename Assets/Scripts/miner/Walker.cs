using System;
using System.Collections.Generic;
using UnityEngine;

public class Walker
{
    private IWalker objectToWalk;
    private WalkerStatus walkerStatus = WalkerStatus.DoingNothing;
    public List<Vector3> pathVectorList = new List<Vector3>();
    public int currentPathIndex;

    private float speed = 0.01f;

    public IStructure targetBlock;
    
    

    private Transform transform
    {
        get
        {
            return objectToWalk.getTransform();
        }
    }

    public Walker(IWalker objectToWalk)
    {
        this.objectToWalk = objectToWalk;
    }

    public void Update(bool wakeup)
    {
        bool target;
        if (targetBlock == null)
            target = false;
        else
            target = true;
        
        Debug.Log("WalkerStatus: " + walkerStatus + ", targetBlock:" + target);
        switch (walkerStatus)
        {
            case (WalkerStatus.CollectingBlocks):
            {
                if (NextNodeIsOccupied(out Block Block)) findNextTarget();
                if(targetBlock == null) findNextTarget();
                HandleMovement();
                break;
            }
            case (WalkerStatus.GoingToSilo):
            {
                if (NextNodeIsOccupied(out Block block))
                {
                    targetBlock = block;
                    walkerStatus = WalkerStatus.Mining;
                }

                if (targetBlock == null || !targetBlock.getPos().Equals(Silo.Instance.getPos()))
                {
                    targetBlock = Silo.Instance;
                    SetTargetPosition(Silo.Instance.getPos());
                }
                if (pathVectorList == null) walkerStatus = WalkerStatus.TradingItems;
                HandleMovement();
                break;
            }
            case (WalkerStatus.DoingNothing):
            {
                if (wakeup)
                {
                    walkerStatus = WalkerStatus.CollectingBlocks;
                    findNextTarget();
                }
                break;
            }
        }
    }

    public WalkerStatus getWalkerStatus() { return walkerStatus; }

    public void setStatusGoingToSilo() { walkerStatus = WalkerStatus.GoingToSilo; }

    public void setStatusCollectingBlocks()
    {
        walkerStatus = WalkerStatus.CollectingBlocks;
        findNextTarget();
    }
        
    public void setWalkerStatusMining(Block block)
    {
        targetBlock = block;
        walkerStatus = WalkerStatus.Mining;
    }

    private bool NextNodeIsOccupied(out Block block)
    {
        block = null;
        if (pathVectorList == null || pathVectorList.Count == 0)
        {
            return false;
        }
        //Debug.Log("pathVectorList "+ pathVectorList.Count +" index " + currentPathIndex);
        PathNode pathNode = objectToWalk.getBay().getPathNode(pathVectorList[currentPathIndex]);
        block = pathNode.structure as Block;
        return !pathNode.isWalkable;
    }


    private void findNextTarget()
    {
        //Debug.Log("Finding new target");
        targetBlock = objectToWalk.getNextTarget();

        if (targetBlock != null)
        {
            SetTargetPosition(targetBlock.getPos());
        }
        else
        {
            // DoNothing = true;
            Debug.Log("Miner is doing nothing");
        }
    }
    private void SetTargetPosition(Vector3 targetPosition)
    {
        //Debug.Log("Started pathfinding from " + GetPosition().ToString() + " to " + targetPosition.ToString());
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(new Vector3((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y), 0), targetPosition);
        
        for (int i = 0; i < pathVectorList.Count - 1; i++)
        {
            Debug.DrawLine(pathVectorList[i], pathVectorList[i + 1], Color.white, pathVectorList.Count);
        }
    }
    
    protected void HandleMovement() {
        if (pathVectorList != null && pathVectorList.Count != 0) {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            
            if (Vector3.Distance(transform.position, targetPosition) > 0.01f){
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                //animatedWalker.SetMoveVector(moveDir);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
            } else {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count) {
                    StopMoving();
                    //animatedWalker.SetMoveVector(Vector3.zero);
                }
            }
        }
        else StopMoving();
    }
    
    private void StopMoving()
    {
        if (walkerStatus == WalkerStatus.CollectingBlocks && Vector2.Distance(objectToWalk.getTransform().position, targetBlock.getPos()) < 1.2f)
            walkerStatus = WalkerStatus.Mining;
        if (walkerStatus == WalkerStatus.GoingToSilo)
            walkerStatus = WalkerStatus.TradingItems;
        pathVectorList = null;
    }


    public bool hasEmptyPathNodeList()
    {
        return pathVectorList == null || pathVectorList.Count == 0;
    }
}

public interface IWalker
{
    public Transform getTransform();
    public Block getNextTarget();

    public Bay getBay();
}

public enum WalkerStatus
{
    CollectingBlocks,
    GoingToSilo,
    Mining,
    TradingItems,
    DoingNothing
}


