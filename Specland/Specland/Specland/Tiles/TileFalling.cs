using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class TileFalling : Tile {

        public TileFalling(string name, RenderType renderType, Material material, int textureX, int textureY)
            : base(name, renderType, material, textureX, textureY) {

        }

        public override void updateNearChange(World world, int x, int y, int tileDepth) {
            if (world.getTileIndex(x, y + 1, tileDepth) == Tile.TileAir.index) {
                world.setTile(x, y, Tile.TileAir, tileDepth);
                world.EntityAddingList.Add(new EntityFallingTile(x*World.tileSizeInPixels, y*World.tileSizeInPixels, index, tileDepth));
                world.setTileForUpdate(x, y-1, tileDepth);
            }
        }


    }
}
