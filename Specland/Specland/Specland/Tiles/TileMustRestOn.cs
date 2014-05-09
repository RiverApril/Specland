using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class TileMustRestOn : Tile {
        private int x8th;
        private int y8th;
        private int restOnX;
        private int restOnY;
        private Tile[] restOns;
        private bool drop;

        public TileMustRestOn(string name, RenderType renderType, Material material, int textureX, int textureY, int x8th, int y8th, int restOnX, int restOnY, Tile restOn, bool drop)
            : this(name, renderType, material, textureX, textureY, x8th, y8th, restOnX, restOnY, new Tile[] { restOn }, drop) {
            
        }

        public TileMustRestOn(string name, RenderType renderType, Material material, int textureX, int textureY, int x8th, int y8th, int restOnX, int restOnY, Tile[] restOns, bool drop)
            : base(name, renderType, material, textureX, textureY) {
            this.x8th = x8th;
            this.y8th = y8th;
            this.restOnX = restOnX;
            this.restOnY = restOnY;
            this.restOns = restOns;
            this.drop = drop;
        }

        public override TextureInfo getTextureInfo(int x, int y, World world, World.TileDepth tileDepth) {
            if(renderType == RenderType.custom){
                return new TextureInfo(get8(x8th, y8th), true);
            } else {
                return base.getTextureInfo(x, y, world, tileDepth);
            }
        }

        public override bool canBePlacedHereOverridable(World world, int x, int y, World.TileDepth tileDepth) {
            if (world.getTileIndex(x, y, tileDepth) != Tile.TileAir.index) {
                return false;
            }
            for (int i = 0; i < restOns.Length;i++ ) {
                if (world.getTileObject(x + restOnX, y + restOnY, tileDepth).index == restOns[i].index) {
                    return true;
                }
            }
            return false;
        }

        public override void updateNearChange(World world, int x, int y, World.TileDepth tileDepth) {
            if (!world.isTileSolid(x + restOnX, y + restOnY, tileDepth)) {
                world.mineTile(x, y, Item.itemSupick, tileDepth);
            }
        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, World.TileDepth tileDepth) {
            return drop?new ItemStack(this, 1):new ItemStack(Item.itemEmpty);
        }

        public override Rectangle getItemRect(int data) {
            return get8(x8th, y8th);
        }
    }
}
