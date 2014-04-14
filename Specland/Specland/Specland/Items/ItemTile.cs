using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class ItemTile : Item {

        public ItemTile(int index, string name, int renderType, Rectangle sourceRectangle)
            : base(index, name, renderType, sourceRectangle) {
                maxStack = 99999;
        }

        public override void drawAsItem(Game game, GameTime gameTime, Rectangle r, int count, int data, Color color) {
            if (renderType == RenderTypeNormal) {
                r.X += r.Width / 4;
                r.Y += r.Height / 4;
                r.Width /= 2;
                r.Height /= 2;
                game.spriteBatch.Draw(Tile.TileSheet, r, Tile.getTileObject(data).get8(3, 3), color);
            }
        }

        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            if (Tile.getTileObject(stack.getData()) != null && distance <= reach) {
                if (game.currentWorld.getTileIndex(xTile, yTile, false) == Tile.TileAir.index) {
                    if (Tile.getTileObject(stack.getData()).canBePlacedHere(game.currentWorld, xTile, yTile, false)) {
                        if (!collision(game.currentWorld, xTile, yTile, Tile.getTileObject(stack.getData()).solid)) {
                            if (game.currentWorld.setTileWithUpdate(xTile, yTile, stack.getData(), false)) {
                                stack.setCount(stack.getCount() - 1);
                            }
                        }
                    }
                }
            }
            return stack;
        }

        public override ItemStack rightClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            if (Tile.getTileObject(stack.getData()) != null && distance <= reach) {
                if (game.currentWorld.getTileIndex(xTile, yTile, true) == Tile.TileAir.index) {
                    if (Tile.getTileObject(stack.getData()).canBeWall) {
                        if (Tile.getTileObject(stack.getData()).canBePlacedHere(game.currentWorld, xTile, yTile, true)) {
                            if (game.currentWorld.setTileWithUpdate(xTile, yTile, stack.getData(), true)) {
                                stack.setCount(stack.getCount() - 1);
                            }
                        }
                    }
                }
            }
            return stack;
        }

        public override string getName(int count, int data) {
            if (maxStack > 1) {
                return (Tile.getTileObject(data).name) + " Tile" + (count == 1 ? "" : "s") + " (" + formatNumber(count) + ")";
            } else {
                return name;
            }
        }

        private string formatNumber(int number) {
            return number.ToString("#,##0");
        }



        public bool collision(World world, int xTile, int yTile, bool isSolid) {
            Rectangle r = new Rectangle((xTile * World.tileSizeInPixels), (yTile * World.tileSizeInPixels), World.tileSizeInPixels, World.tileSizeInPixels);
            foreach(Entity e in world.EntityList){
                if (e.isSolid || (e is EntityPlayer && isSolid) || e is EntityFallingTile) {
                    if (r.Intersects(new Rectangle((int)(e.position.X), (int)(e.position.Y), (int)(e.size.X), (int)(e.size.Y)))) {
                        return true;
                    }
                }

            }
            return false;
        }

        public override void drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            bool tileGoesHere = Tile.getTileObject(currentItem.getData()).canBePlacedHere(game.currentWorld, mouseTileX, mouseTileY, true);
            bool wallGoesHere = Tile.getTileObject(currentItem.getData()).canBePlacedHere(game.currentWorld, mouseTileX, mouseTileY, false);
            if (tileGoesHere || wallGoesHere) {
                game.spriteBatch.Draw(Tile.TileSheet, new Rectangle((mouseTileX * World.tileSizeInPixels) - game.currentWorld.viewOffset.X, (mouseTileY * World.tileSizeInPixels) - game.currentWorld.viewOffset.Y, World.tileSizeInPixels, World.tileSizeInPixels), Tile.getTileObject(currentItem.getData()).getTextureInfo(mouseTileX, mouseTileY, game.currentWorld, false).rectangle, new Color(.5f, .5f, .5f, .5f));
            }
        }


    }
}
