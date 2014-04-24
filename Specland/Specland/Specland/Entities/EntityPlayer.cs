using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Specland {
    public class EntityPlayer : Entity{

        private Rectangle drawBounds;
        public bool facingRight = true;

        Vector2 renderSize;

        int xoff;
        int yoff;

        public Vector2 displayPosition;
        public Vector2 displayPositionOff;

        private float walkSpeed = 5;
        private float walkAcc = .3f;
        private float jumpInAirAcc = .5f;
        private float initalJumpSpeed = 12;
        private float friction = .8f; //1 = never slow down & 0 = stop imediantly
        private float waterFriction = .9f;
        private float swimSpeed = 2;
        private float swimAcc = 1;
        private float jumpInWaterSpeed = 2;

        private bool inWater = false;
        private Vector2 walkUpPosition;

        public int swingTime = 0;

        public EntityPlayer(int x, int y) : base(x, y) { }

        public override void init() {
            renderSize = new Vector2(2 * World.tileSizeInPixels, 3 * World.tileSizeInPixels);
            size = new Vector2(World.tileSizeInPixels, 2 * World.tileSizeInPixels);
            xoff = World.tileSizeInPixels / 2;
            yoff = (int)(World.tileSizeInPixels * 1.5) / 2;
        }

        public override int getMaxHealth() {
            return 100;
        }

        public override void update(Game game, World world) {

            if (game.currentWorld.player.swingTime > 0) {
                game.currentWorld.player.swingTime -= 1;
            }

            drawBounds = new Rectangle((int)(displayPosition.X - world.viewOffset.X - xoff + (facingRight ? 0 : renderSize.X)), (int)displayPosition.Y - world.viewOffset.Y - (yoff), (int)((renderSize.X) * (facingRight ? 1 : -1)), (int)renderSize.Y);

            #region Movement

            inWater = false;

            bool walking = false;

            bool liquidBelow = (world.getLiquid((int)((position.X + (size.X / 2)) / World.tileSizeInPixels), (int)((position.Y + (size.Y)) / World.tileSizeInPixels)) > 50);

            if (world.getLiquid((int)((position.X + (size.X / 2)) / World.tileSizeInPixels), (int)((position.Y + (size.Y / 2)) / World.tileSizeInPixels)) > 50) {
                inWater = true;

                if (game.inputState.getKeyboardState().IsKeyDown(Game.KEY_MOVE_LEFT)) {
                    if (speed.X > -swimSpeed) speed.X += -swimAcc;
                    walking = true;
                    facingRight = false;
                }
                if (game.inputState.getKeyboardState().IsKeyDown(Game.KEY_MOVE_RIGHT)) {
                    if (speed.X < swimSpeed) speed.X += swimAcc;
                    walking = true;
                    facingRight = true;
                }
                if (game.inputState.getKeyboardState().IsKeyDown(Game.KEY_MOVE_JUMP)) {
                    if (speed.Y > -swimSpeed) {
                        speed.Y -= swimSpeed;
                    }
                }
            } else {

                if (game.inputState.getKeyboardState().IsKeyDown(Game.KEY_MOVE_LEFT)) {
                    if (speed.X > -walkSpeed) speed.X += -walkAcc;
                    facingRight = false;
                    walking = true;
                }
                if (game.inputState.getKeyboardState().IsKeyDown(Game.KEY_MOVE_RIGHT)) {
                    if (speed.X < walkSpeed) speed.X += walkAcc;
                    facingRight = true;
                    walking = true;
                }

                if (collision(world, 0, 1)) {
                    if (game.inputState.getKeyboardState().IsKeyDown(Game.KEY_MOVE_JUMP)) {
                        speed.Y = -initalJumpSpeed;
                    }

                }

                if (speed.Y < 0) {
                    if (game.inputState.getKeyboardState().IsKeyDown(Game.KEY_MOVE_JUMP)) {
                        speed.Y -= jumpInAirAcc;
                    }
                }

                if(liquidBelow){
                    if (game.inputState.getKeyboardState().IsKeyDown(Game.KEY_MOVE_JUMP)) {
                        speed.Y -= jumpInWaterSpeed;
                    }
                }

            }

            if (!collision(world, 0, 1)) {
                speed.Y += gravityAcc;
            }

            if (speed.X != 0 && !walking) {
                speed.X *= friction;
            }

            if (inWater) {
                speed *= waterFriction;
            }

            int a = (speed.X > 0 ? 1 : (speed.X < 0 ? -1 : 0));


            walkUpPosition *= .7f;

            if(!collision(world, speed.X, 0)){
                position.X += speed.X;
            } else {
                bool moved = false;
                bool goingRight = speed.X > 0;
                if (!game.inputState.getKeyboardState().IsKeyDown(Game.KEY_MOVE_DOWN)) {
                    if (collision(world, 0, 4) && !collision(world, (goingRight ? World.tileSizeInPixels : -World.tileSizeInPixels) + speed.X, -(int)(World.tileSizeInPixels * 1.2))) {
                        
                        position.X += speed.X;
                        position.Y -= World.tileSizeInPixels * 1.2f;

                        walkUpPosition.X += speed.X;
                        walkUpPosition.Y -= World.tileSizeInPixels * 1.2f;

                        moved = true;

                    } else if (collision(world, 0, 4) && !collision(world, (goingRight ? World.tileSizeInPixels : -World.tileSizeInPixels), -(int)(World.tileSizeInPixels * 1.2))) {
                        position.Y -= World.tileSizeInPixels * 1.2f;

                        walkUpPosition.Y -= World.tileSizeInPixels * 1.2f;

                        moved = true;
                    }
                }
                if(!moved){
                    speed.X = 0;
                }
            }

            if (!collision(world, 0, speed.Y)) {
                position.Y += speed.Y;
            } else {
                speed.Y = 0;
            }
            #endregion

            //displayPosition += (position - displayPosition)/4;
            displayPosition = position - (displayPositionOff + (walkUpPosition - displayPositionOff));

            //Game.updateMessage += "Player Position: "+position.X+", "+position.Y;

        }

        public override void draw(Game game, World world) {
            byte a = (byte)MathHelper.Clamp(world.getLight((int)(position.X + (size.X / 2)) / World.tileSizeInPixels, (int)(position.Y + (size.Y / 2)) / World.tileSizeInPixels), 0, 255);
            Game.drawRectangle(Entity.Texture_Entity_Player, drawBounds, new Rectangle(0, 0, 16, 24), World.grayColors[a], Game.RENDER_DEPTH_PLAYER);

            if (game.inventory.currentItem != null) {
                game.inventory.currentItem.getItem().drawOverPlayer(game, game.inventory.currentItem, facingRight, new Vector2(displayPosition.X - xoff + (facingRight ? renderSize.X*1f : renderSize.X*.5f), displayPosition.Y), World.grayColors[a]);
            }
        }

        public override void beforeRemoval(Game game, World world) {

        }

        public void saveTo(List<byte> bytes) {
            byte[] posX = World.intToBytes((int)position.X);
            byte[] posY = World.intToBytes((int)position.Y);

            bytes.Add(posX[0]);
            bytes.Add(posX[1]);
            bytes.Add(posX[2]);
            bytes.Add(posX[3]);

            bytes.Add(posY[0]);
            bytes.Add(posY[1]);
            bytes.Add(posY[2]);
            bytes.Add(posY[3]);
        }

        internal void loadFrom(byte[] bytes, int index) {
            position.X = World.bytesToInt(bytes, index); index += 4;
            position.Y = World.bytesToInt(bytes, index); index += 4;
        }
    }
}
