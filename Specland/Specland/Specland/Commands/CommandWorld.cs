using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class CommandWorld : Command {

        public CommandWorld(string name)
            : base(name) {

        }

        public override bool processCommand(IStringPrinter printer, string[] options) {
            if (options.Length == 2 || options.Length == 3) {
                options[0] = options[0].ToLower();

                bool forceOverride = false;

                if (options.Length == 3) {
                    options[2] = options[2].ToLower();
                    if (options[2].Equals("overwrite") || options[2].Equals("o")) {
                        forceOverride = true;
                    } else {
                        throw new InvalidArgumentException(options[2], " \"overwrite\" or \"o\".");
                    }

                }

                if(options[0].Equals("save") || options[0].Equals("s")){
                    if (World.save(Game.instance.currentWorld, options[1], forceOverride)) {
                        printer.println((forceOverride ? "World overwritten: " : "World saved as: ") + options[1]);
                    } else {
                        printer.println("Save already exists: \"" + options[1] + "\" please use: /world save " + options[1] + " overwrite");
                    }
                } else if (options[0].Equals("load") || options[0].Equals("l")) {
                    Game.instance.currentWorld = null;
                    Game.instance.currentWorld = World.load(options[1]);
                    printer.println("World loaded: " + options[1]);
                }

                return true;
            } else {
                throw new InvalidNumberOfArgumentsException(options.Length);
            }
        }

        public override string getHelp() {
            return "Saves or Loads a world.";
        }

        public override string getUsage() {
            return "/world <save | load> <filename> [overwrite]";
        }

    }
}
