using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelScript : MonoBehaviour
{
    public Text UpgradeNameText;
    public Image UpgradeImage;
    public Text UpgradeDescriptionText;
    public Text AmountText;
    public Button UpgradeButton;
    public GameObject UpgradeButtonText;

    public Sprite UpgradeSprite;
    [SerializeField]public MinerUpgrade minerUpgrade;

    public void LoadUpgrade(MinerUpgrade minerUpgrade)
    {
        UpgradeImage.sprite = UpgradeSprite;
        this.minerUpgrade = minerUpgrade;
        if (minerUpgrade != null) this.minerUpgrade = minerUpgrade;
        else return;
        UpgradeNameText.text = minerUpgrade.getName();

        UpgradeDescriptionText.text = minerUpgrade.getDescription();
        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(buyUpgrade);
        
        updateAmountText();
        updateBuyButtonText();
    }

    private void buyUpgrade()
    {
        minerUpgrade.BuyUpgrade();
        updateAmountText();
        updateBuyButtonText();
    }

    public void updateAmountText()
    {
        AmountText.text = minerUpgrade.getAmount() + "/" + minerUpgrade.getMaxAmount();
    }

    public void updateBuyButtonText()
    {
        UpgradeButtonText.GetComponent<ButtonTextPrefab>().Load(new List<Item>(minerUpgrade.getUpgradeCost()));
    }

}
