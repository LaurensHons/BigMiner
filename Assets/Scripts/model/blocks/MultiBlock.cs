using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public abstract class MultiBlock : IStructure
{
    protected GameObject BlockObject;
    protected GameObject BlockSpriteRenderer;
    protected Bay bay;
    protected List<PathNode> pathNodeList;

    protected Vector2 temporaryPos;
    protected GameObject temporaryGameObject;
    private bool canPlaceTemporary;

    public bool CanPlaceTemporary => canPlaceTemporary;

    protected PathNode interfaceNode
    {
        get { return getInterfaceNode(); }
    }

    private Sprite BlockSprite;

    protected float baseX;
    protected float baseY;

    protected MultiBlock(float x, float y, Bay bay)
    {


        this.bay = bay;
        baseX = x;
        baseY = y;

        float width = getDimensions().x, height = getDimensions().y;

        BlockObject =
            new GameObject("MultiBlock [x:" + x + ", y:" + y + ", width:" + width + ", height:" + height + "]");

        float xOffset = (width - 1) / 2;
        float yOffset = (height - 1) / 2;
        RectTransform rect = BlockObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);
        BlockObject.transform.SetParent(bay.transform);
        BlockObject.transform.position = new Vector2(x + xOffset, y + yOffset);
        //BlockObject.transform.localScale = new Vector2(width, height);

        BlockSpriteRenderer = new GameObject("BlockSpriteRenderer");
        BlockSpriteRenderer.transform.localPosition = Vector3.zero;
        BlockSpriteRenderer.layer = 3;
        BlockSpriteRenderer.transform.SetParent(BlockObject.transform, false);


        AsyncOperationHandle<Sprite> spriteHandler = Addressables.LoadAssetAsync<Sprite>(getSpritePath());
        spriteHandler.Completed += obj =>
        {
            BlockSpriteRenderer.AddComponent<Image>().sprite = obj.Result;
            RectTransform rect = BlockSpriteRenderer.GetComponent<RectTransform>();
            rect.anchoredPosition = BlockObject.transform.position;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.localPosition = Vector3.zero;
            rect.sizeDelta = new Vector2(width, height);
        };

        BlockObject.AddComponent<MultiBlockGameObjectScript>();
        BlockObject.GetComponent<MultiBlockGameObjectScript>().setStructure(this);

        BoxCollider2D bc = BlockObject.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(width, height);
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

            AsyncOperationHandle<Sprite> spriteHandler = Addressables.LoadAssetAsync<Sprite>(getSpritePath());
            spriteHandler.Completed += obj =>
            {
                temporaryGameObject.AddComponent<Image>().sprite = obj.Result;
                RectTransform rect = temporaryGameObject.GetComponent<RectTransform>();
                rect.anchoredPosition = temporaryGameObject.transform.position;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.localPosition = Vector3.zero;
                rect.sizeDelta = new Vector2(getDimensions().x, getDimensions().y);
            };
            
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

            BlockObject.transform.position = temporaryPos;
            GameObject.Destroy(temporaryGameObject);
        }
    }

    public void cancelTemporaryPlacement()
    {
        GameObject.Destroy(temporaryGameObject);
    }

    public Vector2 getPos()
    {
        return interfaceNode.getPos();
    }

    public abstract void onClick();

    public abstract Vector2 getDimensions();
    public abstract bool isResource();
    public BoxCollider2D getBoxCollider()
    {
        return BlockObject.GetComponent<BoxCollider2D>();
    }
    
    public abstract PathNode getInterfaceNode();
    
    
    public abstract void destroy();
    public abstract string getSpritePath();
}
