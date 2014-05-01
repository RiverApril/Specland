using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    public abstract class Gui : InputUser {

        public static Texture2D guiTexture;

        public abstract void draw(Game game, GameTime gameTime);

        public abstract InputState update(Game game, InputState inputState);

    }
}
