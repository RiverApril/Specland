using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class ItemPick : Item{

        public int power = 1;
        public int delay = 1;
        private int t = 0;

        public ItemPick(int index, string name, int renderType, int textureX, int textureY, int power, int delay, int reach) : base(index, name, renderType, textureX, textureY) {
            this.power = power;
            this.delay = delay;
            this.reach = reach;
        }

        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            return mine(game, stack, xTile, yTile, distance, false);
        }

        public override ItemStack rightClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            return mine(game, stack, xTile, yTile, distance, true);
        }

        public ItemStack mine(Game game, ItemStack stack, int xTile, int yTile, int distance, bool isWall) {
            t++;
            if (t > delay && distance <= reach) {
                t = 0;
                Tile tile = game.currentWorld.getTileObject(xTile, yTile, isWall);
                if (tile.index != Tile.TileAir.index) {
                    SoundEffectPlayer.playSoundWithRandomPitch(SoundEffectPlayer.SoundTink);
                    int m = ((int)game.currentWorld.getCrackNoCheck(xTile, yTile));
                    if (m == 0) {
                        t = 0;
                    }
                    int n = m + power;
                    int max = (Tile.getTileObject(tile.index)).toughness;
                    if (n > max) {
                        n = max;
                    }
                    game.currentWorld.setCrackNoCheck(xTile, yTile, (byte)n);

                    if (n == max) {
                        if (game.currentWorld.mineTile(xTile, yTile, this, isWall)) {
                            game.currentWorld.setCrackNoCheck(xTile, yTile, 0);
                        }
                    }
                }
            }
            return stack;
        }

        public override void drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            if (game.currentWorld!=null) {
                game.spriteBatch.Draw(Game.dummyTexture, new Rectangle((mouseTileX * World.tileSizeInPixels) - game.currentWorld.viewOffset.X, (mouseTileY * World.tileSizeInPixels) - game.currentWorld.viewOffset.Y, World.tileSizeInPixels, World.tileSizeInPixels), new Color(.5f, .5f, .5f, .5f));
            }
        }

    }
}
