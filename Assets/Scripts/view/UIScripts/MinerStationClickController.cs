using System;
using UnityEngine;

public class MinerStationClickController : MonoBehaviour
{
    public MinerStation minerstation;
    public void OnMouseDown()
    {
        GameObject.FindWithTag("UIController").GetComponent<UIController>().OpenMinerMenu(minerstation);
    }
}

public class BatteryClickController : MonoBehaviour
{

    public void OnMouseDrag()
    {
        GameObject.FindWithTag("UIController").GetComponent<UIController>().DragBattery();
    }
}
