using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland.Items {
    public class ItemBucket : Item{

        public ItemBucket(string name, int renderType, Rectangle rectangle)
            : base(name, renderType, rectangle) {
        }


        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            int l = game.currentWorld.getLiquid(xTile, yTile);
            if (l>0) {
                game.currentWorld.setLiquid(xTile, yTile, 0);
                return new ItemStack(Item.itemBucketWater, 1, l);
            }
            return stack;
        }
    }
}
