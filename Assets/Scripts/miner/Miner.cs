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

    public EventHandler MinerXpUpdate;
    public EventHandler MinerLevelUpdate;
    
    public float speed => (float) (Math.Pow(0.99f, getLevel(out double d) - 1));
    
    
    public float damage => activeTool.damage;
    public int blocksMined = 0;

    private int XP = 0;

    public int xp
    {
        get
        {
            return XP;
        }
        set
        {
            int previousLevel = getLevel(out double p);
            XP = value;
            MinerXpUpdate?.Invoke(this, EventArgs.Empty);
            if (getLevel(out  p) > previousLevel)
                MinerLevelUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
    private int xpThreshHold = 50;      //change this for level calculation

    public MiningStrategy miningStrategy = MiningStrategy.Random;

    public int maxBatteryMinutes = 2;
    public int Battery = 0;
    
    public int maxBattery
    {
        get => (int) Math.Round(maxBatteryMinutes * 60 * (1/Time.fixedDeltaTime));
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

    public Tool activeTool;
    public EventHandler toolSwitchUpdate;
    public List<Tool> toolList = new List<Tool>();
    
    public Miner(Vector2 pos, MinerStation minerStation)
    {
        HandleSpriteLoading();

        Battery = maxBatteryMinutes * 60 * (int) Math.Round(1/Time.fixedDeltaTime);

        this.minerStation = minerStation;

        Pickaxe pick = new Pickaxe();
        toolList.Add(pick);
        setActiveTool(pick);
        toolList.Add(new Hammer());
        
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
            if (MININGTIMEMOUT >= activeTool.getSpeed())
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




        foreach (var v2 in activeTool.getAdditionalMiningPos(walker.targetStructure.getPos() - (Vector2) getTransform().position))
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
        pathNode.MineBlock(activeTool.damage, out destroyed, out ItemInventory loot, out int xp);
        if (destroyed)
        {
            blocksMined++;
            this.xp += xp;
            activeTool.xp += xp;
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
        double level =  0.5 + Math.Sqrt(1f + 8f * (xp) / (xpThreshHold)) / 2f;
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

    public void setActiveTool(Tool tool)
    {
        if (toolList.Contains(tool))
        {
            if (activeTool != null) activeTool.isSelected = false;
            activeTool = tool;
            activeTool.isSelected = true;
            toolSwitchUpdate?.Invoke(this, EventArgs.Empty);
            Debug.Log("ACCEPTED");
            return;
        }
        Debug.Log("DENIED BITCH");
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
