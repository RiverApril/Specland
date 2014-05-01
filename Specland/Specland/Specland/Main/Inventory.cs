using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Specland {
    public class Inventory : Gui {

        private static int x = 4;
        private static int y = 24;

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

        ItemStack cursorItem = new ItemStack(Item.itemEmpty);

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

        private static int craftingAreaY = (int)y+(11 * gridSize)-8;

        public ItemStack currentItem;

        public int t = 0;
        private bool mouseInCrafting;

        public Inventory(){
            for (int i = 0; i < items.Length; i++) {
                items[i] = new ItemStack(Item.itemEmpty);
            }
            pickUp(new ItemStack(Item.itemSupick));
            pickUp(new ItemStack(Item.itemWoodPick));
            pickUp(new ItemStack(Item.itemStonePick));
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

            if (game.currentWorld.player == null) {
                return;
            }

            int xx = 0;
            int yy = 0;
            int itemIndex = 0;
            Rectangle r;


            if (currentItem!=null) {
               Game.drawString(currentItem.getDisplayName(false), new Vector2(8, 4), hotBarColor, Game.RENDER_DEPTH_GUI_TEXT);
           }

            for(int i=0; i < 10; i++){
                xx = x + (selectedSlot == i ? (cursorItem.isEmpty() ? 4 : 2) : 0) + inventoryMove + 4;
                yy = y + (i * gridSize);
                r = new Rectangle(xx + squareBorderSize, yy + squareBorderSize, itemSize, itemSize);
                Game.drawRectangle(guiTexture, new Rectangle(xx, yy, squareSize, squareSize), new Rectangle(0, 0, 20, 20), hotBarColor, Game.RENDER_DEPTH_GUI_IMAGE_BG);
                items[itemIndex].draw(game, r, hotBarColor, Game.RENDER_DEPTH_GUI_IMAGE_FG, Game.RENDER_DEPTH_GUI_TEXT);
                Game.drawString((i == 9 ? 0 : i + 1) + "", new Vector2(xx + 22, yy + 3), hotBarColor, Game.RENDER_DEPTH_GUI_TEXT);
                
                itemIndex++;
            }

            if (inventoryFade>0) {
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 4; j++) {
                        xx = x + (j * gridSize) + inventoryMove - (gridSize * 4);
                        yy = y + (i * gridSize);
                        r = new Rectangle(xx + squareBorderSize, yy + squareBorderSize, itemSize, itemSize);
                        Game.drawRectangle(guiTexture, new Rectangle(xx, yy, squareSize, squareSize), new Rectangle(0, 0, 20, 20), inventoryColor, Game.RENDER_DEPTH_GUI_IMAGE_BG);
                        items[itemIndex].draw(game, r, inventoryColor, Game.RENDER_DEPTH_GUI_IMAGE_FG, Game.RENDER_DEPTH_GUI_TEXT);

                        itemIndex++;
                    }
                }
                
                float a = inventoryFade * (mouseInCrafting ? 1 : .5f);
                Color color = new Color(a, a, a, a);
                Game.drawString(" Crafting", new Vector2(inventoryMove - (gridSize * 4), craftingAreaY - gridSize*.5f), color, Game.RENDER_DEPTH_GUI_TEXT);

                for (int i = -2; i <= 2; i++) {
                    int j = i + craftingScroll;
                    if (j >= 0 && j < valaidRecipes.Count()) {
                        xx = x + (i * gridSize) + inventoryMove - (gridSize * 2);
                        yy = y + craftingAreaY;
                        r = new Rectangle(xx + squareBorderSize, yy + squareBorderSize, itemSize, itemSize);
                        int abs = Math.Abs(i);
                        a = (inventoryFade * (abs == 3 ? .1f : (abs == 2 ? .2f : (abs == 1 ? .5f : 1)))) * (mouseInCrafting ? 1 : .1f);
                        color = new Color(a, a, a, a);
                        Game.drawRectangle(guiTexture, new Rectangle(xx, yy, squareSize, squareSize), new Rectangle(0, 0, 20, 20), color, Game.RENDER_DEPTH_GUI_IMAGE_BG);
                        valaidRecipes[j].result.draw(game, r, color, Game.RENDER_DEPTH_GUI_IMAGE_FG, Game.RENDER_DEPTH_GUI_TEXT);
                        for (int k = 0; k < valaidRecipes[j].ingredients.Length; k++) {
                            xx = x + (i * gridSize) + inventoryMove - (gridSize * 2);
                            yy = y + 4 + ((k + 1) * gridSize) + craftingAreaY;
                            r = new Rectangle(xx + squareBorderSize, yy + squareBorderSize, itemSize, itemSize);
                            valaidRecipes[j].ingredients[k].draw(game, r, color, Game.RENDER_DEPTH_GUI_IMAGE_FG, Game.RENDER_DEPTH_GUI_TEXT);
                        }
                    }
                }

            }

            MouseState mouseState = Mouse.GetState();

            xx = mouseState.X + 10;
            yy = mouseState.Y + 10;
            r = new Rectangle(xx + 2, yy + 2, itemSize, itemSize);

            if (inventoryFade > 0 && !cursorItem.isEmpty()) {
                cursorItem.draw(game, r, inventoryColor, Game.RENDER_DEPTH_GUI_CURSOR_IMAGE_FG, Game.RENDER_DEPTH_GUI_CURSOR_TEXT);
                Game.drawString(cursorItem.getDisplayName(true), new Vector2(r.X + 24, r.Y), inventoryColor, Game.RENDER_DEPTH_GUI_CURSOR_TEXT);
            } else if (inventoryFade > 0 && mouseItemSlot != -1 && !items[mouseItemSlot].isEmpty()) {
                Game.drawString(items[mouseItemSlot].getDisplayName(true), new Vector2(xx + 2, yy + 2), inventoryColor, Game.RENDER_DEPTH_GUI_CURSOR_TEXT);
            } else if (inventoryFade > 0 && !items[selectedSlot].isEmpty()) {
                items[selectedSlot].draw(game, r, (mouseTileDistanceFromPlayer <= items[selectedSlot].getItem().reach) ? Color.White : new Color(.1f, .1f, .1f, .1f), Game.RENDER_DEPTH_GUI_CURSOR_IMAGE_FG, Game.RENDER_DEPTH_GUI_TEXT);
            }

            currentItem = cursorItem.isEmpty() ? items[selectedSlot] : cursorItem;

            if ((mouseTileDistanceFromPlayer <= currentItem.getItem().reach)) {
                currentItem.getItem().drawHover(game, mouseTileX, mouseTileY, currentItem);
            }

            Game.drawRectangle(guiTexture, new Rectangle(mouseState.X, mouseState.Y, 16, 16), new Rectangle(0, 20, 16, 16), Game.cursorColor, Game.RENDER_DEPTH_GUI_CURSOR_IMAGE_BG);
        }

        private bool mouseInCraftingArea(InputState inputState) {
            int xx = x + (-2 * gridSize) + inventoryMove - (gridSize * 2);
            int yy = y + craftingAreaY-22;
            return new Rectangle(xx, yy, gridSize * 5, gridSize * 4).Contains(inputState.mouseX(), inputState.mouseY());
        }


        public override InputState update(Game game, InputState inputState) {

            mouseInCrafting = mouseInCraftingArea(inputState);

            if(game.currentWorld.player==null){
                return inputState;
            }

            #region Scrolling and Fading

            int xx = 0;
            int yy = 0;

            int scrollDif = inputState.getScrollWheelValue() - lastScrollValue;

            if (scrollDif != 0) {

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

            if(inputState.pressed(Game.KEY_INV)){
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
                    cursorItem = new ItemStack(Item.itemEmpty);
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
            lastScrollValue = inputState.getScrollWheelValue();

            if(inputState.pressed(Game.KEY_D1)){
                selectedSlot = 0;
            } else if (inputState.pressed(Game.KEY_D2)) {
                selectedSlot = 1;
            } else if (inputState.pressed(Game.KEY_D3)) {
                selectedSlot = 2;
            } else if (inputState.pressed(Game.KEY_D4)) {
                selectedSlot = 3;
            } else if (inputState.pressed(Game.KEY_D5)) {
                selectedSlot = 4;
            } else if (inputState.pressed(Game.KEY_D6)) {
                selectedSlot = 5;
            } else if (inputState.pressed(Game.KEY_D7)) {
                selectedSlot = 6;
            } else if (inputState.pressed(Game.KEY_D8)) {
                selectedSlot = 7;
            } else if (inputState.pressed(Game.KEY_D9)) {
                selectedSlot = 8;
            } else if (inputState.pressed(Game.KEY_D0)) {
                selectedSlot = 9;
            }


            #endregion

            mouseItemSlot = -1;
            mouseCraftSlot = -1;

            int itemIndex = 0;

            for (int i = 0; i < 10; i++) {
                xx = x + (selectedSlot == i ? 4 : 0) + inventoryMove + 4;
                yy = y + (i * gridSize);
                if (new Rectangle(xx, yy, gridSize, gridSize).Contains(inputState.mouseX(), inputState.mouseY())) {
                    mouseItemSlot = itemIndex;
                }
                itemIndex++;
            }
            
            if (inventoryFade > 0) {

                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 4; j++) {
                        xx = x + (j * gridSize) + inventoryMove - (gridSize * 4);
                        yy = y + (i * gridSize);
                        if (new Rectangle(xx, yy, gridSize, gridSize).Contains(inputState.mouseX(), inputState.mouseY())) {
                            mouseItemSlot = itemIndex;
                        }
                        itemIndex++;
                    }
                }

                for (int i = -2; i <= 2; i++) {
                    int j = i + craftingScroll;
                    if (j >= 0 && j < valaidRecipes.Count()) {
                        xx = x + (i * gridSize) + inventoryMove - (gridSize * 2);
                        yy = y + craftingAreaY;
                        if (new Rectangle(xx, yy, gridSize, gridSize).Contains(inputState.mouseX(), inputState.mouseY())) {
                            mouseCraftSlot = j;
                        }
                    }
                }
            }

            bool isCursor = !cursorItem.isEmpty();
            ItemStack currentItem = isCursor ? cursorItem : items[selectedSlot];
            if (game.currentWorld != null) {
                mouseTileX = (inputState.mouseX() + game.currentWorld.viewOffset.X) / World.tileSizeInPixels;
                mouseTileY = (inputState.mouseY() + game.currentWorld.viewOffset.Y) / World.tileSizeInPixels;
                mouseTileDistanceFromPlayer = (int)Vector2.Distance(new Vector2(mouseTileX * World.tileSizeInPixels, mouseTileY * World.tileSizeInPixels), game.currentWorld.player.position + (game.currentWorld.player.size / 2));
            }
            
            

            if (mouseItemSlot == -1 && mouseCraftSlot == -1) {

                if (inputState.downMouse(InputState.Left)) {
                    setCurrentItem(currentItem.getItem().leftClick(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer), isCursor);
                }
                if (inputState.downMouse(InputState.Right)) {
                    setCurrentItem(currentItem.getItem().rightClick(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer), isCursor);
                }
                if(inputState.pressed(Game.KEY_USE)){
                    ItemStack useItem = currentItem;
                    Tile tileWall = game.currentWorld.getTileObject(mouseTileX, mouseTileY, true);
                    Tile tileTile = game.currentWorld.getTileObject(mouseTileX, mouseTileY, false);
                    if (tileWall.index != Tile.TileAir.index || tileTile.index != Tile.TileAir.index) {
                        useItem = (tileTile.index == Tile.TileAir.index ? tileWall : tileTile).use(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer, tileTile.index == Tile.TileAir.index);
                    }
                    if(useItem!=null){
                        setCurrentItem(useItem);
                    }
                }

            }

            currentItem.getItem().updateAfterClick(game, currentItem, mouseTileX, mouseTileY, mouseTileDistanceFromPlayer);

            bool leftClick = false;
            bool rightClick = false;

            int speed1Time = 40;
            int speed2Time = 200;
            int speed3Time = 400;

            if (inputState.downMouse(InputState.Left)) {
                if (leftClickTimer == 0) {
                    leftClick = true;
                } else if (leftClickTimer > speed1Time) {
                    if (leftClickTimer > speed3Time) {
                        leftClick = true;
                    } else if (leftClickTimer > speed2Time) {
                        if (leftClickTimer % 2 == 0) {
                            leftClick = true;
                        }
                    } else if (leftClickTimer % 10 == 0) {
                        leftClick = true;
                    }
                }
                leftClickTimer++;
            } else {
                leftClickTimer = 0;
            }

            if (inputState.downMouse(InputState.Right)) {
                if (rightClickTimer == 0) {
                    rightClick = true;
                } else if (rightClickTimer > speed1Time) {
                    if (rightClickTimer > speed3Time) {
                        rightClick = true;
                    } else if (rightClickTimer > speed2Time) {
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
                        if (inputState.getKeyboardState().IsKeyDown(Game.KEY_INV_MOVE_ITEM_TO_OTHER)) {
                            removeItemStacks(0, 49, valaidRecipes[mouseCraftSlot].ingredients);
                            pickUp(valaidRecipes[mouseCraftSlot].result.clone());
                        } else {
                            if (cursorItem.sameItem(valaidRecipes[mouseCraftSlot].result)) {
                                if (cursorItem.getCount() + valaidRecipes[mouseCraftSlot].result.getCount() <= cursorItem.getItem().maxStack) {
                                    cursorItem.setCount(cursorItem.getCount() + valaidRecipes[mouseCraftSlot].result.getCount());
                                    removeItemStacks(0, 49, valaidRecipes[mouseCraftSlot].ingredients);
                                }
                            } else if (cursorItem.isEmpty()) {
                                cursorItem = valaidRecipes[mouseCraftSlot].result.clone();
                                removeItemStacks(0, 49, valaidRecipes[mouseCraftSlot].ingredients);
                            }
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

            if(inputState.pressed(Game.KEY_DROP)){
                game.currentWorld.EntityAddingList.Add(new EntityItem(game.currentWorld.player.position, new ItemStack(currentItem.getItem(), 1, currentItem.getData()), 60));
                currentItem.setCount(currentItem.getCount()-1);
                setCurrentItem(currentItem);
            }

            tick++;
            inputState.eatMouse();
            inputState.eatKey(Game.KEY_INV);
            inputState.eatKey(Game.KEY_INV_MOVE_ITEM_TO_OTHER);
            inputState.eatKey(Game.KEY_DROP);
            inputState.eatKey(Game.KEY_D0);
            inputState.eatKey(Game.KEY_D1);
            inputState.eatKey(Game.KEY_D2);
            inputState.eatKey(Game.KEY_D3);
            inputState.eatKey(Game.KEY_D4);
            inputState.eatKey(Game.KEY_D5);
            inputState.eatKey(Game.KEY_D6);
            inputState.eatKey(Game.KEY_D7);
            inputState.eatKey(Game.KEY_D8);
            inputState.eatKey(Game.KEY_D9);
            return inputState;
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
            if (item.getItem().index != Item.itemEmpty.index) {
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
                    return new ItemStack(Item.itemEmpty);
                }
            }
            return stack;
        }

        internal void saveTo(List<byte> bytes) {
            for (int i = 0; i < items.Length; i++) {
                items[i].saveTo(bytes);
            }
        }

        internal int loadFrom(byte[] bytes, int index) {
            for (int i = 0; i < items.Length;i++ ) {
                index = items[i].loadFrom(bytes, index);
            }
            return index;
        }

        internal int applyMiningPowerModifier(int power) {
            return power;
        }

        internal int applyMiningDelayModifier(int delay) {
            return delay;
        }
    }
}
