using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Specland {
    public class InputState {

        private KeyboardState keyboardState;
        private KeyboardState lastKeyboardState;

        private MouseState mouseState;
        private MouseState lastMouseState;

        private bool keysOn = true;
        private bool mouseButtonsOn = true;

        private bool[] eatenKeys = new bool[1024];

        private KeyboardState emptyKeyboardState = new KeyboardState();

        public static int Left = 0;
        public static int Right = 1;

        public void set(KeyboardState k, MouseState m) {
            keyboardState = k;
            mouseState = m;
        }

        public void setLast(KeyboardState k, MouseState m) {
            lastKeyboardState = k;
            lastMouseState = m;
        }


        public bool pressed(Keys key) {
            return eatenKeys[(int)key] ? false : keysOn ? (keyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key)) : false;
        }

        public bool released(Keys key) {
            return eatenKeys[(int)key] ? false : keysOn ? (keyboardState.IsKeyUp(key) && !lastKeyboardState.IsKeyUp(key)) : false;
        }

        public bool down(Keys key) {
            return eatenKeys[(int)key] ? false : keysOn ? keyboardState.IsKeyDown(key) : false;
        }


        public bool pressedMouse(int button) {
            return mouseButtonsOn ? (button==Left?mouseState.LeftButton:mouseState.RightButton)==(ButtonState.Pressed) && ((button==Left?lastMouseState.LeftButton:lastMouseState.RightButton)==(ButtonState.Released)) : false;
        }

        public bool releasedMouse(int button) {
            return mouseButtonsOn ? (button==Left?mouseState.LeftButton:mouseState.RightButton)==(ButtonState.Released) && ((button==Left?lastMouseState.LeftButton:lastMouseState.RightButton)==(ButtonState.Pressed)) : false;
        }

        public bool downMouse(int button) {
            return mouseButtonsOn ? (button==Left?mouseState.LeftButton:mouseState.RightButton)==(ButtonState.Pressed) : false;
        }


        internal void eatKeyboard() {
            keysOn = false;
        }

        internal void eatMouse() {
            mouseButtonsOn = false;
        }

        internal void regergitateKeyboardAndMouse() {
            keysOn = true;
            mouseButtonsOn = true;
            Array.Clear(eatenKeys, 0, eatenKeys.Length);
        }

        internal KeyboardState getKeyboardState() {
            return keysOn?keyboardState:emptyKeyboardState;
        }

        internal int mouseX() {
            return mouseState.X;
        }

        internal int mouseY() {
            return mouseState.Y;
        }

        internal int getScrollWheelValue() {
            return mouseState.ScrollWheelValue;
        }

        internal void eatKey(Keys key) {
            eatenKeys[(int)key] = true;
        }
    }
}
