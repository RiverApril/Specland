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

        public override TextureInfo getTextureInfo(int x, int y, World world, bool isWall) {

            int state = world.getTileData(x, y, false);

            int stateUp = world.getTileData(x, y - 1, false);
            int stateUpUp = world.getTileData(x, y - 2, false);

            int stateDown = world.getTileData(x, y + 1, false);
            int stateDownDown = world.getTileData(x, y + 2, false);

            int stateLeft = world.getTileData(x - 1, y, false);
            int stateRight = world.getTileData(x + 1, y, false);


            int leftIndex = world.getTileObject(x - 1, y, isWall).index;
            int rightIndex = world.getTileObject(x + 1, y, isWall).index;

            int downIndex = world.getTileObject(x, y + 1, isWall).index;
            int downdownIndex = world.getTileObject(x, y + 2, isWall).index;
            int upIndex = world.getTileObject(x, y - 1, isWall).index;
            int upupIndex = world.getTileObject(x, y - 2, isWall).index;


            bool left = leftIndex == index && stateLeft == state;
            bool right = rightIndex == index && stateRight == state;

            bool down = downIndex == index && stateDown == state;
            bool downdown = downdownIndex == index && stateDownDown == state;

            bool up = upIndex == index && stateUp == state;
            bool upup = upupIndex == index && stateUpUp == state;


            bool leftSolid = world.isTileSolid(x - 1, y, isWall);
            bool rightSolid = world.isTileSolid(x + 1, y, isWall);
            bool downSolid = world.isTileSolid(x, y + 1, isWall);
            bool upSolid = world.isTileSolid(x, y - 1, isWall);

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

        public override void mine(World world, int x, int y, int data, ItemPick pick, bool isWall) {
            if (world.getTileDataNoCheck(x, y, isWall)!=0) {

            }else{
                if (world.getTileIndex(x, y - 1, isWall) == index && world.getTileIndex(x, y + 1, isWall) == index) {
                    world.setTileWithUpdate(x, y - 1, Tile.TileAir.index, isWall);
                    world.setTileWithUpdate(x, y + 1, Tile.TileAir.index, isWall);
                }else if (world.getTileIndex(x, y - 1, isWall) == index && world.getTileIndex(x, y - 2, isWall) == index) {
                    world.setTileWithUpdate(x, y - 1, Tile.TileAir.index, isWall);
                    world.setTileWithUpdate(x, y - 2, Tile.TileAir.index, isWall);
                }else if (world.getTileIndex(x, y + 1, isWall) == index && world.getTileIndex(x, y + 2, isWall) == index) {
                    world.setTileWithUpdate(x, y + 1, Tile.TileAir.index, isWall);
                    world.setTileWithUpdate(x, y + 2, Tile.TileAir.index, isWall);
                }
            }
        }

        public override void updateNearChange(World world, int x, int y, bool isWall) {
            if (!(world.isTileSolid(x, y + 1, isWall) || world.getTileIndex(x, y + 1, isWall) == index)) {
                world.mineTile(x, y, Item.itemSupick, isWall);
            }
        }

        public override bool canBePlacedHereOverridable(World world, int x, int y, bool isWall) {
            return world.getTileIndex(x, y + 1, isWall) != index && world.isTileSolid(x, y + 1, isWall) && world.getTileObject(x, y, isWall).isAir() && world.getTileObject(x, y - 1, isWall).isAir() && world.getTileObject(x, y - 2, isWall).isAir();
        }

        public override void justPlaced(World world, int x, int y, bool isWall) {
            if (world.getTileDataNoCheck(x, y, isWall)!=0) {
                world.setTileWithDataWithUpdate(x, y, index, 0, isWall);
                world.setTileWithDataWithUpdate(x, y - 1, index, 0, isWall);
                world.setTileWithDataWithUpdate(x, y - 2, index, 0, isWall);
            }else{
                world.setTileWithDataWithUpdate(x, y - 1, index, 1, isWall);
                world.setTileWithDataWithUpdate(x, y - 2, index, 1, isWall);
            }

            updateDoor(Game.instance, stateClosed, x, y);
        }

        public override ItemStack dropStack(World world, ItemPick itemPick, Random rand, int x, int y, bool isWall) {
            return new ItemStack(Item.itemTile, 1, index);
        }

        public override bool drawHover(Game game, int mouseTileX, int mouseTileY, ItemStack currentItem) {
            if (canBePlacedHere(game.currentWorld, mouseTileX, mouseTileY, false)) {
                Rectangle rect = get8(0, 0);
                rect.Height = 24;
                Game.drawRectangle(Tile.TileSheet, new Rectangle((mouseTileX * World.tileSizeInPixels) - game.currentWorld.viewOffset.X, ((mouseTileY-2) * World.tileSizeInPixels) - game.currentWorld.viewOffset.Y, World.tileSizeInPixels, World.tileSizeInPixels*3), rect, new Color(.5f, .5f, .5f, .5f), Game.RENDER_DEPTH_HOVER);
            }
            return true;
        }

        public override ItemStack use(Game game, ItemStack currentItem, int mouseTileX, int mouseTileY, int mouseTileDistanceFromPlayer, bool isWall) {

            int oldState = game.currentWorld.getTileDataNoCheck(mouseTileX, mouseTileY, false);
            int newState = oldState == stateClosed ? (game.currentWorld.player.facingRight ? stateOpenRight : stateOpenLeft) : stateClosed;

            if(newState==stateOpenLeft){
                if (!(game.currentWorld.isTileSolid(mouseTileX - 1, mouseTileY, false) || game.currentWorld.isTileSolid(mouseTileX - 1, mouseTileY - 1, false) || game.currentWorld.isTileSolid(mouseTileX - 1, mouseTileY + 1, false))) {
                    updateDoor(game, newState, mouseTileX, mouseTileY);
                }
            }
            if (newState == stateOpenRight) {
                if (!(game.currentWorld.isTileSolid(mouseTileX + 1, mouseTileY, false) || game.currentWorld.isTileSolid(mouseTileX + 1, mouseTileY - 1, false) || game.currentWorld.isTileSolid(mouseTileX + 1, mouseTileY + 1, false))) {
                    updateDoor(game, newState, mouseTileX, mouseTileY);
                }
            }
            if(newState == stateClosed){
                updateDoor(game, newState, mouseTileX, mouseTileY);
            }


            return base.use(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer, isWall);
        }

        private void updateDoor(Game game, int newState, int x, int y) {
            changeDoorState(game, newState, x, y);

            if (game.currentWorld.getTileIndex(x, y - 1, false) == index) {
                changeDoorState(game, newState, x, y - 1);
                if (game.currentWorld.getTileIndex(x, y - 2, false) == index) {
                    changeDoorState(game, newState, x, y - 2);
                }
            }
            if (game.currentWorld.getTileIndex(x, y + 1, false) == index) {
                changeDoorState(game, newState, x, y + 1);
                if (game.currentWorld.getTileIndex(x, y + 2, false) == index) {
                    changeDoorState(game, newState, x, y + 2);
                }
            }

            game.currentWorld.calculateTileFrame(game, x, y, false);
            game.currentWorld.calculateTileFrame(game, x, y - 1, false);
            game.currentWorld.calculateTileFrame(game, x, y - 2, false);
            game.currentWorld.calculateTileFrame(game, x, y + 1, false);
            game.currentWorld.calculateTileFrame(game, x, y - 1, false);
        }

        private void changeDoorState(Game game, int newDoorState, int mouseTileX, int mouseTileY) {
            game.currentWorld.setTileWithDataWithUpdate(mouseTileX, mouseTileY, index, newDoorState, false);
        }

        public override bool isSolid(World world, int x, int y) {
            if(world==null){
                return true;
            }
            return base.isSolid(world, x, y) && world.getTileDataNoCheck(x, y, false)==stateClosed;
        }

        public override float getDepth(int x, int y, bool isWall) {
            return Game.RENDER_DEPTH_TILE_DOOR;
        }
    }
}
