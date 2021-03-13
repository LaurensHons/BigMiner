
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class ScannerController : MonoBehaviour
{
    public UIController UIControllerObject;
    
    private UIController UIController;
    public GameObject BayGameObject;
    private Bay Bay;

    public Scanner Scanner;
    
    public GameObject ScannerButton;
    public GameObject ScannerPanel;
    public GameObject ScannerPanelList;
    
    public GameObject slider_BlockPrefab;
    

    public List<ScannerSlider> ScannerSliders = new List<ScannerSlider>();
    
    private Dictionary<BlockTypes, float> blocksPerSec = new Dictionary<BlockTypes, float>();
    

    private void Start()
    {
        UIController = UIControllerObject.GetComponent<UIController>();
        
        Scanner = Scanner.Instance;
        Scanner.setUiController(UIController);
        
        Bay = BayGameObject.GetComponent<Bay>();

        ScannerPanel.SetActive(false);

        foreach (var BlockType in Enum.GetNames(typeof(BlockTypes)))
        {
            slider_BlockPrefab.gameObject.name = "Slider" + BlockType.ToString();
            GameObject Slider_BlockPrefab = Instantiate(slider_BlockPrefab, Vector3.zero, Quaternion.identity, ScannerPanelList.transform);
            Slider_BlockPrefab.name = "Slider" + BlockType.ToString();
            Slider_BlockPrefab.GetComponent<ScannerSlider>().ScannerController = this;
        }
        
        
        Vector3 pos = new Vector3(Bay.gridSize/2, Bay.gridSize + .3f);
        ScannerButton.transform.position = pos;
        float scale = ScannerButton.transform.localScale.x;
        ScannerButton.GetComponent<RectTransform>().rect.Set(Bay.gridSize/2, Bay.gridSize + .3f,scale * Bay.gridSize, 30);
        
        updateSliders();
    }

    public void OpenScannerMenu()
    {
        UIController.hasMenuOpen = true;

        Vector2 pos = UIController.camera.transform.position;
        ScannerPanel.transform.position = pos;
        ScannerPanel.SetActive(true);
        updateSliders();
    }

    public void CloseScannerMenu()
    {
        UIController.hasMenuOpen = false;
        ScannerPanel.SetActive(false);
    }

    private void Update()
    {
        Scanner.Update();
    }

    public void RegisterScannerSlider(ScannerSlider scannerSlider)
    {
        ScannerSliders.Add(scannerSlider);
    }
    
    public void updateSliders()
    {
        if (Scanner == null) return;
        float total = 0;
        foreach (var slider in ScannerSliders)
        {
            total += slider.getValue();
        }

        

        foreach (var slider in ScannerSliders)
        {
            float scaleFactor = slider.getValue() / total;
            
            float BPS = slider.getValue() * Scanner.getSearchCapacity() * scaleFactor / getBlockSearchCost(slider.getBlockType());
            Debug.Log(slider.getValue()  + " *  " +  Scanner.getSearchCapacity() + " *  " + scaleFactor + " /  " + getBlockSearchCost(slider.getBlockType()));
            
            //Debug.Log("Scalefactor: " + scaleFactor + ", total: " + total + ", Slidervalue: " + slider.getValue());
            slider.updatePercentageText();
            if (BPS <= 0) Debug.Log("BPS is neg lmao");
            slider.setBPSText(BPS);
            if (blocksPerSec.ContainsKey(slider.getBlockType()))
                blocksPerSec[slider.getBlockType()] = BPS;
            else
                blocksPerSec.Add(slider.getBlockType(), BPS);
        }
        Scanner.setblocksPerSec(blocksPerSec);
        
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
        Scanner.SearchCapcacity *= 1.5f;
    }
    
}
