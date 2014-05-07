using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class TileLightToggle : Tile{

        public TileLightToggle(string name, RenderType renderType, Material material, int textureX, int textureY) : base(name, renderType, material, textureX, textureY) { }

        public override ItemStack use(Game game, ItemStack currentItem, int mouseTileX, int mouseTileY, int mouseTileDistanceFromPlayer, int tileDepth) {
            game.currentWorld.setTileWithDataWithUpdate(mouseTileX, mouseTileY, index, game.currentWorld.getTileData(mouseTileX, mouseTileY, tileDepth)==0?1:0, tileDepth);
            return base.use(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer, tileDepth);
        }

        
        public override int getLight(int x, int y, int tileDepth) {
            if(Game.instance.currentWorld==null){
                return 0;
            }
            return Game.instance.currentWorld.getTileData(x, y, tileDepth)==0?base.getLight(x, y, tileDepth):0;
        }
        public override TextureInfo getTextureInfo(int x, int y, World world, int tileDepth) {
            
            if (renderType == RenderType.placed) {
                bool left = world.isTileSolid(x - 1, y, tileDepth);
                bool right = world.isTileSolid(x + 1, y, tileDepth);
                bool down = world.isTileSolid(x, y + 1, tileDepth);
                bool up = world.isTileSolid(x, y - 1, tileDepth);
                bool data = world.getTileData(x, y, tileDepth)!=0;
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

        public override Rectangle getItemRect(int data) {
            return get8(data!=0 ? 3 : 1, 3);
        }
    }
}
