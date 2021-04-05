using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface IStructure
{
    public List<PathNode> getPathNodeList();
    public IStructure InstantiateBlock(float x, float y); 

    public bool isResource();

    public bool isDestroyed();
    public void destroy();
}
