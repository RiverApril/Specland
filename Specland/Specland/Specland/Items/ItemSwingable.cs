using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class ItemSwingable : Item {

        private int swingMaxTime;

        public ItemSwingable(int index, string name, int renderType, Rectangle sourceRectangle, int swingMaxTime)
            : base(index, name, renderType, sourceRectangle) {
                this.swingMaxTime = swingMaxTime;
        }

        public virtual void updateAfterClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            if (game.currentWorld.player.swingTime > 0) {

            } else {

            }
        }

        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            if (game.currentWorld.player.swingTime<=0) {
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

        public override void drawOverPlayer(Game game, ItemStack currentItem, bool facingRight, Vector2 position, Color color) {
            if (game.currentWorld.player.swingTime > 0) {
                Rectangle rect = currentItem.getItem().textureInfo.rectangle;
                float rotation = (((float)game.currentWorld.player.swingTime / (float)swingMaxTime) * 225.0f) - 135.0f;
                rotation = facingRight ? -rotation : rotation;
                rotation = MathHelper.ToRadians(rotation);
                Vector2 origin = new Vector2(facingRight ? 0 : textureInfo.rectangle.Width, textureInfo.rectangle.Height);
                game.spriteBatch.Draw(ItemSheet, new Rectangle((int)(position.X - (rect.Width / 2) - game.currentWorld.viewOffset.X), (int)(position.Y - game.currentWorld.viewOffset.Y + (rect.Width/2)), rect.Width * 2, rect.Height * 2), rect, color, rotation, origin, facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }
        }
    }
}
