using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class NoSuchCommandException : Exception {
        public string name;

        public NoSuchCommandException(string name) {
            this.name = name;
        }

    }
}
