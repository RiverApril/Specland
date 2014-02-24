using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    public class Tile {

        public static Texture2D TileSheet;

        public static int RenderTypeNone = 0;
        public static int RenderTypeTerrain = 1;
        public static int RenderTypePlaced = 2;
        public static int RenderTypeBuilding = 3;
        public static int RenderTypeCustom = 4;

        public static Tile[] TileList = new Tile[100];

        public static Tile TileAir = new Tile("Air", RenderTypeNone, 0, 0).setTransparent().notSolid();
        public static Tile TileGrass = new TileGrass("Grass", RenderTypeTerrain, 0, 0);
        public static Tile TileDirt = new Tile("Dirt", RenderTypeTerrain, 1, 0);
        public static Tile TileStone = new Tile("Stone", RenderTypeTerrain, 2, 0);
        public static Tile TileStoneBricks = new Tile("StoneBricks", RenderTypeTerrain, 0, 1);
        public static Tile TileGlass = new Tile("Glass", RenderTypeTerrain, 1, 1).setTransparent().setWallBrightness(240);
        public static Tile TileCoalOre = new Tile("CoalOre", RenderTypeTerrain, 3, 0);
        public static Tile TileTorch = new Tile("Torch", RenderTypePlaced, 2, 1).setTransparent().setLight(300).notSolid().notWall();
        public static Tile TileTree = new TileTree("Tree", RenderTypeCustom, 3, 1).setTransparent().notSolid();
        public static Tile TileLeaf = new TileLeaf("Leaf", RenderTypeCustom, 4, 1, TileTree.index).setTransparent().notSolid();
        public static Tile TileWood = new Tile("Wood", RenderTypeTerrain, 4, 0);
        public static Tile TileSand = new TileFalling("Sand", RenderTypeTerrain, 5, 0);
        public static Tile TileSapling = new TileMustRestOn("Sapling", RenderTypeCustom, 5, 1, 3, 3, 0, 1, TileGrass).notSolid().notWall().setTransparent();

        public int index;
        public string name;
        public bool solid = true;
        public int renderType;

        public bool transparent = false;
        public int light = 0;
        public Point textureArea;
        public int toughness = 255;
        public int wallBrightness = 127;
        public bool canBeWall = true;

        private static int nextIndex=0;

        public Tile(string name, int renderType, int textureX, int textureY) {
            index = getNewIndex();
            TileList[index] = this;
            this.name = name;
            this.renderType = renderType;
            this.textureArea = new Point(textureX, textureY);
        }

        private static int getNewIndex() {
            nextIndex++;
            return nextIndex-1;
        }

        private Tile notWall() {
            canBeWall = false;
            return this;
        }

        private Tile setThoughness(int t) {
            toughness = t;
            return this;
        }

        private Tile setTransparent() {
            transparent = true;
            return this;
        }

        private Tile notSolid() {
            solid = false;
            return this;
        }

        private Tile setLight(int power) {
            light = power;
            return this;
        }

        private Tile setWallBrightness(byte b) {
            wallBrightness = b;
            return this;
        }


        public virtual TextureInfo getTextureInfo(int x, int y, World world, bool isWall) {
            if (renderType == RenderTypeTerrain || renderType == RenderTypeBuilding){
                
                bool left = false;
                bool right = false;
                bool down = false;
                bool up = false;

                left = (world.getTileObject(x - 1, y, isWall)).renderType == renderType;
                right = (world.getTileObject(x + 1, y, isWall)).renderType == renderType;
                down = (world.getTileObject(x, y + 1, isWall)).renderType == renderType;
                up = (world.getTileObject(x, y - 1, isWall)).renderType == renderType;

                Rectangle r;
                bool t = transparent;
                if(left){
                    if(right){
                        if(up){
                            if(down){
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
                return new TextureInfo(/*texture2D,*/ r, t);
            } else if (renderType == RenderTypePlaced) {
                bool left = world.getTileObject(x - 1, y, isWall).solid;
                bool right = world.getTileObject(x + 1, y, isWall).solid;
                bool down = world.getTileObject(x, y + 1, isWall).solid;
                bool up = world.getTileObject(x, y - 1, isWall).solid;
                Rectangle r;
                if (down) {
                    r = get8(0, 0);
                } else if (left) {
                    r = get8(1, 0);
                } else if (right) {
                    r = get8(1, 1);
                } else if (up) {
                    r = get8(0, 1);
                } else {
                    r = get8(0, 0);
                }
                return new TextureInfo(/*texture2D,*/ r, true);
            }
            return new TextureInfo(/*Game.dummyTexture,*/ Game.OnexOneRect, true);
        }

        public Rectangle get8(int x, int y) {
            return new Rectangle((textureArea.X * 32) + (8 * x), (textureArea.Y * 32) + (8 * y), 8, 8);
        }

        public static Tile getTileObject(int index) {
            if(index>=0 && index<TileList.Length){
                return TileList[index];
            }
            return null;
        }

        public virtual void updateRandom(World world, int x, int y, bool isWall) {

        }

        public virtual void updateNearChange(World world, int x, int y, bool isWall) {

        }

        public virtual ItemStack dropStack(ItemPick itemPick, Random rand) {
            return new ItemStack(Item.ItemTile, 1, index);
        }

        public virtual void mine(World world, int x, int y, ItemPick pick, bool isWall) {

        }

        public virtual bool canBePlacedHere(World world, int x, int y, bool isWall) {
            return (world.getTileObject(x - 1, y, isWall).solid || world.getTileObject(x + 1, y, isWall).solid || world.getTileObject(x, y - 1, isWall).solid || world.getTileObject(x, y + 1, isWall).solid || !world.getTileObject(x, y, !isWall).isAir()) && world.getTileObject(x, y, isWall).isAir();
        }

        private bool isAir() {
            return (index == TileAir.index);
        }
    }
}
