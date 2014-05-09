using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class ItemSwingable : Item {

        protected int swingMaxTime;

        protected bool swings = true;

        public ItemSwingable(string name, int renderType, Rectangle sourceRectangle, int swingMaxTime)
            : base(name, renderType, sourceRectangle) {
                this.swingMaxTime = swingMaxTime;
        }

        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            if (game.currentWorld.player.swingTime <= 0 || game.currentWorld.player.swingTime > swingMaxTime) {
                game.currentWorld.player.swingTime = swingMaxTime;
            }
            return stack;
        }

        public override ItemStack rightClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            if (game.currentWorld.player.swingTime <= 0 || game.currentWorld.player.swingTime > swingMaxTime) {
                game.currentWorld.player.swingTime = swingMaxTime;
            }
            return stack;
        }

        public override void drawOverPlayer(Game game, ItemStack currentItem, bool facingRight, Vector2 position, Color color, Point mousePosition) {
            if (game.currentWorld.player.swingTime >= 0) {
                Rectangle rect = currentItem.getItem().textureInfo.rectangle;

                float rotation = 0;

                int x = (int)(position.X - (rect.Width / 2) - game.currentWorld.viewOffset.X);
                int y = (int)(position.Y - game.currentWorld.viewOffset.Y + (rect.Width / 2));

                if(swings){
                    rotation = (((float)game.currentWorld.player.swingTime / (float)swingMaxTime) * 225.0f) - 135.0f;
                    rotation = facingRight ? -rotation : rotation;
                } else {
                    rotation = MathHelper.ToDegrees((float)Math.Atan2((y - mousePosition.Y), (x - mousePosition.X))) + 45;
                    rotation = facingRight ? rotation+180 : rotation-90;
                }

                rotation = MathHelper.ToRadians(rotation);
                Vector2 origin = new Vector2(facingRight ? 0 : textureInfo.rectangle.Width, textureInfo.rectangle.Height);
                game.spriteBatch.Draw(ItemSheet, new Rectangle(x, y, rect.Width * 2, rect.Height * 2), rect, color, rotation, origin, facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, Game.RENDER_DEPTH_OVER_PLAYER);
            }
        }
    }
}
