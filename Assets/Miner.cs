using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Grid;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


public class Miner : MonoBehaviour
{
    private MinerStation minerStation;
    public Block targetBlock;
    
    
    public List<Vector3> pathVectorList = new List<Vector3>();
    public int currentPathIndex;

    private Pathfinding Pathfinding;
    
    private Vector3 lastPos;
    public int newTargetTimeout = 0;

    private IEnumerator _coroutine;
    private bool DoNothing = false;
    
    private float speed { get; set; }
    private int MinerDamage = 1;
    void Start()
    {
        speed = 0.005f;
        Pathfinding = new Pathfinding(minerStation.getNodeGrid());
        findNextTarget();
    }

    private void findNextTarget()
    {
        //Debug.Log("Finding new target");
        if (targetBlock != null) targetBlock.stopHighlightBlock();
        targetBlock = minerStation.getNextTarget();

        if (targetBlock != null)
        {
            targetBlock.highLightBlock();
            SetTargetPosition(targetBlock.transform.position);
        }
        else
        {
            DoNothing = true;
            Debug.Log("Miner is doing nothing");
        }
    }

    private void Update()
    {
        if (DoNothing) return;
        HandleMovement();
        
        if (lastPos != null && lastPos.Equals(this.transform.position))
            newTargetTimeout++;
        else
        {
            newTargetTimeout = 0;
        }

        if (newTargetTimeout >= 2000)
        {
            newTargetTimeout = 0;
            findNextTarget();
        }

    }
    
    private void HandleMovement() {
        if (pathVectorList != null && pathVectorList.Count != 0) {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if(!minerStation.getBay().getPathNode(pathVectorList[currentPathIndex]).isWalkable)
                StopMoving();
            else if (Vector3.Distance(transform.position, targetPosition) > 0.01f){
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
        } else {
            if (targetBlock == null)
                findNextTarget();
        }
    }
    
    private void StopMoving() {
        pathVectorList = null;
        Debug.Log("Stopped Moving");
        if (Vector3.Distance(transform.position, targetBlock.transform.position) <= 1.2f)
            StartCoroutine(chargeDrill());
        else 
            findNextTarget();
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
    
    public void SetTargetPosition(Vector3 targetPosition)
    {
        //Debug.Log("Started pathfinding from " + GetPosition().ToString() + " to " + targetPosition.ToString());
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(new Vector3((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y), 0), targetPosition);

        if (pathVectorList == null || pathVectorList.Count == 0)
        {
            findNextTarget();
            return;
        }
        for (int i = 0; i < pathVectorList.Count - 1; i++)
        {
            Debug.DrawLine(pathVectorList[i], pathVectorList[i + 1], Color.white, pathVectorList.Count);
        }
    }
    
    public void setMinerStation(MinerStation minerStation)
    {
        this.minerStation = minerStation;
    }

    private IEnumerator chargeDrill()
    {
        yield return new WaitForSeconds(.5f);
        MineBlock();
    }

    private void MineBlock()
    {
        minerStation.getBay().getPathNode(targetBlock).MineBlock(MinerDamage, out bool destroyed);
        if (destroyed || targetBlock == null)
            findNextTarget();
        else 
            StartCoroutine(chargeDrill());
    }
}
