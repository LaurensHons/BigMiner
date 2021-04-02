
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Processor : MultiBlock, IJobCallStructure
{
    protected float ItemsPerSecond = 1f;
    protected int maxInventory;
    protected Tier currentTier;

    protected Inventory InputInventory;
    protected Inventory OutputInventory;
    
    
    public Processor(float x, float y, float speed, Tier tier) : base(x, y)
    {
        ItemsPerSecond = speed;
        smeltercooldown = (int) (ItemsPerSecond / Time.fixedDeltaTime);
        currentTier = tier;
        
        setMaxInventory(tier);
        OutputInventory = new Inventory();
    }

    private void setMaxInventory(Tier tier)
    {
        switch (tier)
        {
            case Tier.Bronze:
                maxInventory = 10;
                break;
            case Tier.Iron:
                maxInventory = 20;
                break;
            case Tier.Silver:
                maxInventory = 40;
                break;
            case Tier.Gold:
                maxInventory = 60;
                break;
            case Tier.Titanium:
                maxInventory = 80;
                break;
            case Tier.Diamond:
                maxInventory = 100;
                break;
        }

        if (InputInventory == null)
            InputInventory = new Inventory(maxInventory);
        else
            InputInventory.setMaxInventoryWeigh(maxInventory);

    }

    public int smeltercooldown { get; private set; }
    private bool notEnoughItems = true;
    public void fixedUpdate()
    {
        if (notEnoughItems)
            smeltercooldown = (int) (ItemsPerSecond / Time.fixedDeltaTime);
        else if (smeltercooldown <= 0)
        {
            smeltercooldown = (int) (ItemsPerSecond / Time.fixedDeltaTime);
            SmeltItem();
        }
        else smeltercooldown--;
    }

    

    private void SmeltItem()
    {
        if (checkInputInventory())
        {
            foreach (var inputItem in getActualInputItems())
            {
                InputInventory.RemoveItem(inputItem);
            }
            
            foreach (var outputItem in getOutputItems())
            {
                OutputInventory.AddItem(Item.CreateItem(outputItem, outputItem.getAmount()));
            }
        }
    }

    private bool checkInputInventory()
    {
        foreach (var inputItem in getActualInputItems())
        {
            Item itemInInv = InputInventory.TryGetItem(inputItem);
            if (itemInInv.getAmount() < inputItem.getAmount())
            {
                notEnoughItems = true;
                return false;
            }
        }
        notEnoughItems = false;
        return true;
    }
    
    public void depositItem(Item item, Inventory minerInventory, int? amount = null)
    {
        InputInventory.putItem(item, minerInventory, amount);
        checkInputInventory();
    }

    public void takeItem(Item item, Inventory minerInventory, int? amount = null)
    {
        OutputInventory.TakeItem(item, minerInventory, amount);
    }
    public Inventory getInputInventory()
    {
        return InputInventory;
    }
    public Inventory getOutputInventory()
    {
        return OutputInventory;
    }
    public override void onClick()
    {
        
    }

    public void deliverJobCall(Item itemToBeDelivered, Inventory minerInventory)
    {
        InputInventory.TakeItem(itemToBeDelivered, minerInventory);
        JobController.Instance.successJobCall(this, itemToBeDelivered);
    }

    public void pickUpJobCall(Item itemToBeDelivered, Inventory minerInventory)
    {
        int amount = itemToBeDelivered.getAmount();
        if (amount > minerInventory.getLeftOverInventorySpace()) amount = minerInventory.getLeftOverInventorySpace();
        OutputInventory.putItem(itemToBeDelivered, minerInventory, amount);
    }

    public void addInventoryCall(Item item)
    {
        
    }

    public override bool isResource() { return false; }
    public abstract override Vector2 getDimensions();
    public abstract override string getSpritePath();
    public abstract String getName();
    
    public Item[] getActualInputItems()
    {
        List<Item> returnItems = new List<Item>();
        foreach (var baseInputItem in getBaseInputItems())
        {
            Item returnItem = baseInputItem * getTierMultiplier(currentTier);
            returnItems.Add(returnItem);
        }

        return returnItems.ToArray();
    }
    public abstract Item[] getBaseInputItems();      // this amount is multiplied by the Tier enum described below
    public abstract Item[] getOutputItems();     // this amount is never changed (maybe later if tier is 0-1?)
    
    public enum Tier
    {
        Bronze = 10,
        Iron = 8,
        Silver = 7,
        Gold = 6,
        Titanium = 5,
        Diamond = 4
    }

    public int getTierMultiplier(Tier tier)
    {
        switch (tier)
        {
            case Tier.Bronze: return 10;
            case Tier.Iron: return 8;
            case Tier.Silver: return 7;
            case Tier.Gold: return 6;
            case Tier.Titanium: return 5;
            case Tier.Diamond: return 4;
            default: return 10;
        }
        
    }

    
}

