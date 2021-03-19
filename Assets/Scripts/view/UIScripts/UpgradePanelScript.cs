using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UpgradePanelScript : MonoBehaviour
{
    public Text UpgradeNameText;
    public Image UpgradeImage;
    public Sprite UpgradeSprite;
    public Text UpgradeDescriptionText;
    public Button UpgradeButton;
    public Text UpgradeButtonText;

    private MinerUpgrade minerUpgrade;

    public void setActive(MinerUpgrade minerUpgrade)
    {
        if (minerUpgrade != null) this.minerUpgrade = minerUpgrade;
        UpgradeNameText.text = minerUpgrade.getName();

        AsyncOperationHandle<Sprite> upgradeSpriteHandler = Addressables.LoadAssetAsync<Sprite>(minerUpgrade.getSpritePath());
        upgradeSpriteHandler.Completed += LoadUpgradeSpriteWhenReady;
        
        UpgradeDescriptionText.text = minerUpgrade.getDescription();
        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(minerUpgrade.BuyUpgrade);
        UpgradeButtonText.text = "Buy";
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

}
