using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class ItemSwingable : Item {

        public ItemSwingable(int index, string name, int renderType, int textureX, int textureY)
            : base(index, name, renderType, textureX, textureY) {
            
        }

        public void swingUpdate(Game game, ItemStack stack) {

        }

        public override ItemStack leftClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            swingUpdate(game, stack);
            return stack;
        }

        public override ItemStack rightClick(Game game, ItemStack stack, int xTile, int yTile, int distance) {
            swingUpdate(game, stack);
            return stack;
        }

        public override void drawOverPlayer(Game game, ItemStack currentItem, bool facingRight, Vector2 origin, float rotation, Vector2 position) {
            Rectangle rect = currentItem.getItem().textureInfo.rectangle;
            game.spriteBatch.Draw(ItemSheet, new Rectangle((int)position.X - rect.Width/2, (int)position.Y - rect.Height/2, rect.Width, rect.Height), rect, Color.White, rotation, origin, facingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
        }
    }
}
