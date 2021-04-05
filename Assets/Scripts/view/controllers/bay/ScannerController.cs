
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class ScannerController : MonoBehaviour, IMenuController
{
    public UIController UIControllerObject;
    public GameObject ScannerMenu;
    private UIController UIController;
    public GameObject BayGameObject;
    private Bay Bay;
    
    public GameObject ScannerPanelList;
    
    public GameObject slider_BlockPrefab;
    

    public List<ScannerSlider> ScannerSliders = new List<ScannerSlider>();
    
    private Dictionary<BlockTypes, float> blocksPerSec = new Dictionary<BlockTypes, float>();
    

    private void Start()
    {
        ScannerMenu.SetActive(false);
        UIController = UIControllerObject.GetComponent<UIController>();
        Scanner.Instance.setUiController(UIController);
        
        Bay = BayGameObject.GetComponent<Bay>();

        foreach (var BlockType in Enum.GetNames(typeof(BlockTypes)))
        {
            slider_BlockPrefab.gameObject.name = "Slider" + BlockType.ToString();
            GameObject Slider_BlockPrefab = Instantiate(slider_BlockPrefab, Vector3.zero, Quaternion.identity, ScannerPanelList.transform);
            Slider_BlockPrefab.name = "Slider" + BlockType.ToString();
            Slider_BlockPrefab.GetComponent<ScannerSlider>().ScannerController = this;
        }
        updateSliders();
    }

    

    private void FixedUpdate()
    {
        Scanner.Instance.Update();
    }

    public void RegisterScannerSlider(ScannerSlider scannerSlider)
    {
        ScannerSliders.Add(scannerSlider);
    }
    
    public void updateSliders()
    {
        float total = 0;
        foreach (var slider in ScannerSliders)
        {
            total += slider.getValue();
        }
        
        foreach (var slider in ScannerSliders)
        {
            float scaleFactor = slider.getValue() / total;
            
            float BPS = slider.getValue() * Scanner.Instance.getSearchCapacity() * scaleFactor / getBlockSearchCost(slider.getBlockType());
            Debug.Log(slider.getValue()  + " *  " +  Scanner.Instance.getSearchCapacity() + " *  " + scaleFactor + " /  " + getBlockSearchCost(slider.getBlockType()));
            
            //Debug.Log("Scalefactor: " + scaleFactor + ", total: " + total + ", Slidervalue: " + slider.getValue());
            slider.updatePercentageText();
            if (BPS <= 0) Debug.Log("BPS is neg lmao");
            slider.setBPSText(BPS);
            if (blocksPerSec.ContainsKey(slider.getBlockType()))
                blocksPerSec[slider.getBlockType()] = BPS;
            else
                blocksPerSec.Add(slider.getBlockType(), BPS);
        }
        Scanner.Instance.setblocksPerSec(blocksPerSec);
        
    }

    public float getCurrentSearchCapacityUse()
    {
        float total = 0;
        foreach (var blocktype in blocksPerSec)
        {
            total += blocktype.Value*getBlockSearchCost(blocktype.Key);
        }

        if (float.IsNaN(total)) return 0;
        return total;
    }
    
    public int getBlockSearchCost(BlockTypes blocktype)
    {
        BlockTypeSearchCost.TryParse(blocktype.ToString(), out BlockTypeSearchCost blockTypeSearchCost);
        return (int) blockTypeSearchCost;
    }

    public void upgradeScannerPower()
    {
        Scanner.Instance.SearchCapcacity *= 1.5f;
    }

    public void setActive(bool active)
    {
        ScannerMenu.SetActive(active);
        for (int i = 0; i < ScannerPanelList.transform.childCount; i++)
        {
            Destroy(ScannerPanelList.transform.GetChild(i).gameObject);
        }
        
        
        foreach (var BlockType in Enum.GetNames(typeof(BlockTypes)))
        {
            slider_BlockPrefab.gameObject.name = "Slider" + BlockType.ToString();
            GameObject Slider_BlockPrefab = Instantiate(slider_BlockPrefab, Vector3.zero, Quaternion.identity, ScannerPanelList.transform);
            Slider_BlockPrefab.name = "Slider" + BlockType.ToString();
            Slider_BlockPrefab.GetComponent<ScannerSlider>().ScannerController = this;
        }
        updateSliders();
    }

    public GameObject getSubpanel()
    {
        return null;
    }
}
