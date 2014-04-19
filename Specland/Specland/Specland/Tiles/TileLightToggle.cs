using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class TileLightToggle : Tile{

        public TileLightToggle(string name, int renderType, int textureX, int textureY) : base(name, renderType, textureX, textureY) { }

        public override ItemStack use(Game game, ItemStack currentItem, int mouseTileX, int mouseTileY, int mouseTileDistanceFromPlayer, bool isWall) {
            game.currentWorld.setTileWithDataWithUpdate(mouseTileX, mouseTileY, index, game.currentWorld.getTileData(mouseTileX, mouseTileY, isWall)==0?1:0, isWall);
            return base.use(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer, isWall);
        }

        public override int getLight(int x, int y, bool isWall) {
            return Game.instance.currentWorld.getTileData(x, y, isWall)==0?base.getLight(x, y, isWall):0;
        }
    }
}
