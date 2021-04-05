
using UnityEngine;

public class Saw : Processor
{
    public override Vector2 getDimensions()
    {
        return new Vector2(2, 2);
    }

    public override string getSpritePath()
    {
        return "Assets/Addressables/Blocks/Saw.png";
    }

    public override string getName()
    {
        return currentTier + " Saw";
    }

    public override Item[] getBaseInputItems()
    {
        return new Item[]
        {
            new DirtBlockItem(2),
        };
    }

    public override Item[] getOutputItems()
    {
        return new Item[]
        {
            new StoneBlockItem(2),
        };
    }
}
