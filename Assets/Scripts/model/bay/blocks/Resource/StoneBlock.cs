using System.IO;
using UnityEngine;

namespace Blocks
{
    public class StoneBlock: Block
    {
        public override int getMaxHealth()
        {
            return 5;
        }

        public override IInventory getLoot()
        {
            IInventory iteminv = new ItemInventory();
            iteminv.AddItem(new StoneBlockItem(1), null);
            return iteminv;
        }

        public override int getXpOnMine()
        {
            return 3;
        }


        public override string getSpritePath()
        {
            return "StoneBlock";
        }

        public override int getSearchCost()
        {
            return 3;
        }
    }
}