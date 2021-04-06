using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiBlock : MonoBehaviour, IStructure
{
    protected GameObject BlockSpriteRenderer;
    protected Bay bay;
    protected List<PathNode> pathNodeList;

    protected Vector2 temporaryPos;
    protected GameObject temporaryGameObject;
    private bool canPlaceTemporary;

    public bool CanPlaceTemporary => canPlaceTemporary;
    public bool isDestroyed { get; private set; } = false;

    public Sprite BlockSprite;

    protected float baseX;
    protected float baseY;
    
    public IStructure InstantiateBlock(float x, float y)
    {
        this.bay = GameObject.FindWithTag("Bay").GetComponent<Bay>();
        moveBlockStructure(new Vector2(x, y));
        return this;
    }

    public List<PathNode> getPathNodeList()
    {
        return getPathNodeList(new Vector2(baseX, baseY));
    }

    public List<PathNode> getPathNodeList(Vector2 pos)
    {
        float width = getDimensions().x, height = getDimensions().y;

        List<PathNode> pathNodes = new List<PathNode>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                pathNodes.Add(bay.getPathNode((int) pos.x + x, (int) pos.y + y));

            }
        }

        return pathNodes;
    }

    public void moveBlockStructure(Vector2 pos)
    {
        baseX = pos.x;
        baseY = pos.y;
        
         
            
        float width = getDimensions().x, height = getDimensions().y;

        name = ("MultiBlock [x:" + baseX + ", y:" + baseY + ", width:" + width + ", height:" + height + "]");

        float xOffset = (width - 1) / 2;
        float yOffset = (height - 1) / 2;
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);
        transform.SetParent(bay.transform);
        transform.position = new Vector2(baseX + xOffset, baseY + yOffset);
        //BlockObject.transform.localScale = new Vector2(width, height);

        BlockSpriteRenderer = new GameObject("BlockSpriteRenderer");
        BlockSpriteRenderer.transform.localPosition = Vector3.zero;
        BlockSpriteRenderer.layer = 3;
        BlockSpriteRenderer.transform.SetParent(transform, false);


        
        BlockSpriteRenderer.AddComponent<Image>().sprite = BlockSprite;
        RectTransform rectt = BlockSpriteRenderer.GetComponent<RectTransform>();
        rectt.anchoredPosition = transform.position;
        rectt.anchorMin = Vector2.zero;
        rectt.anchorMax = Vector2.zero;
        rectt.localPosition = Vector3.zero;
        rectt.sizeDelta = new Vector2(width, height);
        

        gameObject.AddComponent<MultiBlockGameObjectScript>();
        GetComponent<MultiBlockGameObjectScript>().setStructure(this);

        BoxCollider2D bc = gameObject.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(width, height);
    }

    public void moveTemporaryStructure(Vector2 pos)
    {
        temporaryPos = pos;
        if (temporaryGameObject == null)
        {
            temporaryGameObject = new GameObject("Temp Multiblock");
            temporaryGameObject.transform.localPosition = Vector3.zero;
            temporaryGameObject.layer = 3;
            temporaryGameObject.transform.SetParent(bay.transform, false);
            MultiBlockGameObjectScript mbgos = temporaryGameObject.AddComponent<MultiBlockGameObjectScript>();
            mbgos.setStructure(this);
            mbgos.isTemporaryStructure = true;

            temporaryGameObject.transform.position = new Vector2(temporaryPos.x, temporaryPos.y);

            BoxCollider2D bc = temporaryGameObject.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(getDimensions().x, getDimensions().y);

            
            temporaryGameObject.AddComponent<Image>().sprite = BlockSprite;
            RectTransform rect = temporaryGameObject.GetComponent<RectTransform>();
            rect.anchoredPosition = temporaryGameObject.transform.position;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.localPosition = Vector3.zero;
            rect.sizeDelta = new Vector2(getDimensions().x, getDimensions().y);
            
            
            EditController.addTemporaryPlacement(this);
        }
        else
        {
            temporaryGameObject.transform.position = new Vector2(temporaryPos.x, temporaryPos.y);
        }

        canPlaceTemporary = true;

        foreach (var pathNode in getPathNodeList(pos))
        {
            if (pathNode == null || (!pathNode.isWalkable && pathNode.structure != this))
                canPlaceTemporary = false;
        }

        Image image = temporaryGameObject.GetComponent<Image>();
        temporaryGameObject.GetComponent<MultiBlockGameObjectScript>().canPlace = canPlaceTemporary;
        if (image == null) return;
    }

    public void commitTemporaryStructure()
    {
        if (!canPlaceTemporary)
            throw new Exception("Cannot commit changes, structure is in a occupied space");
        else
        {
            baseX = temporaryPos.x;
            baseY = temporaryPos.y;

            transform.position = temporaryPos;
            Destroy(temporaryGameObject);
        }
    }

    public void cancelTemporaryPlacement()
    {
        Destroy(temporaryGameObject);
    }

    bool IStructure.isDestroyed()
    {
        return isDestroyed;
    }
    
    public  Vector2 getPos()
    {
        return bay.getPathNode((int) baseX, (int) baseY).getPos();
    }

    public void destroy()
    {
        Destroy(this);
    }

    public virtual void onClick() { }

    public virtual Vector2 getDimensions() { return Vector2.one * 2; }

    public virtual bool isResource() { return false; }
    
    public virtual string getSpritePath() { return null; }
}
