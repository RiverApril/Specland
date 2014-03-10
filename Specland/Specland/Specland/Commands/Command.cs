using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Specland {
    public abstract class Command {

        public static List<Command> commands = new List<Command>();

        public static CommandHelp commandHelp = new CommandHelp("help");
        public static CommandGive commandGive = new CommandGive("give");
        public static CommandWorld commandWorld = new CommandWorld("world");

        public string name;

        public static Command getCommand(string n) {
            foreach(Command c in commands){
                if(c.name.Equals(n)){
                    return c;
                }
            }
            throw new NoSuchCommandException(n);
        }

        public Command(string name) {
            this.name = name;
            commands.Add(this);
        }

        public int toInt(string s) {
            try {
                return Convert.ToInt16(s);
            } catch (FormatException) {
                throw new NumberInvalidException(s);
            }
        }

        public bool equalsIgnoreCase(string s1, string s2) {
            return s1.ToLower().Equals(s2.ToLower());
        }

        public abstract bool processCommand(IStringPrinter printer, string[] options);

        public abstract string getHelp();

        public abstract string getUsage();
    }
}
