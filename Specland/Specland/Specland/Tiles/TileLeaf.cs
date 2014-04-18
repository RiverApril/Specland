using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    class TileLeaf : Tile {

        private int tree = 0;

        public TileLeaf(string name, int renderType, int textureX, int textureY, int tree) : base(name, renderType, textureX, textureY) {
            this.tree = tree;
        }

        public override TextureInfo getTextureInfo(int x, int y, World world, bool isWall) {
            
            int leftIndex = world.getTileObject(x - 1, y, isWall).index;
            int rightIndex = world.getTileObject(x + 1, y, isWall).index;
            int downIndex = world.getTileObject(x, y + 1, isWall).index;
            int upIndex = world.getTileObject(x, y - 1, isWall).index;

            bool left = leftIndex == index;// || leftIndex == tree;
            bool right = rightIndex == index;// || rightIndex == tree;
            bool down = downIndex == index;// || downIndex == tree;
            bool up = upIndex == index;// || upIndex == tree;

            bool leftSolid = world.isTileSolid(x - 1, y, isWall);
            bool rightSolid = world.isTileSolid(x + 1, y, isWall);
            bool downSolid = world.isTileSolid(x, y + 1, isWall);
            bool upSolid = world.isTileSolid(x, y - 1, isWall);

            Rectangle r;
            bool t = transparent;
            if (left) {
                if (right) {
                    if (up) {
                        if (down) {
                            r = get8(1, 1);
                        } else {
                            r = get8(1, 2);
                            t = true;
                        }
                    } else {
                        if (down) {
                            r = get8(1, 0);
                            t = true;
                        } else {
                            r = get8(1, 3);
                            t = true;
                        }
                    }
                } else {
                    if (up) {
                        if (down) {
                            r = get8(2, 1);
                            t = true;
                        } else {
                            r = get8(2, 2);
                            t = true;
                        }
                    } else {
                        if (down) {
                            r = get8(2, 0);
                            t = true;
                        } else {
                            r = get8(2, 3);
                            t = true;
                        }
                    }
                }
            } else {
                if (right) {
                    if (up) {
                        if (down) {
                            r = get8(0, 1);
                            t = true;
                        } else {
                            r = get8(0, 2);
                            t = true;
                        }
                    } else {
                        if (down) {
                            r = get8(0, 0);
                            t = true;
                        } else {
                            r = get8(0, 3);
                            t = true;
                        }
                    }
                } else {
                    if (up) {
                        if (down) {
                            r = get8(3, 1);
                            t = true;
                        } else {
                            r = get8(3, 2);
                            t = true;
                        }
                    } else {
                        if (down) {
                            r = get8(3, 0);
                            t = true;
                        } else {
                            r = get8(3, 3);
                            t = true;
                        }
                    }
                }
            }
            return new TextureInfo(r, t);
        }

        public override void mine(World world, int x, int y, int data, ItemPick pick, bool isWall) {
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
            int range = 4;

            bool a = false;

            for (int i = x - range; i < x + range; i++) {
                for (int j = y - range; j < y + range; j++) {
                    if(world.getTileIndex(i, j, isWall)==tree){
                        a = true;
                        break;
                    }
                }
            }
            if(!a){
                world.mineTileNoNearUpdate(x, y, Item.itemSupick, isWall);
            }

        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, bool isWall) {
            return rand.Next(8) == 0 ? new ItemStack(Item.itemTile, 1, Tile.TileSapling.index) : new ItemStack(Item.itemEmpty);
        }

    }
}
