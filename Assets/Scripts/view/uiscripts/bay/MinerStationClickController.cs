using System;
using UnityEngine;
public class BatteryClickController : MonoBehaviour
{
    public void OnMouseDrag()
    {
        GameObject.FindWithTag("UIController").GetComponent<UIController>().DragBattery();
    }
}


//TODO refactor to remove this file lmao