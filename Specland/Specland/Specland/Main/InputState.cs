using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Specland {
    public class InputState {

        private KeyboardState keyboardState;
        private KeyboardState lastKeyboardState;

        public MouseState mouseState;
        public MouseState lastMouseState;

        public bool keysOn = true;

        private KeyboardState emptyKeyboardState = new KeyboardState();
        private KeyboardState emptyLastKeyboardState = new KeyboardState();

        public void set(KeyboardState k, MouseState m) {
            keyboardState = k;
            mouseState = m;
        }

        public void setLast(KeyboardState k, MouseState m) {
            lastKeyboardState = k;
            lastMouseState = m;
        }

        public bool pressed(Keys key) {
            return keysOn ? (keyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key)) : false;
        }

        public bool released(Keys key) {
            return keysOn ? (keyboardState.IsKeyUp(key) && !lastKeyboardState.IsKeyUp(key)) : false;
        }

        public KeyboardState getKeyboardState() {
            return keysOn ? keyboardState : emptyKeyboardState;
        }

        public KeyboardState getLastKeyboardState() {
            return keysOn ? lastKeyboardState : emptyLastKeyboardState;
        }

        public bool pressedIgnore(Keys key) {
            return keyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key);
        }

        public KeyboardState getKeyboardStateIgnore() {
            return keyboardState;
        }
    }
}
