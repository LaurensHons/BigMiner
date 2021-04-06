using System;
using System.IO;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ToolPanelScript : MonoBehaviour
{
    public Slider XPSlider;
    public Text LevelText;
    public Button SelectButton;
    public Text ToolName;
    public Image ToolImage;
    
    public Text DescriptionText;
    public Text SpeedText;
    public Text DamageText;
    public Text BaseDamageText;
    
    public Button UpgradeDamageButton;

    private Tool tool;
    private MinerToolController minerController;

    public void setActive(bool active, MinerToolController minerToolController = null, Tool tool = null)
    {
        if (minerToolController != null) this.minerController = minerToolController;
        if (tool != null) this.tool = tool;
        if (active)
        {
            ToolName.text = tool.GetType().ToString();
            DescriptionText.text = tool.getDecriptionText();
            BaseDamageText.text = "Base Damage\n" + Math.Round(tool.getBaseDamage(), 2);

            updateSelected(tool.isSelected);
            minerToolController.getMinerStation().Miner.toolSwitchUpdate += updateSelected;
            
            
            tool.ToolDamageUpdate += updateDamageText;
            updateDamageText(this, EventArgs.Empty);
            
            ToolImage.sprite = tool.getSprite();
        }
        else
        {
            if (tool != null)
            {
                tool.ToolXpUpdate -= updateXpBar;
                tool.ToolLevelUpdate -= updateLevelText;
                tool.ToolLevelUpdate -= updateSpeedText;
                tool.ToolDamageUpdate -= updateDamageText;
            }
        }
    }

    private void updateSelected(bool selected)
    {
        if (selected)
        {
            tool.ToolXpUpdate += updateXpBar;
            updateXpBar(this, EventArgs.Empty);
            tool.ToolLevelUpdate += updateLevelText;
            updateLevelText(this, EventArgs.Empty);
            tool.ToolLevelUpdate += updateSpeedText;
            updateSpeedText(this, EventArgs.Empty);
            SelectButton.gameObject.SetActive(false);
            
        }
        else
        {
            SelectButton.gameObject.SetActive(true);
            tool.ToolXpUpdate -= updateXpBar;
            tool.ToolLevelUpdate -= updateLevelText;
            tool.ToolLevelUpdate -= updateSpeedText;
        }
    }
    
    private void updateSelected(object obj, EventArgs eventArgs)
    {
        updateSelected(tool.isSelected);
    }

    private void updateXpBar(object obj, EventArgs eventArgs)
    {
        int lvl = tool.getLevel(out double xpPercentLeft);
        Debug.Log("percentleft: "+ xpPercentLeft);
        XPSlider.value = (float) xpPercentLeft;
    }

    private void updateLevelText(object obj, EventArgs eventArgs)
    {
        LevelText.text = "Level " + tool.getLevel(out double d);
    }

    private void updateSpeedText(object obj, EventArgs eventArgs)
    {
        SpeedText.text = "Speed\n" + Math.Round((double)1/( tool.getSpeed() * Time.fixedDeltaTime), 2) + " Attacks/Sec";
    }

    private void updateDamageText(object obj, EventArgs eventArgs)
    {
        DamageText.text = "Damage\n" + Math.Round((double) tool.damage, 2);
    }



    public void BuyDamageUpgrade()      //Damage Upgrade Button
    {
        if (Silo.Instance.buyItem(tool.getUpgradeCost()))
            tool.damageUpgrades += 1;
    }

    public void SelectTool()        //Select button
    {
        minerController.getMinerStation().Miner.setActiveTool(tool);
    }

    public Tool getTool()
    {
        return tool;
    }
}
