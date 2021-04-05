
using System;
using System.Collections.Generic;
using UnityEngine;

public class MinerToolController : MonoBehaviour, IMenuController
{
    public GameObject MinerToolMenu;
    public GameObject ToolList;
    public GameObject SubPanel;
    
    public GameObject ToolPanelPrefab;
    private MinerStation minerstation;
    
    public MinerToolController MinerStation(MinerStation minerStation)
    {
        this.minerstation = minerStation;
        return this;
    }
    
    public Dictionary<Tool, ToolPanelScript> toolPanels = new Dictionary<Tool, ToolPanelScript>();

    private void Start()
    {
        MinerToolMenu.SetActive(false);
    }

    public void setActive(bool active)
    {
        MinerToolMenu.SetActive(active);
        if (ToolPanelPrefab == null) throw new Exception("Tool Panel Prefab not found");

        if (active)
        {
            foreach (var tool in minerstation.Miner.toolList)
            {
                if (!toolPanels.ContainsKey(tool))
                {
                    GameObject ToolPanel = Instantiate(ToolPanelPrefab, ToolList.transform);
                    toolPanels.Add(tool, ToolPanel.GetComponent<ToolPanelScript>());
                }
                Debug.Log(tool.GetType());
                toolPanels[tool].setActive(true, this, tool);
            } 
        }
        else
        {
            foreach (var keyValuePair in toolPanels)
            {
                keyValuePair.Value.setActive(false);
            }
        }
    }

    public GameObject getSubpanel()
    {
        return SubPanel;
    }

    public MinerStation getMinerStation()
    {
        return minerstation;
    }
}
