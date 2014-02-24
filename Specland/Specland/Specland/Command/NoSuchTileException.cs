using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class NoSuchTileException : Exception {
        public string s;

        public NoSuchTileException(string s) {
            this.s = s;
        }
    }
}
