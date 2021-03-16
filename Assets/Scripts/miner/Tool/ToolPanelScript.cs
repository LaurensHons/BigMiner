using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class ToolPanelScript : MonoBehaviour
{
    public Slider XPSlider;
    public Image ToolImage;
    public Text ToolName;
    public Text DescriptionText;
    public Button UpgradeDamageButton;

    private Tool tool;

    public void setActive(Tool tool)
    {
        this.tool = tool;
        int lvl = tool.getLevel(out double xpPercentLeft);
        XPSlider.value = (float) xpPercentLeft;
        ToolName.text = tool.GetType().ToString();
        DescriptionText.text = tool.getDecriptionText();

        AsyncOperationHandle<Sprite> toolSpriteHandler = Addressables.LoadAssetAsync<Sprite>(tool.getSpritePath());
        toolSpriteHandler.Completed += LoadToolSpriteWhenReady;
    }

    public void updateUI()
    {
        int lvl = tool.getLevel(out double xpPercentLeft);
        XPSlider.value = (float) xpPercentLeft;
    }

    private void LoadToolSpriteWhenReady(AsyncOperationHandle<Sprite> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite toolSprite = obj.Result;
            
            if (toolSprite == null) throw new Exception("No tool sprite found");
            ToolImage.sprite = toolSprite;
        }
        else throw new Exception("Loading sprite failed");
    }

    public Tool getTool()
    {
        return tool;
    }
}
