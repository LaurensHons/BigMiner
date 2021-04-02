
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour, IMenuController
{
    public GameObject BuildingMenu;
    public GameObject ActiveBuildingList;
    public GameObject InActiveBuildingList;
    public List<ActiveBuildingPanelScript> ActiveBuildingPanelScripts;

    public GameObject ActiveBuildingPanelPrefab;
    

    private void Start()
    {
        BuildingMenu.SetActive(false);
    }

    public void setActive(bool active)
    {
        BuildingMenu.SetActive(active);

        if (ActiveBuildingPanelScripts == null) ActiveBuildingPanelScripts = new List<ActiveBuildingPanelScript>();
        List<Processor> processors = GameObject.FindWithTag("Bay").GetComponent<Bay>().getProcessors();
        for (int i = 0; i < processors.Count; i++)
        {
            Debug.Log("One processor");
            if (!processors[i].isDestroyed)  //Active building prefab
            {
                if (ActiveBuildingPanelScripts.Count >= i)
                {
                    GameObject ActiveBuildingPrefab = Instantiate(ActiveBuildingPanelPrefab, ActiveBuildingList.transform);
                    ActiveBuildingPanelScripts.Add(ActiveBuildingPrefab.GetComponent<ActiveBuildingPanelScript>());
                }
                ActiveBuildingPanelScripts[i].setActive(true, processors[i]); 
            }
            
        }
    }
}
