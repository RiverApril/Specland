using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    class TileTree : Tile {

        public TileTree(string name, int renderType, int textureX, int textureY) : base(name, renderType, textureX, textureY){ }

        public override TextureInfo getTextureInfo(int x, int y, World world, bool isWall) {

            bool left = world.getTileObject(x - 1, y, isWall).index == index;
            bool right = world.getTileObject(x + 1, y, isWall).index == index;
            bool down = world.getTileObject(x, y + 1, isWall).index == index;
            bool up = world.getTileObject(x, y - 1, isWall).index == index;

            bool leftSolid = world.getTileObject(x - 1, y, isWall).solid;
            bool rightSolid = world.getTileObject(x + 1, y, isWall).solid;
            bool downSolid = world.getTileObject(x, y + 1, isWall).solid;
            bool upSolid = world.getTileObject(x, y - 1, isWall).solid;

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

        public override void mine(World world, int x, int y, ItemPick pick, bool isWall) {
            if (world.getTileIndex(x, y - 1, isWall) == index) {
                world.mineTile(x, y - 1, pick, isWall);
            }
            if (world.getTileIndex(x + 1, y, isWall) == index) {
                world.mineTile(x + 1, y, pick, isWall);
            }
            if (world.getTileIndex(x - 1, y, isWall) == index) {
                world.mineTile(x - 1, y, pick, isWall);
            }
        }

        public override void updateNearChange(World world, int x, int y, bool isWall) {
            if (world.getTileIndex(x, y - 1, isWall) == index && world.getTileIndex(x, y + 1, isWall) != Tile.TileAir.index) {

            } else {
                world.mineTileNoNearUpdate(x, y, Item.ItemSupick, isWall);
            }
        }

        public override ItemStack dropStack(ItemPick itemPick, Random rand) {
            return rand.Next(2)==0?new ItemStack(Item.ItemTile, 1, Tile.TileWood.index):new ItemStack(Item.ItemEmpty);
        }

    }
}
