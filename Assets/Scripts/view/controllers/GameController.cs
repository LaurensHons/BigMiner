using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject Bay;

    public static int baySize = 5;
    public static float blockScale = 0.19f;

    void Start()
    {
        new Pathfinding(Bay.GetComponent<Bay>().pathNodeGrid);
        Time.fixedDeltaTime = 0.02f;
    }
    
    void Update()
    {
        
    }

    public static int getGridSize()
    {
        return baySize;
    }

    public static float getBlockScale()
    {
        return blockScale;
    }

    public static void setGridSize(int value)
    {
        baySize = value;
    }

    public static void setBlockScale(float value)
    {
        blockScale = value;
    }
}
