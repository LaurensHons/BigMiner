
using System;
using System.Collections.Generic;
using UnityEngine;

public class SiloController : MonoBehaviour, IMenuController
{
    public GameObject SiloMenu;
    public GameObject ItemList;
    public GameObject SiloItemPrefab;
    private Dictionary<String, SiloItemPrefabScript> SiloItems = new Dictionary<String, SiloItemPrefabScript>();

    private void Start()
    {
        SiloMenu.SetActive(false);
    }

    public void setActive(bool active)
    {
        SiloMenu.SetActive(active);
        if (active)
            updateUi();
        else
        {
            foreach (Transform child in ItemList.transform)
            {
                Destroy(child.gameObject);
            }

            SiloItems = new Dictionary<String, SiloItemPrefabScript>();
        }
    }

    public GameObject getSubpanel()
    {
        return null;
    }

    private void updateUi()
    {
        foreach (var Item in Silo.Instance.Inventory.getInventory())
        {
            if (!SiloItems.ContainsKey(Item.getName()))
            {
                GameObject go = Instantiate(SiloItemPrefab, ItemList.transform);
                SiloItemPrefabScript sips = go.GetComponent<SiloItemPrefabScript>();
                sips.setItem(Item);
                SiloItems.Add(Item.getName(), sips);
            }
        }
    }

    
}
