
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour, IStructure
{
    protected GameObject BlockSpriteRenderer;
    protected GameObject HealthBar;

    public Sprite BlockSprite;
    public GameObject healthBarPrefab;

    public float HP;
    public bool isDestroyed { get; private set; } = false;

    public IStructure InstantiateBlock(float x, float y)
    {
        

        name = ("Block [x:"  + x + ", y:" + y + "]");
        transform.position = new Vector3(x, y, 0);
        transform.localScale = Vector3.one;
        transform.SetParent(GameObject.FindWithTag("Bay").GetComponent<Bay>().transform, false);
        
        BlockSpriteRenderer = new GameObject("BlockSpriteRenderer");
        BlockSpriteRenderer.transform.localPosition = new Vector3(0, 0, 0);
        BlockSpriteRenderer.AddComponent<Image>();
        BlockSpriteRenderer.layer = 3;
        BlockSpriteRenderer.transform.SetParent(transform, false);
        
        HandleSpriteLoading();

        HP = getMaxHealth();
        return this;
    }

    private void HandleSpriteLoading()
    {
        BlockSpriteRenderer.GetComponent<Image>().sprite = BlockSprite;
        BlockSpriteRenderer.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
    }
    
    public void Mine(float hit, out bool destroyed)
    {
        if (HP - hit <= 0)
        {
            destroyed = true;
            destroy();
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
        if (HealthBar == null)
        {
            
            if (healthBarPrefab == null) return;
            HealthBar = Instantiate(healthBarPrefab, Vector3.zero, Quaternion.identity, transform);
            HealthBar.transform.position = transform.position;
            HealthBar.layer = 5;
            
            HealthBar.transform.localScale = Vector3.one * 0.005517403f;
            HealthBar.transform.Translate(0f, 0.4f,  0);
            
            //rect.offsetMax = new Vector2(-10, -5);
            //rect.offsetMin = new Vector2(-10, 50);

        }
        
        float hpPercentage = HP / (float) getMaxHealth();
        HealthBar.GetComponent<Slider>().value = hpPercentage;
        
    }
    

    public void setParent(Transform transform)
    {
        transform.SetParent(transform);
    }

    public Vector2 getPos()
    {
        return transform.position;
    }

    public List<PathNode> getPathNodeList()
    {
        Bay bay = GameObject.FindWithTag("Bay").GetComponent<Bay>();
        return new List<PathNode> {bay.getPathNode((int) transform.position.x, (int) transform.position.y) };
    }

    public bool isResource()                                    
    {
        return true;
    }

    bool IStructure.isDestroyed()
    {
        return isDestroyed;
    }

    public void destroy()
    {
        GameObject.FindWithTag("Bay").GetComponent<Bay>().removeBlock(this);
        isDestroyed = true;
        Destroy(gameObject);
    }

    public override string ToString()
    {
        return "Block: " + name + ", HP: " + HP + ", Structure: " + GetType() + 
               "\nGameObject:[" + transform.position.x + "," + transform.position.y + "]";
    }


    public virtual int getMaxHealth()
    {
        return 1;
    }

    public virtual IInventory getLoot()
    {
        return null;
    }

    public virtual int getXpOnMine()
    {
        return 0;
    }

    public virtual string getSpritePath()
    {
        return null;
    }

    public virtual int getSearchCost()
    {
        return 1;
    }

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
