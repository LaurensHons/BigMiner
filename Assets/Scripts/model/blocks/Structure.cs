using System.Collections.Generic;
using UnityEngine;

public interface IStructure
{
    public Vector2 getPos();
    public List<PathNode> getPathNodeList();

    public bool isResource();

    public void destroy();
}
