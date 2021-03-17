using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Analytics;
using UnityEngine.Animations;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Random = System.Random;

public class MinerController : MonoBehaviour
{
    private MinerStation minerstation;

    public Text BigMinerNameText;

    public Slider LVLSlider;
    public Text levelText;
    
    public Text NameText;
    public Text DamageText;
    public Text SpeedText;
    public Text BlocksMinedText;

    public Image MinerSprite;

    public Image BatteryImage;
    public Text BatteryText;

    public Text MiningStrategyText;

    public GameObject ToolList;
    public Dictionary<Tool, ToolPanelScript> toolPanels = new Dictionary<Tool, ToolPanelScript>();
    private String ToolPanelPrefabPath = "Assets/Prefabs/ToolPrefab.prefab";
    private GameObject ToolPanelPrefab;
    
    private MiningStrategy[] miningStrategies = (MiningStrategy[]) Enum.GetValues(typeof(MiningStrategy)).Cast<MiningStrategy>();
    private int miningStrategyIndex = 0;

    private float DefaultBatteryWidth;      //gets battery width at starts and scales the battery according to this value;
    
    public void Start()
    {
        BatteryImage.gameObject.AddComponent<BatteryClickController>();
        DefaultBatteryWidth = BatteryImage.GetComponent<RectTransform>().rect.width;
        
        AsyncOperationHandle<GameObject> toolpanelHandler = Addressables.LoadAssetAsync<GameObject>(ToolPanelPrefabPath);
        toolpanelHandler.Completed += LoadToolPanels;
    }

    public void loadTools()
    {
        if (ToolPanelPrefab == null) throw new Exception("Tool Panel Prefab not found");
        
        foreach (var tool in minerstation.Miner.toolList)
        {
            if (!toolPanels.ContainsKey(tool))
            {
                GameObject ToolPanel = Instantiate(ToolPanelPrefab, ToolList.transform);
                toolPanels.Add(tool, ToolPanel.GetComponent<ToolPanelScript>());
            }
            Debug.Log(tool.GetType());
            toolPanels[tool].setActive(this, tool, true);
        }
    }

    private void LoadToolPanels(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject toolPanelprefab = obj.Result;
            
            if (toolPanelprefab == null) throw new Exception("Tool Panel Prefab not found");
            ToolPanelPrefab = toolPanelprefab;
        }
        else throw new Exception("Loading Tool Panel Prefab failed");
    }

    public MinerStation getMinerStation()
    {
        return minerstation;
    }

    public void setMinerStation(MinerStation minerStation)
    {
        minerstation = minerStation;
        Debug.Log(this.minerstation);
    }

    public void setActive(bool Active)
    {
        if (Active)
        {
            minerstation.Miner.MinerXpUpdate += updateXpBar;
            minerstation.Miner.MinerLevelUpdate += updateLevelText;
            minerstation.Miner.MinerLevelUpdate += updateSpeedText;
            minerstation.Miner.activeTool.ToolDamageUpdate += updateDamageText;
            
            minerstation.Miner.MinerXpUpdate(this, EventArgs.Empty);
            minerstation.Miner.MinerLevelUpdate(this, EventArgs.Empty);
            minerstation.Miner.activeTool.ToolDamageUpdate(this, EventArgs.Empty);
                
            if (MinerSprite.sprite == null)
            {
                AsyncOperationHandle<Sprite> minerSpriteHandler = Addressables.LoadAssetAsync<Sprite>(minerstation.Miner.getSpritePath());
                minerSpriteHandler.Completed += LoadminerSpriteWhenReady; 
            }
            
        
            InvokeRepeating("updateUI", 0, 1f);
        }
        else
        {
            minerstation.Miner.MinerXpUpdate = null;
            minerstation.Miner.MinerLevelUpdate = null;
            minerstation.Miner.activeTool.ToolDamageUpdate = null;
            
            CancelInvoke();
        }
    }

    private void updateUI()
    {
        updateBattery();
        updateBlocksMined(this, EventArgs.Empty);
    }
    private void updateXpBar(object obj, EventArgs eventArgs)
    {
        int level = minerstation.Miner.getLevel(out double percentLeft);
        levelText.text = "Level " + level;
        LVLSlider.value = (float) percentLeft;
    }

    private void updateLevelText(object obj, EventArgs eventArgs)
    {
        levelText.text = "Level " + minerstation.Miner.getLevel(out double d);
    }
    private void updateNameText(object obj, EventArgs eventArgs)
    {
        NameText.text = minerstation.Miner.name;
    }
    private void updateSpeedText(object obj, EventArgs eventArgs)
    {
        SpeedText.text = "Speed\n" + Math.Round(1/minerstation.Miner.speed, 2) + " M/Sec";
    }
    private void updateDamageText(object obj, EventArgs eventArgs)
    {
        DamageText.text = "Damage\n" + Math.Round(minerstation.Miner.damage, 2);
    }

    private void updateBlocksMined(object obj, EventArgs eventArgs)
    {
        BlocksMinedText.text = "Blocks Mined\n" + minerstation.Miner.blocksMined;
    }
    
    

    private void updateBattery()
    {
        int battery = minerstation.Miner.Battery;
        float maxBattery = minerstation.Miner.maxBattery;
        BatteryImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,(battery / maxBattery) * DefaultBatteryWidth);

        int batterySeconds = (int) Math.Round(battery * Time.fixedDeltaTime);
        
        if (batterySeconds < 100)
        {
            setBatteryString(batterySeconds + " Sec until empty");
            return;
        }
        int batteryMinutes = batterySeconds / 60;
        if (batteryMinutes < 100)
        {
            setBatteryString(batteryMinutes + " Min until empty");
            return;
        }

        int batteryHours = batteryMinutes / 60;
        setBatteryString(batteryHours + " Hrs until empty");
    }

    

    private void setBatteryString(String s)
    {
        BatteryText.text = s;
    }

    public void selectPreviousMiningStrategy()
    {
        if (miningStrategyIndex == 0)
            miningStrategyIndex = miningStrategies.Length;
        miningStrategyIndex--;
        minerstation.Miner.miningStrategy = miningStrategies[miningStrategyIndex];
        MiningStrategyText.text = getMiningStrategyString(minerstation.Miner.miningStrategy);

    }

    public void selectNextMiningStrategy()
    {
        if (miningStrategyIndex == miningStrategies.Length - 1)
            miningStrategyIndex = -1;
        miningStrategyIndex++;
        minerstation.Miner.miningStrategy = miningStrategies[miningStrategyIndex];
        MiningStrategyText.text = getMiningStrategyString(minerstation.Miner.miningStrategy);
    }
    
    public Tool getActiveTool()
    {
        return minerstation.Miner.activeTool;
    }
    
    private Vector3 cameraOriginPosition;
    private Vector3 cameraDifference;
    private bool Drag;
    
    public void DragBattery()
    {
        if (Input.GetMouseButton (0)) {
            cameraDifference = (Camera.main.ScreenToWorldPoint (Input.mousePosition))- Camera.main.transform.position;
            if (Drag == false){
                Drag = true;
                cameraOriginPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            }
        } else {
            Drag = false;
        }
        if (Drag == true){
            int onePercentBattery = minerstation.Miner.maxBattery / 100;
            int batteryValue = (int) Math.Round(
                onePercentBattery * 20 * (cameraDifference.x - cameraOriginPosition.x));
            if (batteryValue <= minerstation.Miner.Battery) return;
            if (batteryValue > minerstation.Miner.maxBattery)
                batteryValue = minerstation.Miner.maxBattery;
            minerstation.Miner.Battery = batteryValue;
            Debug.Log("Batterydrag: " + 20 * (cameraDifference.x - cameraOriginPosition.x));
        }
    }


    private string getMiningStrategyString(MiningStrategy miningStrategy)
    {
        switch (miningStrategy)
        {
            case MiningStrategy.Random:
                return "Random Block";
            case MiningStrategy.Closest:
               return "Closest Block";
            case MiningStrategy.MinValue:
                return "Lowest Value Block";
            case MiningStrategy.MaxValue:
                return "Highest Value Block";
        }
        return null;
    }

    private void LoadminerSpriteWhenReady(AsyncOperationHandle<Sprite> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite minerSprite = obj.Result;
            
            if (minerSprite == null) throw new Exception("No block sprite found, maybe file named wrong?");
            MinerSprite.sprite = minerSprite;
        }
        else throw new Exception("Loading sprite failed");
    }

    
}
