using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    class EntityFallingTile : Entity{

        private int index;
        private bool isWall;
        private ItemStack stack;
        private float terminalVelocity = World.tileSizeInPixels;

        public EntityFallingTile(int x, int y, int index, bool isWall) : base(x, y) {
            this.index = index;
            this.isWall = isWall;
            stack = new ItemStack(Item.itemTile, 1, index);
        }

        public override void init(){
            size = new Vector2(World.tileSizeInPixels * .75f, World.tileSizeInPixels * .75f);
            if(isWall){
                isSolid = false;
            }else{
                isSolid = index == 0 ? false : Tile.getTileObject(index).isSolid();
            }
        }
        
        public override int getMaxHealth() {
            return 1;
        }

        public override void draw(Game game, World world) {
            byte a = (byte)MathHelper.Clamp(world.getLight((int)(position.X + (size.X / 2)) / World.tileSizeInPixels, (int)(position.Y + (size.Y / 2)) / World.tileSizeInPixels), 0, 255);
            if (isWall) {
                Tile t = Tile.getTileObject(stack.getItem().index);
                a = (byte)Clamp(a - (255 - t.wallBrightness), 0, t.wallBrightness);
            }
            stack.drawNoCount(game, new Rectangle((int)position.X - world.viewOffset.X - (World.tileSizeInPixels / 2), (int)position.Y - world.viewOffset.Y - (World.tileSizeInPixels / 2), World.tileSizeInPixels * 2, World.tileSizeInPixels * 2), new Color(a, a, a), Game.RENDER_DEPTH_ENTITY_FALLING_SAND);
        }

        public static int Clamp(int value, int min, int max) {
            return value > max ? max : (value < min ? min : (value));
        }

        public override void update(Game game, World world) {
            if(index == Tile.TileAir.index){
                remove(game, world);
                return;
            }
            if (collisionCustom(world, 0, 0, isWall)) {
                int x = (int)(position.X / World.tileSizeInPixels);
                int y = (int)(position.Y / World.tileSizeInPixels);
                remove(game, world);
                if(world.getTileIndex(x, y, isWall) == Tile.TileAir.index){
                    world.setTile(x, y, index, isWall);
                } else {
                    world.EntityAddingList.Add(new EntityItem(position, new ItemStack(Item.itemTile, 1, index)));
                }
            }
            if(speed.Y < terminalVelocity){
                speed.Y += gravityAcc;
            }

            position += speed;
        }

        public bool collisionCustom(World world, float x, float y, bool isWall) {
            Rectangle r = new Rectangle((int)(position.X + x), (int)(position.Y + y), (int)size.X, (int)size.Y);
            for (int i = (r.Left / World.tileSizeInPixels); i < (r.Right / World.tileSizeInPixels) + 1; i++) {
                for (int j = (r.Top / World.tileSizeInPixels); j < (r.Bottom / World.tileSizeInPixels) + 1; j++) {
                    Tile t = world.getTileObject(i, j, isWall);
                    if (t != null) {
                        if (t.isSolid(world, i, j)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
