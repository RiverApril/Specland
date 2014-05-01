using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class ItemPick : ItemSwingable {

        
        public static int furniturePower = 256;

        private byte[] powers = new byte[] { };
        private int delay = 1;

        public ItemPick(string name, int renderType, Rectangle sourceRectangle, byte[] powers, int delay, int reach)
            : base(name, renderType, sourceRectangle, Math.Max(delay, 5)) {
            this.powers = powers;
            this.delay = delay;
            this.reach = reach;
        }

        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {

            if (game.currentWorld.player.swingTime <= 0 && mine(game, stack, xTile, yTile, distance, false)) {
                game.currentWorld.player.swingTime = swingMaxTime;
            }
            return stack;
        }

        public override ItemStack rightClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {

            if (getPower(game.currentWorld.getTileObjectNoCheck(xTile, yTile, false).material) <= 0) {
                return stack;
            }

            if (game.currentWorld.player.swingTime <= 0 && mine(game, stack, xTile, yTile, distance, true)) {
                game.currentWorld.player.swingTime = swingMaxTime;
            }
            return stack;
        }

        public bool mine(Game game, ItemStack stack, int xTile, int yTile, int distance, bool isWall) {
            int del = delay;
            Tile.Material mat = game.currentWorld.getTileObjectNoCheck(xTile, yTile, isWall).material;

            if (mat == Tile.Material.furniture) {
                del = 20;
            }
            int power = getPower(mat);
            if (power <= 0) {
                return false;
            }
            game.inventory.t++;
            if (game.inventory.t >= del && distance <= reach) {
                game.inventory.t = 0;
                Tile tile = game.currentWorld.getTileObject(xTile, yTile, isWall);
                if (tile.index != Tile.TileAir.index) {
                    SoundEffectPlayer.playSoundWithRandomPitch(SoundEffectPlayer.SoundTink);
                    int m = ((int)game.currentWorld.getCrackNoCheck(xTile, yTile));
                    if (m == 0) {
                        game.inventory.t = 0;
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
                    return true;
                }
            }
            return false;
        }

        public int getPower(Tile.Material material) {
            return Game.instance.inventory.applyMiningPowerModifier(powers[(int)material]);
        }

        public int getDelay() {
            return Game.instance.inventory.applyMiningDelayModifier(delay);
        }

        public override void drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            if (game.currentWorld != null && (game.currentWorld.getTileIndex(mouseTileX, mouseTileY, false) != Tile.TileAir.index || game.currentWorld.getTileIndex(mouseTileX, mouseTileY, true) != Tile.TileAir.index)) {
                Rectangle r = new Rectangle((mouseTileX * World.tileSizeInPixels) - game.currentWorld.viewOffset.X, (mouseTileY * World.tileSizeInPixels) - game.currentWorld.viewOffset.Y, World.tileSizeInPixels, World.tileSizeInPixels);
                Game.drawRectangle(Game.dummyTexture, r, r, new Color(.5f, .5f, .5f, .5f), Game.RENDER_DEPTH_HOVER);
            }
        }

        public static byte[] createPowers(float pick, float shovel, float axe) {

            byte[] powers = new byte[(int)Tile.Material.SIZE];

            powers[(int)Tile.Material.none] = 0;
            powers[(int)Tile.Material.stone] = (byte)Math.Min(pick * (256.0 / 16.0), 255);
            powers[(int)Tile.Material.dirt] = (byte)Math.Min(shovel * (256.0 / 16.0), 255);
            powers[(int)Tile.Material.wood] = (byte)Math.Min(axe * (256.0 / 16.0), 255);
            powers[(int)Tile.Material.furniture] = 255;

            return powers;
        }
    }
}
