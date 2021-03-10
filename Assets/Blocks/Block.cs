using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class Block
{
    private GameObject BlockObject;
    private BlockType blockType;

    public Transform transform
    {
        get
        {
            return BlockObject.transform;
        }
    }

    public Block(float x, float y, GameObject blockObjectPrefab)
    {
        BlockObject = GameObject.Instantiate(blockObjectPrefab, new Vector3(x, y), Quaternion.identity);
        BlockObject.transform.localScale = Vector3.one * GameController.getScale();
    }

    public void highLightBlock()
    {
        BlockObject.GetComponent<SpriteRenderer>().material.SetColor(0, Color.blue);
    }

    public void stopHighlightBlock()
    {
        BlockObject.GetComponent<SpriteRenderer>().material.SetColor(0, Color.clear);
    }

    public BlockType getBlockType()
    {
        return blockType;
    }


    public void Destroy()
    {
        GameObject.Destroy(BlockObject);
    }
}
