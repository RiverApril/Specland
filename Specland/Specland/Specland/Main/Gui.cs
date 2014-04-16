using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    public abstract class Gui {

        public static Texture2D guiTexture;

        public abstract void draw(Game game, GameTime gameTime);

    }
}
