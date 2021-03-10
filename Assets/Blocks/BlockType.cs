
using UnityEngine;

public abstract class BlockType
{
    private Block block;

    public abstract string getBlockType();
    public abstract int getMaxHealth();
    public abstract void addMaterial();
    

}

public class DirtBlock : BlockType
{
    public void Spawn()
    {
        
    }

    public void Destroy()
    {
        throw new System.NotImplementedException();
    }

    public override int getMaxHealth()
    {
        return 5;
    }

    public override string getBlockType()
    {
        return "Dirt Block";
    }

    public override void addMaterial()
    {
        throw new System.NotImplementedException();
    }
}
