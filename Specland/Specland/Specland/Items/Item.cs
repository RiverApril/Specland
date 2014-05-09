using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Specland.Items;

namespace Specland {
    public class Item {

        public static Texture2D ItemSheet;

        public static int nextIndex = 0;

        public static int RenderTypeNone = 0;
        public static int RenderTypeNormal = 1;

        public static Item[] ItemList = new Item[100];

        public static Item itemEmpty = new Item("", RenderTypeNone, Rectangle.Empty);
        public static ItemTile itemTile = new ItemTile("Tile", RenderTypeNormal, Rectangle.Empty);

        
        public static ItemPick itemStonePick = (ItemPick)new ItemPick("StonePick", RenderTypeNormal, new Rectangle(0, 0, 16, 16), ItemPick.createPowers(2, 1, 0), 20, 6 * World.tileSizeInPixels).setDisplayName("Stone Pickaxe");
        public static ItemPick itemCopperPick = (ItemPick)new ItemPick("CopperPick", RenderTypeNormal, new Rectangle(0, 16, 16, 16), ItemPick.createPowers(3, 1.5f, 0), 18, 6 * World.tileSizeInPixels).setDisplayName("Copper Pickaxe");
        public static ItemPick itemIronPick = (ItemPick)new ItemPick("IronPick", RenderTypeNormal, new Rectangle(0, 16*2, 16, 16), ItemPick.createPowers(4, 2, 0), 20, 6 * World.tileSizeInPixels).setDisplayName("Iron Pickaxe");
        public static ItemPick itemSteelPick = (ItemPick)new ItemPick("SteelPick", RenderTypeNormal, new Rectangle(0, 16*3, 16, 16), ItemPick.createPowers(5, 2.5f, 0), 16, 6 * World.tileSizeInPixels).setDisplayName("Iron Pickaxe");
        public static ItemPick itemQuartzPick = (ItemPick)new ItemPick("QuartzPick", RenderTypeNormal, new Rectangle(0, 16*4, 16, 16), ItemPick.createPowers(6, 3, 0), 14, 6 * World.tileSizeInPixels).setDisplayName("Quartz Pickaxe");
        public static ItemPick itemEmeraldPick = (ItemPick)new ItemPick("EmeraldPick", RenderTypeNormal, new Rectangle(0, 16*5, 16, 16), ItemPick.createPowers(7, 3.5f, 0), 12, 6 * World.tileSizeInPixels).setDisplayName("Emerald Pickaxe");
        public static ItemPick itemSaphirePick = (ItemPick)new ItemPick("SaphirePick", RenderTypeNormal, new Rectangle(0, 16 * 6, 16, 16), ItemPick.createPowers(8, 4, 0), 10, 6 * World.tileSizeInPixels).setDisplayName("Saphire Pickaxe");
        public static ItemPick itemDiamondPick = (ItemPick)new ItemPick("DiamondPick", RenderTypeNormal, new Rectangle(0, 16*7, 16, 16), ItemPick.createPowers(9, 4.5f, 0), 8, 6 * World.tileSizeInPixels).setDisplayName("Diamond Pickaxe");
        
        public static ItemPick itemSteelDrill = (ItemPick)new ItemPick("SteelDrill", RenderTypeNormal, new Rectangle(16, 16*3, 16, 16), ItemPick.createPowers(3, 1.5f, 0), 0, 6 * World.tileSizeInPixels, false).setDisplayName("Steel Drill");
        public static ItemPick itemQuartzDrill = (ItemPick)new ItemPick("QuartzSteelDrill", RenderTypeNormal, new Rectangle(16, 16*4, 16, 16), ItemPick.createPowers(4, 2f, 0), 0, 6 * World.tileSizeInPixels, false).setDisplayName("Quartz Tipped Steel Drill");
        public static ItemPick itemEmeraldDrill = (ItemPick)new ItemPick("EmeraldSteelDrill", RenderTypeNormal, new Rectangle(16, 16*5, 16, 16), ItemPick.createPowers(5, 2.5f, 0), 0, 6 * World.tileSizeInPixels, false).setDisplayName("Emerald Tipped Steel Drill");
        public static ItemPick itemSaphireDrill = (ItemPick)new ItemPick("SaphireSteelDrill", RenderTypeNormal, new Rectangle(16, 16 * 6, 16, 16), ItemPick.createPowers(6, 3f, 0), 0, 6 * World.tileSizeInPixels, false).setDisplayName("Saphire Tipped Steel Drill");
        public static ItemPick itemDiamondDrill = (ItemPick)new ItemPick("DiamondSteelDrill", RenderTypeNormal, new Rectangle(16, 16*7, 16, 16), ItemPick.createPowers(7, 3.5f, 0), 0, 6 * World.tileSizeInPixels, false).setDisplayName("Diamond Tipped Steel Drill");


        public static ItemPick itemSupick = new ItemPick("Supick", RenderTypeNormal, new Rectangle(16, 0, 16, 16), ItemPick.createPowers(16, 16, 16), 0, 100 * World.tileSizeInPixels);
        
        public static Item itemBucket = new ItemBucket("Bucket", RenderTypeNormal, new Rectangle(16 * 2, 0, 16, 16));
        public static Item itemBucketWater = new ItemBucketFilled("BucketWater", RenderTypeNormal, new Rectangle(16 * 3, 0, 16, 16)).setDisplayName("Bucket of Water");



        public int index;
        public string name;
        public int maxStack = 1;
        public int renderType;

        public string displayName;
        public string displayNamePlural;

        public TextureInfo textureInfo;
        public int reach = World.tileSizeInPixels * 6;

        public Item(string name, int renderType, Rectangle sourceRectangle) {
            index = getNewIndex();
            ItemList[index] = this;
            this.name = name;
            this.displayName = name;
            this.displayNamePlural = displayName + "s";
            this.renderType = renderType;
            this.textureInfo = new TextureInfo(sourceRectangle, true);
        }

        private Item setDisplayName(string n) {
            displayName = n;
            displayNamePlural = n + "s";
            return this;
        }

        private Item setDisplayNamePlural(string n) {
            displayNamePlural = n;
            return this;
        }

        private static int getNewIndex() {
            nextIndex++;
            return nextIndex - 1;
        }

        public Item setMaxStack(int q) {
            maxStack = q;
            return this;
        }


        public virtual void drawAsItem(Game game, Rectangle r, int count, int data, Color color, float depth) {
            if(renderType == RenderTypeNormal){
                Game.drawRectangle(ItemSheet, r, textureInfo.rectangle, color, depth);
            }
        }

        public virtual ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            return stack;
        }

        public virtual ItemStack rightClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            return stack;
        }

        public virtual void updateAfterClick(Game game, ItemStack currentItem, int mouseTileX, int mouseTileY, int mouseTileDistanceFromPlayer) {
            
        }

        public virtual string getName(int count, int data) {
            return name;
        }

        public virtual string getDisplayName(int count, int data) {
            return count == 1 ? displayName : displayNamePlural;
        }

        public virtual void drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            
        }

        public virtual void drawOverPlayer(Game game, ItemStack currentItem, bool facingRight, Vector2 position, Color color, Point mousePosition) {
            
        }

        public static Item getItemObject(int index) {
            if (index >= 0 && index < ItemList.Length) {
                return ItemList[index];
            }
            return null;
        }
    }
}
