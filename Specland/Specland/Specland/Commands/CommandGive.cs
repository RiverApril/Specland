using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class CommandGive : Command {

        public CommandGive(string name)
            : base(name) {

        }

        public override bool processCommand(IStringPrinter printer, string[] options) {
            if (options.Length == 1 || options.Length == 2 || options.Length == 3) {
                int id = 0;
                int count = 1;
                int data = 0;

                try {
                    id = toInt(options[0]);
                } catch (NumberInvalidException e) {
                    foreach(Item item in Item.Itemlist){
                        if(item != null){
                            if (equalsIgnoreCase(item.name, e.s)) {
                                id = item.index;
                                break;
                            }
                        }
                    }
                    if(id == 0){
                        throw new NoSuchItemException(e.s);
                    }
                }

                if(options.Length >= 2){
                    count = toInt(options[1]);
                }

                if (options.Length == 3) {
                    try {
                        data = toInt(options[2]);
                    } catch (NumberInvalidException e) {
                        if (id == Item.ItemTile.index) {
                            foreach (Tile tile in Tile.TileList) {
                                if (tile != null) {
                                    if (equalsIgnoreCase(tile.name, e.s)) {
                                        data = tile.index;
                                        break;
                                    }
                                }
                            }
                            if (data == 0) {
                                throw new NoSuchTileException(e.s);
                            }
                        } else {
                            throw e;
                        }
                    }
                }

                if(id != Item.ItemEmpty.index){
                    ItemStack stack = new ItemStack(Item.Itemlist[id], count, data);
                    printer.println("Gave you "+stack.getDisplayName());
                    Game.instance.inventory.pickUp(stack);
                }

                return true;
            } else {
                throw new InvalidNumberOfArgumentsException(options.Length);
            }
        }

        public override string getHelp() {
            return "Gives you an Item";
        }

        public override string getUsage() {
            return "/give <id> [count] [data]";
        }

    }
}
