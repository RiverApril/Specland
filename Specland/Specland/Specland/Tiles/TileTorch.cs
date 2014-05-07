using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class TileTorch : Tile{

        public TileTorch(string name, RenderType renderType, Material material, int textureX, int textureY) : base(name, renderType, material, textureX, textureY) { }

        public override void updateRandom(World world, int x, int y, int tileDepth) {
            if (Game.rand.Next(10)==0) {
                world.setTileWithUpdate(x, y, Tile.TileAir.index, tileDepth);
            }
        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, int tileDepth) {
            return new ItemStack(Item.itemEmpty);
        }

    }
}
