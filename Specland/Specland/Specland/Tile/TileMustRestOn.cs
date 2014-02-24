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
        private Tile restOn;

        public TileMustRestOn(string name, int renderType, int textureX, int textureY, int x8th, int y8th, int restOnX, int restOnY, Tile restOn) : base(name, renderType, textureX, textureY){
            this.x8th = x8th;
            this.y8th = y8th;
            this.restOnX = restOnX;
            this.restOnY = restOnY;
            this.restOn = restOn;
        }

        public override TextureInfo getTextureInfo(int x, int y, World world, bool isWall) {
            if(renderType == RenderTypeCustom){
                return new TextureInfo(get8(x8th, y8th), true);
            } else {
                return base.getTextureInfo(x, y, world, isWall);
            }
        }

        public override bool canBePlacedHere(World world, int x, int y, bool isWall) {
            return world.getTileObject(x + restOnX, y + restOnY, isWall).index == restOn.index;
        }

        public override void updateNearChange(World world, int x, int y, bool isWall) {
            if (!world.getTileObject(x + restOnX, y + restOnY, isWall).solid) {
                world.mineTile(x, y, Item.ItemSupick,  isWall);
            }
        }

        public override ItemStack dropStack(ItemPick itemPick, Random rand) {
            return new ItemStack(Item.ItemEmpty);
        } 
    }
}
