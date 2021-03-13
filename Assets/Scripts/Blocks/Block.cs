
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Slider = UnityEngine.UIElements.Slider;

public abstract class Block : IStructure
{
    protected GameObject BlockObject;
    protected GameObject BlockSpriteRenderer;
    protected GameObject HealthBar;

    private PathNode pathNode;

    private Sprite BlockSprite;
    private GameObject healthBarPrefab;

    public int HP;

    protected Block(float x, float y, PathNode pathNode)
    {
        HandleSpriteLoading();
        
        this.pathNode = pathNode;
        
        BlockObject = new GameObject("Block [x:"  + x + ", y:" + y + "]");
        BlockObject.transform.position = new Vector3(x, y, 0);
        BlockObject.transform.localScale = Vector3.one;
        
        BlockSpriteRenderer = new GameObject("BlockSpriteRenderer");
        BlockSpriteRenderer.transform.localPosition = new Vector3(0, 0, 0);
        BlockSpriteRenderer.transform.localScale = Vector3.one * GameController.getBlockScale();
        BlockSpriteRenderer.AddComponent(typeof(SpriteRenderer));
        BlockSpriteRenderer.layer = 3;
        BlockSpriteRenderer.transform.SetParent(BlockObject.transform, false);
        
        
        

        HP = getMaxHealth();
    }

    private void HandleSpriteLoading()
    {
        AsyncOperationHandle<Sprite> BlockSpriteHandler = Addressables.LoadAssetAsync<Sprite>(getSpritePath());
        BlockSpriteHandler.Completed += LoadBlockSpriteWhenReady;
        
        
        AsyncOperationHandle<GameObject> HealthBaPrefabHandler = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/HealthBarPrefab.prefab");
        HealthBaPrefabHandler.Completed += LoadHealthBarWhenReady;
    }
    
    public void Mine(int hit, out bool destroyed)
    {
        if (HP - hit <= 0)
        {
            GameObject.Destroy(BlockObject);
            //GameObject.Destroy(HealthBar);
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
        return;             //TODO
        if (HealthBar == null)
        {
            
            if (healthBarPrefab == null) return;
            HealthBar = GameObject.Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity,
                BlockObject.transform);
            HealthBar.transform.position = BlockObject.transform.position;
            HealthBar.layer = 5;
            
            HealthBar.transform.localScale = new Vector3(0.019f, 0.019f, 0.019f);
            HealthBar.transform.Translate(0f, 0f,  -2);
            RectTransform rect = HealthBar.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.rect.Set(0f, 0f, 15f, 5f);
            HealthBar.GetComponent<RectTransform>().rect.Set(0f, 0f, 15f, 5f);

            //rect.offsetMax = new Vector2(-10, -5);
            //rect.offsetMin = new Vector2(-10, 50);

        }
        else
        {
            //float hpPercentage = HP / (float) getMaxHealth();
            //HealthBar.GetComponent<Slider>().value = hpPercentage;
            //HealthBar.transform.localScale = scale;
        }
       
    }
    
    private void LoadBlockSpriteWhenReady(AsyncOperationHandle<Sprite> handleToCheck)
    {
        if(handleToCheck.Status == AsyncOperationStatus.Succeeded)
        {
            BlockSprite = handleToCheck.Result;
            
            if (BlockSprite == null) Debug.Log("No block sprite found, maybe file named wrong?");
            else
            {
                BlockSpriteRenderer.GetComponent<SpriteRenderer>().sprite = BlockSprite;
            }
        }
    }

    private void LoadHealthBarWhenReady(AsyncOperationHandle<GameObject> handleToCheck)
    {
        if(handleToCheck.Status == AsyncOperationStatus.Succeeded)
        {
            healthBarPrefab = handleToCheck.Result;
        }
    }
    
    public void highLightBlock()
    {
        //BlockSpriteRenderer.GetComponent<SpriteRenderer>().material.SetColor(0, Color.blue);
    }

    public void stopHighlightBlock()
    {
        //BlockSpriteRenderer.GetComponent<SpriteRenderer>().material.SetColor(0, Color.clear);
    }

    public void setParent(Transform transform)
    {
        BlockObject.transform.SetParent(transform);
    }

    public Vector2 getPos()
    {
        return BlockObject.transform.position;
    }

    public List<PathNode> getPathNodeList()
    {
        return new List<PathNode>{pathNode};
    }

    public void setPathNode(PathNode pathNode)
    {
        this.pathNode = pathNode;
    }
                                                                
    public bool isResource()                                    
    {
        return true;
    }

    public void destroy()
    {
        GameObject.Destroy(BlockObject);
    }


    public abstract int getMaxHealth();
    public abstract void addMaterial();

    public abstract string getSpritePath();

    public abstract int getSearchCost();
    
}

public enum BlockTypes
{
    DirtBlock,
    StoneBlock 
}

public enum BlockTypeSearchCost
{
    DirtBlock = 2, 
    StoneBlock = 4
}
