using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class TileLightToggle : Tile{

        public TileLightToggle(string name, int renderType, int material, int textureX, int textureY) : base(name, renderType, material, textureX, textureY) { }

        public override ItemStack use(Game game, ItemStack currentItem, int mouseTileX, int mouseTileY, int mouseTileDistanceFromPlayer, bool isWall) {
            game.currentWorld.setTileWithDataWithUpdate(mouseTileX, mouseTileY, index, game.currentWorld.getTileData(mouseTileX, mouseTileY, isWall)==0?1:0, isWall);
            return base.use(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer, isWall);
        }

        
        public override int getLight(int x, int y, bool isWall) {
            if(Game.instance.currentWorld==null){
                return 0;
            }
            return Game.instance.currentWorld.getTileData(x, y, isWall)==0?base.getLight(x, y, isWall):0;
        }
        public override TextureInfo getTextureInfo(int x, int y, World world, bool isWall) {
            
            if (renderType == RENDER_TYPE_PLACED) {
                bool left = world.isTileSolid(x - 1, y, isWall);
                bool right = world.isTileSolid(x + 1, y, isWall);
                bool down = world.isTileSolid(x, y + 1, isWall);
                bool up = world.isTileSolid(x, y - 1, isWall);
                bool data = world.getTileData(x, y, isWall)!=0;
                Rectangle r;
                if (down) {
                    r = get8(data?2:0, 0);
                } else if (left) {
                    r = get8(data ? 3 : 1, 0);
                } else if (right) {
                    r = get8(data ? 3 : 1, 1);
                } else if (up) {
                    r = get8(data ? 2 : 0, 1);
                } else {
                    r = get8(data ? 2 : 0, 0);
                }
                return new TextureInfo(r, true);
            }
            return new TextureInfo(Game.OnexOneRect, true);
        }
    }
}
