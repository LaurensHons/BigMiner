using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScannerSlider : MonoBehaviour
{
    private float accuracy = 4;
    public float Accuracy
    {
        get { return accuracy; }
        set
        {
            slider.maxValue = value;
            accuracy = value;
        }
    }

    private Slider slider;
    public Text PercentageText;
    public Text BPSText;
    public Text BlockTypeText;
    public Button UpgradeAccuracyButton;

    private float GETSLIDERVAUE;
    
    public ScannerController ScannerController;

    private BlockTypes blockType;
    
    private void Start()
    {
        this.transform.position = Vector3.zero;
        slider = GetComponentInChildren<Slider>();

        slider.maxValue = accuracy;
        slider.onValueChanged.AddListener(delegate { ScannerController.updateSliders(); });
        ScannerController.RegisterScannerSlider(this);

        string gameObjectName = gameObject.name.Remove(0, 6);
        BlockTypes.TryParse(gameObjectName, out BlockTypes result);
        blockType = result;
        BlockTypeText.text = result.ToString();

        if (blockType == BlockTypes.DirtBlock) slider.value = slider.maxValue;
        
        UpgradeAccuracyButton.onClick.AddListener(upgradeScannerAccuracy);
        
        updatePercentageText();
    }

    private void Update()
    {
        GETSLIDERVAUE = slider.value / accuracy;
    }

    public void updatePercentageText()
    {
        setPercentage( slider.value * 100f / accuracy);
    }
    private void setPercentage(float percent)
    {
        if (float.IsNaN(percent)) return;
        PercentageText.text = Math.Round(percent) + "%";
    }

    
    public void setBPSText(float percent)
    {
        if (float.IsNaN(percent)) BPSText.text = "0 Blocks/sec";
        else if (percent <= 1/5f) BPSText.text = Math.Round(percent*60, 3) + " Blocks/min";
        else BPSText.text = Math.Round(percent, 3) + " Blocks/sec";
        
    }

    public float getValue()
    {
        return slider.value / accuracy;
    }

    public BlockTypes getBlockType()
    {
        return blockType;
    }

    public void upgradeScannerAccuracy()
    {
        accuracy++;
    }

    
}


