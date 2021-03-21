using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UpgradePanelScript : MonoBehaviour
{
    public Text UpgradeNameText;
    public Image UpgradeImage;
    private Sprite UpgradeSprite;
    public Text UpgradeDescriptionText;
    public Text AmountText;
    public Button UpgradeButton;
    public GameObject UpgradeButtonText;

    [SerializeField]public MinerUpgrade minerUpgrade;

    public void LoadUpgrade(MinerUpgrade minerUpgrade)
    {
        this.minerUpgrade = minerUpgrade;
        if (minerUpgrade != null) this.minerUpgrade = minerUpgrade;
        else return;
        UpgradeNameText.text = minerUpgrade.getName();

        AsyncOperationHandle<Sprite> upgradeSpriteHandler = Addressables.LoadAssetAsync<Sprite>(minerUpgrade.getSpritePath());
        upgradeSpriteHandler.Completed += LoadUpgradeSpriteWhenReady;
        
        UpgradeDescriptionText.text = minerUpgrade.getDescription();
        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(buyUpgrade);
        
        updateAmountText();
        updateBuyButtonText();
    }

    private void LoadUpgradeSpriteWhenReady(AsyncOperationHandle<Sprite> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            UpgradeSprite = obj.Result;
            
            if (UpgradeSprite == null) throw new Exception("No block sprite found, maybe file named wrong?");
            UpgradeImage.sprite = UpgradeSprite;
        }
        else throw new Exception("Loading sprite failed");
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
