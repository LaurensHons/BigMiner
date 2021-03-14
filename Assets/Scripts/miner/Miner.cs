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
    
    public void Mine()
    {
        if (walker.targetStructure == null)
        {
            walker.StopAction();
        }
        StartCoroutine(IEChargeDrill());
    }

    private IEnumerator IEChargeDrill()
    {
        Debug.Log("Starting drill");
        yield return new WaitForSeconds(.5f);
        MineBlock();
    }
    
    private void MineBlock()
    {
        walker.targetStructure.getPathNodeList()[0].MineBlock(MinerDamage, out bool destroyed, out ItemInventory loot);
        if (destroyed)
        {
            Inventory.PullInventory(loot);
            walker.targetStructure = null;
            walker.StopAction();
        }
        else
            StartCoroutine(IEChargeDrill());

        Debug.Log("Used: " + Inventory.getUsedCapacity() + ", Max:" + Inventory.getMaxCapacity());
    }
    
    

    public void startDepositingItems()
    {
        StartCoroutine(IEDepositItems());
    }
    
    private IEnumerator IEDepositItems()
    {
        yield return new WaitForSeconds(.5f);
        DepositItems();
    }

    private void DepositItems()
    {
        Inventory.PushInventory(Silo.Instance.Inventory);
        walker.StopAction();
    }
    
    public ItemInventory getItemInventory()
    {
        return Inventory;
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
