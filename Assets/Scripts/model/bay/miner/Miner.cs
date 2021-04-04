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
using UnityEngine.UI;
using Object = UnityEngine.Object;


public class Miner : MonoBehaviour, IWalker
{
    private MinerStation minerStation;
    

    public EventHandler MinerXpUpdate;
    public EventHandler MinerLevelUpdate;

    public Sprite minerImage;
    
    public float speed => (float) (Math.Pow(1.01f, getLevel(out double d) - 1) + speedUpgradesFlat) * speedUpgradesPercent;
    private float speedUpgradesFlat = 0;
    private float speedUpgradesPercent = 1;

    public List<MinerUpgrade> upgrades { get; private set; }
    public float damage => (activeTool.damage + damageUpgradesFlat) * damageUpgradesPercent;
    private float damageUpgradesFlat = 0;
    private float damageUpgradesPercent = 1;

    public EventHandler recalculateStats;
    public EventHandler updatedStats;
    
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
    
    private int xpThreshHold = 2;      //change this for level calculation

    public MiningStrategy miningStrategy = MiningStrategy.Random;

    public int maxBatteryMinutes = 20;
    public int Battery = 0;
    
    public int maxBattery
    {
        get => (int) Math.Round(maxBatteryMinutes * 60 * (1/Time.fixedDeltaTime));
    }
    
    private Walker walker;

    private Vector3 lastPos;

    public Inventory Inventory;

    public Tool activeTool;
    public EventHandler toolSwitchUpdate;
    public List<Tool> toolList = new List<Tool>();

    public Miner Instantiate(Vector2 pos, MinerStation minerStation)
    {
        Battery = maxBatteryMinutes * 60 * (int) Math.Round(1/Time.fixedDeltaTime);

        this.minerStation = minerStation;

        

        Pickaxe pick = new Pickaxe();
        toolList.Add(pick);
        setActiveTool(pick);
        toolList.Add(new Hammer());
        
        Inventory = new Inventory(5);
        walker = new Walker(this, speed);
        
        //GetComponent<SpriteRenderer>().sprite = minerImage;
        transform.position = pos;
        transform.localScale = Vector3.one * GameController.getBlockScale();
        transform.SetParent(getBay().transform);

        upgrades = new List<MinerUpgrade> { new Upgrade1(this), new Upgrade2(this) };

        MinerLevelUpdate += recalculateStats;
        activeTool.ToolDamageUpdate += recalculateStats;
        recalculateStats += updateStatCalculations;
        return this;
    }

    private int MININGTIMEMOUT = -1;
    private int DEPOSITINGTIMEOUT = -1;
    private int TAKINGTIMEOUT = -1;
    
    private void FixedUpdate()
    {
        if (Battery >= 0)
            Battery -= 1;

        if (MININGTIMEMOUT >= 0)
        {
            MININGTIMEMOUT += 1;
            if (MININGTIMEMOUT >= activeTool.getSpeed())
                IEChargeTool();
        }
        if (DEPOSITINGTIMEOUT >= 0)
        {
            DEPOSITINGTIMEOUT += 1;
            if (DEPOSITINGTIMEOUT >= 50)
                IEDepositItems();
        }
        
        if (TAKINGTIMEOUT >= 0)
        {
            TAKINGTIMEOUT += 1;
            if (TAKINGTIMEOUT >= 50)
                IETakeItems();
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
        if (walker.targetStructure == null || walker.targetStructure.isDestroyed())
        {
            walker.DecideNextAction();
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

        if (walker.targetStructure.isDestroyed())
        {
            walker.StopAction();
            return;
        }

        Block block = (Block) walker.targetStructure;
        foreach (var v2 in activeTool.getAdditionalMiningPos(block.getPos() - (Vector2) getTransform().position).ToArray())
        {
            //outstring += v2 + ", ";
            PathNode blockToMine = getBay().getPathNode(
                (int) Math.Round(block.getPos().x + v2.x ), 
                (int) Math.Round(block.getPos().y + v2.y));
            if (blockToMine == null || blockToMine.isWalkable || blockToMine.structure.Equals(walker.targetStructure)) continue;
            MineBlock(blockToMine, out bool d);
            
        }
        //Debug.Log("target: " + walker.targetStructure.getPos());
        //Debug.Log("Additionals: " + outstring);
        MineBlock(getBay().getPathNode(block.getPos()), out bool destroyed);
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
        pathNode.MineBlock(activeTool.damage, out destroyed, out Inventory loot, out int xp);
        if (destroyed)
        {
            blocksMined++;
            this.xp += xp;
            activeTool.xp += xp;
            if (!Inventory.isFull())
            {
                loot?.DepositInventory(Inventory);
            }
            //Debug.Log(Inventory);
        }

        //Debug.Log("Used: " + Inventory.getInventoryWeight() + ", Max:" + Inventory.getMaxInventoryweight());
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
        DEPOSITINGTIMEOUT = 0;
    }

    public void startTakingItems()
    {
        TAKINGTIMEOUT = 0;
    }
    
    
    private void IEDepositItems()
    {
        DEPOSITINGTIMEOUT = -1;
        if (walker.getActiveJobCall() == null)
            Inventory.DepositInventory(Silo.Instance.Inventory);
        else
        {
            ((IJobCallStructure) walker.getActiveJobCall().targetStructure).deliverJobCall(walker.getActiveJobCall().itemToBeDelivered, Inventory); 
        }
        walker.StopAction();
    }

    
    
    private void IETakeItems()
    {
        TAKINGTIMEOUT = -1;
        JobCall jobCall = walker.getActiveJobCall();
        
        ((IJobCallStructure) jobCall.originStructure).pickUpJobCall(jobCall.itemToBeDelivered, Inventory);
        walker.StopAction();
    }

        
    public Inventory getItemInventory()
    {
        return Inventory;
    }

    public void updateStatCalculations(object sender, EventArgs eventArgs)
    {
        if (upgrades == null) return;
        upgrades.Sort(MinerUpgrade.CompareByMinimumLvl);
        maxBatteryMinutes = 20;
        int inventorySize = 5;
        
        Debug.Log("Updating stats");
        
        foreach (var minerUpgrade in upgrades)
        {
            switch (minerUpgrade.getUpgradeType())
            {
                case UpgradeType.Speed:     // Speed formula ( 0.99 ^ ( lvl - 1 ) ) + speedUpgradesFlat ) * speedUpgradesPercent
                {
                    switch (minerUpgrade.getOperatorType())
                    {
                        case operatorType.Flat:
                            speedUpgradesFlat += minerUpgrade.getUpgradeImpact() * minerUpgrade.getAmount();
                            break;
                        case operatorType.Percent:
                            speedUpgradesPercent *= 1 + minerUpgrade.getUpgradeImpact() * minerUpgrade.getAmount();
                            break;
                    }
                    break;
                }
                
                case UpgradeType.Damage:        // Damage Formula (activeTool.damage + damageUpgradesFlat) * damageUpgradesPercent
                {
                    switch (minerUpgrade.getOperatorType())
                    {
                        case operatorType.Flat:
                            damageUpgradesFlat += minerUpgrade.getUpgradeImpact() * minerUpgrade.getAmount();
                            break;
                        case operatorType.Percent:
                            damageUpgradesPercent *= 1 + minerUpgrade.getUpgradeImpact() * minerUpgrade.getAmount();
                            break;
                    }
                    break;
                }
                
                case UpgradeType.Battery:        // Battery starts at 20 min and stacks up multipliers;
                {
                    switch (minerUpgrade.getOperatorType())
                    {
                        case operatorType.Flat:
                            maxBatteryMinutes += (int) minerUpgrade.getUpgradeImpact() * minerUpgrade.getAmount();
                            break;
                        case operatorType.Percent:
                            maxBatteryMinutes *= 1 + (int) minerUpgrade.getUpgradeImpact() * minerUpgrade.getAmount();
                            break;
                    }
                    break;
                }
                
                case UpgradeType.InventorySize:        // Inventory starts at 5 and stacks up multipliers;
                {
                    switch (minerUpgrade.getOperatorType())
                    {
                        case operatorType.Flat:
                            inventorySize += (int) minerUpgrade.getUpgradeImpact() * minerUpgrade.getAmount();
                            break;
                        case operatorType.Percent:
                            inventorySize *= 1 + (int) minerUpgrade.getUpgradeImpact() * minerUpgrade.getAmount();
                            break;
                    }
                    break;
                }
            }
        }

        if (inventorySize != Inventory.getMaxInventoryweight())
        {
            Inventory.setMaxInventoryWeigh(inventorySize);
        }
        
        updatedStats?.Invoke(this, EventArgs.Empty);
    }    

    public Transform getTransform()
    {
        return transform;
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
