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

        public TileMustRestOn(string name, int renderType, int textureX, int textureY, int x8th, int y8th, int restOnX, int restOnY, Tile restOn, bool drop)
            : this(name, renderType, textureX, textureY, x8th, y8th, restOnX, restOnY, new Tile[] { restOn }, drop) {
            
        }

        public TileMustRestOn(string name, int renderType, int textureX, int textureY, int x8th, int y8th, int restOnX, int restOnY, Tile[] restOns, bool drop)
            : base(name, renderType, textureX, textureY) {
            this.x8th = x8th;
            this.y8th = y8th;
            this.restOnX = restOnX;
            this.restOnY = restOnY;
            this.restOns = restOns;
            this.drop = drop;
        }

        public override TextureInfo getTextureInfo(int x, int y, World world, bool isWall) {
            if(renderType == RenderTypeCustom){
                return new TextureInfo(get8(x8th, y8th), true);
            } else {
                return base.getTextureInfo(x, y, world, isWall);
            }
        }

        public override bool canBePlacedHereOverridable(World world, int x, int y, bool isWall) {
            if(world.getTileIndex(x, y, isWall) != Tile.TileAir.index){
                return false;
            }
            for (int i = 0; i < restOns.Length;i++ ) {
                if (world.getTileObject(x + restOnX, y + restOnY, isWall).index == restOns[i].index) {
                    return true;
                }
            }
            return false;
        }

        public override void updateNearChange(World world, int x, int y, bool isWall) {
            if (!world.isTileSolid(x + restOnX, y + restOnY, isWall)) {
                world.mineTile(x, y, Item.itemSupick,  isWall);
            }
        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, bool isWall) {
            return drop?new ItemStack(this, 1):new ItemStack(Item.itemEmpty);
        }

        public override Rectangle getItemRect() {
            return get8(x8th, y8th);
        }
    }
}
