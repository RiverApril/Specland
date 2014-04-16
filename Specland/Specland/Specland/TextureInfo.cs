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
        public Point offset = new Point(0, 0);
        public bool flipH = false;
        public bool flipV = false;

        public TextureInfo(/*Texture2D texture2D, */Rectangle rectangle, bool transparent) {
            //this.texture2D = texture2D;
            this.rectangle = rectangle;
            this.transparent = transparent;
        }

        public TextureInfo(Rectangle r, bool t, Point offset) : this(r, t){
            if (offset!=null) {
                this.offset = offset;
            }
        }

        public TextureInfo(Rectangle r, bool t, Point offset, bool h, bool v) : this(r, t) {
            flipH = h;
            flipV = v;
            this.offset = offset;
        }

    }
}
