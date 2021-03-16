using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class MultiBlock : IStructure
{
    protected GameObject BlockObject;
    protected Bay bay;
    protected List<PathNode> pathNodeList;

    protected PathNode interfaceNode
    {
        get
        {
            return getInterfaceNode();
        }
    }

    private Sprite BlockSprite;

    protected float baseX;
    protected float baseY;

    protected MultiBlock(float x, float y, Bay bay)
    {
        HandleSpriteLoading();

        this.bay = bay;
        baseX = x;
        baseY = y;

        float width = getDimensions().x, height = getDimensions().y;
        
        BlockObject = new GameObject("MultiBlock [x:"  + x + ", y:" + y + ", width:" + width + ", height:" + height + "]");
        BlockObject.AddComponent(typeof(SpriteRenderer));

        float xOffset = (width - 1 )/ 2;
        float yOffset = (height - 1) / 2;
        
        BlockObject.transform.position = new Vector3(x + xOffset, y + yOffset);
        BlockObject.transform.localScale = new Vector3(GameController.getBlockScale() * width,
            GameController.getBlockScale() * height);
        BlockObject.transform.SetParent(bay.transform);

        
        setPathNodeList();
    }

    private void setPathNodeList()
    {
        float width = getDimensions().x, height = getDimensions().y;

        List<PathNode> pathNodes = new List<PathNode>();

        for (int x = (int) baseX; x < width; x++)
        {
            for (int y = (int) baseY ; y < height; y++)
            {
                pathNodes.Add(bay.getPathNode((int) baseX + x, (int) baseY + y));
            }
        }

        pathNodeList = pathNodes;
    }

    public List<PathNode> getPathNodeList()
    {
        float width = getDimensions().x, height = getDimensions().y;

        List<PathNode> pathNodes = new List<PathNode>();

        for (int x = (int) baseX; x < width; x++)
        {
            for (int y = (int) baseY ; y < height; y++)
            {
                pathNodes.Add(bay.getPathNode((int) baseX + x, (int) baseY + y));
            }
        }

        return pathNodes;
    }
    
    private void HandleSpriteLoading()
    {
        AsyncOperationHandle<Sprite> HealthBarSpriteHandler = Addressables.LoadAssetAsync<Sprite>(getSpritePath());
        HealthBarSpriteHandler.Completed += LoadSpriteWhenReady;
    }
    
    private void LoadSpriteWhenReady(AsyncOperationHandle<Sprite> handleToCheck)
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

    public Vector2 getPos()
    {
        return interfaceNode.getPos();
    }

    public abstract Vector2 getDimensions();
    public abstract bool isResource();



    public abstract PathNode getInterfaceNode();
    
    
    public abstract void destroy();
    public abstract string getSpritePath();
}
