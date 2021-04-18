using System;
using System.Collections.Generic;

public class JobCall
{
    public IStructure targetStructure { get; private set; }
    public IStructure originStructure { get; private set; }
    public Item itemToBeDelivered { get; private set; }
    public int itemsInTransit { get;  set; }
    private JobController jobController;

    public JobCall(IStructure originStructure, IStructure targetStructure, Item item, JobController jobController)
    {
        this.jobController = jobController;
        setOriginStructure(originStructure);
        setTargetStructure(targetStructure);
        setItem(item);
    }
    
    private void setOriginStructure(IStructure originStructure)
    {
        if (originStructure == null) throw new ArgumentException();
        this.originStructure = originStructure;
    }
    private void setTargetStructure(IStructure targetStructure)
    {
        if (targetStructure == null) throw new ArgumentException();
        this.targetStructure = targetStructure;
    }
    private void setItem(Item item)
    {
        if (item == null) throw new ArgumentException();
        itemToBeDelivered = item;
    }

    public static JobCall operator ++(JobCall a)
    {
        a.itemToBeDelivered.addAmount(1);
        return a;
    }

    public static JobCall operator --(JobCall a)
    {
        a.itemToBeDelivered.addAmount(-1);
        return a;
    }
    
    public static JobCall operator +(JobCall a, int b)
    {
        a.itemToBeDelivered.addAmount(b);
        return a;
    }

    public static JobCall operator -(JobCall a, int b)
    {
        a.itemToBeDelivered.addAmount(b);
        return a;
    }
}

public interface IJobCallStructure
{
    public void deliverJobCall(Item itemToBeDelivered, IInventory minerInventory);
    public void pickUpJobCall(Item itemToBeDelivered, IInventory minerInventory);
    public void addInventoryCall(Item item);
}
    



