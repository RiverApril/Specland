using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class TileFalling : Tile {
        
        public TileFalling(string name, int renderType, int textureX, int textureY) : base(name, renderType, textureX, textureY){

        }

        public override void updateNearChange(World world, int x, int y, bool isWall) {
            if (world.getTileIndex(x, y + 1, isWall) == Tile.TileAir.index) {
                world.setTile(x, y, Tile.TileAir, isWall);
                world.EntityList.Add(new EntityFallingTile(x*World.tileSizeInPixels, y*World.tileSizeInPixels, index, isWall));
                world.setTileForUpdate(x, y-1, isWall);
            }
        }


    }
}
