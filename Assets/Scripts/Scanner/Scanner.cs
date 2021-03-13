using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public class Scanner
{
    public UIController UIController;

    private Dictionary<BlockTypes, float> sliderValues;
    private Dictionary<BlockTypes, Tuple<float, float>> blocksPerSec = new Dictionary<BlockTypes, Tuple<float, float>>();

    public float SearchCapcacity = 1f;

    private float timer;
    
    
    private static Scanner instance = null;
    public static Scanner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Scanner();
                
            }
            return instance;
        }
    }

    private Scanner()
    {
        blocksPerSec.Add(BlockTypes.DirtBlock, new Tuple<float, float>(0f, SearchCapcacity/getBlockSearchCost(BlockTypes.DirtBlock)));
    }
    
    public void Update()
    {
        foreach (var blockType in blocksPerSec.ToList())
        {
            float nextActionTime = blockType.Value.Item1;
            float bps = blockType.Value.Item2;
            if (bps == 0) continue;
            if (Time.time > nextActionTime) {
                nextActionTime = Time.time + (1f/bps);
                blocksPerSec[blockType.Key] = new Tuple<float, float>(nextActionTime, bps);
                UIController.getBay().spawnBlockType(blockType.Key);
            }  
        }

        bool debug = false;
        if (debug)
        {
            string s = "";
            foreach (var blockType in blocksPerSec.ToList())
            {
                s += "BlockType: " + blockType.Key.ToString() + ", BPS: " + blockType.Value.Item2 +
                     ", nextActionTime: " + blockType.Value.Item1 + "\n";
            }
            Debug.Log(s);
        }
        

    }
    
    
    public float getSearchCapacity()
    {
        return SearchCapcacity;
    }
    
    public int getBlockSearchCost(BlockTypes blocktype)
    {
        BlockTypeSearchCost.TryParse(blocktype.ToString(), out BlockTypeSearchCost blockTypeSearchCost);
        return (int) blockTypeSearchCost;
    }

    public void setblocksPerSec(Dictionary<BlockTypes, float> values)
    {
        foreach (var value in values)
        {
            if (blocksPerSec.ContainsKey(value.Key))
                blocksPerSec[value.Key] = new Tuple<float, float>(Time.time + (1f/value.Value), value.Value);
            else
                blocksPerSec.Add(value.Key, new Tuple<float, float>(Time.time + (1f/value.Value), value.Value));
        }
    }

    public void setUiController(UIController uiController)
    {
        this.UIController = uiController; 
    }


}
