
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Processor : MultiBlock
{
    private float ItemsPerSecond = 1f;
    private int maxInventory;
    private Tier currentTier;

    private Inventory inputInventory;
    private Inventory outputInventory;
    
    public Processor(float x, float y, float speed, Tier tier) : base(x, y)
    {
        ItemsPerSecond = speed;
        smeltercooldown = (int) (ItemsPerSecond / Time.fixedDeltaTime);
        currentTier = tier;
        
        setMaxInventory(tier);
        outputInventory = new Inventory();
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

        if (inputInventory == null)
            inputInventory = new Inventory(maxInventory);
        else
            inputInventory.setMaxInventoryWeigh(maxInventory);

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
                inputInventory.RemoveItem(inputItem);
            }
            
            foreach (var outputItem in getOutputItems())
            {
                outputInventory.AddItem(Item.CreateItem(outputItem, outputItem.getAmount()));
            }
        }
    }

    private bool checkInputInventory()
    {
        foreach (var inputItem in getActualInputItems())
        {
            Item itemInInv = inputInventory.TryGetItem(inputItem);
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
        inputInventory.putItem(item, minerInventory, amount);
        checkInputInventory();
    }

    public void takeItem(Item item, Inventory minerInventory, int? amount = null)
    {
        outputInventory.TakeItem(item, minerInventory, amount);
    }
    
    public override void onClick()
    {
        
    }
    

    public override bool isResource() { return false; }
    public abstract override Vector2 getDimensions();
    public abstract override string getSpritePath();

    public Item[] getActualInputItems()
    {
        List<Item> returnItems = new List<Item>();
        foreach (var baseInputItem in getBaseInputItems())
        {
            Item returnItem = baseInputItem * Int32.Parse(currentTier.ToString());
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
}

