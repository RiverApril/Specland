using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class ItemPick : ItemSwingable {

        
        public static int furniturePower = 256;

        private int stonePower = 1;
        private int dirtPower = 1;
        private int woodPower = 1;
        private int delay = 1;

        public ItemPick(string name, int renderType, Rectangle sourceRectangle, int stonePower, int dirtPower, int woodPower, int delay, int reach)
            : base(name, renderType, sourceRectangle, Math.Max(delay, 5)) {
            this.stonePower = stonePower;
            this.dirtPower = dirtPower;
            this.woodPower = woodPower;
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
            int mat = game.currentWorld.getTileObjectNoCheck(xTile, yTile, isWall).material;

            if (mat == Tile.MATERIAL_FURNITURE || mat == Tile.MATERIAL_NONE) {
                del = 20;
            }
            int power = getPower(mat);
            if (power <= 0) {
                return false;
            }
            game.inventory.t++;
            if (game.inventory.t > del && distance <= reach) {
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

        public int getPower(int mat) {
            int power = 0;

            if (mat == Tile.MATERIAL_STONE) {
                power = stonePower;
            } else if (mat == Tile.MATERIAL_DIRT) {
                power = dirtPower;
            } else if (mat == Tile.MATERIAL_WOOD) {
                power = woodPower;
            } else if (mat == Tile.MATERIAL_FURNITURE) {
                power = furniturePower;
            } else {
                power = furniturePower;
            }
            power = Game.instance.inventory.applyMiningPowerModifier(power);
            return power;
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
    }
}
