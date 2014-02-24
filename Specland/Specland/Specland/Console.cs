using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Specland {
    public class Console : IStringPrinter{

        public List<String> lines = new List<string>();
        public List<String> previousInputs = new List<string>();
        public string input = "";
        public int inputPosition = 0;

        private int width = 300;
        private int historyHeight = 17*5;
        private int historyYFromBottom = 28;
        private int tick = 0;

        private bool consoleOpen = false;
        private Keys[] lastPressedKeys = new Keys[0];

        private float textFade = 0f;
        private int historyLookBackIndex = 0;

        public Console() {

        }

        public void println(string line){
            textFade = 2;
            lines.Add(line);
        }

        private void proccessInput(string input) {
            if(input.Length>0){
                if(input.StartsWith("/")){
                    string[] ss = input.Substring(1).Split(new char[] { ' ' }, 2);
                    string name = ss.Length > 0 ? ss[0] : "";
                    string options = ss.Length > 1 ? ss[1] : "";
                    string[] optionList = options.Split(new char[] { ' ' });
                    optionList = optionList[0].Equals("") && optionList.Length == 1 ? new String[0] : optionList;
                    try {
                        Command c = Command.getCommand(name);
                        try {
                            c.processCommand(this, optionList);
                        } catch (InvalidNumberOfArgumentsException e) {
                            println("Propper Usage: " + c.getUsage());
                        } catch (NoSuchItemException e) {
                            println("No such item: " + e.s);
                        } catch (NoSuchTileException e) {
                            println("No such tile: " + e.s);
                        } catch (NumberInvalidException e) {
                            println("Could not parse int: " + e.s);
                        } catch (FileAlreadyExistsException e) {
                            println("File already exists: " + e.s);
                        } catch (InvalidArgumentException e) {
                            println("invailaid Argument: \"" + e.invailaidArgument + "\"  should be "+e.correction);
                        }
                    } catch (NoSuchCommandException e) {
                        println("No such command: "+e.name);
                    }
                } else {
                    println(input);
                }
            }
        }

        public void update(Game game){
            updateKeys(game);
            if(consoleOpen){
                if (game.inputState.pressedIgnore(Keys.Up)) {
                    if (historyLookBackIndex>0) {
                        historyLookBackIndex--;
                    }
                }
                if (game.inputState.pressedIgnore(Keys.Down)) {
                    if (historyLookBackIndex < previousInputs.Count()-1) {
                        historyLookBackIndex++;
                    }
                }
                if (game.inputState.pressedIgnore(Keys.Up) || game.inputState.pressedIgnore(Keys.Down)) {
                    if (historyLookBackIndex > 0 && historyLookBackIndex < previousInputs.Count()) {
                        input = previousInputs[historyLookBackIndex];
                        inputPosition = input.Length;
                        consoleOpen = true;
                        game.inputState.keysOn = false;
                    }
                }
                if (game.inputState.pressedIgnore(Keys.Enter)) {
                    previousInputs.Add(input);
                    historyLookBackIndex = previousInputs.Count();
                    consoleOpen = false;
                    proccessInput(input);
                    game.inputState.keysOn = true;
                    input = "";
                    inputPosition = 0;
                }
            } else {
                if (game.inputState.pressed(Keys.T) || game.inputState.pressedIgnore(Keys.Enter)) {
                    input = "";
                    inputPosition = 0;
                    consoleOpen = true;
                    game.inputState.keysOn = false;
                }
                if (game.inputState.pressed(Keys.OemQuestion)) {
                    input = "/";
                    inputPosition = 1;
                    consoleOpen = true;
                    game.inputState.keysOn = false;
                }
                textFade -= .01f;
            }
        }

        public void draw(Game game) {

            Color color = new Color(textFade, textFade, textFade, textFade);

            int a = 0;

            if (consoleOpen) {

                color = Color.White;
                //a = 0;

                //game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(4, game.Window.ClientBounds.Height - 24, 4, 20), new Rectangle(0, 0, 4, 20), Color.White);
                //game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(8, game.Window.ClientBounds.Height - 24, width, 20), new Rectangle(4, 0, 12, 20), Color.White);
                //game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(8 + width, game.Window.ClientBounds.Height - 24, 4, 20), new Rectangle(16, 0, 4, 20), Color.White);
                string text = (inputPosition == input.Length ? (input+(tick%60<30?"_":"")) : input);
                game.spriteBatch.DrawString(Game.fontNormal, text, new Vector2(12, game.Window.ClientBounds.Height - 22), Color.White);
                if(inputPosition < 0){
                    inputPosition = 0;
                }
                if (inputPosition > input.Length) {
                    inputPosition = input.Length;
                }
                if (inputPosition != input.Length && tick % 60 < 30) {
                    game.spriteBatch.DrawString(Game.fontNormal, "|", new Vector2(8 + Game.fontNormal.MeasureString(input.Substring(0, inputPosition)).X, game.Window.ClientBounds.Height - 22), Color.White);
                }
            }

            /*
            game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(4, game.Window.ClientBounds.Height - historyHeight - historyYFromBottom + a, 4, 4), new Rectangle(0, 0, 4, 4), color);
            game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(4, game.Window.ClientBounds.Height - historyHeight - (historyYFromBottom - 4) + a, 4, historyHeight - 8), new Rectangle(0, 4, 4, 12), color);
            game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(4, game.Window.ClientBounds.Height - (historyYFromBottom + 4) + a, 4, 4), new Rectangle(0, 16, 4, 4), color);

            game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(8, game.Window.ClientBounds.Height - historyHeight - historyYFromBottom + a, width, 4), new Rectangle(4, 0, 12, 4), color);
            game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(8, game.Window.ClientBounds.Height - historyHeight - (historyYFromBottom - 4) + a, width, historyHeight - 8), new Rectangle(4, 4, 12, 12), color);
            game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(8, game.Window.ClientBounds.Height - (historyYFromBottom + 4) + a, width, 4), new Rectangle(4, 16, 12, 4), color);

            game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(8 + width, game.Window.ClientBounds.Height - historyHeight - historyYFromBottom + a, 4, 4), new Rectangle(16, 0, 4, 4), color);
            game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(8 + width, game.Window.ClientBounds.Height - historyHeight - (historyYFromBottom - 4) + a, 4, historyHeight - 8), new Rectangle(16, 4, 4, 12), color);
            game.spriteBatch.Draw(Gui.guiTexture, new Rectangle(8 + width, game.Window.ClientBounds.Height - (historyYFromBottom + 4) + a, 4, 4), new Rectangle(16, 16, 4, 4), color);

            */
            for (int i = 0; i < Math.Min(lines.Count(), 10);i++ ) {
                string text = lines[lines.Count() - (i+1)];
                game.spriteBatch.DrawString(Game.fontNormal, text, new Vector2(12, game.Window.ClientBounds.Height - historyYFromBottom - a - 18 - (i * 16)), color);
            }

            tick++;
        }



        public void updateKeys(Game game) {
            Keys[] pressedKeys = game.inputState.getKeyboardStateIgnore().GetPressedKeys();

            foreach (Keys key in lastPressedKeys) {
                if (!pressedKeys.Contains(key))
                    OnKeyUp(game, key);
            }

            foreach (Keys key in pressedKeys) {
                if (!lastPressedKeys.Contains(key))
                    OnKeyDown(game, key);
            }

            lastPressedKeys = pressedKeys;
        }

        private void OnKeyDown(Game game, Keys key) {
            if (consoleOpen) {
                String s = keyToString(game, key);
                if(s.Length!=0){
                    if (inputPosition == input.Length) {
                        input += s;
                    } else {
                        input = input.Substring(0, inputPosition) + s + input.Substring(inputPosition);
                    }
                    inputPosition++;
                }
            }
        }

        private void OnKeyUp(Game game, Keys key) {

        }

        private string keyToString(Game game, Keys key) {
            bool shift = false;
            if(game.inputState.getKeyboardStateIgnore().IsKeyDown(Keys.LeftShift) || game.inputState.getKeyboardStateIgnore().IsKeyDown(Keys.RightShift)){
                shift = !shift;
            }
            switch (key) {
                case Keys.Back: { if (inputPosition > 0) { input = input.Substring(0, inputPosition - 1) + input.Substring(inputPosition); inputPosition--; } return ""; }
                case Keys.A: return shift ? "A" : "a";
                case Keys.B: return shift ? "B" : "b";
                case Keys.C: return shift ? "C" : "c";
                case Keys.D: return shift ? "D" : "d";
                case Keys.E: return shift ? "E" : "e";
                case Keys.F: return shift ? "F" : "f";
                case Keys.G: return shift ? "G" : "g";
                case Keys.H: return shift ? "H" : "h";
                case Keys.I: return shift ? "I" : "i";
                case Keys.J: return shift ? "J" : "j";
                case Keys.K: return shift ? "K" : "k";
                case Keys.L: return shift ? "L" : "l";
                case Keys.M: return shift ? "M" : "m";
                case Keys.N: return shift ? "N" : "n";
                case Keys.O: return shift ? "O" : "o";
                case Keys.P: return shift ? "P" : "p";
                case Keys.Q: return shift ? "Q" : "q";
                case Keys.R: return shift ? "R" : "r";
                case Keys.S: return shift ? "S" : "s";
                case Keys.T: return shift ? "T" : "t";
                case Keys.U: return shift ? "U" : "u";
                case Keys.V: return shift ? "V" : "v";
                case Keys.W: return shift ? "W" : "w";
                case Keys.X: return shift ? "X" : "x";
                case Keys.Y: return shift ? "Y" : "y";
                case Keys.Z: return shift ? "Z" : "z";
                case Keys.D1: return shift ? "!" : "1";
                case Keys.D2: return shift ? "@" : "2";
                case Keys.D3: return shift ? "#" : "3";
                case Keys.D4: return shift ? "$" : "4";
                case Keys.D5: return shift ? "%" : "5";
                case Keys.D6: return shift ? "^" : "6";
                case Keys.D7: return shift ? "&" : "7";
                case Keys.D8: return shift ? "*" : "8";
                case Keys.D9: return shift ? "(" : "9";
                case Keys.D0: return shift ? ")" : "0";
                case Keys.OemMinus: return shift ? "_" : "-";
                case Keys.OemPlus: return shift ? "+" : "=";
                case Keys.OemQuestion: return shift ? "?" : "/";
                case Keys.OemComma: return shift ? "<" : ",";
                case Keys.OemPeriod: return shift ? ">" : ".";
                case Keys.OemSemicolon: return shift ? ":" : ";";
                case Keys.OemTilde: return shift ? "\"" : "\'";
                case Keys.OemQuotes: return shift ? "|" : "\\";
                case Keys.OemOpenBrackets: return shift ? "{" : "[";
                case Keys.OemCloseBrackets: return shift ? "}" : "]";
                case Keys.Tab: return "    ";
                case Keys.Space: return " ";
                case Keys.Left: { inputPosition--; return ""; }
                case Keys.Right: { inputPosition++; return ""; }
            }
            return "";
        }
    }
}
