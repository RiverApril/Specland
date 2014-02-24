using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class NoSuchItemException : Exception {
        public string s;

        public NoSuchItemException(string s) {
            this.s = s;
        }
    }
}
