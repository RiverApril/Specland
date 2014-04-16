using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Specland {
    public class Inventory : Gui {

        int x = 4;
        int y = 8;

        int selectedSlot = 0;

        int lastScrollValue;

        Color hotBarColor = Color.White;

        float hotBarFade = 0;
        bool hotBarFadingIn = true;

        Color inventoryColor = Color.White;

        float inventoryFade = 0;
        bool inventoryFadingIn = false;
        int inventoryMove = 0;

        public static int scale = 2;

        public static int gridSize = 11 * scale;
        public static int squareSize = 10 * scale;
        public static int itemSize = 8 * scale;
        public static int squareBorderSize = (squareSize - itemSize) / 2;

        ItemStack[] items = new ItemStack[100];

        ItemStack cursorItem = new ItemStack(Item.ItemEmpty);

        private int mouseItemSlot = -1;
        private int mouseCraftSlot = -1;

        private long tick = 0;
        private int craftingScroll = 0;
        private List<Recipe> valaidRecipes = new List<Recipe>();
        public int mouseTileX;
        public int mouseTileY;
        public int mouseTileDistanceFromPlayer;

        private int leftClickTimer = 0;
        private int rightClickTimer = 0;

        private static int craftingAreaY = (int)(11 * gridSize);

        public ItemStack currentItem;

        public Inventory(){
            for (int i = 0; i < items.Length; i++) {
                items[i] = new ItemStack(Item.ItemEmpty);
            }
            pickUp(new ItemStack(Item.ItemSupick));
            pickUp(new ItemStack(Item.ItemCrapick));
            //pickUp(new ItemStack(Item.ItemTile, 999, Tile.TileWoodTable.index));
            updateValaidRecipes();
        }

        private void updateValaidRecipes() {
            valaidRecipes.Clear();
            foreach(Recipe r in Recipe.Recipes){
                bool a = true;
                foreach (ItemStack i in r.ingredients) {
                    if (!inventoryContainsStack(0, 49, i)) {
                        a = false;
                        break;
                    }
                }
                if(a){
                    valaidRecipes.Add(r);
                }
            }
        }

        private bool inventoryContainsStack(int minSlot, int maxSlot, ItemStack stack) {
            for (int i = minSlot; i <= maxSlot; i++) {
                if (items[i].getItem().index == stack.getItem().index) {
                    if (items[i].getData() == stack.getData()) {
                        if (items[i].getCount() >= stack.getCount()) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override void draw(Game game, GameTime gameTime) {
            int xx = 0;
            int yy = 0;
            int itemIndex = 0;
            Rectangle r;

            bool mouseInCrafting = mouseInCraftingArea(game);

            for(int i=0; i < 10; i++){
                xx = x + (selectedSlot == i ? (cursorItem.isEmpty() ? 4 : 2) : 0) + inventoryMove + 4;
                yy = y + (i * gridSize);
                r = new Rectangle(xx + squareBorderSize, yy + squareBorderSize, itemSize, itemSize);
                game.spriteBatch.Draw(guiTexture, new Rectangle(xx, yy, squareSize, squareSize), new Rectangle(0, 0, 20, 20), hotBarColor);
                items[itemIndex].draw(game, gameTime, r, hotBarColor);

                itemIndex++;
            }

            if (inventoryFade>0) {
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 4; j++) {
                        xx = x + (j * gridSize) + inventoryMove - (gridSize * 4);
                        yy = y + (i * gridSize);
                        r = new Rectangle(xx + squareBorderSize, yy + squareBorderSize, itemSize, itemSize);
                        game.spriteBatch.Draw(guiTexture, new Rectangle(xx, yy, squareSize, squareSize), new Rectangle(0, 0, 20, 20), inventoryColor);
                        items[itemIndex].draw(game, gameTime, r, inventoryColor);

                        itemIndex++;
                    }
                }
                
                float a = inventoryFade * (mouseInCrafting ? 1 : .5f);
                Color color = new Color(a, a, a, a);
                game.spriteBatch.DrawString(Game.fontNormal, " Crafting", new Vector2(inventoryMove - (gridSize * 4), craftingAreaY - gridSize*.5f), color);

                for (int i = -2; i <= 2; i++) {
                    int j = i + craftingScroll;
                    if (j >= 0 && j < valaidRecipes.Count()) {
                        xx = x + (i * gridSize) + inventoryMove - (gridSize * 2);
                        yy = y + craftingAreaY;
                        r = new Rectangle(xx + squareBorderSize, yy + squareBorderSize, itemSize, itemSize);
                        int abs = Math.Abs(i);
                        a = (inventoryFade * (abs == 3 ? .1f : (abs == 2 ? .2f : (abs == 1 ? .5f : 1)))) * (mouseInCrafting ? 1 : .1f);
                        color = new Color(a, a, a, a);
                        game.spriteBatch.Draw(guiTexture, new Rectangle(xx, yy, squareSize, squareSize), new Rectangle(0, 0, 20, 20), color);
                        valaidRecipes[j].result.draw(game, gameTime, r, color);
                        for (int k = 0; k < valaidRecipes[j].ingredients.Length; k++) {
                            xx = x + (i * gridSize) + inventoryMove - (gridSize * 2);
                            yy = y + 4 + ((k + 1) * gridSize) + craftingAreaY;
                            r = new Rectangle(xx + squareBorderSize, yy + squareBorderSize, itemSize, itemSize);
                            valaidRecipes[j].ingredients[k].draw(game, gameTime, r, color);
                        }
                    }
                }

            }

            MouseState mouseState = Mouse.GetState();

            xx = mouseState.X + 10;
            yy = mouseState.Y + 10;
            r = new Rectangle(xx + 2, yy + 2, itemSize, itemSize);

            if (inventoryFade > 0 && !cursorItem.isEmpty()) {
                cursorItem.draw(game, gameTime, r, inventoryColor);
                game.spriteBatch.DrawString(Game.fontNormal, cursorItem.getDisplayName(), new Vector2(r.X + 24, r.Y), inventoryColor);
            } else if (inventoryFade > 0 && mouseItemSlot != -1 && !items[mouseItemSlot].isEmpty()) {
                game.spriteBatch.DrawString(Game.fontNormal, items[mouseItemSlot].getDisplayName(), new Vector2(xx + 2, yy + 2), inventoryColor);
            } else if (!items[selectedSlot].isEmpty()) {
                items[selectedSlot].draw(game, gameTime, r, (mouseTileDistanceFromPlayer <= items[selectedSlot].getItem().reach) ? Color.White : new Color(.1f, .1f, .1f, .1f));
            }

            currentItem = cursorItem.isEmpty() ? items[selectedSlot] : cursorItem;

            if ((mouseTileDistanceFromPlayer <= currentItem.getItem().reach)) {
                currentItem.getItem().drawHover(game, mouseTileX, mouseTileY, currentItem);
            }

            game.spriteBatch.Draw(guiTexture, new Rectangle(mouseState.X, mouseState.Y, 16, 16), new Rectangle(0, 20, 16, 16), Game.cursorColor);
        }

        private bool mouseInCraftingArea(Game game) {
            int xx = x + (-2 * gridSize) + inventoryMove - (gridSize * 2);
            int yy = y + craftingAreaY-1;
            return new Rectangle(xx, yy, gridSize * 5, gridSize * 2).Contains(game.inputState.mouseState.X, game.inputState.mouseState.Y);
        }


        public void update(Game game, GameTime gameTime) {
            #region Scrolling and Fading

            int xx = 0;
            int yy = 0;

            int scrollDif = game.inputState.mouseState.ScrollWheelValue - lastScrollValue;

            if (scrollDif != 0) {

                bool mouseInCrafting = mouseInCraftingArea(game);

                if (mouseInCrafting && inventoryFadingIn) {
                    if (scrollDif > 0) {
                        scrollDif -= 1;
                        craftingScroll -= 1;
                    }
                    if (scrollDif < 0) {
                        scrollDif += 1;
                        craftingScroll += 1;
                    }
                    while (craftingScroll < 0) {
                        craftingScroll++;
                    }
                    while (craftingScroll >= valaidRecipes.Count()) {
                        craftingScroll--;
                    }
                }else{
                    if (scrollDif > 0) {
                        scrollDif -= 1;
                        selectedSlot -= 1;
                    }
                    if (scrollDif < 0) {
                        scrollDif += 1;
                        selectedSlot += 1;
                    }
                    while (selectedSlot >= 10) {
                        selectedSlot -= 10;
                    }
                    while (selectedSlot < 0) {
                        selectedSlot += 10;
                    }
                    hotBarFadingIn = true;
                }
            }

            if (hotBarFadingIn) {
                if (hotBarFade < 2f) {
                    hotBarFade += .05f;
                } else {
                    hotBarFadingIn = false;
                }
            } else {
                if (hotBarFade > .2f) {
                    hotBarFade -= .01f;
                }
            }

            if(game.inputState.pressed(Keys.Q)){
                inventoryFadingIn = !inventoryFadingIn;
            }
            if (inventoryFadingIn) {
                if (inventoryFade < 1f) {
                    inventoryFade += .05f;
                }
            } else {
                if (inventoryFade > 0f) {
                    inventoryFade -= .05f;
                } else {
                    dropItem(game, cursorItem);
                    cursorItem = new ItemStack(Item.ItemEmpty);
                }
            }

            inventoryMove = (int)(inventoryFade * ((gridSize * 3) + gridSize + 2));

            if (hotBarFade<0) {
                hotBarFade = 0;
            }

            if (inventoryFade < 0) {
                inventoryFade = 0;
            }

            float f = Math.Max(hotBarFade, inventoryFade);
            hotBarColor = new Color(f, f, f, f);
            inventoryColor = new Color(inventoryFade, inventoryFade, inventoryFade, inventoryFade);
            lastScrollValue = game.inputState.mouseState.ScrollWheelValue;

            #endregion

            mouseItemSlot = -1;
            mouseCraftSlot = -1;

            int itemIndex = 0;

            for (int i = 0; i < 10; i++) {
                xx = x + (selectedSlot == i ? 4 : 0) + inventoryMove + 4;
                yy = y + (i * gridSize);
                if (new Rectangle(xx, yy, gridSize, gridSize).Contains(game.inputState.mouseState.X, game.inputState.mouseState.Y)) {
                    mouseItemSlot = itemIndex;
                }
                itemIndex++;
            }
            
            if (inventoryFade > 0) {

                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 4; j++) {
                        xx = x + (j * gridSize) + inventoryMove - (gridSize * 4);
                        yy = y + (i * gridSize);
                        if (new Rectangle(xx, yy, gridSize, gridSize).Contains(game.inputState.mouseState.X, game.inputState.mouseState.Y)) {
                            mouseItemSlot = itemIndex;
                        }
                        itemIndex++;
                    }
                }

                for (int i = -2; i <= 2; i++) {
                    int j = i + craftingScroll;
                    if (j >= 0 && j < valaidRecipes.Count()) {
                        xx = x + (i * gridSize) + inventoryMove - (gridSize * 2);
                        yy = y + craftingAreaY;;
                        if (new Rectangle(xx, yy, gridSize, gridSize).Contains(game.inputState.mouseState.X, game.inputState.mouseState.Y)) {
                            mouseCraftSlot = j;
                        }
                    }
                }
            }

            bool isCursor = !cursorItem.isEmpty();
            ItemStack currentItem = isCursor ? cursorItem : items[selectedSlot];
            if (game.currentWorld != null) {
                mouseTileX = (game.inputState.mouseState.X + game.currentWorld.viewOffset.X) / World.tileSizeInPixels;
                mouseTileY = (game.inputState.mouseState.Y + game.currentWorld.viewOffset.Y) / World.tileSizeInPixels;
                mouseTileDistanceFromPlayer = (int)Vector2.Distance(new Vector2(mouseTileX * World.tileSizeInPixels, mouseTileY * World.tileSizeInPixels), game.currentWorld.player.position + (game.currentWorld.player.size / 2));
            }
            
            

            if (mouseItemSlot == -1 && mouseCraftSlot == -1) {

                if (game.inputState.mouseState.LeftButton == ButtonState.Pressed) {
                    setCurrentItem(currentItem.getItem().leftClick(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer), isCursor);
                }
                if (game.inputState.mouseState.RightButton == ButtonState.Pressed) {
                    setCurrentItem(currentItem.getItem().rightClick(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer), isCursor);
                }
                if(game.inputState.pressed(Keys.E)){
                    ItemStack useItem = currentItem;
                    Tile tileWall = game.currentWorld.getTileObject(mouseTileX, mouseTileY, true);
                    Tile tileTile = game.currentWorld.getTileObject(mouseTileX, mouseTileY, false);
                    if (tileWall.index != Tile.TileAir.index || tileTile.index != Tile.TileAir.index) {
                        useItem = (tileTile.index == Tile.TileAir.index ? tileWall : tileTile).use(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer);
                    }
                    if(useItem!=null){
                        setCurrentItem(useItem);
                    }
                }

            }

            currentItem.getItem().updateAfterClick(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer);

            bool leftClick = false;
            bool rightClick = false;

            if (game.inputState.mouseState.LeftButton == ButtonState.Pressed) {
                if (leftClickTimer == 0) {
                    leftClick = true;
                } else if (leftClickTimer > 60) {
                    if (leftClickTimer > 480) {
                        leftClick = true;
                    } else if (leftClickTimer > 240) {
                        if (leftClickTimer % 2 == 0) {
                            leftClick = true;
                        }
                    } else  if (leftClickTimer % 10 == 0) {
                        leftClick = true;
                    }
                }
                leftClickTimer++;
            } else {
                leftClickTimer = 0;
            }

            if (game.inputState.mouseState.RightButton == ButtonState.Pressed) {
                if (rightClickTimer == 0) {
                    rightClick = true;
                } else if (rightClickTimer > 60) {
                    if (rightClickTimer > 480) {
                        rightClick = true;
                    } else if (rightClickTimer > 240) {
                        if (rightClickTimer % 2 == 0) {
                            rightClick = true;
                        }
                    } else if (rightClickTimer % 10 == 0) {
                        rightClick = true;
                    }
                }
                rightClickTimer++;
            } else {
                rightClickTimer = 0;
            }

            if(inventoryFade == 0){
                if (mouseItemSlot >= 0 && mouseItemSlot < 10) {
                    hotBarFadingIn = true;

                    if (leftClick) {
                        selectedSlot = mouseItemSlot;
                    }
                }
            }else if (!(mouseItemSlot == -1 && mouseCraftSlot == -1)) {
                if (mouseItemSlot == -1 && mouseCraftSlot == craftingScroll) {
                    if (leftClick) {
                        if (cursorItem.sameItem(valaidRecipes[mouseCraftSlot].result)) {
                            if (cursorItem.getCount() + valaidRecipes[mouseCraftSlot].result.getCount() <= cursorItem.getItem().maxStack) {
                                cursorItem.setCount(cursorItem.getCount() + valaidRecipes[mouseCraftSlot].result.getCount());
                                removeItemStacks(0, 49, valaidRecipes[mouseCraftSlot].ingredients);
                            }
                        } else if (cursorItem.isEmpty()) {
                            cursorItem = valaidRecipes[mouseCraftSlot].result.clone();
                            removeItemStacks(0, 49, valaidRecipes[mouseCraftSlot].ingredients);
                        }
                        updateValaidRecipes();
                    }
                } else if (mouseItemSlot == -1 && (mouseCraftSlot >= 0)) {
                    if (leftClick) {
                        craftingScroll = mouseCraftSlot;
                    }
                } else {
                    if (leftClick) {
                        if (cursorItem.sameItem(items[mouseItemSlot])) {
                            if (items[mouseItemSlot].getCount() < cursorItem.getItem().maxStack && cursorItem.getCount() > 0) {
                                cursorItem.setCount(items[mouseItemSlot].setCount(items[mouseItemSlot].getCount() + cursorItem.getCount()));
                            } else {
                                ItemStack c = cursorItem.clone();
                                cursorItem = items[mouseItemSlot].clone();
                                items[mouseItemSlot] = c.clone();
                            }
                        } else {
                            ItemStack c = cursorItem.clone();
                            cursorItem = items[mouseItemSlot].clone();
                            items[mouseItemSlot] = c.clone();
                        }
                        updateValaidRecipes();
                    }
                    if (rightClick) {
                        if (cursorItem.isEmpty() && items[mouseItemSlot].isEmpty()) {

                        }else if (cursorItem.isEmpty()) {

                            double c = items[mouseItemSlot].getCount() / 2.0;
                            Game.updateMessage += c;
                            cursorItem = items[mouseItemSlot].clone();
                            cursorItem.setCount((int)Math.Ceiling(c));
                            items[mouseItemSlot].setCount((int)Math.Floor(c));

                        } else if (items[mouseItemSlot].isEmpty()) {

                            items[mouseItemSlot] = cursorItem.clone();
                            items[mouseItemSlot].setCount(1);
                            cursorItem.setCount(cursorItem.getCount() - 1);

                        } else {
                            if (cursorItem.sameItem(items[mouseItemSlot])) {
                                if (!items[mouseItemSlot].isMax()) {
                                    items[mouseItemSlot].setCount(items[mouseItemSlot].getCount()+1);
                                    cursorItem.setCount(cursorItem.getCount() - 1);
                                }
                            } else {
                                ItemStack c = cursorItem.clone();
                                cursorItem = items[mouseItemSlot].clone();
                                items[mouseItemSlot] = c.clone();
                            }
                        }
                        updateValaidRecipes();
                    }
                }
            }

            if(game.inputState.pressed(Keys.R)){
                game.currentWorld.EntityAddingList.Add(new EntityItem(game.currentWorld.player.position, new ItemStack(currentItem.getItem(), 1, currentItem.getData()), 60));
                currentItem.setCount(currentItem.getCount()-1);
                setCurrentItem(currentItem);
            }

            tick++;
        }

        private void removeItemStacks(int minSlot, int maxSlot, ItemStack[] stacks) {
            foreach(ItemStack stack in stacks){
                for (int i = minSlot; i <= maxSlot; i++) {
                    if (items[i].sameItem(stack)) {
                        if (items[i].getCount() >= stack.getCount()) {
                            items[i].setCount(items[i].getCount()-stack.getCount());
                        }
                    }
                }
            }
        }

        private void setCurrentItem(ItemStack itemStack) {
            if (cursorItem.isEmpty()) {
                items[selectedSlot] = itemStack;
            } else {
                cursorItem = itemStack;
            }
        }

        private void setCurrentItem(ItemStack itemStack, bool cursor) {
            if (cursor) {
                cursorItem = itemStack;
            } else {
                items[selectedSlot] = itemStack;
            }
        }

        public void dropItem(Game game, ItemStack item) {
            if (item.getItem().index != Item.ItemEmpty.index) {
                Entity e = new EntityItem(game.currentWorld.player.position, item, 40);
                game.currentWorld.EntityList.Add(e);
            }
        }

        public ItemStack pickUp(ItemStack stack) {
            for (int i = 0; i < items.Length;i++ ) {
                if(items[i].sameItem(stack)){
                    if (items[i].getCount() < stack.getItem().maxStack && stack.getCount() > 0) {
                        stack.setCount(items[i].setCount(items[i].getCount() + stack.getCount()));
                        updateValaidRecipes();
                        return stack;
                    }
                }
            }
            for (int i = 0; i < items.Length;i++ ) {
                if(items[i].isEmpty()){
                    items[i] = stack;
                    updateValaidRecipes();
                    return new ItemStack(Item.ItemEmpty);
                }
            }
            return stack;
        }
    }
}
