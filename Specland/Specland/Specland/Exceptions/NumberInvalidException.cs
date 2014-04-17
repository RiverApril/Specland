using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class NumberInvalidException : Exception {
        public string s;

        public NumberInvalidException(string s) {
            this.s = s;
        }

    }
}
