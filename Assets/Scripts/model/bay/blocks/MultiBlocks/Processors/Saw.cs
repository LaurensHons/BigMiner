
using UnityEngine;

public class Saw : Processor
{
    public Saw(float x, float y, float speed, Tier tier) : base(x, y, speed, tier)
    {
        JobController.Instance.addJobCall(Silo.Instance, this, new DirtBlockItem(3));
    }

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
