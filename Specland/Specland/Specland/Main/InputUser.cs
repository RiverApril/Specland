using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public interface InputUser {

        InputState update(Game game, InputState inputState);
    }
}
