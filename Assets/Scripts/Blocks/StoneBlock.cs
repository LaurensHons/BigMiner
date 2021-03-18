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

        public override Inventory getLoot()
        {
            Inventory iteminv = new Inventory();
            iteminv.AddItem(new StoneBlockItem(1), null);
            return iteminv;
        }

        public override int getXpOnMine()
        {
            return 3;
        }


        public override string getSpritePath()
        {
            return "Assets/Images/stone.png";
        }

        public override int getSearchCost()
        {
            return 3;
        }
    }
}