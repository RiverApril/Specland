using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class TileFurniture : Tile {

        private Point size = new Point(0, 0);

        public TileFurniture(string name, int renderType, int textureX, int textureY, int width, int height)
            : base(name, renderType, textureX, textureY) {
            size = new Point(width, height);
        }

        public override TextureInfo getTextureInfo(int x, int y, World world, bool isWall) {

            Rectangle r = get8(3, 3);

            int l = world.getTileDataNoCheck(x, y, isWall);

            int k = 1;

            bool kk = false;

            for (int i = 0; i < size.X; i++) {
                for (int j = size.Y - 1; j >= 0; j--) {
                    if (k == l || k == -l) {
                        r = get8(l<0?(size.X-1-i):i, j);
                        kk = true;
                        break;
                    }
                    k++;
                }
                if (kk) {
                    break;
                }
            }


            return new TextureInfo(r, true, Point.Zero, l<0, false);
        }

        public override void mine(World world, int x, int y, int data, ItemPick pick, bool isWall) {

            int k = 1;

            int ii = 0;
            int jj = 0;
            bool kk = false;

            for (int i = 0; i < size.X; i++) {
                for (int j = size.Y-1; j >=0 ; j--) {
                    if (k == data || k == -data) {
                        ii = i;
                        jj = j;
                        kk = true;
                        break;
                    }
                    k++;
                }
                if (kk) {
                    break;
                }
            }

            for (int i = 0; i < size.X; i++) {
                for (int j = size.Y - 1; j >= 0; j--) {
                    world.setTileWithUpdate(x + i - ii, y + j - jj, Tile.TileAir.index, isWall);
                }
            }

        }

        public override void updateNearChange(World world, int x, int y, bool isWall) {
            if (!(world.isTileSolid(x, y + 1, isWall) || world.getTileIndex(x, y + 1, isWall) == index)) {
                world.mineTile(x, y, Item.itemSupick, isWall);
            }
        }

        public override bool canBePlacedHereOverridable(World world, int x, int y, bool isWall) {

            for (int i = 0; i < size.X; i++) {
                for (int j = size.Y - 1; j >= 0; j--) {

                    if (!world.getTileObject(x + i, y + j - (size.Y - 1), isWall).isAir()) {
                        return false;
                    }
                }
            }

            for (int i = 0; i < size.X; i++) {
                if (!world.isTileSolid(x + i, y + 1, isWall)) {
                    return false;
                }
            }

            return true;
        }

        public override void justPlaced(World world, int x, int y, bool isWall) {

            bool right = Game.instance.currentWorld.player.facingRight;

            int k = 1;

            for (int i = 0; i < size.X; i++) {
                for (int j = size.Y - 1; j >= 0; j--) {
                    world.setTileWithDataWithUpdate(x + i, y + j-(size.Y-1), index, right?k:-k, isWall);
                    k++;
                }
            }

        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, bool isWall) {
            return new ItemStack(Item.itemTile, 1, index);
        }

        public override bool drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            if (canBePlacedHere(game.currentWorld, mouseTileX, mouseTileY, false)) {
                Rectangle rect = get8(0, 0);
                rect.Width = 8 * size.X;
                rect.Height = 8 * size.Y;
                game.spriteBatch.Draw(Tile.TileSheet, new Rectangle(((mouseTileX) * World.tileSizeInPixels) - game.currentWorld.viewOffset.X, ((mouseTileY - (size.Y - 1)) * World.tileSizeInPixels) - game.currentWorld.viewOffset.Y, World.tileSizeInPixels * size.X, World.tileSizeInPixels * size.Y), rect, new Color(.5f, .5f, .5f, .5f), 0, Vector2.Zero, game.currentWorld.player.facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, World.RENDER_DEPTH_HOVER);
            }
            return true;
        }

        private void updateFurniture(Game game, int x, int y) {

            for (int i = -size.X; i < size.X; i++) {
                for (int j = -size.Y; j < size.Y; j++) {
                    game.currentWorld.calculateTileFrame(game, x + i, y + j, false);
                }
            }

        }

        public override bool isSolid(int x, int y) {
            return base.isSolid(x, y);
        }
    }
}
