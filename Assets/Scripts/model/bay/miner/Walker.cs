using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class Walker
{
    private IWalker objectToWalk;
    private WalkerStatus walkerStatus = WalkerStatus.CollectingBlocks;
    private IEnumerator sleeping;
    public List<Vector3> pathVectorList = new List<Vector3>();
    public int currentPathIndex;
    public bool checkForInventoryCalls = true;
    public float speed;

    public IStructure targetStructure;

    private Transform transform
    {
        get
        {
            return objectToWalk.getTransform();
        }
    }

    private JobCall activeJobCall;

    public Walker(IWalker objectToWalk, float speed)
    {
        this.objectToWalk = objectToWalk;
        this.speed = speed;

        walkerStatus = WalkerStatus.CollectingBlocks;
        
        
    }

    private bool activated = false;
    public void Update(bool wakeup)
    {
        if (targetStructure != null)
            activated = true;
        if (wakeup && !activated)
        {
            DecideNextAction();
        }
        if (objectToWalk.isBatteryZero())
        {
            Debug.Log("BatteryZero");
            return;
        }
        HandleMovement();
    }

    public WalkerStatus getWalkerStatus() { return walkerStatus; }

    public void DecideNextAction()
    {
        Debug.Log("Deciding next action");
        switch (walkerStatus)
        {
            case WalkerStatus.CollectingBlocks:
            {
                if (objectToWalk.getItemInventory().isFull())
                {
                    depositBlocksInSilo();
                    return;
                }

                if (targetStructure == null || targetStructure.isDestroyed())
                {
                    findNextTarget();
                    return;
                }

                if (isStructureCloseEnough(targetStructure))
                {
                    Debug.Log("mining");
                    objectToWalk.Mine();
                    return;
                }
                else
                {
                    Debug.Log("not close enough to mine");
                }

                break;
            }

            case WalkerStatus.InventoryCalls:
            {
                Inventory inventory = objectToWalk.getItemInventory();
                Debug.Log("Status: Inventory Call");
        
                if (activeJobCall == null)
                {
                    JobCall jobCall = JobController.Instance.getNextJobCall();
                    if (jobCall == null)
                    {
                        Debug.Log("Found no Inventory calls");
                        walkerStatus = WalkerStatus.CollectingBlocks;
                        DecideNextAction();
                        return;
                    }
            
                    activeJobCall = jobCall;
            
                    if (inventory.TryGetItem(jobCall.itemToBeDelivered) == null)
                    {
                        targetStructure = Silo.Instance;
                    }
                    else
                    {
                        targetStructure = activeJobCall.targetStructure;
                    } 
                }
        
                if (inventory.TryGetItem(activeJobCall.itemToBeDelivered) == null)
                {
                    if (!inventory.isEmpty())
                    {
                        depositBlocksInSilo();
                        return;
                    }

                    TakeItems();
                    return;
                }

                DepositItems();
                break;
            }
        }
    }

    private IEnumerator Sleep()
    {
        yield return new WaitForSeconds(1);
        DecideNextAction();
        sleeping = null;
    }

    public void depositBlocksInSilo()
    {
        if (!isStructureCloseEnough(Silo.Instance))
        {
            Debug.Log("You're too far to deposit items mate");
            targetStructure = Silo.Instance;
            return;
        }
        
        Debug.Log("Miner: Depositing Items");
        objectToWalk.startDepositingItems();
    }
    
    public void DepositItems()
    {
        if (!isStructureCloseEnough(activeJobCall.targetStructure))
        {
            Debug.Log("You're too far mate");
            targetStructure = activeJobCall.targetStructure;
        }
        else
        {
            Debug.Log("Miner: Depositing Items");
            objectToWalk.startDepositingItems();
        }
    }

    public void TakeItems()
    {
        if (!isStructureCloseEnough(activeJobCall.originStructure))
        {
            Debug.Log("You're too far mate");
            if (targetStructure == null)
                Debug.Log("wtf bruh in the walker ln 230");
            else targetStructure = activeJobCall.originStructure;
        }
        else
        {
            Debug.Log("Miner: Taking Items");
            objectToWalk.startTakingItems();
        }
    }

    private bool isStructureCloseEnough(IStructure structure)
    {
        if (structure == null || structure.isDestroyed()) return false;
        bool closeEnough = false;
        foreach (var pathNode in structure.getPathNodeList())
        {
            //Debug.Log("Distance: " + Vector2.Distance(transform.position, pathNode.getPos()));
            if (Vector2.Distance(transform.position, pathNode.getPos()) <= 1.1f)
                closeEnough = true;
        }

        return closeEnough;
    }
    private void findNextTarget()
    {
        Debug.Log("Finding new target");
        targetStructure = objectToWalk.getNextTarget();
        Debug.Log("TargetStructure: " + targetStructure);

        if (targetStructure != null && !targetStructure.isDestroyed())
        {
            Debug.Log("TargetStructure: " + targetStructure);
            SetTargetPosition(targetStructure);
        }
        else
        {
            Debug.Log("getNextTarget found no target, sleeping for 1 sec");
            activated = false;
        }
    }
    private void SetTargetPosition(IStructure targetStructure, bool forced = false)
    {
        //Debug.Log("Started pathfinding from " + GetPosition().ToString() + " to " + targetPosition.ToString());
        currentPathIndex = 0;

        List<Vector3> fastestPath = null;
        PathNode closestNode = null;
        foreach (var targetNode in targetStructure.getPathNodeList())
        {
            List<Vector3> vectorList = Pathfinding.Instance.FindPath(new Vector3((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y), 0), targetNode.getPos());
            if (closestNode == null || Vector2.Distance(transform.position, targetNode.getPos()) <
                Vector2.Distance(transform.position, closestNode.getPos()))                             //Checks for closest available pathnodeList
                closestNode = targetNode;
            if (vectorList != null && (fastestPath == null || vectorList.Count <= fastestPath.Count))       //If path is valid, keep it
            {
                fastestPath = vectorList;
            }
        }

        pathVectorList = fastestPath;
        if (closestNode == null)
            closestNode = targetStructure.getPathNodeList()[0];
        
        if (pathVectorList == null && forced)                                                              //If no path was found, force one from to the closest possible block
            pathVectorList = Pathfinding.Instance.FindPath(new Vector3((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y), 0), closestNode.getPos(), true);

        if (pathVectorList == null)
        {
            String outstring;
            outstring = forced ? "No Forced " : "No ";

            Debug.Log(outstring + "path found from [" + Math.Round(transform.position.x) + "," + Math.Round(transform.position.y) + "] to closest structure node [" + 
                      closestNode.getPos().x +", " + closestNode.getPos().y + "]");
            findNextTarget();
            return;
        }
        for (int i = 0; i < pathVectorList.Count - 1; i++)
        {
            Debug.DrawLine(pathVectorList[i], pathVectorList[i + 1], Color.white, pathVectorList.Count);
        }
    }
    
    protected void HandleMovement()
    {
        if (targetStructure == null)
        {
            //Debug.Log("no target struct");
            return;
        }
        if (checkNextNode())
            return;
        
        //Debug.Log("freee");
        if (pathVectorList != null && pathVectorList.Count != 0 && pathVectorList.Count != currentPathIndex) {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            
            if (Vector3.Distance(transform.position, targetPosition) > 0.01f){
                //Vector3 moveDir = (targetPosition - transform.position).normalized;

                //float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                //animatedWalker.SetMoveVector(moveDir);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);
            } else {
                currentPathIndex++;
                if (currentPathIndex + 1 >= pathVectorList.Count) {
                    //Debug.Log("Path ended: currentPathIndex: " + currentPathIndex + ", pathVectorListCount: " + pathVectorList.Count);
                    StopAction();
                    //animatedWalker.SetMoveVector(Vector3.zero);
                }
            }
        }
    }
    private bool checkNextNode()
    {
        if (pathVectorList != null && pathVectorList.Count > 0 && pathVectorList.Count > currentPathIndex)
        {
            //Debug.Log("count: " + pathVectorList.Count  + ", index: " + currentPathIndex);
            PathNode pathNode = objectToWalk.getBay().getPathNode(pathVectorList[currentPathIndex]);
            if (!pathNode.isWalkable)
            {
                targetStructure = pathNode.structure;
                DecideNextAction();
            } 
        }
        //Debug.Log("pathVectorList "+ pathVectorList.Count +" index " + currentPathIndex);
        
        return false;
    }
    
    
    
    public void StopAction()
    {
        DecideNextAction();
    }

    public JobCall getActiveJobCall()
    {
        return activeJobCall;
    }
}

public interface IWalker
{
    public Transform getTransform();
    public Block getNextTarget();

    public void Mine();

    public Inventory getItemInventory();

    public void startDepositingItems();
    public void startTakingItems();

    public Bay getBay();

    public bool isBatteryZero();
}

public enum WalkerStatus
{
    CollectingBlocks,
    InventoryCalls
}


