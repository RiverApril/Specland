using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class TileTorch : Tile{

        public TileTorch(string name, int renderType, int textureX, int textureY) : base(name, renderType, textureX, textureY) { }

        public override void updateRandom(World world, int x, int y, bool isWall) {
            if (Game.rand.Next(10)==0) {
                world.setTileWithUpdate(x, y, Tile.TileAir.index, isWall);
            }
        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, bool isWall) {
            return new ItemStack(Item.itemEmpty);
        }

    }
}
