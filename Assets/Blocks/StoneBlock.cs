using UnityEngine;

namespace Blocks
{
    public class StoneBlock: Block
    {
        public StoneBlock(float x, float y) : base(x, y)
        {
        }

        public override int getMaxHealth()
        {
            return 5;
        }

        public override void addMaterial()
        {
            throw new System.NotImplementedException();
        }

        public override string getSpritePath()
        {
            return "Assets/Images/StoneBlock.png";
        }

        public override int getSearchCost()
        {
            return 3;
        }
    }
}