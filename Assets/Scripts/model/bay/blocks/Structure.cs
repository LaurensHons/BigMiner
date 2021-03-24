using System.Collections.Generic;
using UnityEngine;

public interface IStructure
{
    public List<PathNode> getPathNodeList();

    public bool isResource();

    public bool isDestroyed();
    public void destroy();
}
