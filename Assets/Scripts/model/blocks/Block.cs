
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public abstract class Block : IStructure
{
    protected GameObject BlockObject;
    protected GameObject BlockSpriteRenderer;
    protected GameObject HealthBar;

    private PathNode pathNode;

    private Sprite BlockSprite;
    private GameObject healthBarPrefab;

    public float HP;

    protected Block(float x, float y, PathNode pathNode)
    {
        HandleSpriteLoading();
        
        this.pathNode = pathNode;
        pathNode.SetStructure(this);

        BlockObject = new GameObject("Block [x:"  + x + ", y:" + y + "]");
        BlockObject.transform.position = new Vector3(x, y, 0);
        BlockObject.transform.localScale = Vector3.one;
        
        
        BlockSpriteRenderer = new GameObject("BlockSpriteRenderer");
        BlockSpriteRenderer.transform.localPosition = new Vector3(0, 0, 0);
        BlockSpriteRenderer.AddComponent<Image>();
        BlockSpriteRenderer.layer = 3;
        BlockSpriteRenderer.transform.SetParent(BlockObject.transform, false);
        
        BoxCollider2D bc = BlockObject.AddComponent<BoxCollider2D>();


        HP = getMaxHealth();
    }

    private void HandleSpriteLoading()
    {
        AsyncOperationHandle<Sprite> BlockSpriteHandler = Addressables.LoadAssetAsync<Sprite>(getSpritePath());
        BlockSpriteHandler.Completed += LoadBlockSpriteWhenReady;
        
        
        AsyncOperationHandle<GameObject> HealthBaPrefabHandler = Addressables.LoadAssetAsync<GameObject>("Assets/Addressables/Prefabs/HealthBarPrefab.prefab");
        HealthBaPrefabHandler.Completed += LoadHealthBarWhenReady;
    }
    
    public void Mine(float hit, out bool destroyed)
    {
        if (HP - hit <= 0)
        {
            GameObject.Destroy(BlockObject);
            GameObject.FindWithTag("Bay").GetComponent<Bay>().removeBlock(this);
            destroyed = true;
        }
        else
        {
            HP = HP - hit;
            UpdateHealthBar();
            destroyed = false;
        }
    }
    
    private void LoadBlockSpriteWhenReady(AsyncOperationHandle<Sprite> handleToCheck)
    {
        if(handleToCheck.Status == AsyncOperationStatus.Succeeded)
        {
            BlockSprite = handleToCheck.Result;
            
            if (BlockSprite == null) throw new Exception("No block sprite found, maybe file named wrong?");
            else
            {
                BlockSpriteRenderer.GetComponent<Image>().sprite = BlockSprite;
                BlockSpriteRenderer.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
            }
        }

        else throw new Exception("Loading sprite failed");
    }

    private void LoadHealthBarWhenReady(AsyncOperationHandle<GameObject> handleToCheck)
    {
        if(handleToCheck.Status == AsyncOperationStatus.Succeeded)
        {
            healthBarPrefab = handleToCheck.Result;
        }
        else throw new Exception("Loading HealthBar failed");
    }

    public void UpdateHealthBar()
    {
        if (HealthBar == null)
        {
            
            if (healthBarPrefab == null) return;
            HealthBar = GameObject.Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity,
                BlockObject.transform);
            HealthBar.transform.position = BlockObject.transform.position;
            HealthBar.layer = 5;
            
            HealthBar.transform.localScale = Vector3.one * 0.005517403f;
            HealthBar.transform.Translate(0f, 0.4f,  0);
            
            //rect.offsetMax = new Vector2(-10, -5);
            //rect.offsetMin = new Vector2(-10, 50);

        }
        
        float hpPercentage = HP / (float) getMaxHealth();
        HealthBar.GetComponent<Slider>().value = hpPercentage;
        
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
        return new List<PathNode>{ pathNode };
    }

    public bool isResource()                                    
    {
        return true;
    }

    public BoxCollider2D getBoxCollider()
    {
        return BlockObject.GetComponent<BoxCollider2D>();
    }

    public override string ToString()
    {
        return "Block: " + this.GetType().Name + ", HP: " + HP + ", Structure: " + GetType() + 
               "\nPathNode:[" + pathNode.x + "," + pathNode.y + "], GameObject:[" + BlockObject.transform.position.x + "," + BlockObject.transform.position.y;
    }


    public abstract int getMaxHealth();
    public abstract Inventory getLoot();

    public abstract int getXpOnMine();

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
