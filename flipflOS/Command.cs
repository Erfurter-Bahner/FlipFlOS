using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flipflOS
{
    public class CommandManager
    {
        public static Command[] commands = new Command[]
        {
            new Command("time", "time", "for getting current runtime"),
            new Command("help", "help", "for getting help :D"),
            new Command("write", "write [index] [byte]", "Writing data"),
            new Command("read", "read [index]", "reading data"),
            new Command("cd", "cd [directory]", "changes directory to chosen directory. `cd ..` for parent"),
            new Command("gcd", "gcd", "prints current path of directory"),
            new Command("ls", "ls", "prints list of all elements in current directory"),
            new Command("mkdir", "mkdir [directory]", "creates subdirectory in current directory."),
            new Command("touch", "touch [file]", "creates File"),
            new Command("writeFile", "writeFile [file] [content]", "writes Strings into destined File"),
            new Command("readFile", "readFile [file]", "prints content of file"),
            new Command("commands", "commands", "list all available commands"),
            new Command("commands+","commands+","list all available commands with usage"),
            new Command("clear","clear","clears whole Terminal"),
            new Command("loadingScreen","loadingScreen [seconds]","shows the Loading Screen for given seconds")
        };
        public static Command getCommand(String name)
        {
            foreach(var Command in commands){
                if (Command.name == name) return Command;
            }
            return null;
        }
    }
    public class Command
    {
        public string name;
        public String usage;
        public String info;
        public Command(String name, String usage, String info)
        {
            this.name = name;
            this.usage = usage;
            this.info = info;
        }
    }
}
