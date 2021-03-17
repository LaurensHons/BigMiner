using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Grid;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;


public class Miner : IWalker
{
    private MinerStation minerStation;

    protected GameObject minerObject;
    private Sprite minerSprite;

    public float speed = 0.1f;
    public int damage => tool.damage;
    public int blocksMined = 0;

    public int xp = 0;
    private int xpThreshHold = 50;      //change this for level calculation

    public MiningStrategy miningStrategy = MiningStrategy.Random;

    public int maxBatteryMinutes = 2;
    public int Battery = 0;
    
    public int maxBattery
    {
        get => (int) Math.Round(maxBatteryMinutes * 60 * (1/Time.fixedDeltaTime));
    }
    
    public float Speed
    {
        get
        {
            return walker.speed;
        }

        set
        {
            if (value >= 0)
            {
                walker.speed = value;
            }
        }
    }
    
    public String name
    {
        get
        {
            return minerObject.name;
        }

        set
        {
            minerObject.name = value;
        }
    }
    
    private Walker walker;

    private Vector3 lastPos;

    public ItemInventory Inventory;

    public Tool tool;
    public List<Tool> toolList = new List<Tool>();
    
    public Miner(Vector2 pos, MinerStation minerStation)
    {
        HandleSpriteLoading();

        Battery = maxBatteryMinutes * 60 * (int) Math.Round(1/Time.fixedDeltaTime);

        this.minerStation = minerStation;

        this.tool = new Pickaxe();
        toolList.Add(tool);
        
        Inventory = new ItemInventory(5);
        walker = new Walker(this, speed);
        minerObject = new GameObject("Miner 1");
        minerObject.transform.localPosition = pos;
        minerObject.AddComponent<SpriteRenderer>();
        minerObject.transform.localScale = Vector3.one * GameController.getBlockScale();
        minerObject.transform.SetParent(getBay().transform);
    }

    private void HandleSpriteLoading()
    {
        AsyncOperationHandle<Sprite> minerSpriteHandler = Addressables.LoadAssetAsync<Sprite>(getSpritePath());
        minerSpriteHandler.Completed += LoadminerSpriteWhenReady;
    }

    private void LoadminerSpriteWhenReady(AsyncOperationHandle<Sprite> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            minerSprite = obj.Result;
            
            if (minerSprite == null) throw new Exception("No block sprite found, maybe file named wrong?");
            minerObject.GetComponent<SpriteRenderer>().sprite = minerSprite;
        }
        else throw new Exception("Loading sprite failed");
    }


    private int MININGTIMEMOUT = -1;
    private int TRADINGTIMEOUT = -1;
    
    public void Update(object sender, EventArgs eventArgs)
    {
        if (Battery >= 0)
            Battery -= 1;

        if (MININGTIMEMOUT >= 0)
        {
            MININGTIMEMOUT += 1;
            if (MININGTIMEMOUT >= tool.getSpeed())
                IEChargeTool();
        }
        if (TRADINGTIMEOUT >= 0)
        {
            TRADINGTIMEOUT += 1;
            if (TRADINGTIMEOUT >= 50)
                IEDepositItems();
        }
        

        bool wakeup = Time.time % 500 == 0 || Time.time < 200;
        walker.Update(wakeup);
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
            return;
        }
        MININGTIMEMOUT = 0;
    }

    private void IEChargeTool()
    {
        MININGTIMEMOUT = -1;
        SwingTool();
    }

    private void SwingTool()
    {
        //String outstring = "Additinal mining pos:\n";




        foreach (var v2 in tool.getAdditionalMiningPos(walker.targetStructure.getPos() - (Vector2) getTransform().position))
        {
            //outstring += v2 + ", ";
            PathNode blockToMine = getBay().getPathNode(
                (int) Math.Round(walker.targetStructure.getPos().x + v2.x ), 
                (int) Math.Round(walker.targetStructure.getPos().y + v2.y));
            if (blockToMine == null || blockToMine.isWalkable || blockToMine.structure.Equals(walker.targetStructure)) continue;
            MineBlock(blockToMine, out bool d);
            
        }
        //Debug.Log("target: " + walker.targetStructure.getPos());
        //Debug.Log("Additionals: " + outstring);
        MineBlock(getBay().getPathNode(walker.targetStructure.getPos()), out bool destroyed);
        if (destroyed)
        {
            walker.targetStructure = null;
            walker.StopAction();
        }
        else
            Mine();
    }
    
    private void MineBlock(PathNode pathNode, out bool destroyed)
    {
        pathNode.MineBlock(tool.damage, out destroyed, out ItemInventory loot, out int xp);
        if (destroyed)
        {
            blocksMined++;
            this.xp += xp;
            Inventory.PullInventory(loot);   
        }

        Debug.Log("Used: " + Inventory.getUsedCapacity() + ", Max:" + Inventory.getMaxCapacity());
    }

    public bool isBatteryZero()
    {
        return Battery <= 0;
    }

    public int getLevel(out double percentLeft)
    {
        double level =  0.5 + Math.Sqrt(1 + 8 * (xp) / (xpThreshHold)) / 2;
        percentLeft = level % 1;
        return (int) level;
    }

    public void startDepositingItems()
    {
        TRADINGTIMEOUT = 0;
    }
    
    private void IEDepositItems()
    {
        TRADINGTIMEOUT = -1;
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
        return minerObject.transform;
    }

    public Block getNextTarget()
    {
        return minerStation.getNextTarget();
    }

    

    public Bay getBay()
    {
        return minerStation.getBay();
    }

    public String getSpritePath()
    {
        return "Assets/Images/Miner.png";
    }
}
