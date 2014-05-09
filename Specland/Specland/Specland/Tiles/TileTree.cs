using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    class TileTree : Tile {

        public TileTree(string name, RenderType renderType, Material material, int textureX, int textureY) : base(name, renderType, material, textureX, textureY) { }

        public override TextureInfo getTextureInfo(int x, int y, World world, World.TileDepth tileDepth) {

            bool left = world.getTileObject(x - 1, y, tileDepth).index == index;
            bool right = world.getTileObject(x + 1, y, tileDepth).index == index;
            bool down = world.getTileObject(x, y + 1, tileDepth).index == index;
            bool up = world.getTileObject(x, y - 1, tileDepth).index == index;

            bool leftSolid = world.isTileSolid(x - 1, y, tileDepth);
            bool rightSolid = world.isTileSolid(x + 1, y, tileDepth);
            bool downSolid = world.isTileSolid(x, y + 1, tileDepth);
            bool upSolid = world.isTileSolid(x, y - 1, tileDepth);

            Rectangle r;
            bool t = transparent;
            if(left && right){
                if (downSolid) {
                    r = get8(1, 2);
                } else {
                    r = get8(1, 1);
                }
            } else if (right) {
                if (downSolid) {
                    r = get8(0, 2);
                } else if (down) {
                    r = get8(0, 0);
                } else {
                    r = get8(0, 1);
                }
            } else if (left) {
                if (downSolid) {
                    r = get8(2, 2);
                } else if (down) {
                    r = get8(2, 0);
                } else {
                    r = get8(2, 1);
                }
            } else {
                if (downSolid) {
                    r = get8(1, 2);
                } else if (down) {
                    r = get8(1, 0);
                } else {
                    r = get8(3, 3);
                }
            }
            return new TextureInfo(r, t);
        }

        public override bool mustHaveTileBelow(World world, int x, int y, World.TileDepth tileDepth) {
            if (world.getTileData(x, y, tileDepth) == 0) {
                return false;
            }
            return true;
        }
        
        public override void mine(World world, int x, int y, int data, ItemPick pick, World.TileDepth tileDepth) {
            if (world.isTileSolid(x, y + 1, tileDepth)) {
                if (world.getTileIndex(x + 1, y, tileDepth) == index) {
                    world.mineTile(x + 1, y, pick, tileDepth);
                }
                if (world.getTileIndex(x - 1, y, tileDepth) == index) {
                    world.mineTile(x - 1, y, pick, tileDepth);
                }
            }
        }

        public override void updateNearChange(World world, int x, int y, World.TileDepth tileDepth) {
            if (world.getTileIndex(x, y + 1, tileDepth) == Tile.TileAir.index) {
                world.mineTile(x, y, Item.itemSupick, tileDepth);
            }
        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, World.TileDepth tileDepth) {
            return rand.Next(2)==0?new ItemStack(Item.itemTile, 1, Tile.TileWood.index):new ItemStack(Item.itemEmpty);
        }

    }
}
