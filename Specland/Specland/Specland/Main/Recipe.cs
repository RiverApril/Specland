using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class Recipe {

        public static List<Recipe> Recipes = new List<Recipe>();

        //public static Recipe RecipeWoodenPick = new Recipe(new ItemStack[] { new ItemStack(Tile.TileWood, 12) }, new ItemStack(Item.itemWoodPick));
        //public static Recipe RecipeStonePick = new Recipe(new ItemStack[] { new ItemStack(Tile.TileWood, 6) , new ItemStack(Tile.TileStone, 6)}, new ItemStack(Item.itemStonePick));
        public static Recipe RecipeStoneBricks = new Recipe(new ItemStack[] { new ItemStack(Tile.TileStone, 2) }, new ItemStack(Tile.TileStoneBricks, 1));
        public static Recipe RecipeTorch = new Recipe(new ItemStack[] { new ItemStack(Tile.TileWood, 1) }, new ItemStack(Tile.TileTorch, 4));
        public static Recipe RecipeLamp = new Recipe(new ItemStack[] { new ItemStack(Tile.TileWood, 1) , new ItemStack(Tile.TilePlantGlow, 1), new ItemStack(Tile.TileGlass, 1)}, new ItemStack(Tile.TileLamp, 4));
        public static Recipe RecipeGlass = new Recipe(new ItemStack[] { new ItemStack(Tile.TileSand, 1) }, new ItemStack(Tile.TileGlass, 1));
        public static Recipe RecipeWoodDoor = new Recipe(new ItemStack[] { new ItemStack(Tile.TileWood, 6) }, new ItemStack(Tile.TileWoodDoor, 1));
        public static Recipe RecipeWoodTable = new Recipe(new ItemStack[] { new ItemStack(Tile.TileWood, 6) }, new ItemStack(Tile.TileWoodTable, 1));
        public static Recipe RecipeWoodChair = new Recipe(new ItemStack[] { new ItemStack(Tile.TileWood, 2) }, new ItemStack(Tile.TileWoodChair, 1));
        public static Recipe RecipeWoodPlatform = new Recipe(new ItemStack[] { new ItemStack(Tile.TileWood, 1) }, new ItemStack(Tile.TileWoodPlatform, 2));

        public ItemStack[] ingredients = new ItemStack[0];
        public ItemStack result = null;

        public Recipe(ItemStack[] ingredients, ItemStack result) {
            Recipes.Add(this);
            this.ingredients = ingredients;
            this.result = result;
        }

    }
}
