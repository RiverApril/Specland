using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class Recipe {

        public static List<Recipe> Recipes = new List<Recipe>();

        public static Recipe RecipeWoodenPick = new Recipe(new ItemStack[] { new ItemStack(Item.ItemTile, 12, Tile.TileWood.index) }, new ItemStack(Item.ItemCrapick));
        public static Recipe RecipeStoneBricks = new Recipe(new ItemStack[] { new ItemStack(Item.ItemTile, 2, Tile.TileStone.index) }, new ItemStack(Item.ItemTile, 1, Tile.TileStoneBricks.index));
        public static Recipe RecipeTorch = new Recipe(new ItemStack[] { new ItemStack(Item.ItemTile, 1, Tile.TileWood.index) }, new ItemStack(Item.ItemTile, 1, Tile.TileTorch.index));
        public static Recipe RecipeGlass = new Recipe(new ItemStack[] { new ItemStack(Item.ItemTile, 1, Tile.TileSand.index) }, new ItemStack(Item.ItemTile, 1, Tile.TileGlass.index));
        public static Recipe RecipeWoodDoor = new Recipe(new ItemStack[] { new ItemStack(Item.ItemTile, 6, Tile.TileWood.index) }, new ItemStack(Item.ItemTile, 1, Tile.TileWoodDoor.index));
        public static Recipe RecipeWoodTable = new Recipe(new ItemStack[] { new ItemStack(Item.ItemTile, 6, Tile.TileWood.index) }, new ItemStack(Item.ItemTile, 1, Tile.TileWoodTable.index));
        public static Recipe RecipeWoodChair = new Recipe(new ItemStack[] { new ItemStack(Item.ItemTile, 2, Tile.TileWood.index) }, new ItemStack(Item.ItemTile, 1, Tile.TileWoodChair.index));

        public ItemStack[] ingredients = new ItemStack[0];
        public ItemStack result = null;

        public Recipe(ItemStack[] ingredients, ItemStack result) {
            Recipes.Add(this);
            this.ingredients = ingredients;
            this.result = result;
        }

    }
}
