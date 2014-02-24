using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    public class Item {

        public static Texture2D ItemSheet;

        public static int RenderTypeNone = 0;
        public static int RenderTypeNormal = 1;

        public static Item[] Itemlist = new Item[100];

        public static Item ItemEmpty = new Item(0, "", RenderTypeNone, 0, 0);
        public static ItemTile ItemTile = new ItemTile(1, "Tile", RenderTypeNormal, 0, 0);
        public static ItemPick ItemCrapick = new ItemPick(2, "Crapick", RenderTypeNormal, 0, 0, 64, 20, 6 * World.tileSizeInPixels);
        public static ItemPick ItemSupick = new ItemPick(3, "Supick", RenderTypeNormal, 1, 0, 255, 1, 10 * World.tileSizeInPixels);

        public int index;
        public string name;
        public int maxStack = 1;
        public int renderType;

        public Point textureArea;
        public int reach = World.tileSizeInPixels * 6;

        public Item(int index, string name, int renderType, int textureX, int textureY) {
            Itemlist[index] = this;
            this.index = index;
            this.name = name;
            this.renderType = renderType;
            this.textureArea = new Point(textureX, textureY);
        }

        public Item setMaxStack(int q) {
            maxStack = q;
            return this;
        }


        public virtual void drawAsItem(Game game, GameTime gameTime, Rectangle r, int count, int data, Color color) {
            if(renderType == RenderTypeNormal){
                game.spriteBatch.Draw(ItemSheet, r, getRect(), color);
            }
        }

        private Rectangle getRect() {
            return new Rectangle(textureArea.X * 16, textureArea.Y * 16, 16, 16);
        }

        public virtual ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            return stack;
        }

        public virtual ItemStack rightClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            return stack;
        }

        public virtual string getName(int count, int data) {
            return name;
        }

        public virtual void drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            
        }
    }
}
