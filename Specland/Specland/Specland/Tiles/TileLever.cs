using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class TileLever : TileLightToggle {

        public TileLever(string name, RenderType renderType, Material material, int textureX, int textureY) : base(name, renderType, material, textureX, textureY) { }

        public override ItemStack use(Game game, ItemStack currentItem, int mouseTileX, int mouseTileY, int mouseTileDistanceFromPlayer, World.TileDepth tileDepth) {

            return base.use(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer, tileDepth);
        }
    }
}
