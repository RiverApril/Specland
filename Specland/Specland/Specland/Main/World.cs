using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Specland {
    public class World {

        public enum TileDepth {
            tile=0, wall=1
        }


        public string name = "";

        public Rectangle viewPortInTiles = new Rectangle();
        public Rectangle viewPortInPixels = new Rectangle();
        public Point viewOffset = new Point(0, 0);

        public static int tileSizeInPixels = 16;
        public static int tileScale = 2;

        public static int nearSurfaceY = 40;
        public static int undergroundY = 40;
        public static int undergroundWaterY = 40;

        public static int previousUniqueID = 0;

        public Point sizeInTiles;

        public int[] heightMap;

        public int[, ,] TileMatrix;
        public int[, ,] TileDataMatrix;

        public int[,] LiquidMatrix;

        public bool[,] LiquidNeedsUpdateMatrix;

        public bool[, ,] TileNeedsUpdateMatrix;

        public int[,] LightMatrix;
        public int[,] TempLightMatrix;

        public byte[,] CrackMatrix;

        public byte[,] wireMatrix;

        public int[] skyY;



        //public Rectangle[,] drawRectangles;
        public TextureInfo[, ,] textureTileInfo;
        public TextureInfo[,] textureLiquidInfo;

        public int skyLightBrightness = 0;

        public List<Entity> EntityList = new List<Entity>();
        public List<Entity> EntityRemovalList = new List<Entity>();
        public List<Entity> EntityAddingList = new List<Entity>();

        public double time = 7000;

        public EntityPlayer player;

        public bool lightingNeedsUpdate = false;

        public FpsControl lightingThreadFps;
        public FpsControl liquidThreadFps;

        Rectangle dr = new Rectangle(0, 0, tileSizeInPixels, tileSizeInPixels);

        public static Color[] grayColors = new Color[256];

        public static Point defaultSize = new Point(1000, 1000);

        private Color currentBgColor;

        public World(Point size, string name) {
            this.sizeInTiles = size;
            TileMatrix = new int[size.X, size.Y, 2];
            TileDataMatrix = new int[size.X, size.Y, 2];
            LiquidMatrix = new int[size.X, size.Y];
            //drawRectangles = new Rectangle[size.X, size.Y];
            LightMatrix = new int[size.X, size.Y];
            TempLightMatrix = new int[size.X, size.Y];
            CrackMatrix = new byte[size.X, size.Y];
            TileNeedsUpdateMatrix = new bool[size.X, size.Y, 2];
            LiquidNeedsUpdateMatrix = new bool[size.X, size.Y];
            wireMatrix = new byte[size.X, size.Y];
            skyY = new int[size.X];
            heightMap = new int[size.X];

            lightingThreadFps = new FpsControl();
            liquidThreadFps = new FpsControl();

            for(int i=0;i<256;i++){
                grayColors[i] = new Color((byte)i, (byte)i, (byte)i);
            }
        }

        public bool calculateTileFrames(Game game) {

            textureTileInfo = new TextureInfo[sizeInTiles.X, sizeInTiles.Y, 2];
            textureLiquidInfo = new TextureInfo[sizeInTiles.X, sizeInTiles.Y];

            for (int x = 0; x < sizeInTiles.X; x++) {
                setSkyTile(x);
                for (int y = 0; y < sizeInTiles.Y; y++) {
                    calculateTileFrame(game, x, y, World.TileDepth.tile);
                    calculateTileFrame(game, x, y, World.TileDepth.wall);
                    calculateLiquidFrame(game, x, y);
                    lightUpdate1(x, y);
                }
            }
            for (int x = 0; x < sizeInTiles.X; x++) {
                lightUpdate2(x, 0, true);
                for (int y = 1; y < sizeInTiles.Y; y++) {
                    lightUpdate2(x, y, false);
                }
            }

            LightMatrix = (int[,])TempLightMatrix.Clone();

            return true;

        }

        public void calculateTileFrame(Game game, int x, int y, World.TileDepth tileDepth) {
            if ((textureTileInfo) != null) {
                Tile tile = getTileObject(x, y, tileDepth);
                if (tile != null && inWorld(x, y)) {
                    textureTileInfo[x, y, (int)tileDepth] = tile.getTextureInfo(x, y, this, tileDepth);
                }
            }
        }

        private void calculateLiquidFrame(Game game, int x, int y) {
            if (textureLiquidInfo != null) {
                if (inWorld(x, y)) {
                    if (getLiquid(x, y - 1) > 0) {
                        textureLiquidInfo[x, y] = getLiquidTextureInfoVertical(x, y);
                    } else {
                        textureLiquidInfo[x, y] = getLiquidTextureInfoHorizontal(x, y);
                    }
                }
            }
        }

        public int getLight(int x, int y) {
            if(inWorld(x, y)){
                return LightMatrix[x, y];
            }
            return 0;
        }

        private TextureInfo getLiquidTextureInfoVertical(int x, int y) {
            int a = (int)(MathHelper.Clamp((float)Math.Ceiling(LiquidMatrix[x, y] * (1.0f / 8.0f)), 0, 8));
            return new TextureInfo(/*Tile.TextureLiquidWater,*/ new Rectangle((32) + ((a % 3) * 8), (64) + ((a / 3) * 8), 8, 8), true);
        }

        private TextureInfo getLiquidTextureInfoHorizontal(int x, int y) {
            int a = (int)(MathHelper.Clamp((float)Math.Ceiling(LiquidMatrix[x, y] * (1.0f / 8.0f)), 0, 8));
            return new TextureInfo(/*Tile.TextureLiquidWater,*/ new Rectangle(((a % 3) * 8), (64) + ((a / 3) * 8), 8, 8), true);
        }

        public Tile getTileObject(int x, int y, World.TileDepth tileDepth) {
            Tile t = Tile.getTileObject(getTileIndex(x, y, tileDepth));
            return (t!=null?t:Tile.TileAir);
        }

        public Tile getTileObjectNoCheck(int x, int y, World.TileDepth tileDepth) {
            Tile t = Tile.getTileObject(getTileIndexNoCheck(x, y, tileDepth));
            return (t != null ? t : Tile.TileAir);
        }

        public int getTileData(int x, int y, World.TileDepth tileDepth) {
            if (inWorld(x, y)) {
                return TileDataMatrix[x, y, (int)tileDepth];
            }
            return 0;
        }

        public int getTileDataNoCheck(int x, int y, World.TileDepth tileDepth) {
            return TileDataMatrix[x, y, (int)tileDepth];
        }

        public int getTileIndex(int x, int y, World.TileDepth tileDepth) {
            if(inWorld(x, y)){
                return TileMatrix[x, y, (int)tileDepth];
            }
            return Tile.TileDirt.index;
        }

        public int getTileIndexNoCheck(int x, int y, World.TileDepth tileDepth) {
            return TileMatrix[x, y, (int)tileDepth];
        }

        internal void setTileForUpdate(int x, int y, World.TileDepth tileDepth) {
            if (inWorld(x, y)) {
                TileNeedsUpdateMatrix[x, y, (int)tileDepth] = true;
            }
        }

        public int getLiquid(int x, int y) {
            if (inWorld(x, y)) {
                return LiquidMatrix[x, y];
            }
            return 0;
        }

        public int getLiquidNoCheck(int x, int y) {
            return LiquidMatrix[x, y];
        }

        public bool setLiquid(int x, int y, int l) {
            if (inWorld(x, y)) {
                LiquidMatrix[x, y] = l;
                calculateLiquidFrame(Game.instance, x, y);
                calculateLiquidFrame(Game.instance, x + 1, y);
                calculateLiquidFrame(Game.instance, x - 1, y);
                calculateLiquidFrame(Game.instance, x, y + 1);
                calculateLiquidFrame(Game.instance, x, y - 1);
                lightingNeedsUpdate = true;
                return true;
            }
            return false;
        }

        public bool mineTile(int x, int y, ItemPick pick, World.TileDepth tileDepth) {
            if (!getTileObject(x, y, tileDepth).canBeDestroyed(this, x, y, tileDepth)) {
                return false;
            }
            if (mineTileNoNearUpdate(x, y, pick, tileDepth)) {
                setTileForUpdate(x - 1, y, tileDepth);
                setTileForUpdate(x + 1, y, tileDepth);
                setTileForUpdate(x, y - 1, tileDepth);
                setTileForUpdate(x, y + 1, tileDepth);
                return true;
            }
            return false;
        }

        public bool setTileWithDataWithUpdate(int x, int y, int index, int data, World.TileDepth tileDepth) {
            if (setTileWithData(x, y, index, data, tileDepth)) {
                TileNeedsUpdateMatrix[x, y, (int)tileDepth] = true;
                setTileForUpdate(x - 1, y, tileDepth);
                setTileForUpdate(x + 1, y, tileDepth);
                setTileForUpdate(x, y - 1, tileDepth);
                setTileForUpdate(x, y + 1, tileDepth);
                return true;
            }
            return false;
        }

        public bool setTileWithUpdate(int x, int y, int index, World.TileDepth tileDepth) {
            if (setTile(x, y, index, tileDepth)) {
                TileNeedsUpdateMatrix[x, y, (int)tileDepth] = true;
                setTileForUpdate(x - 1, y, tileDepth);
                setTileForUpdate(x + 1, y, tileDepth);
                setTileForUpdate(x, y - 1, tileDepth);
                setTileForUpdate(x, y + 1, tileDepth);
                return true;
            }
            return false;
        }


        public bool mineTileNoNearUpdate(int x, int y, ItemPick pick, World.TileDepth tileDepth) {
            Tile tile = getTileObject(x, y, tileDepth);
            int data = getTileDataNoCheck(x, y, tileDepth);
            if (setTile(x, y, Tile.TileAir.index, tileDepth)) {
                Entity e = new EntityItem(new Vector2((x) * World.tileSizeInPixels, (y) * World.tileSizeInPixels), tile.dropStack(this, pick, Game.rand, x, y, tileDepth));
                tile.mine(this, x, y, data, pick, tileDepth);
                EntityAddingList.Add(e);
                return true;
            }
            return false;
        }

        public bool setTileWithData(int x, int y, int index, int data, World.TileDepth tileDepth) {
            if (inWorld(x, y)) {
                TileMatrix[x, y, (int)tileDepth] = index;
                TileDataMatrix[x, y, (int)tileDepth] = data;
                calculateTileFrame(Game.instance, x, y, tileDepth);
                calculateTileFrame(Game.instance, x + 1, y, tileDepth);
                calculateTileFrame(Game.instance, x - 1, y, tileDepth);
                calculateTileFrame(Game.instance, x, y + 1, tileDepth);
                calculateTileFrame(Game.instance, x, y - 1, tileDepth);
                setSkyTile(x);
                lightingNeedsUpdate = true;
                return true;
            }
            return false;
        }

        public bool setTile(int x, int y, int index, World.TileDepth tileDepth) {
            return setTileWithData(x, y, index, 0, tileDepth);
        }

        public void setCrackNoCheck(int x, int y, byte amount) {
            CrackMatrix[x, y] = amount;
        }

        public byte getCrackNoCheck(int x, int y) {
            return CrackMatrix[x, y];
        }

        private int getCrack(int x, int y) {
            if (inWorld(x, y)) {
                return CrackMatrix[x, y];
            } else {
                return 0;
            }
        }

        public void setWireNoCheck(int x, int y, byte value) {
            wireMatrix[x, y] = value;
        }

        public void setWire(int x, int y, byte value) {
            if (inWorld(x, y)) {
                wireMatrix[x, y] = value;
            }
        }

        public byte getWire(int x, int y) {
            if (inWorld(x, y)) {
                return wireMatrix[x, y];
            } else {
                return 0;
            }
        }

        private void setSkyTile(int x) {
            int i;
            for (i = 0; i < sizeInTiles.Y;i++ ) {
                if (!getTileObject(x, i, World.TileDepth.tile).transparent) {
                    break;
                }
            }
            skyY[x] = i;
        }

        public bool setTile(int x, int y, Tile tile, World.TileDepth tileDepth) {
            return tile == null ? false : setTile(x, y, tile.index, tileDepth);
        }

        public bool inWorld(int x, int y) {
            return x >= 0 && x < sizeInTiles.X && y >= 0 && y < sizeInTiles.Y;
        }

        public bool inWorld(int x, int y, int border) {
            return x >= border && x < sizeInTiles.X - border && y >= border && y < sizeInTiles.Y - border;
        }


        public void Update(Game game, long tick) {

            foreach (Entity entity in EntityList) {
                entity.update(game, this);
            }

            while (EntityRemovalList.Count() > 0) {
                EntityList.Remove(EntityRemovalList[0]);
                EntityRemovalList.RemoveAt(0);
            }

            while (EntityAddingList.Count() > 0) {
                EntityList.Add(EntityAddingList[0]);
                EntityAddingList.RemoveAt(0);
            }

            if(player!=null){
                viewOffset.X = (int)player.displayPosition.X - (viewPortInPixels.Width / 2);
                viewOffset.Y = (int)player.displayPosition.Y - (viewPortInPixels.Height / 2);
            }

            if (viewOffset.X < 0) {
                viewOffset.X = 0;
            }
            if (viewOffset.Y < 0) {
                viewOffset.Y = 0;
            }
            if (viewOffset.X + viewPortInPixels.Width > sizeInTiles.X * tileSizeInPixels) {
                viewOffset.X = (sizeInTiles.X * tileSizeInPixels) - viewPortInPixels.Width;
            }
            if (viewOffset.Y + viewPortInPixels.Height > sizeInTiles.Y * tileSizeInPixels) {
                viewOffset.Y = (sizeInTiles.Y * tileSizeInPixels) - viewPortInPixels.Height;
            }

            viewPortInPixels.X = viewOffset.X;
            viewPortInPixels.Y = viewOffset.Y;
            viewPortInPixels.Width = game.Window.ClientBounds.Width;
            viewPortInPixels.Height = game.Window.ClientBounds.Height;

            viewPortInTiles = divideRectangle(viewPortInPixels, tileSizeInPixels);
            viewPortInTiles.Width += 1;
            viewPortInTiles.Height += 1;

            //TempLiquidMatrix = (int[,])LiquidMatrix.Clone();

            TileUpdates(tick);


            if (time > 5000 && time < 8000) {
                skyLightBrightness = (int)(((time - 5000) / 3000.0) * 300);
            }

            if (time > 16000 && time < 19000) {
                skyLightBrightness = (int)(((1.0 - (time - 16000) / 3000.0)) * 300);
            }

            if (skyLightBrightness<16) {
                skyLightBrightness = 16;
            }

            if(time>24000){
                time -=24000;
            }
            time += .1;
            lightingNeedsUpdate = true;
        }

        private void TileUpdates(long tick) {

            int border = 16;

            for (int x = viewPortInTiles.X - border; x < viewPortInTiles.X + viewPortInTiles.Width + 1 + border; x++) {
                for (int y = viewPortInTiles.Y + viewPortInTiles.Height + 1 + border; y > viewPortInTiles.Y - border; y--) {
                    if (inWorld(x, y)) {
                        if (getCrackNoCheck(x, y) > 0 && tick % 10 == 0) {
                            CrackMatrix[x, y] --;
                        }
                        if (LiquidNeedsUpdateMatrix[x, y]) {
                            updateLiquid(x, y);
                            LiquidNeedsUpdateMatrix[x, y] = false;
                        }
                        if (TileNeedsUpdateMatrix[x, y, (int)World.TileDepth.tile]) {
                            getTileObjectNoCheck(x, y, World.TileDepth.tile).updateNearChange(this, x, y, World.TileDepth.tile);
                            TileNeedsUpdateMatrix[x, y, (int)World.TileDepth.tile] = false;
                            LiquidNeedsUpdateMatrix[x, y] = true;
                        }
                        if (TileNeedsUpdateMatrix[x, y, (int)World.TileDepth.wall]) {
                            getTileObjectNoCheck(x, y, World.TileDepth.wall).updateNearChange(this, x, y, World.TileDepth.wall);
                            TileNeedsUpdateMatrix[x, y, (int)World.TileDepth.wall] = false;
                        }
                    }
                    /*if (x >= viewPortInTiles.X && y >= viewPortInTiles.Y && x <= viewPortInTiles.X + viewPortInTiles.Width + 1 && y <= viewPortInTiles.Y + viewPortInTiles.Height + 1) {
                        if (inWorld(x, y)) {
                            
                        }
                    }*/
                }
            }

            for (int i = 0; i < 100; i++) {
                int x = Game.rand.Next(sizeInTiles.X);
                int y = Game.rand.Next(sizeInTiles.Y);
                int d = Game.rand.Next() % 2;
                getTileObjectNoCheck(x, y, (TileDepth)d).updateRandom(this, x, y, (TileDepth)d);
            }
        }

        private void updateLiquid(int x, int y) {
            if (!(inWorld(x, y, 1))) {
                return;
            }

            if (!isTileSolid(x, y - 1, World.TileDepth.tile)) LiquidNeedsUpdateMatrix[x, y - 1] = true;
            if (!isTileSolid(x, y + 1, World.TileDepth.tile)) LiquidNeedsUpdateMatrix[x, y + 1] = true;
            if (!isTileSolid(x + 1, y, World.TileDepth.tile)) LiquidNeedsUpdateMatrix[x + 1, y] = true;
            if (!isTileSolid(x - 1, y, World.TileDepth.tile)) LiquidNeedsUpdateMatrix[x - 1, y] = true;


            int l = LiquidMatrix[x, y];
            int b = LiquidMatrix[x, y+1];

            if (l > 0) {
                if (getTileObjectNoCheck(x, y, World.TileDepth.tile).washedAwayByWater) {
                    mineTile(x, y, Item.itemSupick, World.TileDepth.tile);
                }
                if (!isTileSolid(x, y + 1, World.TileDepth.tile)) {
                    while(l>0 && b<100){
                        l--;
                        b++;
                    }
                    setLiquid(x, y, l);
                    setLiquid(x, y + 1, b);
                }
                l = LiquidMatrix[x, y];
                b = LiquidMatrix[x, y + 1];
                if (isTileSolid(x, y + 1, World.TileDepth.tile) || b >= 50) {
                    bool ls = isTileSolid(x - 1, y, World.TileDepth.tile);
                    bool rs = isTileSolid(x + 1, y, World.TileDepth.tile);

                    int lb = getLiquid(x - 1, y);
                    int rb = getLiquid(x + 1, y);

                    if ((!ls && lb < 100) && (!rs && rb < 100)) {

                        while ((l > rb || l > lb) && l > 0) {
                            if (l > 0 && l > lb) {
                                l--;
                                lb++;
                            }
                            if (l > 0 && l > rb) {
                                l--;
                                rb++;
                            }
                        }

                        setLiquid(x, y, l);
                        setLiquid(x - 1, y, lb);
                        setLiquid(x + 1, y, rb);

                    } else if ((!rs && rb < 100)) {

                        if (!inWorld(x + 1, y)) {
                            return;
                        }

                        while (l > 0 && l > rb) {
                            l--;
                            rb++;
                        }

                        setLiquid(x, y, l);
                        setLiquid(x + 1, y, rb);

                    } else if ((!ls && lb < 100)) {

                        while (l > 0 && l > lb) {
                            l--;
                            lb++;
                        }

                        setLiquid(x, y, l);
                        setLiquid(x - 1, y, lb);

                    }
                }
            }
        }

        public void Draw(Game game, GameTime gameTime) {

            Profiler.start("draw tiles");
            for (int x = viewPortInTiles.X; x < viewPortInTiles.X + viewPortInTiles.Width + 3; x++) {
                for (int y = viewPortInTiles.Y; y < viewPortInTiles.Y + viewPortInTiles.Height + 3; y++) {
                    if (inWorld(x, y)) {
                        Tile tile = getTileObjectNoCheck(x, y, World.TileDepth.tile);
                        Tile wall = getTileObjectNoCheck(x, y, World.TileDepth.wall);
                        dr.X = (tileSizeInPixels * x) - viewOffset.X;
                        dr.Y = (tileSizeInPixels * y) - viewOffset.Y;
                        
                        if (textureTileInfo[x, y, (int)World.TileDepth.tile].transparent && wall.renderType != Tile.RenderType.none) {
                            byte b = (byte)MathHelper.Clamp(LightMatrix[x, y] - (255 - wall.wallBrightness), 0, wall.wallBrightness);
                            dr.X += textureTileInfo[x, y, (int)World.TileDepth.wall].offset.X;
                            dr.Y += textureTileInfo[x, y, (int)World.TileDepth.wall].offset.Y;
                            dr.Width = textureTileInfo[x, y, (int)World.TileDepth.wall].rectangle.Width * tileScale;
                            dr.Height = textureTileInfo[x, y, (int)World.TileDepth.wall].rectangle.Height * tileScale;
                            drawRect(game, Tile.TileSheet, dr, textureTileInfo[x, y, (int)World.TileDepth.wall].rectangle, grayColors[b], wall.getDepth(x, y, World.TileDepth.wall), textureTileInfo[x, y, (int)World.TileDepth.wall].flipH, textureTileInfo[x, y, (int)World.TileDepth.wall].flipV);
                        }
                        byte a = (byte)MathHelper.Clamp(LightMatrix[x, y], 0, 255);
                        if (textureTileInfo[x, y, (int)World.TileDepth.tile].transparent) {
                            if (LiquidMatrix[x, y] > 0 && textureLiquidInfo[x, y] != null) {
                                dr.X += textureLiquidInfo[x, y].offset.X;
                                dr.Y += textureLiquidInfo[x, y].offset.Y;
                                dr.Width = textureLiquidInfo[x, y].rectangle.Width * tileScale;
                                dr.Height = textureLiquidInfo[x, y].rectangle.Height * tileScale;
                                drawRect(game, Tile.TileSheet, dr, textureLiquidInfo[x, y].rectangle, grayColors[a], Game.RENDER_DEPTH_LIQUID, textureLiquidInfo[x, y].flipH, textureLiquidInfo[x, y].flipV);
                            }
                        }
                        if(tile.index != Tile.TileAir.index){
                            if (textureTileInfo[x, y, (int)World.TileDepth.tile] != null) {
                                dr.X += textureTileInfo[x, y, (int)World.TileDepth.tile].offset.X;
                                dr.Y += textureTileInfo[x, y, (int)World.TileDepth.tile].offset.Y;
                                dr.Width = textureTileInfo[x, y, (int)World.TileDepth.tile].rectangle.Width * tileScale;
                                dr.Height = textureTileInfo[x, y, (int)World.TileDepth.tile].rectangle.Height * tileScale;
                                drawRect(game, Tile.TileSheet, dr, textureTileInfo[x, y, (int)World.TileDepth.tile].rectangle, grayColors[a], tile.getDepth(x, y, World.TileDepth.tile), textureTileInfo[x, y, (int)World.TileDepth.tile].flipH, textureTileInfo[x, y, (int)World.TileDepth.tile].flipV);
                            }
                         
                        }
                        if(Game.debugEnabled){
                            if(getCrackNoCheck(x, y)>0){
                                Game.drawString(((int)(getCrackNoCheck(x, y)/2.56))+"", new Vector2(dr.X, dr.Y), Color.White, Game.RENDER_DEPTH_TILE_CRACK_DEBUG_TEXT);
                            }
                        }
                    }
                }
            }
            Profiler.end("draw tiles");

            Profiler.start("draw entities");
            foreach (Entity entity in EntityList) {
                entity.draw(game, this);
            }
            Profiler.end("draw entities");

        }

        private void drawRect(Game game, Texture2D texture, Rectangle destination, Rectangle source, Color color, float depth, bool flipH, bool flipV) {
            game.spriteBatch.Draw(texture, destination, source, color, 0, Vector2.Zero, flipH ? (flipV ? SpriteEffects.None : SpriteEffects.FlipHorizontally) : (flipV ? SpriteEffects.FlipVertically : SpriteEffects.None), depth);
        }

        private Rectangle subtractPointToRectangle(Point point, Rectangle rectangle) {
            return new Rectangle(rectangle.X-point.X, rectangle.Y-point.Y, rectangle.Width, rectangle.Height);
        }

        private Rectangle divideRectangle(Rectangle rectangle, int number) {
            return new Rectangle(rectangle.X / number, rectangle.Y / number, rectangle.Width / number, rectangle.Height / number);
        }

        internal void liquidThreadUpdate() {
            Profiler.start("liquid");

            int border = 16;

            for (int x = viewPortInTiles.X - border; x < viewPortInTiles.X + viewPortInTiles.Width + 1 + border; x++) {
                for (int y = viewPortInTiles.Y + viewPortInTiles.Height + 1 + border; y > viewPortInTiles.Y - border; y--) {
                    if (inWorld(x, y)) {
                        if (LiquidNeedsUpdateMatrix[x, y]) {
                            updateLiquid(x, y);
                            LiquidNeedsUpdateMatrix[x, y] = false;
                        }
                    }
                }
            }

            Profiler.end("liquid");

            liquidThreadFps.update();
        }

        internal void lightingThreadUpdate() {

            if (lightingNeedsUpdate) {
                Profiler.start("lighting");

                TempLightMatrix = (int[,])LightMatrix.Clone();

                int border = 16;

                for (int x = viewPortInTiles.X - border; x < viewPortInTiles.X + viewPortInTiles.Width + 1 + border; x++) {
                    for (int y = viewPortInTiles.Y - border; y < viewPortInTiles.Y + viewPortInTiles.Height + 1 + border; y++) {
                        lightUpdate1(x, y);
                    }
                }

                for (int x = viewPortInTiles.X - border; x < viewPortInTiles.X + viewPortInTiles.Width + 1 + border; x++) {
                    lightUpdate2(x, viewPortInTiles.Y - border, true);
                    for (int y = viewPortInTiles.Y + 1 - border; y < viewPortInTiles.Y + viewPortInTiles.Height + 1 + border; y++) {
                        lightUpdate2(x, y, false);
                    }
                }

                LightMatrix = (int[,])TempLightMatrix.Clone();

                lightingNeedsUpdate = false;

                Profiler.end("lighting");

                lightingThreadFps.update();
            }
        }

        private void lightUpdate1(int x, int y) {
            if (inWorld(x, y)) {
                TempLightMatrix[x, y] = 0;
            }
        }

        private void lightUpdate2(int x, int y, bool firstY) {
            if (inWorld(x, y)) {
                //if (firstY && y < skyY[x]) {
                if (textureTileInfo[x, y, (int)World.TileDepth.tile].transparent && textureTileInfo[x, y, (int)World.TileDepth.wall].transparent && y < heightMap[x] + undergroundY) {
                    setLightFromSource(x, y, true, skyLightBrightness);
                }
                Tile tile = getTileObject(x, y, World.TileDepth.tile);
                Tile wall = getTileObject(x, y, World.TileDepth.wall);
                if (tile.getLight(x, y, World.TileDepth.tile) > 0 || wall.getLight(x, y, World.TileDepth.wall) > 0) {
                    setLightFromSource(x, y, false, Math.Max(tile.getLight(x, y, World.TileDepth.tile), wall.getLight(x, y, World.TileDepth.wall)));
                }
            }
        }

        private void setLightFromSource(int x, int y, bool isSkyLight, int power) {
            spreadLight(x, y, power, 0, isSkyLight, false);
            spreadLight(x, y, power, 1, isSkyLight, true);
            spreadLight(x, y, power, 2, isSkyLight, false);
            spreadLight(x, y, power, 3, isSkyLight, true);
        }

        private void spreadLight(int x, int y, int a, int q, bool isSkyLight, bool d) {
            if (inWorld(x, y)) {

                int b = a;

                //bool t = false;

                if (TempLightMatrix[x, y] < b) {
                    TempLightMatrix[x, y] = b;
                } else {
                    return;
                }

                if (getTileObjectNoCheck(x, y, World.TileDepth.tile).transparent) {
                    b = a - 10;
                    //t = true;
                } else {
                    b = a - 100;
                }

                if (b > 0) {
                    //if (q == 0 || ((q == 1 || q == 3) && d)) {
                        spreadLight(x - 1, y, b, 0, isSkyLight, d);//left
                    //}
                    //if (q == 1 || ((q == 0 || q == 2) && !d)) {
                        //spreadLight(x, y + 1, (isSkyLight && t) ? a : b, 1, isSkyLight, d);//down
                        spreadLight(x, y + 1, b, 1, isSkyLight, d);//down
                    //}
                    //if (q == 2 || ((q == 1 || q == 3) && d)) {
                        spreadLight(x + 1, y, b, 2, isSkyLight, d);//right
                    //}
                    //if (q == 3 || ((q == 0 || q == 2) && !d)) {
                        spreadLight(x, y - 1, b, 3, isSkyLight, d);//up
                    //}
                }
            }
        }

        /*public static int Clamp(int value, int min, int max) {
            return value > max ? max : (value < min ? min : (value));
        }*/

        public Color getBgColor() {
            if(player!=null){
                Color realColor = getRealBgColor();
                currentBgColor.R += (byte)((realColor.R - currentBgColor.R) / 16);
                currentBgColor.G += (byte)((realColor.G - currentBgColor.G) / 16);
                currentBgColor.B += (byte)((realColor.B - currentBgColor.B) / 16);
                currentBgColor.A = 1;
            }
            return currentBgColor;
        }

        private Color getRealBgColor() {
            float a = 0;
            if (player.position.Y < (getHeightMap((int)player.position.X / tileSizeInPixels) + undergroundY) * tileSizeInPixels) {
                a = (MathHelper.Clamp(skyLightBrightness, 0, 255)) / 255.0f;
            }
            return new Color((byte)(100 * a), (byte)(149 * a), (byte)(237 * a));
        }

        private int getHeightMap(int x) {
            if(x>=0 && x<sizeInTiles.X){
                return heightMap[x];
            } else {
                return heightMap[0];
            }
        }

        public void generate(int type) {
            WorldGenerator.Generate(this, type);
        }

        public static long getnewUniqueID() {
            previousUniqueID++;
            return previousUniqueID;
        }

        public static bool save(World world, string name, bool forceOverride = true){

            String file = Game.saveLocation + @"\" + name + "." + Game.saveExtention;

            if (System.IO.File.Exists(file) && !forceOverride) {
                return false;
            }

            List<Byte> bytes = new List<byte>();
            byte[] xBytes = intToBytes(world.sizeInTiles.X);
            byte[] yBytes = intToBytes(world.sizeInTiles.Y);

            bytes.AddRange(xBytes);

            bytes.AddRange(yBytes);

            byte[] timeBytes = intToBytes((int)world.time);

            bytes.AddRange(timeBytes);

            for (int x = 0; x < world.sizeInTiles.X; x++) {
                for (int y = 0; y < world.sizeInTiles.Y; y++) {
                    bytes.Add((byte)world.TileMatrix[x, y, 0]);
                    bytes.Add((byte)world.TileMatrix[x, y, 1]);
                    bytes.Add((byte)world.LiquidMatrix[x, y]);
                    bytes.Add((byte)(world.TileDataMatrix[x, y, 0] < 0 ? 0 : world.TileDataMatrix[x, y, 0]));
                    bytes.Add((byte)(world.TileDataMatrix[x, y, 0] < 0 ? -world.TileDataMatrix[x, y, 0] : 0));
                    bytes.Add((byte)(world.TileDataMatrix[x, y, 1] < 0 ? 0 : world.TileDataMatrix[x, y, 1]));
                    bytes.Add((byte)(world.TileDataMatrix[x, y, 1] < 0 ? -world.TileDataMatrix[x, y, 1] : 0));
                }
                bytes.AddRange(intToBytes(world.heightMap[x]));
            }

            Game.instance.inventory.saveTo(bytes);

            world.player.saveTo(bytes);

            byte[] byteArray = bytes.ToArray<byte>();
            //Game.instance.console.println("Save Bytes: " + byteArray.Length);
            System.IO.File.WriteAllBytes(file, byteArray);
            return true;
        }

        public static World load(string name) {
            byte[] bytes = System.IO.File.ReadAllBytes(Game.saveLocation + @"\" + name + "." + Game.saveExtention);

            //Game.instance.console.println("Load Bytes: "+bytes.Length);
            //return null;

            World world = new World(new Point(bytesToInt(bytes, 0), bytesToInt(bytes, 4)), name);
            world.time = bytesToInt(bytes, 8);
            
            int index = 12;
            for (int x = 0; x < world.sizeInTiles.X; x++) {
                for (int y = 0; y < world.sizeInTiles.Y; y++) {
                    world.TileMatrix[x, y, 0] = bytes[index];
                    index++;
                    world.TileMatrix[x, y, 1] = bytes[index];
                    index++;
                    world.LiquidMatrix[x, y] = bytes[index];
                    index++;
                    world.TileDataMatrix[x, y, 0] = bytes[index] + (-bytes[index+1]);
                    index+=2;
                    world.TileDataMatrix[x, y, 1] = bytes[index] + (-bytes[index+1]);
                    index+=2;
                }
                world.heightMap[x] = bytesToInt(bytes, index);
                index += 4;
            }

            Game.instance.inventory = new Inventory();

            index = Game.instance.inventory.loadFrom(bytes, index);

            world.player = new EntityPlayer(0, 0);
            world.player.loadFrom(bytes, index);
            world.EntityAddingList.Add(world.player);
            world.calculateTileFrames(Game.instance);

            return world;
        }

        public static byte[] intToBytes(int value) {
            return new byte[4] { 
                    (byte)(value & 0xFF), 
                    (byte)((value >> 8) & 0xFF), 
                    (byte)((value >> 16) & 0xFF), 
                    (byte)((value >> 24) & 0xFF) };
        }

        public static int bytesToInt(byte[] value, int index) {
            return (int)(
                value[0 + index] << 0 |
                value[1 + index] << 8 |
                value[2 + index] << 16 |
                value[3 + index] << 24);
        }

        public void SimUpdate(long tick) {
            TileUpdates(tick);
        }

        public bool isTileSolid(int x, int y, World.TileDepth tileDepth) {
            return getTileObject(x, y, tileDepth).isSolid(this, x, y);
        }
    }
}
