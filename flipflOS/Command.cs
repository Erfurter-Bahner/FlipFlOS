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
            new Command("cd", "cd [directory]", "changes directory to chosen directory. `cd ..` for parent"),
            new Command("gcd", "gcd", "prints current path of directory"),
            new Command("ls", "ls", "prints list of all elements in current directory"),
            new Command("mkdir", "mkdir [directory]", "creates subdirectory in current directory."),
            new Command("removeDir","removeDir [directory]","removed Empty Directory"),
            new Command("touch", "touch [file or dir with File]", "creates File"),
            new Command("readFile", "readFile [file or dir with File]", "prints content of file"),
            new Command("removeFile","removeFile [file or dir with File]","removes File in current or other directory"),
            new Command("moveFile","moveFile [file or dir with File] [destination]","moves File to destined directory"),
            new Command("copyFile","copyFle [file or dir with File] [destination]","copies File to destined directory"),
            new Command("edit","edit [file or dir with File]","starts file editor for the file"),
            new Command("rename","rename [file or dir with File] [newname]","renames file, while keeping the input"),
            new Command("commands", "commands", "list all available commands"),
            new Command("commands+","commands+","list all available commands with usage"),
            new Command("clear","clear","clears whole Terminal"),
            new Command("loadingScreen","loadingScreen [seconds]","shows the Loading Screen for given seconds"),
            new Command("colorchange","colorchange","changes colormode, whitemode is ugly tho")
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
