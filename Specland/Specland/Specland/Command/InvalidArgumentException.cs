using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class InvalidArgumentException : Exception {
        public String invailaidArgument;
        public String correction;

        public InvalidArgumentException(String invailaidArgument, String correction) {
            this.invailaidArgument = invailaidArgument;
            this.correction = correction;
        }
    }
}
