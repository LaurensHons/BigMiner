using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScannerMainInfoPanel : MonoBehaviour
{
    public Slider ScannerCapacitySlider;
    public Text CurrentSearchCapacityText;
    public ScannerController ScannerController;
    public Button UpgradeScannerPowerButton;
    
    

    private float MaxCapacity;
    private float CurrentCapacity;

    private void Start()
    {
        UpgradeScannerPowerButton.onClick.AddListener(UpgradePower);
    }

    private void UpgradePower()
    {
        ScannerController.upgradeScannerPower();
    }

    void Update()
    {
        ScannerCapacitySlider.maxValue = Scanner.Instance.getSearchCapacity();
        float currentSearchCapacityUse = ScannerController.getCurrentSearchCapacityUse();
        ScannerCapacitySlider.value = currentSearchCapacityUse;
        CurrentSearchCapacityText.text = Math.Round(currentSearchCapacityUse, 1).ToString();
    }
}
