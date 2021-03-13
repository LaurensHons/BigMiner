using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Grid;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


public class Miner : MonoBehaviour, IWalker
{
    private MinerStation minerStation;
    private Walker walker;
    
    
    public List<Vector3> pathVectorList = new List<Vector3>();
    public int currentPathIndex;

    private Vector3 lastPos;
    public int newTargetTimeout = 0;

    private bool IeChargeDrill = false;
    private bool IeDepositItems = false;

    public ItemInventory Inventory;

    private float speed { get; set; }
    private int MinerDamage = 1;
    void Start()
    {
        Inventory = new ItemInventory(5);
        speed = 0.005f;
        walker = new Walker(this);

    }
    

    private void Update()
    {
        if (walker.getWalkerStatus().Equals(WalkerStatus.TradingItems) && IeDepositItems == false &&
            Vector2.Distance(transform.position, Silo.Instance.getPathNode().getPos()) < 1.1f)
            StartCoroutine(IEDepositItems());
        if (walker.getWalkerStatus().Equals(WalkerStatus.Mining) && walker.targetBlock != null && 
            IeChargeDrill == false && walker.hasEmptyPathNodeList())
            StartCoroutine(IEChargeDrill());
        if (Inventory.getUsedCapacity() >= Inventory.getMaxCapacity())
        {
            walker.setStatusGoingToSilo();
            Debug.Log(Inventory.getUsedCapacity() + ", " + Inventory.getMaxCapacity());
        }
            
        bool wakeup = Time.time % 500 == 0 || Time.time < 200;
        walker.Update(wakeup);
        
    }
    

    public Vector3 GetPosition() {
        return transform.position;
    }
    
    
    public void setMinerStation(MinerStation minerStation)
    {
        this.minerStation = minerStation;
    }

    private IEnumerator IEChargeDrill()
    {
        IeChargeDrill = true;
        Debug.Log("Starting drill");
        yield return new WaitForSeconds(.5f);
        MineBlock();
    }
    private void MineBlock()
    {
        walker.targetBlock.getPathNodeList()[0].MineBlock(MinerDamage, out bool destroyed, out ItemInventory loot);
        if (destroyed)
        {
            Inventory.PullInventory(loot);
            walker.targetBlock = null;
            walker.setStatusCollectingBlocks();
            IeChargeDrill = false;
        }
        else
            StartCoroutine(IEChargeDrill());
    }
    

    private IEnumerator IEDepositItems()
    {
        IeDepositItems = true;
        yield return new WaitForSeconds(.5f);
        DepositItems();
    }

    private void DepositItems()
    {
        Inventory.safePushInventory(Silo.Instance.Inventory);
        IeDepositItems = false;
    }

    

    public Transform getTransform()
    {
        return transform;
    }

    public Block getNextTarget()
    {
        return minerStation.getNextTarget();
    }

    public Bay getBay()
    {
        return minerStation.getBay();
    }
}
