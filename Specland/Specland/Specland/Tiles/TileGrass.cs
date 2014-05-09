using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    class TileGrass : Tile {

        public TileGrass(string name, RenderType renderType, Material material, int textureX, int textureY) : base(name, renderType, material, textureX, textureY) { }


        public override void updateRandom(World world, int x, int y, World.TileDepth tileDepth) {
            if (world.getTileIndex(x - 1, y, tileDepth) == Tile.TileDirt.index) {
                growTo(world, x - 1, y, tileDepth);
            }
            if (world.getTileIndex(x + 1, y, tileDepth) == Tile.TileDirt.index) {
                growTo(world, x + 1, y, tileDepth);
            }
            if (world.getTileIndex(x, y - 1, tileDepth) == Tile.TileDirt.index) {
                growTo(world, x, y - 1, tileDepth);
            }
            if (world.getTileIndex(x, y + 1, tileDepth) == Tile.TileDirt.index) {
                growTo(world, x, y + 1, tileDepth);
            }

            if (world.getTileIndex(x - 1, y - 1, tileDepth) == Tile.TileDirt.index) {
                growTo(world, x - 1, y - 1, tileDepth);
            }
            if (world.getTileIndex(x + 1, y - 1, tileDepth) == Tile.TileDirt.index) {
                growTo(world, x + 1, y - 1, tileDepth);
            }
            if (world.getTileIndex(x + 1, y + 1, tileDepth) == Tile.TileDirt.index) {
                growTo(world, x + 1, y + 1, tileDepth);
            }
            if (world.getTileIndex(x - 1, y + 1, tileDepth) == Tile.TileDirt.index) {
                growTo(world, x - 1, y + 1, tileDepth);
            }
        }

        private void growTo(World world, int x, int y, World.TileDepth tileDepth) {
            bool grow = false;
            if (world.getTileObject(x - 1, y, tileDepth).transparent) {
                grow = true;
            }
            if (world.getTileObject(x + 1, y, tileDepth).transparent) {
                grow = true;
            }
            if (world.getTileObject(x, y - 1, tileDepth).transparent) {
                grow = true;
            }
            if (world.getTileObject(x, y + 1, tileDepth).transparent) {
                grow = true;
            }
            if (grow) {
                world.setTile(x, y, Tile.TileGrass, tileDepth);
            }
        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, World.TileDepth tileDepth) {
            return new ItemStack(Item.itemTile, 1, Tile.TileDirt.index);
        }
    }
}
