using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class ItemTile : Item {

        public ItemTile(string name, int renderType, Rectangle sourceRectangle)
            : base(name, renderType, sourceRectangle) {
                maxStack = 99999;
        }

        public override void drawAsItem(Game game, Rectangle r, int count, int data, Color color, float depth) {
            if (renderType == RenderTypeNormal) {
                r.X += r.Width / 4;
                r.Y += r.Height / 4;
                r.Width /= 2;
                r.Height /= 2;
                Game.drawRectangle(Tile.TileSheet, r, Tile.getTileObject(data).getItemRect(), color, depth);
            }
        }

        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            if (Tile.getTileObject(stack.getData()) != null && distance <= reach) {
                if (game.currentWorld.getTileIndex(xTile, yTile, false) == Tile.TileAir.index) {
                    if (Tile.getTileObject(stack.getData()).canBePlacedHere(game.currentWorld, xTile, yTile, false)) {
                        if (!collision(game.currentWorld, xTile, yTile, Tile.getTileObject(stack.getData()).isSolid(xTile, yTile))) {
                            if (game.currentWorld.setTileWithUpdate(xTile, yTile, stack.getData(), false)) {
                                game.currentWorld.getTileObjectNoCheck(xTile, yTile, false).justPlaced(game.currentWorld, xTile, yTile, false);
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
                                game.currentWorld.getTileObjectNoCheck(xTile, yTile, true).justPlaced(game.currentWorld, xTile, yTile, true);
                                stack.setCount(stack.getCount() - 1);
                            }
                        }
                    }
                }
            }
            return stack;
        }

        public override string getDisplayName(int count, int data) {
            if (maxStack > 1) {
                Tile tile = Tile.getTileObject(data);
                return (count == 1 ? tile.displayName : (tile.displayNamePlural+" (" + formatNumber(count) + ")"));
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
            if (!Tile.getTileObject(currentItem.getData()).drawHover(game, mouseTileX, mouseTileY, currentItem)) {
                Tile tile = Tile.getTileObject(currentItem.getData());
                bool tileGoesHere = tile.canBePlacedHere(game.currentWorld, mouseTileX, mouseTileY, true);
                bool wallGoesHere = tile.canBePlacedHere(game.currentWorld, mouseTileX, mouseTileY, false);
                if (tileGoesHere || wallGoesHere) {
                    Game.drawRectangle(Tile.TileSheet, new Rectangle((mouseTileX * World.tileSizeInPixels) - game.currentWorld.viewOffset.X, (mouseTileY * World.tileSizeInPixels) - game.currentWorld.viewOffset.Y, World.tileSizeInPixels, World.tileSizeInPixels), Tile.getTileObject(currentItem.getData()).getTextureInfo(mouseTileX, mouseTileY, game.currentWorld, false).rectangle, new Color(.5f, .5f, .5f, .5f), Game.RENDER_DEPTH_HOVER);
                }
            }

        }


    }
}
