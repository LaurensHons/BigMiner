
using System;
using System.Collections.Generic;
using UnityEngine;

public class MinerUpgradeController : MonoBehaviour, IMenuController
{
    public GameObject MinerUpgradeMenu;
    public GameObject UpgradeList;
    public GameObject SubPanel;
    public GameObject UpgradePanelPrefab;
    private List<UpgradePanelScript> upgradePanelScripts;
    
    private MinerStation minerstation;
    public MinerUpgradeController MinerStation(MinerStation minerStation)
    {
        this.minerstation = minerStation;
        return this;
    }


    private void Start()
    {
        MinerUpgradeMenu.SetActive(false);
    }

    public void setActive(bool active)
    {
        MinerUpgradeMenu.SetActive(active);
        int amountOfPanels = UpgradeList.transform.childCount;
        for (int i = 0; i < amountOfPanels; i++)
        {
            if (minerstation.Miner.upgrades == null || i >= minerstation.Miner.upgrades.Count) return;
            MinerUpgrade upgrade = minerstation.Miner.upgrades[i];
            if (upgrade != null)
                UpgradeList.transform.GetChild(i).gameObject.GetComponent<UpgradePanelScript>().LoadUpgrade(upgrade);
            else return;
        }
        
    }

    public GameObject getSubpanel()
    {
        return SubPanel;
    }
}
