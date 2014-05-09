using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class TileDoor : Tile{

        private static int stateClosed = 0;
        private static int stateOpenRight = 1;
        private static int stateOpenLeft = 2;

        public TileDoor(string name, RenderType renderType, int textureX, int textureY)
            : base(name, renderType, Material.furniture, textureX, textureY) {

        }

        public override TextureInfo getTextureInfo(int x, int y, World world, World.TileDepth tileDepth) {

            int state = world.getTileData(x, y, tileDepth);

            int stateUp = world.getTileData(x, y - 1, tileDepth);
            int stateUpUp = world.getTileData(x, y - 2, tileDepth);

            int stateDown = world.getTileData(x, y + 1, tileDepth);
            int stateDownDown = world.getTileData(x, y + 2, tileDepth);

            int stateLeft = world.getTileData(x - 1, y, tileDepth);
            int stateRight = world.getTileData(x + 1, y, tileDepth);


            int leftIndex = world.getTileObject(x - 1, y, tileDepth).index;
            int rightIndex = world.getTileObject(x + 1, y, tileDepth).index;

            int downIndex = world.getTileObject(x, y + 1, tileDepth).index;
            int downdownIndex = world.getTileObject(x, y + 2, tileDepth).index;
            int upIndex = world.getTileObject(x, y - 1, tileDepth).index;
            int upupIndex = world.getTileObject(x, y - 2, tileDepth).index;


            bool left = leftIndex == index && stateLeft == state;
            bool right = rightIndex == index && stateRight == state;

            bool down = downIndex == index && stateDown == state;
            bool downdown = downdownIndex == index && stateDownDown == state;

            bool up = upIndex == index && stateUp == state;
            bool upup = upupIndex == index && stateUpUp == state;


            bool leftSolid = world.isTileSolid(x - 1, y, tileDepth);
            bool rightSolid = world.isTileSolid(x + 1, y, tileDepth);
            bool downSolid = world.isTileSolid(x, y + 1, tileDepth);
            bool upSolid = world.isTileSolid(x, y - 1, tileDepth);

            Rectangle r = get8(3, 3);
            Point offset = Point.Zero;
            bool hOff = false;
            bool t = transparent;
            if (stateOpenRight == state) {
                if (up && down) {
                    r = get8(1, 1, 2, 1);
                } else if (upup && up) {
                    r = get8(1, 2, 2, 1);
                } else if (downdown && down) {
                    r = get8(1, 0, 2, 1);
                }
            } else if (stateOpenLeft == state) {
                if (up && down) {
                    r = get8(3, 1, -2, 1);
                } else if (upup && up) {
                    r = get8(3, 2, -2, 1);
                } else if (downdown && down) {
                    r = get8(3, 0, -2, 1);
                }
                hOff = true;
                offset = new Point(World.tileSizeInPixels, 0);
            } else {
                if(up && down){
                    r = get8(0, 1);
                } else if (upup && up && !down) {
                    r = get8(0, 2);
                } else if (downdown && down && !up) {
                    r = get8(0, 0);
                }
            }
            return new TextureInfo(r, t, offset, hOff, false);
        }

        public override void mine(World world, int x, int y, int data, ItemPick pick, World.TileDepth tileDepth) {
            if (world.getTileDataNoCheck(x, y, tileDepth)!=0) {

            }else{
                if (world.getTileIndex(x, y - 1, tileDepth) == index && world.getTileIndex(x, y + 1, tileDepth) == index) {
                    world.setTileWithUpdate(x, y - 1, Tile.TileAir.index, tileDepth);
                    world.setTileWithUpdate(x, y + 1, Tile.TileAir.index, tileDepth);
                }else if (world.getTileIndex(x, y - 1, tileDepth) == index && world.getTileIndex(x, y - 2, tileDepth) == index) {
                    world.setTileWithUpdate(x, y - 1, Tile.TileAir.index, tileDepth);
                    world.setTileWithUpdate(x, y - 2, Tile.TileAir.index, tileDepth);
                }else if (world.getTileIndex(x, y + 1, tileDepth) == index && world.getTileIndex(x, y + 2, tileDepth) == index) {
                    world.setTileWithUpdate(x, y + 1, Tile.TileAir.index, tileDepth);
                    world.setTileWithUpdate(x, y + 2, Tile.TileAir.index, tileDepth);
                }
            }
        }

        public override void updateNearChange(World world, int x, int y, World.TileDepth tileDepth) {
            if (!(world.isTileSolid(x, y + 1, tileDepth) || world.getTileIndex(x, y + 1, tileDepth) == index)) {
                world.mineTile(x, y, Item.itemSupick, tileDepth);
            }
        }

        public override bool canBePlacedHereOverridable(World world, int x, int y, World.TileDepth tileDepth) {
            return world.getTileIndex(x, y + 1, tileDepth) != index && world.isTileSolid(x, y + 1, tileDepth) && world.getTileObject(x, y, tileDepth).isAir() && world.getTileObject(x, y - 1, tileDepth).isAir() && world.getTileObject(x, y - 2, tileDepth).isAir();
        }

        public override void justPlaced(World world, int x, int y, World.TileDepth tileDepth) {
            if (world.getTileDataNoCheck(x, y, tileDepth)!=0) {
                world.setTileWithDataWithUpdate(x, y, index, 0, tileDepth);
                world.setTileWithDataWithUpdate(x, y - 1, index, 0, tileDepth);
                world.setTileWithDataWithUpdate(x, y - 2, index, 0, tileDepth);
            }else{
                world.setTileWithDataWithUpdate(x, y - 1, index, 1, tileDepth);
                world.setTileWithDataWithUpdate(x, y - 2, index, 1, tileDepth);
            }

            updateDoor(Game.instance, stateClosed, x, y);
        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, World.TileDepth tileDepth) {
            return new ItemStack(Item.itemTile, 1, index);
        }

        public override bool drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            if (canBePlacedHere(game.currentWorld, mouseTileX, mouseTileY, World.TileDepth.tile)) {
                Rectangle rect = get8(0, 0);
                rect.Height = 24;
                Game.drawRectangle(Tile.TileSheet, new Rectangle((mouseTileX * World.tileSizeInPixels) - game.currentWorld.viewOffset.X, ((mouseTileY-2) * World.tileSizeInPixels) - game.currentWorld.viewOffset.Y, World.tileSizeInPixels, World.tileSizeInPixels*3), rect, new Color(.5f, .5f, .5f, .5f), Game.RENDER_DEPTH_HOVER);
            }
            return true;
        }

        public override ItemStack use(Game game, ItemStack currentItem, int mouseTileX, int mouseTileY, int mouseTileDistanceFromPlayer, World.TileDepth tileDepth) {

            int oldState = game.currentWorld.getTileDataNoCheck(mouseTileX, mouseTileY, tileDepth);
            int newState = oldState == stateClosed ? (game.currentWorld.player.facingRight ? stateOpenRight : stateOpenLeft) : stateClosed;

            if(newState==stateOpenLeft){
                if (!(game.currentWorld.isTileSolid(mouseTileX - 1, mouseTileY, tileDepth) || game.currentWorld.isTileSolid(mouseTileX - 1, mouseTileY - 1, tileDepth) || game.currentWorld.isTileSolid(mouseTileX - 1, mouseTileY + 1, tileDepth))) {
                    updateDoor(game, newState, mouseTileX, mouseTileY);
                }
            }
            if (newState == stateOpenRight) {
                if (!(game.currentWorld.isTileSolid(mouseTileX + 1, mouseTileY, tileDepth) || game.currentWorld.isTileSolid(mouseTileX + 1, mouseTileY - 1, tileDepth) || game.currentWorld.isTileSolid(mouseTileX + 1, mouseTileY + 1, tileDepth))) {
                    updateDoor(game, newState, mouseTileX, mouseTileY);
                }
            }
            if(newState == stateClosed){
                updateDoor(game, newState, mouseTileX, mouseTileY);
            }


            return base.use(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer, tileDepth);
        }

        private void updateDoor(Game game, int newState, int x, int y) {
            changeDoorState(game, newState, x, y);

            if (game.currentWorld.getTileIndex(x, y - 1, World.TileDepth.tile) == index) {
                changeDoorState(game, newState, x, y - 1);
                if (game.currentWorld.getTileIndex(x, y - 2, World.TileDepth.tile) == index) {
                    changeDoorState(game, newState, x, y - 2);
                }
            }
            if (game.currentWorld.getTileIndex(x, y + 1, World.TileDepth.tile) == index) {
                changeDoorState(game, newState, x, y + 1);
                if (game.currentWorld.getTileIndex(x, y + 2, World.TileDepth.tile) == index) {
                    changeDoorState(game, newState, x, y + 2);
                }
            }

            game.currentWorld.calculateTileFrame(game, x, y, World.TileDepth.tile);
            game.currentWorld.calculateTileFrame(game, x, y - 1, World.TileDepth.tile);
            game.currentWorld.calculateTileFrame(game, x, y - 2, World.TileDepth.tile);
            game.currentWorld.calculateTileFrame(game, x, y + 1, World.TileDepth.tile);
            game.currentWorld.calculateTileFrame(game, x, y - 1, World.TileDepth.tile);
        }

        private void changeDoorState(Game game, int newDoorState, int mouseTileX, int mouseTileY) {
            game.currentWorld.setTileWithDataWithUpdate(mouseTileX, mouseTileY, index, newDoorState, World.TileDepth.tile);
        }

        public override bool isSolid(World world, int x, int y) {
            if(world==null){
                return true;
            }
            return base.isSolid(world, x, y) && world.getTileDataNoCheck(x, y, World.TileDepth.tile) == stateClosed;
        }

        public override float getDepth(int x, int y, World.TileDepth tileDepth) {
            return Game.RENDER_DEPTH_TILE_DOOR;
        }
    }
}
