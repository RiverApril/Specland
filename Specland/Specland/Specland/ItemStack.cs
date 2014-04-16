﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class ItemStack {

        private Item item = Item.ItemEmpty;
        private int count = 1;
        private int data = 0;


        public ItemStack(Item item) : this(item, 1, 0){

        }

        public ItemStack(Item item, int count, int data) {
            this.count = count;
            this.data = data;
            this.item = item;
            verify();
        }

        public void drawNoCount(Game game, GameTime gameTime, Rectangle r, Color color) {
            item.drawAsItem(game, gameTime, r, count, data, color);
        }

        public void draw(Game game, GameTime gameTime, Rectangle r, Color color) {
            drawNoCount(game, gameTime, r, color);
            if(count>1){
                bool a = count > 999;
                int c = a?(count/1000):count;
                game.spriteBatch.DrawString(Game.fontNormal, c+(a?"k":""), new Vector2(r.X - 2, r.Y + 8), color);
            }
        }

        public Item getItem() {
            return item;
        }

        public int getData() {
            return data;
        }

        public ItemStack clone() {
            return new ItemStack(item, count, data);
        }

        public bool isEmpty() {
            return getItem().index == Item.ItemEmpty.index;
        }

        public int getCount() {
            return count;
        }

        public int setCount(int a) {
            count = a;
            return verify();
        }

        public int verify() {

            if (item.index == Item.ItemTile.index) {
                if (data == Tile.TileAir.index) {
                    count = 0;
                }
            }

            if (count > item.maxStack) {
                int a = count - item.maxStack;
                count = item.maxStack;
                return a;
            }

            if (count <= 0) {
                int a = count;
                count = 1;
                item = Item.ItemEmpty;
                data = 0;
                return a;
            }

            return 0;
        }

        public string getDisplayName() {
            return item.getName(count, data);
        }

        public bool isMax() {
            return count == item.maxStack;
        }

        public bool sameItem(ItemStack i) {
            return i.data == data && i.getItem().index == getItem().index;
        }

        public override bool Equals(System.Object obj) {
            if (obj == null) {
                return false;
            }

            ItemStack i = obj as ItemStack;
            return Equals(i);
        }

        public bool Equals(ItemStack i) {
            if ((object)i == null) {
                return false;
            }

            return item.index == i.item.index && count == i.count && data == i.data;
        }

        public override int GetHashCode() {
            return item.index ^ count ^ data;
        }
    }
}
