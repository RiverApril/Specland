using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    class WorldGenerator {
        public const int TypeEmpty = 0;
        public const int TypeFlat = 1;
        public const int TypeNatural = 2;

        public static void Generate(World world, int type) {
            Generate(world, type, new Random());
        }

        public static void Generate(World world, int type, int seed) {
            Generate(world, type, new Random(seed));
        }

        public static void Generate(World world, int type, Random rand) {
            switch(type){
                case (TypeEmpty): { break; };
                case (TypeFlat): { GenerateFlat(world, rand); break; };
                case (TypeNatural): { GenerateNatural(world, rand); break; };
            }

            for (int i = 0; i < 100; i++) {
                world.SimUpdate(0);
            }
        }

        public static void GenerateFlat(World world, Random rand) {
            for (int x = 0; x < world.sizeInTiles.X; x++) {
                for (int y = 0; y < world.sizeInTiles.Y; y++) {
                    if (y < 100) {
                        continue;
                    } else if (y == 100) {
                        world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileGrass.index;
                    } else if (y < 120) {
                        world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileDirt.index;
                        world.TileMatrix[x, y, (int)World.TileDepth.wall] = Tile.TileDirt.index;
                    } else {
                        bool a = rand.Next(2) == 0;
                        world.TileMatrix[x, y, (int)World.TileDepth.tile] = ((a) ? Tile.TileDirt : Tile.TileStone).index;
                        world.TileMatrix[x, y, (int)World.TileDepth.wall] = ((a) ? Tile.TileDirt : Tile.TileStone).index;
                    }
                    world.LiquidNeedsUpdateMatrix[x, y] = true;
                }
            }
        }

        public static void GenerateNatural(World world, Random rand) {
            int width = world.sizeInTiles.X;
            int height = world.sizeInTiles.Y;
            int[] heightMap = new int[width];
            int minHeight = (height / 10);
            int maxHeight = (height / 10)*2;
            int last = (maxHeight - minHeight) / 2;
            bool goingUp = rand.Next(1)==0;
            int freq = 5;
            bool justChanged = false;
            for (int i = 0; i < width; i ++) {
                heightMap[i] = last;
                if(rand.Next(10)==0 && !justChanged){
                    goingUp = rand.Next(2) == 0;
                }
                if(last>=maxHeight){
                    goingUp = true;
                }
                if(last<=minHeight){
                    goingUp = false;
                }
                
                if(rand.Next(10)==0){
                    freq += (rand.Next(2) == 0) ? 1 : -1;
                    if (freq < 2) {
                        freq = 2;
                    }
                    if (freq > 10) {
                        freq = 10;
                    }
                }

                justChanged = false;

                if (rand.Next(freq) == 0) {
                    last += goingUp ? -1 : 1;
                    justChanged = true;
                }
            }

            int biomeSize = 0;

            int biomeGrass = 0;
            int biomeDesert = 1;
            int biomeWater = 10;

            int biome = biomeGrass;

            for (int x = 0; x < world.sizeInTiles.X; x++) {

                if(biomeSize > 50 && rand.Next(20)==0){
                    biome = rand.Next(2);
                    biomeSize = 0;
                }
                biomeSize++;

                for (int y = 0; y < world.sizeInTiles.Y; y++) {
                    if (y < heightMap[x]) {
                        continue;
                    } else if (y == heightMap[x]) {
                        if (biome == biomeDesert) {
                            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileSand.index;
                        } else if (biome == biomeWater) {
                            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileAir.index;

                        } else {
                            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileGrass.index;
                        }
                    } else if (y < heightMap[x]+20) {
                        if (biome == biomeDesert) {
                            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileSand.index;
                            world.TileMatrix[x, y, (int)World.TileDepth.wall] = Tile.TileSand.index;
                        } else if (biome == biomeWater) {
                            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileAir.index;
                            world.TileMatrix[x, y, (int)World.TileDepth.wall] = Tile.TileDirt.index;
                            world.LiquidMatrix[x, y] = 100;
                        } else {
                            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileDirt.index;
                            world.TileMatrix[x, y, (int)World.TileDepth.wall] = Tile.TileDirt.index;
                        }
                    } else {
                        bool a = rand.Next(2) == 0;
                        world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileStone.index;
                        world.TileMatrix[x, y, (int)World.TileDepth.wall] = Tile.TileStone.index;
                    }
                }
            }

            int tree = 3;
            for (int x = 1; x < world.sizeInTiles.X - 1; x++) {
                if (heightMap[x] == heightMap[x + 1] && heightMap[x] == heightMap[x - 1]) {
                    tree--;
                    if (world.TileMatrix[x, heightMap[x] - 1, (int)World.TileDepth.tile] == Tile.TileAir.index && world.TileMatrix[x + 1, heightMap[x + 1] - 1, (int)World.TileDepth.tile] == Tile.TileAir.index && world.TileMatrix[x - 1, heightMap[x - 1] - 1, (int)World.TileDepth.tile] == Tile.TileAir.index) {
                        if (world.TileMatrix[x, heightMap[x], (int)World.TileDepth.tile] == Tile.TileGrass.index && world.TileMatrix[x + 1, heightMap[x + 1], (int)World.TileDepth.tile] == Tile.TileGrass.index && world.TileMatrix[x - 1, heightMap[x - 1], (int)World.TileDepth.tile] == Tile.TileGrass.index) {
                            if (rand.Next(5) == 0 && tree <= 0) {
                                tree = 4;


                                world.TileMatrix[x + 1, heightMap[x + 1] - 1, (int)World.TileDepth.tile] = Tile.TileTree.index;
                                world.TileDataMatrix[x + 1, heightMap[x + 1] - 1, (int)World.TileDepth.tile] = 1;

                                world.TileMatrix[x - 1, heightMap[x - 1] - 1, (int)World.TileDepth.tile] = Tile.TileTree.index;
                                world.TileDataMatrix[x - 1, heightMap[x - 1] - 1, (int)World.TileDepth.tile] = 1;

                                world.TileDataMatrix[x, heightMap[x] - 1, (int)World.TileDepth.tile] = 1;


                                int h = rand.Next(20) + 10;
                                int right = 3;
                                int left = 3;
                                for (int i = 0; i > -h; i--) {
                                    world.TileMatrix[x, heightMap[x] - 1 + i, (int)World.TileDepth.tile] = Tile.TileTree.index;
                                    right--;
                                    if (right <= 0 && rand.Next(6) == 0) {
                                        right = 2;
                                        world.TileMatrix[x + 1, heightMap[x + 1] - 1 + i, (int)World.TileDepth.tile] = Tile.TileTree.index;
                                    }
                                    left--;
                                    if (left <= 0 && rand.Next(6) == 0) {
                                        left = 2;
                                        world.TileMatrix[x - 1, heightMap[x - 1] - 1 + i, (int)World.TileDepth.tile] = Tile.TileTree.index;
                                    }
                                }
                                CreateCircle(world, 5, x, heightMap[x + 1] - 1 - h, FillLeaf);
                            }
                        }
                    }
                }
            }

            //Dirt:
            for (int i = 0; i < 1000; i++) {
                int x = rand.Next(width);
                int y = heightMap[x] + World.nearSurfaceY;
                CreateBlob(world, rand, 20, 50, x, y + rand.Next(height - y), FillDirt);
            }

            //Sand:
            for (int i = 0; i < 500; i++) {
                int x = rand.Next(width);
                int y = heightMap[x] + World.nearSurfaceY;
                CreateBlob(world, rand, 20, 50, x, y + rand.Next(height - y), FillSand);
            }

            //Coal Ore:
            /*for (int i = 0; i < 1000;i++ ) {
                int x = rand.Next(width);
                int y = heightMap[x] + 40;
                y += rand.Next(height-y);
                CreateBlob(world, rand, 4, 10, x, y, FillOre, Tile.TileCoalOre);
            }*/

            //Caves:
            for (int i = 0; i < 1000; i++) {
                int x = rand.Next(width);
                int y = heightMap[x] + World.undergroundWaterY;
                CreateBlob(world, rand, 20, 100, x, y + rand.Next(height - y), FillAir);
            }

            //Water:
            for (int i = 0; i < 300; i++) {
                int x = rand.Next(width);
                int y = heightMap[x] + World.undergroundWaterY;
                CreateBlob(world, rand, 20, 100, x, y + rand.Next(height - y), FillWater);
            }

            for (int i = 0; i < world.sizeInTiles.X;i++ ) {
                for (int j = 0; j < world.sizeInTiles.Y; j++) {
                    //world.TileNeedsUpdateMatrix[i, j, World.TileDepth.tile] = true;
                    //world.TileNeedsUpdateMatrix[i, j, World.TileDepth.wall] = true;
                    world.LiquidNeedsUpdateMatrix[i, j] = true;
                }
            }

            for (int i = 0; i < 10000; i++) {
                int x = rand.Next(width);
                int y = heightMap[x] + World.undergroundY;
                y += rand.Next(height - y);
                if (world.inWorld(x, y) && Tile.TilePlantGlow.canBePlacedHere(world, x, y, World.TileDepth.tile)) {
                    world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TilePlantGlow.index;
                }

            }

            world.heightMap = heightMap;
        }

        private static void CreateBlob(World world, Random rand, int circleSize, int blobSize, int x, int y, Action<World, int, int> action) {
            Vector2 center = new Vector2(0, 0);
            int sizeSquared = blobSize ^ 2;
            for (int i = -blobSize; i < blobSize; i++) {
                for (int j = -blobSize; j < blobSize; j++) {
                    if (Vector2.DistanceSquared(new Vector2(i, j), center) < sizeSquared) {
                        if (rand.Next(blobSize / 4) == 0) {
                            CreateCircle(world, circleSize + (rand.Next(circleSize / 10) - (circleSize / 20)), x + i, y + j, action);
                        }
                    }
                }
            }
        }

        private static void CreateCircle(World world, int size, int x, int y, Action<World, int, int> action) {
            Vector2 center = new Vector2(0, 0);
            int sizeSquared = size ^ 2;
            for (int i = -size; i < size; i++) {
                for (int j = -size; j < size; j++) {
                    if (Vector2.DistanceSquared(new Vector2(i, j), center) < sizeSquared) {
                        if (world.inWorld(x + i, y + j)) {
                            action(world, x+i, y+j);
                        }
                    }
                }
            }
        }

        public static void FillWater(World world, int x, int y) {
            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileAir.index;
            world.LiquidMatrix[x, y] = 50;
        }

        private static void FillAir(World world, int x, int y) {
            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileAir.index;
        }

        private static void FillDirt(World world, int x, int y) {
            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileDirt.index;
            world.TileMatrix[x, y, (int)World.TileDepth.wall] = Tile.TileDirt.index;
        }

        private static void FillSand(World world, int x, int y) {
            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileSand.index;
            world.TileMatrix[x, y, (int)World.TileDepth.wall] = Tile.TileSand.index;
        }

        private static void FillOre(World world, int x, int y, int index) {
            world.TileMatrix[x, y, (int)World.TileDepth.tile] = index;
        }

        private static void FillLeaf(World world, int x, int y) {
            world.TileMatrix[x, y, (int)World.TileDepth.tile] = Tile.TileLeaf.index;
        }
    }
}
