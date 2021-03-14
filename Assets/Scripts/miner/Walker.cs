using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Walker
{
    private IWalker objectToWalk;
    private WalkerStatus walkerStatus = WalkerStatus.DoingNothing;
    public List<Vector3> pathVectorList = new List<Vector3>();
    public int currentPathIndex;

    private float speed = 0.01f;

    public IStructure targetStructure;

    public event EventHandler ActionEnd;

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

        setStatusCollectingBlocks(this, EventArgs.Empty);
    }

    public void Update(bool wakeup)
    {
        switch (walkerStatus)
        {
            case (WalkerStatus.CollectingBlocks):
            {
                HandleMovement();
                break;
            }
            case (WalkerStatus.GoingToTargetStructure):
            {
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

    public void setStatusGoingToTargetBlock(object sender, EventArgs args)
    {
        Debug.Log("Miner: Going to TargetStructure");
        walkerStatus = WalkerStatus.GoingToTargetStructure;
        SetTargetPosition(targetStructure, true);
        ActionEnd = setStatusDepositingItems;
    }

    public void setStatusCollectingBlocks(object sender, EventArgs args)
    {
        if (checkIfInventoryFull())
        {
            StopAction();
            return;
        }
        Debug.Log("Miner: Collecting blocks");
        walkerStatus = WalkerStatus.CollectingBlocks;
        findNextTarget();
        ActionEnd = setStatusMining;
    }
        
    public void setStatusMining(object sender, EventArgs args)
    {
        if (checkIfInventoryFull())
        {
            StopAction();
            return;
        }
        Debug.Log("Miner: Mining Block");
        objectToWalk.Mine();
        walkerStatus = WalkerStatus.Mining;
        ActionEnd = setStatusCollectingBlocks;
        
    }

    public void setStatusDepositingItems(object sender, EventArgs args)
    {
        bool closeEnough = false;
        foreach (var pathNode in targetStructure.getPathNodeList())
        {
            if (Vector2.Distance(transform.position, pathNode.getPos()) < 1.1f) closeEnough = true;
        }
        if (!closeEnough)
        {
            Debug.Log("You're too far mate");
            ActionEnd = setStatusGoingToTargetBlock;
            StopAction();
            return;
        }
        Debug.Log("Miner: Depositing Items");
        walkerStatus = WalkerStatus.TradingItems;
        objectToWalk.startDepositingItems();
        ActionEnd = setStatusCollectingBlocks;
    }

    private bool checkNextNode()
    {
        if (pathVectorList == null || pathVectorList.Count == 0)
        {
            return false;
        }
        //Debug.Log("pathVectorList "+ pathVectorList.Count +" index " + currentPathIndex);
        PathNode pathNode = objectToWalk.getBay().getPathNode(pathVectorList[currentPathIndex]);
        if (!pathNode.isWalkable)
        {
            targetStructure = pathNode.structure;
            ActionEnd = setStatusMining;
            return true;
        }
        return false;
    }

    private bool checkIfInventoryFull()
    {
        if (objectToWalk.getItemInventory().isFull())
        {
            targetStructure = Silo.Instance;
            ActionEnd = setStatusGoingToTargetBlock;
            return true;
        }

        return false;
    }


    private void findNextTarget()
    {
        //Debug.Log("Finding new target");
        targetStructure = objectToWalk.getNextTarget();

        if (targetStructure != null)
        {
            SetTargetPosition(targetStructure);
        }
        else
        {
            walkerStatus = WalkerStatus.DoingNothing;
        }
    }
    private void SetTargetPosition(IStructure targetStructure, bool forced = false)
    {
        //Debug.Log("Started pathfinding from " + GetPosition().ToString() + " to " + targetPosition.ToString());
        currentPathIndex = 0;

        List<Vector3> fastestPath = null;
        foreach (var targetNode in targetStructure.getPathNodeList())
        {
            List<Vector3> vectorList = Pathfinding.Instance.FindPath(new Vector3((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y), 0), targetNode.getPos());
            if (vectorList != null && (fastestPath == null || vectorList.Count <= fastestPath.Count))
            {
                fastestPath = vectorList;
            }
        }

        pathVectorList = fastestPath;
        
        if (pathVectorList == null && forced)
            pathVectorList = Pathfinding.Instance.FindPath(new Vector3((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y), 0), targetStructure.getPos(), true);

        if (pathVectorList == null)
        {
            String outstring;
            outstring = forced ? "No Forced " : "No ";

            Debug.Log(outstring + "path found from [" + Math.Round(transform.position.x) + "," + Math.Round(transform.position.y) + "] to [" + 
                      targetStructure.getPos().x +", " + targetStructure.getPos().y + "]");
            findNextTarget();
            return;
        }
        for (int i = 0; i < pathVectorList.Count - 1; i++)
        {
            Debug.DrawLine(pathVectorList[i], pathVectorList[i + 1], Color.white, pathVectorList.Count);
        }
    }
    
    protected void HandleMovement() {
        
        if (checkNextNode())
        {
            StopAction();
            return;
        }

        if (pathVectorList != null && pathVectorList.Count != 0) {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            
            if (Vector3.Distance(transform.position, targetPosition) > 0.01f){
                //Vector3 moveDir = (targetPosition - transform.position).normalized;

                //float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                //animatedWalker.SetMoveVector(moveDir);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
            } else {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count) {
                    Debug.Log("Path ended");
                    StopAction();
                    //animatedWalker.SetMoveVector(Vector3.zero);
                }
            }
        }
    }
    
    public void StopAction()
    {
        
        ActionEnd?.Invoke(this,EventArgs.Empty);
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

    public void Mine();

    public ItemInventory getItemInventory();

    public void startDepositingItems();

    public Bay getBay();
}

public enum WalkerStatus
{
    CollectingBlocks,
    GoingToTargetStructure,
    Mining,
    TradingItems,
    DoingNothing
}


