using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class EntityItem : Entity {

        public static bool playedSoundEffectRecently = false;

        private ItemStack stack = new ItemStack(Item.ItemEmpty);

        private int maxPickupDelay = 20;
        private int pickUpDelay = 20;

        public EntityItem(Vector2 position, ItemStack stack) : base(position){
            this.stack = stack;
        }

        public EntityItem(Vector2 position, ItemStack stack, int delay) : this(position, stack){
            maxPickupDelay = delay;
            pickUpDelay = maxPickupDelay;
        }

        public override void init() {
            size = new Vector2(12, 12);
        }

        public override void beforeRemoval(Game game, World world) {

        }

        public override int getMaxHealth() {
            return 1;
        }

        public override void draw(Game game, World world) {
            byte a = (byte)MathHelper.Clamp(world.getLight((int)(position.X + (size.X / 2)) / World.tileSizeInPixels, (int)(position.Y + (size.Y / 2)) / World.tileSizeInPixels), 0, 255);
            stack.drawNoCount(game, null, new Rectangle((int)position.X - world.viewOffset.X - (World.tileSizeInPixels/2), (int)position.Y - world.viewOffset.Y - (World.tileSizeInPixels / 2), World.tileSizeInPixels * 2, World.tileSizeInPixels * 2), new Color(a, a, a));
        }

        public override void update(Game game, World world) {

            bool gravityOn = true;

            if (pickUpDelay <= 0) {
                float d = Vector2.Distance(world.player.position, position);
                Rectangle pr = new Rectangle((int)world.player.position.X, (int)world.player.position.Y, (int)world.player.size.X, (int)world.player.size.Y);
                Rectangle r = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

                foreach(Entity e in world.EntityList){
                    if (e is EntityItem) {
                        EntityItem ei = ((EntityItem)e);
                        if (e.uniqueID < uniqueID) {
                            if(ei.stack.getItem().index == stack.getItem().index && ei.stack.getData() == stack.getData()){
                                if(Vector2.Distance(ei.position, position) < (World.tileSizeInPixels)){
                                    ei.stack.setCount(stack.setCount(stack.getCount() + ei.stack.getCount()));
                                }
                            }
                        }
                    }
                }

                if (pr.Intersects(r)) {
                    stack = game.inventory.pickUp(stack);
                    if(stack.isEmpty()){
                        if (!playedSoundEffectRecently) {
                            SoundEffectPlayer.playSoundWithRandomPitch(SoundEffectPlayer.SoundPop);
                        }
                    }
                    pickUpDelay = maxPickupDelay;
                } else if (d < (6 * World.tileSizeInPixels)) {
                    position += ((world.player.position - position)/d) * 4;
                    gravityOn = false;
                }
            }

            if (stack.isEmpty()) {
                remove(game, world);
            }

            if (!collision(world, speed.X, 0)) {
                position.X += speed.X;
            } else {
                speed.X = 0;
            }

            if (!collision(world, 0, speed.Y)) {
                position.Y += speed.Y;
            } else {
                speed.Y = 0;
            }

            if (collision(world, 0, 1)) {

            } else if (gravityOn) {
                speed.Y += gravityAcc;
            }

            pickUpDelay--;
        }
    }
}
