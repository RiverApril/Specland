using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland.Entities {
    public class MovementHandeler : InputUser{
        public bool left;
        public bool right;
        public bool down;
        public bool jump;

        public InputState update(Game game, InputState inputState) {
            left = inputState.down(Game.KEY_MOVE_LEFT); inputState.eatKey(Game.KEY_MOVE_LEFT);
            right = inputState.down(Game.KEY_MOVE_RIGHT); inputState.eatKey(Game.KEY_MOVE_RIGHT);
            down = inputState.down(Game.KEY_MOVE_DOWN); inputState.eatKey(Game.KEY_MOVE_DOWN);
            jump = inputState.down(Game.KEY_MOVE_JUMP); inputState.eatKey(Game.KEY_MOVE_JUMP);
            return inputState;
        }
    }
}
