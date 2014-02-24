using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    public class TextureInfo {
        //public Texture2D texture2D;
        public Rectangle rectangle;
        public bool transparent;

        public TextureInfo(/*Texture2D texture2D, */Rectangle rectangle, bool transparent) {
            //this.texture2D = texture2D;
            this.rectangle = rectangle;
            this.transparent = transparent;
        }

    }
}
