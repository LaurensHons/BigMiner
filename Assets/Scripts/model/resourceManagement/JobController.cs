using System;
using System.Collections.Generic;
using System.Linq;

public class JobController
{
    public static JobController Instance { get; private set; }
    private List<JobCall> JobCalls;
    
    
    public JobController()
    {
        Instance = this;
        JobCalls = new List<JobCall>();
    }

    public JobCall addJobCall(IStructure originStructure, IStructure targetStructure, Item item)
    {
        JobCall jobCall = new JobCall(originStructure, targetStructure, item, this);
        if (jobCall == null) throw new ArgumentException();
        JobCalls.Add(jobCall);
        return jobCall;
    }
    public void successJobCall(IJobCallStructure jobCallStructure, Item item)
    {
        foreach (var call in JobCalls)
        {
            if (call.targetStructure.Equals(jobCallStructure) && call.itemToBeDelivered.GetType() == item.GetType())
            {
                call.itemToBeDelivered.addAmount(-item.getAmount());
                if (call.itemToBeDelivered.getAmount() == 0)
                    JobCalls.Remove(call);
                return;
            }
        }
    }

    public JobCall getNextJobCall()
    {
        if (JobCalls != null && JobCalls.Count >= 1) return JobCalls[0];
        return null;
    }
    

}
