using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    class TileGrass : Tile {

        public TileGrass(string name, int renderType, int textureX, int textureY) : base(name, renderType, textureX, textureY) { }


        public override void updateRandom(World world, int x, int y, bool isWall) {
            if (world.getTileIndex(x - 1, y, isWall) == Tile.TileDirt.index) {
                growTo(world, x - 1, y, isWall);
            }
            if (world.getTileIndex(x + 1, y, isWall) == Tile.TileDirt.index) {
                growTo(world, x + 1, y, isWall);
            }
            if (world.getTileIndex(x, y - 1, isWall) == Tile.TileDirt.index) {
                growTo(world, x, y - 1, isWall);
            }
            if (world.getTileIndex(x, y + 1, isWall) == Tile.TileDirt.index) {
                growTo(world, x, y + 1, isWall);
            }

            if (world.getTileIndex(x - 1, y - 1, isWall) == Tile.TileDirt.index) {
                growTo(world, x - 1, y - 1, isWall);
            }
            if (world.getTileIndex(x + 1, y - 1, isWall) == Tile.TileDirt.index) {
                growTo(world, x + 1, y - 1, isWall);
            }
            if (world.getTileIndex(x + 1, y + 1, isWall) == Tile.TileDirt.index) {
                growTo(world, x + 1, y + 1, isWall);
            }
            if (world.getTileIndex(x - 1, y + 1, isWall) == Tile.TileDirt.index) {
                growTo(world, x - 1, y + 1, isWall);
            }
        }

        private void growTo(World world, int x, int y, bool isWall) {
            bool grow = false;
            if (world.getTileObject(x - 1, y, isWall).transparent) {
                grow = true;
            }
            if (world.getTileObject(x + 1, y, isWall).transparent) {
                grow = true;
            }
            if (world.getTileObject(x, y - 1, isWall).transparent) {
                grow = true;
            }
            if (world.getTileObject(x, y + 1, isWall).transparent) {
                grow = true;
            }
            if (grow) {
                world.setTile(x, y, Tile.TileGrass, isWall);
            }
        }

        public override ItemStack dropStack(ItemPick itemPick, Random rand) {
            return new ItemStack(Item.ItemTile, 1, Tile.TileDirt.index);
        }
    }
}
