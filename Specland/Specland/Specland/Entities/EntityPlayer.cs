using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Specland {
    public class EntityPlayer : Entity{

        private Rectangle drawBounds;
        private bool facingRight = true;

        Vector2 renderSize;

        int xoff;
        int yoff;

        public Vector2 displayPosition;

        private float walkSpeed = 16;
        private float walkAcc = 4;
        private float jumpInAirAcc = .5f;
        private float initalJumpSpeed = 12;
        private float friction = .5f;
        private float waterFriction = .9f;
        private float swimSpeed = 1;

        private bool inWater = false;

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

            drawBounds = new Rectangle((int)(displayPosition.X - world.viewOffset.X - xoff + (facingRight ? 0 : renderSize.X)), (int)displayPosition.Y - world.viewOffset.Y - (yoff), (int)((renderSize.X) * (facingRight ? 1 : -1)), (int)renderSize.Y);

            #region Movement
            if (game.inputState.getKeyboardState().IsKeyDown(Keys.A)) {
                if (speed.X > -walkSpeed) speed.X += -walkAcc;
                facingRight = false;
            }
            if (game.inputState.getKeyboardState().IsKeyDown(Keys.D)) {
                if (speed.X < walkSpeed) speed.X += walkAcc;
                facingRight = true;
            }

            if(speed.Y<0){
                if (game.inputState.getKeyboardState().IsKeyDown(Keys.Space)) {
                    speed.Y -= jumpInAirAcc;
                }
            }

            inWater = false;
            if (world.getLiquid((int)((position.X + (size.X / 2)) / World.tileSizeInPixels), (int)((position.Y + (size.Y / 2)) / World.tileSizeInPixels)) > 50) {
                inWater = true;
                if (game.inputState.getKeyboardState().IsKeyDown(Keys.Space)) {
                    if(speed.Y > -swimSpeed){
                        speed.Y -= swimSpeed;
                    }
                }
            }

            if (collision(world, 0, 1)) {
                if (game.inputState.getKeyboardState().IsKeyDown(Keys.Space)) {
                    speed.Y = -initalJumpSpeed;
                }

            } else {
                speed.Y += gravityAcc;
            }

            if (speed.X != 0) {
                speed.X *= friction;
            }

            int a = (speed.X > 0 ? 1 : (speed.X < 0 ? -1 : 0));

            if(inWater){
                speed *= waterFriction;
            }

            if(!collision(world, speed.X, 0)){
                position.X += speed.X;
            } else {
                if (!game.inputState.getKeyboardState().IsKeyDown(Keys.S)) {
                    if (collision(world, 0, 4) && !collision(world, (facingRight ? World.tileSizeInPixels : -World.tileSizeInPixels) + speed.X, -(int)(World.tileSizeInPixels * 1.2))) {
                        position.X += speed.X;
                        position.Y -= World.tileSizeInPixels*1.2f;
                    } else if (collision(world, 0, 4) && !collision(world, (facingRight ? World.tileSizeInPixels : -World.tileSizeInPixels), -(int)(World.tileSizeInPixels * 1.2))) {
                        //position.X += a;
                        position.Y -= World.tileSizeInPixels * 1.2f;
                    }
                }
            }

            if (!collision(world, 0, speed.Y)) {
                position.Y += speed.Y;
            } else {
                speed.Y = 0;
            }
            #endregion



            displayPosition += (position - displayPosition)/4;
        }

        public override void draw(Game game, World world) {
            byte a = (byte)MathHelper.Clamp(world.getLight((int)(position.X + (size.X / 2)) / World.tileSizeInPixels, (int)(position.Y + (size.Y / 2)) / World.tileSizeInPixels), 0, 255);
            game.spriteBatch.Draw(Entity.Texture_Entity_Player, drawBounds, new Rectangle(0, 0, 16, 24), new Color(a, a, a));

            if (game.inventory.currentItem != null) {
                game.inventory.currentItem.getItem().drawOverPlayer(game, game.inventory.currentItem, facingRight, Vector2.Zero, 0, position);
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
