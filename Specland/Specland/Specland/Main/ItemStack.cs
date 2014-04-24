using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class ItemStack {

        private Item item = Item.itemEmpty;
        private int count = 1;
        private int data = 0;


        public ItemStack(Item item) : this(item, 1, 0){

        }

        public ItemStack(Tile tile, int count) : this(Item.itemTile, count, tile.index) {

        }

        public ItemStack(Item item, int count, int data) {
            this.count = count;
            this.data = data;
            this.item = item;
            verify();
        }

        public void drawNoCount(Game game, Rectangle r, Color color, float depth) {
            item.drawAsItem(game, r, count, data, color, depth);
        }

        public void draw(Game game, Rectangle r, Color color, float depthI, float depthT) {
            drawNoCount(game, r, color, depthI);
            if(count>1){
                bool a = count > 999;
                int c = a?(count/1000):count;
                Game.drawString(c+(a?"k":""), new Vector2(r.X - 2, r.Y + 8), color, depthT);
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
            return getItem().index == Item.itemEmpty.index;
        }

        public int getCount() {
            return count;
        }

        public int setCount(int a) {
            count = a;
            return verify();
        }

        public int verify() {

            if (item.index == Item.itemTile.index) {
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
                item = Item.itemEmpty;
                data = 0;
                return a;
            }

            return 0;
        }

        public string getDisplayName(bool extended) {
            string s = item.getDisplayName(count, data);
            if(extended){
                if(item is ItemTile){
                    s += "\n  Placeable";
                    Tile tile = Tile.getTileObject(data);
                    if(tile is TileDoor){
                        s += "\n  Size: 1x3";
                    }else if (tile is TileFurniture) {
                        TileFurniture furniture = (TileFurniture)tile;
                        s += "\n  Size: "+furniture.size.X+"x"+furniture.size.Y;
                    }
                }else if(item is ItemPick){
                    ItemPick pick = (ItemPick)item;
                    if (pick.stonePower>0) s += "\n  Stone Power: " + Math.Ceiling(((pick.stonePower / 256.0) * 100)) + "%";
                    if (pick.dirtPower > 0) s += "\n  Dirt Power: " + Math.Ceiling(((pick.dirtPower / 256.0) * 100)) + "%";
                    if (pick.woodPower > 0) s += "\n  Wood Power: " + Math.Ceiling(((pick.woodPower / 256.0) * 100)) + "%";
                    s += "\n  Speed: " + Math.Max((20 - pick.delay) * 5, 1) + "%";
                }
            }
            return s;
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

        internal void saveTo(List<byte> bytes) {
            bytes.AddRange(World.intToBytes(item.index));
            bytes.AddRange(World.intToBytes(count));
            bytes.AddRange(World.intToBytes(data));
        }

        internal int loadFrom(byte[] bytes, int index) {
            item = Item.getItemObject(World.bytesToInt(bytes, index)); index += 4;
            count = World.bytesToInt(bytes, index); index += 4;
            data = World.bytesToInt(bytes, index); index += 4;
            return index;
        }
    }
}
