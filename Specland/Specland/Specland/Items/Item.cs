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

        public static ItemPick itemWoodPick = (ItemPick)new ItemPick("WoodPick", RenderTypeNormal, new Rectangle(0, 0, 16, 16), ItemPick.createPowers(1, .5f, 0), 20, 6 * World.tileSizeInPixels).setDisplayName("Wooden Pickaxe");
        public static ItemPick itemStonePick = (ItemPick)new ItemPick("StonePick", RenderTypeNormal, new Rectangle(0, 16, 16, 16), ItemPick.createPowers(2, 1, 0), 20, 6 * World.tileSizeInPixels).setDisplayName("Stone Pickaxe");

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

        public virtual void drawOverPlayer(Game game, ItemStack currentItem, bool facingRight, Vector2 position, Color color) {
            
        }

        public static Item getItemObject(int index) {
            if (index >= 0 && index < ItemList.Length) {
                return ItemList[index];
            }
            return null;
        }
    }
}
