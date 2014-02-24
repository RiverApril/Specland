using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    class InvalidNumberOfArgumentsException : Exception {
        public int numberOfArguments;

        public InvalidNumberOfArgumentsException(int numberOfArguments) {
            this.numberOfArguments = numberOfArguments;
        }
    }
}
