using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public class CommandHelp : Command{

        public CommandHelp(string name) : base(name){
            
        }

        public override bool processCommand(IStringPrinter printer, string[] options) {
            if (options.Length == 0) {
                printer.println("Commands: ");
                foreach(Command c in commands){
                    printer.println("  " + c.name + " - " + c.getHelp());
                }
                return true;
            } else if (options.Length == 1) {
                foreach (Command c in commands) {
                    if (c.name.Equals(options[0])) {
                        printer.println(c.getUsage() + " - " + c.getHelp());
                        return true;
                    }
                }
                throw new NoSuchCommandException(options[0]);
            } else {
                throw new InvalidNumberOfArgumentsException(options.Length);
            }
        }

        public override string getHelp() {
            return "Gets help on commands.";
        }

        public override string getUsage() {
            return "/help [command]";
        }

    }
}
