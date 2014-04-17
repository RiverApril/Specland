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

        public GraphicsDeviceManager graphicsDeviceManager;
        public SpriteBatch spriteBatch;

        public FpsControl updateFps = new FpsControl();
        public FpsControl drawFps = new FpsControl();

        public Console console = new Console();

        public World currentWorld = null;

        public Inventory inventory = new Inventory();

        public InputState inputState = new InputState();

        public static string drawMessage = "";
        public static string updateMessage = "";
        
        public static Random rand = new Random();

        public static Game instance;

        public static SpriteFont fontNormal;
        public static SpriteFont fontSmall;

        public static Texture2D dummyTexture;

        public static Rectangle OnexOneRect = new Rectangle(0, 0, 1, 1);
        private Thread lightingThread;

        public bool debugEnabled = false;

        public static Color cursorColor = Color.LightGray;


        public static float volume = .05f;

        public static string saveLocation;
        public static string saveExtention = "slw";


        public Game() {
            instance = this;
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";

            saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Specland";

            System.IO.Directory.CreateDirectory(saveLocation);

            lightingThread = new Thread(new ThreadStart(lightingThreadUpdate));
            lightingThread.Start();
        }

        private void lightingThreadUpdate() {
            while(true){
                if (currentWorld != null) {
                    currentWorld.lightingThreadUpdate();
                }
            }
        }

        protected override void Initialize() {
            currentWorld = new World(World.defaultSize, "World");
            currentWorld.generate(WorldGenerator.TypeNatural);
            currentWorld.player = new EntityPlayer((currentWorld.sizeInTiles.X*World.tileSizeInPixels)/2, 10);
            currentWorld.EntityList.Add(currentWorld.player);
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fontNormal = Content.Load<SpriteFont>("Fonts\\fontNormal");
            fontSmall = Content.Load<SpriteFont>("Fonts\\fontSmall");

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
        }

        protected override void Update(GameTime gameTime) {
            updateMessage = "";
            inputState.set(Keyboard.GetState(), Mouse.GetState());

            if (inputState.pressed(Keys.Escape)) {
                this.Exit();
            }
            if(inputState.pressed(Keys.F3)){
                debugEnabled = !debugEnabled;
            }

            SoundEffectPlayer.update();
            
            if (currentWorld != null) {
                currentWorld.Update(this);
                if (inputState.getKeyboardState().IsKeyDown(Keys.F4)) {
                    currentWorld.time += 10;
                }
            }

            if (inventory != null) {
                inventory.update(this, gameTime);
            }

            console.update(this);

            EntityItem.playedSoundEffectRecently = false;

            inputState.setLast(Keyboard.GetState(), Mouse.GetState());
            base.Update(gameTime);
            updateFps.update();
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(currentWorld==null?Color.CornflowerBlue:currentWorld.getBgColor());
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

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
                string p = "Rendering:\n  Gui: " + Profiler.get("draw gui") + "\n  Tile: " + Profiler.get("draw tiles") + "\n  Entity: " + Profiler.get("draw entities");
                p += "\n\nLighting: " + Profiler.get("lighting");
                string ti = "T: " + currentWorld.getTileIndex(inventory.mouseTileX, inventory.mouseTileY, false) + "`" + currentWorld.getTileData(inventory.mouseTileX, inventory.mouseTileY, false);
                ti += "  W: " + currentWorld.getTileIndex(inventory.mouseTileX, inventory.mouseTileY, true) + "`" + currentWorld.getTileData(inventory.mouseTileX, inventory.mouseTileY, true);

                spriteBatch.DrawString(fontNormal, d + "\n" + u + "\n" + l + "\n" + e + "\n" + t + "\n" + p + "\n" + ti + "\n" + drawMessage + "\n" + updateMessage, new Vector2(140, 10), Color.White);
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
    }
}
