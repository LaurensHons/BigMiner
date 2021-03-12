
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public abstract class Block
{
    protected GameObject BlockObject;
    protected GameObject HealthBar;

    private Sprite BlockSprite;
    private Sprite healthBarSprite;
    
    public int HP;
    
    public Transform transform
    {
        get
        {
            return BlockObject.transform;
        }
    }
    
    protected Block(float x, float y)
    {
        HandleSpriteLoading();
        
        BlockObject = new GameObject("Block [x:"  + x + ", y:" + y + "]");
        BlockObject.AddComponent(typeof(SpriteRenderer));
        
        BlockObject.transform.position = new Vector3(x, y);
        BlockObject.transform.localScale = Vector3.one * GameController.blockScale; 
        
        BlockObject.transform.localScale = Vector3.one * GameController.getBlockScale();
        
        

        HP = getMaxHealth();
    }

    private void HandleSpriteLoading()
    {
        AsyncOperationHandle<Sprite> HealthBarSpriteHandler = Addressables.LoadAssetAsync<Sprite>("Assets/Images/HealthBar.png");
        HealthBarSpriteHandler.Completed += LoadHealthBarWhenReady;
        
        AsyncOperationHandle<Sprite> BlockSpriteHandler = Addressables.LoadAssetAsync<Sprite>(getSpritePath());
        BlockSpriteHandler.Completed += LoadBlockSpriteWhenReady;
    }
    
    public void Mine(int hit, out bool destroyed)
    {
        if (HP - hit <= 0)
        {
            GameObject.Destroy(BlockObject);
            GameObject.Destroy(HealthBar);
            destroyed = true;
        }
        else
        {
            HP = HP - hit;
            UpdateHealthBar();
            destroyed = false;
        }
    }

    public void UpdateHealthBar()
    {
        if (HealthBar == null && healthBarSprite != null)
        {
            HealthBar = new GameObject("HealthBar");
            HealthBar.transform.SetParent(BlockObject.transform);
            
            HealthBar.AddComponent(typeof(SpriteRenderer));
            
            if (HealthBar == null) Debug.Log("No healthbar sprite found");
            else HealthBar.GetComponent<SpriteRenderer>().sprite = healthBarSprite;

            HealthBar.transform.localPosition = new Vector3(0, 6f / 5f, -1); 
        }
        
        float hpPercentage = HP / (float) getMaxHealth();
        Vector3 scale = new Vector3(0.9f * hpPercentage, GameController.blockScale * 0.4f);
        HealthBar.transform.localScale = scale;
    }
    
    private void LoadBlockSpriteWhenReady(AsyncOperationHandle<Sprite> handleToCheck)
    {
        if(handleToCheck.Status == AsyncOperationStatus.Succeeded)
        {
            BlockSprite = handleToCheck.Result;
            
            if (BlockSprite == null) Debug.Log("No block sprite found, maybe file named wrong?");
            else
            {
                BlockObject.GetComponent<SpriteRenderer>().sprite = BlockSprite;
                BlockObject.layer = 3;
            }
        }
    }

    private void LoadHealthBarWhenReady(AsyncOperationHandle<Sprite> handleToCheck)
    {
        if(handleToCheck.Status == AsyncOperationStatus.Succeeded)
        {
            healthBarSprite = handleToCheck.Result;
        }
    }
    
    public void highLightBlock()
    {
        this.BlockObject.GetComponent<SpriteRenderer>().material.SetColor(0, Color.blue);
    }

    public void stopHighlightBlock()
    {
        BlockObject.GetComponent<SpriteRenderer>().material.SetColor(0, Color.clear);
    }

    public void setParent(Transform parent)
    {
        BlockObject.transform.SetParent(parent);
    }


    public abstract int getMaxHealth();
    public abstract void addMaterial();

    public abstract string getSpritePath();

    public abstract int getSearchCost();
}

public enum BlockTypes
{
    DirtBlock ,
    StoneBlock 
}

public enum BlockTypeSearchCost
{
    DirtBlock = 2, 
    StoneBlock = 4
}
