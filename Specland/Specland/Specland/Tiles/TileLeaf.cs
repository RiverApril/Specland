using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    class TileLeaf : Tile {

        private int tree = 0;

        public TileLeaf(string name, RenderType renderType, Material material, int textureX, int textureY, int tree)
            : base(name, renderType, material, textureX, textureY) {
            this.tree = tree;
        }

        /*public override TextureInfo getTextureInfo(int x, int y, World world, World.TileDepth tileDepth) {
            
            int leftIndex = world.getTileObject(x - 1, y, tileDepth).index;
            int rightIndex = world.getTileObject(x + 1, y, tileDepth).index;
            int downIndex = world.getTileObject(x, y + 1, tileDepth).index;
            int upIndex = world.getTileObject(x, y - 1, tileDepth).index;

            bool left = leftIndex == index;// || leftIndex == tree;
            bool right = rightIndex == index;// || rightIndex == tree;
            bool down = downIndex == index;// || downIndex == tree;
            bool up = upIndex == index;// || upIndex == tree;

            bool leftSolid = world.isTileSolid(x - 1, y, tileDepth);
            bool rightSolid = world.isTileSolid(x + 1, y, tileDepth);
            bool downSolid = world.isTileSolid(x, y + 1, tileDepth);
            bool upSolid = world.isTileSolid(x, y - 1, tileDepth);

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
        }*/

        public override void mine(World world, int x, int y, int data, ItemPick pick, World.TileDepth tileDepth) {

        }

        public override void updateNearChange(World world, int x, int y, World.TileDepth tileDepth) {
            if (world.getTileIndex(x, y + 1, tileDepth) != index && world.getTileIndex(x, y + 1, tileDepth) != tree) {
                world.mineTile(x, y, Item.itemSupick, tileDepth);
            }
        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, World.TileDepth tileDepth) {
            return rand.Next(8) == 0 ? new ItemStack(Item.itemTile, 1, Tile.TileSapling.index) : new ItemStack(Item.itemEmpty);
        }

    }
}
