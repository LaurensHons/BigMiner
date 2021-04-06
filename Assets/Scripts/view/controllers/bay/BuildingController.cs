
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour, IMenuController
{
    public GameObject BuildingMenu;
    public GameObject ActiveBuildingList;
    public GameObject InActiveBuildingList;
    public List<BuildingPanelScript> ActiveBuildingPanelScripts;

    public GameObject ActiveBuildingPanelPrefab;
    

    private void Start()
    {
        BuildingMenu.SetActive(false);
    }

    public void setActive(bool active)
    {
        BuildingMenu.SetActive(active);

        if (ActiveBuildingPanelScripts == null) ActiveBuildingPanelScripts = new List<BuildingPanelScript>();
        destroyAllactiveBuilidingPanels();
        List<Processor> processors = GameObject.FindWithTag("Bay").GetComponent<Bay>().getProcessors();

        for (int i = 0; i < processors.Count; i++)
        {
            GameObject ActiveBuildingPrefab = Instantiate(ActiveBuildingPanelPrefab, ActiveBuildingList.transform);
            BuildingPanelScript script = ActiveBuildingPrefab.GetComponent<BuildingPanelScript>();
            script.setActive(true, processors[i]); 
            ActiveBuildingPanelScripts.Add(script);
        }
    }

    private void destroyAllactiveBuilidingPanels()
    {
        if (ActiveBuildingList == null) return;
        foreach (var activeBuildingPanelScript in ActiveBuildingPanelScripts)
        {
            Destroy(activeBuildingPanelScript.gameObject);
        }
        ActiveBuildingPanelScripts = new List<BuildingPanelScript>();
    }
    
    private BuildingPanelScript getActiveBuildingPanelScript(Processor processor)
    {
        foreach (var activeBuildingPanelScript in ActiveBuildingPanelScripts)
        {
            if (activeBuildingPanelScript.Processor.Equals(processor)) return activeBuildingPanelScript;
        }
        return null;
    }

    public GameObject getSubpanel()
    {
        return null;
    }
}
