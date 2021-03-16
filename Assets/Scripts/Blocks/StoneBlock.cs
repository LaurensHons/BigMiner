using System.IO;
using UnityEngine;

namespace Blocks
{
    public class StoneBlock: Block
    {
        public StoneBlock(float x, float y, PathNode pathNode) : base(x, y, pathNode)
        {
        }

        public override int getMaxHealth()
        {
            return 5;
        }

        public override ItemInventory getLoot()
        {
            ItemInventory iteminv = new ItemInventory();
            iteminv.addItemToInventory(new DirtBlockItem(1), 1, out int actualAmount);
            return iteminv;
        }

        public override int getXpOnMine()
        {
            return 3;
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