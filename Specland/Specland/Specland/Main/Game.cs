using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Specland {

    public class Game : Microsoft.Xna.Framework.Game {
        

        private static float nextRenderDepthFloat = 1;

        public static float nextRenderDepth(){
            nextRenderDepthFloat -= .01f;
            return nextRenderDepthFloat;
        }


        public static float RENDER_DEPTH_BG = nextRenderDepth();

        public static float RENDER_DEPTH_WALL = nextRenderDepth();
        public static float RENDER_DEPTH_LIQUID = nextRenderDepth();
        public static float RENDER_DEPTH_TILE = nextRenderDepth();
        public static float RENDER_DEPTH_TILE_DOOR = nextRenderDepth();
        public static float RENDER_DEPTH_TILE_CRACK_DEBUG_TEXT = nextRenderDepth();

        public static float RENDER_DEPTH_ENTITY = nextRenderDepth();
        public static float RENDER_DEPTH_OVER_ENTITY = nextRenderDepth();
        public static float RENDER_DEPTH_PLAYER = nextRenderDepth();
        public static float RENDER_DEPTH_ENTITY_FALLING_SAND = nextRenderDepth();
        public static float RENDER_DEPTH_OVER_PLAYER = nextRenderDepth();

        public static float RENDER_DEPTH_HOVER = nextRenderDepth();

        public static float RENDER_DEPTH_GUI_IMAGE_BG = nextRenderDepth();
        public static float RENDER_DEPTH_GUI_IMAGE_FG = nextRenderDepth();
        public static float RENDER_DEPTH_GUI_TEXT = nextRenderDepth();

        public static float RENDER_DEPTH_CONSOLE = nextRenderDepth();

        public static float RENDER_DEPTH_GUI_CURSOR_IMAGE_BG = nextRenderDepth();
        public static float RENDER_DEPTH_GUI_CURSOR_IMAGE_FG = nextRenderDepth();
        public static float RENDER_DEPTH_GUI_CURSOR_TEXT = nextRenderDepth();


        public static Keys KEY_INV_MOVE_ITEM_TO_OTHER = Keys.LeftShift;

        public static Keys KEY_DROP = Keys.R;

        public static Keys KEY_INV = Keys.Q;
        public static Keys KEY_D1 = Keys.D1;
        public static Keys KEY_D2 = Keys.D2;
        public static Keys KEY_D3 = Keys.D3;
        public static Keys KEY_D4 = Keys.D4;
        public static Keys KEY_D5 = Keys.D5;
        public static Keys KEY_D6 = Keys.D6;
        public static Keys KEY_D7 = Keys.D7;
        public static Keys KEY_D8 = Keys.D8;
        public static Keys KEY_D9 = Keys.D9;
        public static Keys KEY_D0 = Keys.D0;

        public static Keys KEY_ESCAPE = Keys.Escape;
        public static Keys KEY_DEBUG_MENU = Keys.F3;
        public static Keys KEY_FAST_FORWARD_CLOCK = Keys.F4;

        public static Keys KEY_MOVE_LEFT = Keys.A;
        public static Keys KEY_MOVE_RIGHT = Keys.D;
        public static Keys KEY_MOVE_DOWN = Keys.S;
        public static Keys KEY_MOVE_JUMP = Keys.Space;
        public static Keys KEY_USE = Keys.E;

        public static Keys KEY_CONSOLE_OPEN_COMMAND = Keys.OemQuestion;
        public static Keys KEY_CONSOLE_OPEN = Keys.T;
        public static Keys KEY_CONSOLE_ENTER = Keys.Enter;
        public static Keys KEY_CONSOLE_UP = Keys.Up;
        public static Keys KEY_CONSOLE_DOWN = Keys.Down;

        private List<InputUser> userInputList = new List<InputUser>();

        private BlendState normalBlendState = new BlendState();

        public GraphicsDeviceManager graphicsDeviceManager;
        public SpriteBatch spriteBatch;

        public FpsControl updateFps = new FpsControl();
        public FpsControl drawFps = new FpsControl();

        public Console console = new Console();

        public World currentWorld = null;

        public Inventory inventory = new Inventory();

        private InputState inputState = new InputState();

        public static string drawMessage = "";
        public static string updateMessage = "";
        
        public static Random rand = new Random();

        public static Game instance;

        public static SpriteFont fontNormal;

        public static Texture2D dummyTexture;

        public static Rectangle OnexOneRect = new Rectangle(0, 0, 1, 1);

        private Thread lightingThread;
        private Thread liquidThread;

        public static bool debugEnabled = false;

        public static Color cursorColor = Color.LightGray;


        public static float volume = .05f;

        public static string saveLocation;
        public static string saveExtention = "slw";


        public static long tick = 0;

        public Game() {
            instance = this;
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";

            saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Specland";

            System.IO.Directory.CreateDirectory(saveLocation);

            lightingThread = new Thread(new ThreadStart(lightingThreadUpdate));
            lightingThread.Start();

            liquidThread = new Thread(new ThreadStart(liquidThreadUpdate));
            liquidThread.Start();

            normalBlendState = BlendState.AlphaBlend;
        }

        private void lightingThreadUpdate() {
            while(true){
                if (currentWorld != null) {
                    currentWorld.lightingThreadUpdate();
                }
            }
        }

        private void liquidThreadUpdate() {
            while (true) {
                if (currentWorld != null) {
                    currentWorld.liquidThreadUpdate();
                }
            }
        }

        protected override void Initialize() {
            currentWorld = new World(World.defaultSize, "World");
            currentWorld.generate(WorldGenerator.TypeNatural);
            currentWorld.player = new EntityPlayer((currentWorld.sizeInTiles.X*World.tileSizeInPixels)/2, 10);
            currentWorld.EntityList.Add(currentWorld.player);

            resetUserInputList();

            base.Initialize();
        }

        public void resetUserInputList() {
            userInputList.Clear();
            userInputList.Add(console);
            userInputList.Add(inventory);
            if(currentWorld!=null){
                if (currentWorld.player!=null) {
                    userInputList.Add(currentWorld.player.movement);
                }
            }
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fontNormal = Content.Load<SpriteFont>("Fonts\\fontNormal");

            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });

            loadTextures();
            loadSounds();

            currentWorld.calculateTileFrames(this);
        }

        private void loadTextures() {
            Tile.TileSheet = Content.Load<Texture2D>("Images\\TileSheet");
            Item.ItemSheet = Content.Load<Texture2D>("Images\\ItemSheet");
            Gui.guiTexture = Content.Load<Texture2D>("Images\\Gui");
            Entity.loadContent(Content);
        }

        private void loadSounds() {
            SoundEffectPlayer.loadSounds(Content);
        }

        protected override void UnloadContent() {
            lightingThread.Abort();
            liquidThread.Abort();
        }

        protected override void Update(GameTime gameTime) {
            tick++;
            updateMessage = "";
            inputState.set(Keyboard.GetState(), Mouse.GetState());

            foreach(InputUser i in userInputList){
                inputState = i.update(this, inputState);
            }
            inputState.regergitateKeyboardAndMouse();

            if (inputState.pressed(Game.KEY_ESCAPE)) {
                this.Exit();
            }
            if(inputState.pressed(Game.KEY_DEBUG_MENU)){
                debugEnabled = !debugEnabled;
            }

            SoundEffectPlayer.update();
            
            if (currentWorld != null) {
                currentWorld.Update(this, tick);
                if (inputState.getKeyboardState().IsKeyDown(Game.KEY_FAST_FORWARD_CLOCK)) {
                    currentWorld.time += 10;
                }
            }

            EntityItem.playedSoundEffectRecently = false;

            inputState.setLast(Keyboard.GetState(), Mouse.GetState());
            base.Update(gameTime);
            updateFps.update();
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(currentWorld==null?Color.CornflowerBlue:currentWorld.getBgColor());
            spriteBatch.Begin(SpriteSortMode.BackToFront, normalBlendState, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
            
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            string e = "";
            string t = "";

            if (currentWorld != null) {
                currentWorld.Draw(this, gameTime);
                e = "Entity Count: " + currentWorld.EntityList.Count;

                int hr = (int)(currentWorld.time / 1000);

                t = "Time: " + (hr>12?hr-12:(hr==0?12:hr)) + ":" + formatToHave2Digits(((int)(((currentWorld.time % 1000.0) / 1000.0) * 60.0))) + " " + (hr >= 12 ? "PM" : "AM") + " (" + (int)currentWorld.time + ")";
            }

            Profiler.start("draw gui");
            if(inventory!=null){
                inventory.draw(this, gameTime);
            }
            Profiler.end("draw gui");

            if(debugEnabled){
                string d = "Draw Fps: " + drawFps.getFps() + " (ms/t:" + drawFps.getMpt() + ") " + percent(drawFps.getFps(), 60) + "%";
                string u = "Update Fps: " + updateFps.getFps() + " (ms/t:" + updateFps.getMpt() + ") " + percent(updateFps.getFps(), 60) + "%";
                string l = "Lighting Fps: " + currentWorld.lightingThreadFps.getFps() + " (ms/t:" + currentWorld.lightingThreadFps.getMpt() + ") ";
                string lq = "Liquid Fps:   " + currentWorld.liquidThreadFps.getFps() + " (ms/t:" + currentWorld.liquidThreadFps.getMpt() + ") ";
                string p = "Rendering:\n  Gui: " + Profiler.get("draw gui") + "\n  Tile: " + Profiler.get("draw tiles") + "\n  Entity: " + Profiler.get("draw entities");
                p += "\n\nLighting: " + Profiler.get("lighting");
                string ti = "T: " + currentWorld.getTileIndex(inventory.mouseTileX, inventory.mouseTileY, false) + "`" + currentWorld.getTileData(inventory.mouseTileX, inventory.mouseTileY, false);
                ti += "  W: " + currentWorld.getTileIndex(inventory.mouseTileX, inventory.mouseTileY, true) + "`" + currentWorld.getTileData(inventory.mouseTileX, inventory.mouseTileY, true);
                string pp = "Position: " + currentWorld.player.position.X + ", " + currentWorld.player.position.Y;

                spriteBatch.DrawString(fontNormal, d + "\n" + u + "\n" + l + "\n" + lq + "\n" + e + "\n" + t + "\n" + p + "\n" + ti + "\n" + pp + "\n" + drawMessage + "\n" + updateMessage, new Vector2(140, 10), Color.White);
            }
            drawMessage = "";

            console.draw(this, gameTime);

            spriteBatch.End();
            base.Draw(gameTime);
            drawFps.update();
        }

        private string formatToHave2Digits(int p) {
            return (p<10?"0":"")+p;
        }

        private int percent(int value, int total) {
            return (int)(((100.0 / total) * value));
        }

        public static void drawRectangle(Texture2D guiTexture, Rectangle dst, Rectangle src, Color color, float depth) {
            instance.spriteBatch.Draw(guiTexture, dst, src, color, 0, Vector2.Zero, SpriteEffects.None, depth);
        }

        public static void drawString(string text, Vector2 position, Color color, float depth) {
            instance.spriteBatch.DrawString(fontNormal, text, position, color, 0, Vector2.Zero, 1, SpriteEffects.None, depth);
        }

        public static void drawString(SpriteFont font, string text, Vector2 position, Color color, float depth) {
            instance.spriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero, 1, SpriteEffects.None, depth);
        }
    }
}
