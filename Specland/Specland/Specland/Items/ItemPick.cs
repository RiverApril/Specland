using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class ItemPick : ItemSwingable {

        
        public static int furniturePower = 256;

        public int stonePower = 1;
        public int dirtPower = 1;
        public int woodPower = 1;
        public int delay = 1;
        private int t = 0;

        public ItemPick(string name, int renderType, Rectangle sourceRectangle, int stonePower, int dirtPower, int woodPower, int delay, int reach)
            : base(name, renderType, sourceRectangle, Math.Max(delay, 5)) {
            this.stonePower = stonePower;
            this.dirtPower = dirtPower;
            this.woodPower = woodPower;
            this.delay = delay;
            this.reach = reach;
        }

        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            
            int power = 0;
            int mat = game.currentWorld.getTileObjectNoCheck(xTile, yTile, false).material;
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
            if (power <= 0) {
                return stack;
            }

            if (game.currentWorld.player.swingTime <= 0 && mine(game, stack, xTile, yTile, distance, false)) {
                game.currentWorld.player.swingTime = swingMaxTime;
            }
            return stack;
        }

        public override ItemStack rightClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {

            int power = 0;
            int mat = game.currentWorld.getTileObjectNoCheck(xTile, yTile, true).material;
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
            if (power <= 0) {
                return stack;
            }

            if (game.currentWorld.player.swingTime <= 0 && mine(game, stack, xTile, yTile, distance, true)) {
                game.currentWorld.player.swingTime = swingMaxTime;
            }
            return stack;
        }

        public bool mine(Game game, ItemStack stack, int xTile, int yTile, int distance, bool isWall) {
            int power = 0;
            int del = delay;
            int mat = game.currentWorld.getTileObjectNoCheck(xTile, yTile, isWall).material;
            if (mat == Tile.MATERIAL_STONE) {
                power = stonePower;
            } else if (mat == Tile.MATERIAL_DIRT) {
                power = dirtPower;
            } else if (mat == Tile.MATERIAL_WOOD) {
                power = woodPower;
            } else if (mat == Tile.MATERIAL_FURNITURE) {
                power = furniturePower;
                del = 20;
            } else {
                power = furniturePower;
                del = 20;
            }
            if(power<=0){
                return false;
            }
            t++;
            if (t > del && distance <= reach) {
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
                    return true;
                }
            }
            return false;
        }

        public override void drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            if (game.currentWorld != null && (game.currentWorld.getTileIndex(mouseTileX, mouseTileY, false) != Tile.TileAir.index || game.currentWorld.getTileIndex(mouseTileX, mouseTileY, true) != Tile.TileAir.index)) {
                Rectangle r = new Rectangle((mouseTileX * World.tileSizeInPixels) - game.currentWorld.viewOffset.X, (mouseTileY * World.tileSizeInPixels) - game.currentWorld.viewOffset.Y, World.tileSizeInPixels, World.tileSizeInPixels);
                Game.drawRectangle(Game.dummyTexture, r, r, new Color(.5f, .5f, .5f, .5f), Game.RENDER_DEPTH_HOVER);
            }
        }

    }
}
