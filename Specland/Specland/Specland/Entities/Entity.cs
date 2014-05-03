using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    public abstract class Entity {

        public static Texture2D Texture_Entity_Player;

        public static float gravityAcc = World.tileSizeInPixels / 16f;

        public Vector2 position = new Vector2(0, 0);
        public Vector2 size = new Vector2(10, 10);

        public Vector2 speed = new Vector2(0, 0);

        public int health = 0;

        public bool markedForRemoval = false;

        public bool isSolid = false;

        public long uniqueID;

        public Entity(Vector2 position) : this(position.X, position.Y){}

        public Entity(float x, float y) {
            uniqueID = World.getnewUniqueID();
            position.X = x;
            position.Y = y;
            init();
            health = getMaxHealth();
        }

        public abstract void init();

        public virtual int getMaxHealth() {
            return 1;
        }

        public abstract void update(Game game, World world);

        public abstract void draw(Game game, World world);

        public virtual void beforeRemoval(Game game, World world) {

        }

        public bool collisionOnlyTiles(World world, float x, float y, bool ignorePlatforms = false) {
            Rectangle r = new Rectangle((int)(position.X + x), (int)(position.Y + y), (int)size.X, (int)size.Y);
            collisionOnlyTiles(world, x, y, r, ignorePlatforms);
            return false;
        }

        public bool collisionOnlyTiles(World world, float x, float y, Rectangle r, bool ignorePlatforms = false) {
            for (int i = (r.Left / World.tileSizeInPixels); i < (r.Right / World.tileSizeInPixels) + 1; i++) {
                for (int j = (r.Top / World.tileSizeInPixels); j < (r.Bottom / World.tileSizeInPixels) + 1; j++) {
                    Tile t = world.getTileObject(i, j, false);
                    if (t != null) {
                        if (t.isSolid(world, i, j)) {
                            return true;
                        }
                        if (!ignorePlatforms && t.isPlatform(world, i, j, false)) {
                            if (r.Bottom <= ((j+1) * World.tileSizeInPixels) && speed.Y>=0) {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool collision(World world, float x, float y, bool ignorePlatforms = false) {
            Rectangle r = new Rectangle((int)(position.X + x), (int)(position.Y + y), (int)size.X, (int)size.Y);
            if (collisionOnlyTiles(world, x, y, r, ignorePlatforms)) {
                return true;
            }
            foreach (Entity e in world.EntityList) {
                if(e==null){
                    continue;
                }
                if (e.isSolid) {
                    if (r.Intersects(new Rectangle((int)(e.position.X), (int)(e.position.Y), (int)(e.size.X), (int)(e.size.Y)))) {
                        return true;
                    }
                }

            }
            return false;
        }

        public void remove(Game game, World world) {
            beforeRemoval(game, world);
            world.EntityRemovalList.Add(this);
            markedForRemoval = true;
        }

        public static void loadContent(ContentManager Content) {
            Texture_Entity_Player = Content.Load<Texture2D>("Images\\Entity_Player");
        }
    }
}
