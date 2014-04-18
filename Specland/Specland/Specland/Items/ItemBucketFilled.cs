using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland.Items {
    public class ItemBucketFilled : Item {

        public ItemBucketFilled(string name, int renderType, Rectangle rectangle)
            : base(name, renderType, rectangle) {
        }


        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            int l = game.currentWorld.getLiquid(xTile, yTile);
            if (l + stack.getData() > 100) {
                game.currentWorld.setLiquid(xTile, yTile, 100);
                return new ItemStack(Item.itemBucketWater, 1, (l + stack.getData()) - 100);
            } else {
                game.currentWorld.setLiquid(xTile, yTile, l + stack.getData());
                return new ItemStack(Item.itemBucket, 1, 0);
            }
        }

        public override string getDisplayName(int count, int data) {
            return displayName + (" "+data+"% Full");
        }
    }
}
