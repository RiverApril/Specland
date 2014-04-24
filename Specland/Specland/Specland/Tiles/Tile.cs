using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    public class Tile {

        public static Texture2D TileSheet;

        public static int RENDER_TYPE_NONE = 0;
        public static int RENDER_TYPE_TERRAIN = 1;
        public static int RENDER_TYPE_PLACED = 2;
        public static int RENDER_TYPE_BUILDING = 3;
        public static int RENDER_TYPE_ATTACH_TO_SELF;
        public static int RENDER_TYPE_CUSTOM = 5;

        public static int MATERIAL_NONE = 0;
        public static int MATERIAL_DIRT = 1;
        public static int MATERIAL_STONE = 2;
        public static int MATERIAL_WOOD = 3;
        public static int MATERIAL_FURNITURE = 4;

        public static Tile[] TileList = new Tile[100];

        public static Tile TileAir = new Tile("Air", RENDER_TYPE_NONE, MATERIAL_NONE, 0, 0).setTransparent().notSolid();
        public static Tile TileGrass = new TileGrass("Grass", RENDER_TYPE_TERRAIN, MATERIAL_DIRT, 0, 0);
        public static Tile TileDirt = new Tile("Dirt", RENDER_TYPE_TERRAIN, MATERIAL_DIRT, 1, 0);
        public static Tile TileStone = new Tile("Stone", RENDER_TYPE_TERRAIN, MATERIAL_STONE, 2, 0);
        public static Tile TileStoneBricks = new Tile("StoneBricks", RENDER_TYPE_TERRAIN, MATERIAL_STONE, 0, 1).setDisplayName("Stone Bricks");
        public static Tile TileGlass = new Tile("Glass", RENDER_TYPE_ATTACH_TO_SELF, MATERIAL_STONE, 1, 1).setTransparent().setWallBrightness(240);
        public static Tile TileTorch = new TileTorch("Torch", RENDER_TYPE_PLACED, MATERIAL_FURNITURE, 2, 1).setTransparent().setLight(200).notSolid().notWall().setWashedAwayByWater();
        public static Tile TileTree = new TileTree("Tree", RENDER_TYPE_CUSTOM, MATERIAL_WOOD, 3, 1).setTransparent().notSolid();
        public static Tile TileLeaf = new TileLeaf("Leaf", RENDER_TYPE_CUSTOM, MATERIAL_WOOD, 4, 1, TileTree.index).setTransparent().notSolid();
        public static Tile TileWood = new Tile("Wood", RENDER_TYPE_TERRAIN, MATERIAL_WOOD, 4, 0).setDisplayName("Wooden Plank");
        public static Tile TileSand = new TileFalling("Sand", RENDER_TYPE_TERRAIN, MATERIAL_DIRT, 5, 0);
        public static Tile TileSapling = new TileMustRestOn("Sapling", RENDER_TYPE_CUSTOM, MATERIAL_FURNITURE, 5, 1, 3, 3, 0, 1, TileGrass, true).notSolid().notWall().setTransparent().setWashedAwayByWater();
        public static Tile TileWoodDoor = new TileDoor("WoodDoor", RENDER_TYPE_CUSTOM, 2, 2).notWall().setTransparent().setDisplayName("Wooden Door");
        public static Tile TileWoodTable = new TileFurniture("WoodTable", RENDER_TYPE_CUSTOM, 3, 2, 3, 2).notSolid().notWall().setTransparent().setDisplayName("Wooden Table");
        public static Tile TileWoodChair = new TileFurniture("WoodChair", RENDER_TYPE_CUSTOM, 4, 2, 1, 2).notSolid().notWall().setTransparent().setDisplayName("Wooden Chair");
        public static Tile TilePlantGlow = new TileMustRestOn("GlowLeaf", RENDER_TYPE_CUSTOM, MATERIAL_FURNITURE, 5, 1, 2, 3, 0, 1, new Tile[] { TileDirt, TileStone }, true).notSolid().notWall().setTransparent().setLight(120).setWashedAwayByWater().setDisplayName("Glow Leaf").setDisplayNamePlural("Glow Leaves");
        public static Tile TileLamp = new TileLightToggle("Lamp", RENDER_TYPE_PLACED, MATERIAL_FURNITURE, 6, 1).setTransparent().setLight(300).notSolid().notWall().setWashedAwayByWater();

        public int index;
        public string name;
        public int renderType;
        public int material;

        public bool transparent = false;
        private int light = 0;
        public Point textureArea;
        public int toughness = 255;
        public int wallBrightness = 127;
        public bool canBeWall = true;
        public bool washedAwayByWater = false;
        public string displayName;
        public string displayNamePlural;

        private static int nextIndex=0;

        private bool defaultSolid = true;

        public Tile(string name, int renderType, int material, int textureX, int textureY) {
            index = getNewIndex();
            TileList[index] = this;
            this.name = name;
            this.material = material;
            this.displayName = name;
            this.displayNamePlural = displayName;
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

        private Tile setWashedAwayByWater() {
            washedAwayByWater = true;
            return this;
        }

        private Tile setTransparent() {
            transparent = true;
            return this;
        }

        private Tile notSolid() {
            defaultSolid = false;
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

        private Tile setDisplayName(string n) {
            displayName = n;
            displayNamePlural = n + "s";
            return this;
        }

        private Tile setDisplayNamePlural(string n) {
            displayNamePlural = n;
            return this;
        }


        public virtual TextureInfo getTextureInfo(int x, int y, World world, bool isWall) {
            if (renderType == RENDER_TYPE_TERRAIN || renderType == RENDER_TYPE_BUILDING || renderType == RENDER_TYPE_ATTACH_TO_SELF) {
                
                bool left = false;
                bool right = false;
                bool down = false;
                bool up = false;

                left = (world.getTileObject(x - 1, y, isWall)).renderType == renderType;
                right = (world.getTileObject(x + 1, y, isWall)).renderType == renderType;
                down = (world.getTileObject(x, y + 1, isWall)).renderType == renderType;
                up = (world.getTileObject(x, y - 1, isWall)).renderType == renderType;

                if(renderType == RENDER_TYPE_ATTACH_TO_SELF){
                    left = (world.getTileObject(x - 1, y, isWall)).index == index;
                    right = (world.getTileObject(x + 1, y, isWall)).index == index;
                    down = (world.getTileObject(x, y + 1, isWall)).index == index;
                    up = (world.getTileObject(x, y - 1, isWall)).index == index;
                }

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
            } else if (renderType == RENDER_TYPE_PLACED) {
                bool left = world.isTileSolid(x - 1, y, isWall);
                bool right = world.isTileSolid(x + 1, y, isWall);
                bool down = world.isTileSolid(x, y + 1, isWall);
                bool up = world.isTileSolid(x, y - 1, isWall);
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

        public Rectangle get8(int x, int y, int w, int h) {
            return new Rectangle((textureArea.X * 32) + (8 * x), (textureArea.Y * 32) + (8 * y), 8*w, 8*h);
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

        public virtual ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, bool isWall) {
            return new ItemStack(Item.itemTile, 1, index);
        }

        public virtual void mine(World world, int x, int y, int data, ItemPick pick, bool isWall) {

        }

        public bool canBePlacedHere(World world, int x, int y, bool isWall) {
            if(isWall && !canBeWall){
                return false;
            }
            return canBePlacedHereOverridable(world, x, y, isWall);
        }

        public virtual bool canBePlacedHereOverridable(World world, int x, int y, bool isWall) {
            if (renderType == RENDER_TYPE_PLACED) {
                return (world.isTileSolid(x - 1, y, isWall) || world.isTileSolid(x + 1, y, isWall) || world.isTileSolid(x, y - 1, isWall) || world.isTileSolid(x, y + 1, isWall) || !world.getTileObject(x, y, !isWall).isAir()) && world.getTileObject(x, y, isWall).isAir();
            }
            return (!world.getTileObject(x - 1, y, isWall).isAir() || !world.getTileObject(x + 1, y, isWall).isAir() || !world.getTileObject(x, y - 1, isWall).isAir() || !world.getTileObject(x, y + 1, isWall).isAir() || !world.getTileObject(x, y, !isWall).isAir()) && world.getTileObject(x, y, isWall).isAir();
        }

        public bool isAir() {
            return (index == TileAir.index);
        }

        public virtual void justPlaced(World world, int x, int y, bool isWall) {

        }

        public virtual bool drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            return false;
        }

        public virtual ItemStack use(Game game, ItemStack currentItem, int mouseTileX, int mouseTileY, int mouseTileDistanceFromPlayer, bool isWall) {
            return null;
        }

        public virtual bool isSolid(int x, int y) {
            return defaultSolid;
        }

        public virtual bool isSolid() {
            return defaultSolid;
        }

        public virtual Rectangle getItemRect() {
            return get8(3, 3);
        }

        public virtual int getLight(int x, int y, bool isWall) {
            return light;
        }

        public virtual float getDepth(int x, int y, bool isWall) {
            return isWall?Game.RENDER_DEPTH_WALL:Game.RENDER_DEPTH_TILE;
        }
    }
}
