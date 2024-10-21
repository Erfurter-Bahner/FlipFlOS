using System;
using System.Collections.Generic;
using Sys = Cosmos.System;

namespace flipflOS
{
    public class Kernel : Sys.Kernel
    {
        DateTime start;
        Memory mem = new Memory();
        public Directory currentdir;
        protected override void BeforeRun()
        {
            createRoot();
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
            start = DateTime.Now;

        }

        protected override void Run()
        {
            List<string> commandHistory = new List<string>();
            int historyIndex = -1;
            string currentInput = "";

            while (true)
            {
                Console.Write(currentdir.getPath() + " : ");
                currentInput = "";

                while (true)
                {
                    var key = Console.ReadKey(intercept: true);

                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        if (!string.IsNullOrWhiteSpace(currentInput))
                        {
                            commandHistory.Add(currentInput);
                            historyIndex = commandHistory.Count; // Index to end
                        }
                        break;
                    }
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        if (currentInput.Length > 0)
                        {
                            currentInput = currentInput.Substring(0, currentInput.Length - 1);
                            ClearCurrentLine();
                            Console.Write(currentdir.getPath() + " : " + currentInput);
                        }
                    }
                    else if (key.Key == ConsoleKey.UpArrow)
                    {
                        if (historyIndex > 0)
                        {
                            historyIndex--;
                            currentInput = commandHistory[historyIndex]; // Fetch command from history
                            ClearCurrentLine();
                            Console.Write(currentdir.getPath() + " : " + currentInput);
                        }
                    }
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        if (historyIndex < commandHistory.Count - 1)
                        {
                            historyIndex++;
                            currentInput = commandHistory[historyIndex]; // Fetch next command from history
                            ClearCurrentLine();
                            Console.Write(currentdir.getPath() + " : " + currentInput);
                        }
                        else
                        {
                            currentInput = ""; // No more commands, clear input
                            ClearCurrentLine();
                            Console.Write(currentdir.getPath() + " : ");
                        }
                    }
                    else
                    {
                        currentInput += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }

                string[] args = currentInput.Split(' ');
                switch (args[0])
                {
                    case "help":
                        help(args);
                        break;
                    case "time":
                        TimeSpan runtime = DateTime.Now - start;
                        Console.WriteLine("running for: " + runtime.TotalSeconds + " seconds");
                        break;
                    case "write":
                        writeToMemory(args);
                        break;
                    case "read":
                        Console.WriteLine(readFromMemory(args));
                        break;
                    case "cd":
                        changeDir(args);
                        break;
                    case "gcd":
                        printCurrentDir();
                        break;
                    case "ls":
                        printAllFilesAndDirs();
                        break;
                    case "mkdir":
                        currentdir.createSubDirectory(args[1]);
                        ClearCurrentLine();  // Make sure to clear the line after mkdir
                        break;
                    case "touch":
                        currentdir.createFile(args[1]);
                        break;
                    case "writeFile":
                        writeToFile(args);
                        break;
                    case "readFile":
                        readFile(args[1]);
                        break;
                    default:
                        Console.WriteLine("command not known. Please use 'help' for help.");
                        break;
                }

                // Ensure the prompt is reset after every command execution
                ClearCurrentLine();
            }
        }


        public void help(String[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("need help with: " + args[1]);
            }
            else
            {
                Console.WriteLine("time" +
                    "\n\tfor getting current runtime" +
                    "\nhelp" +
                    "\n\tfor getting help :D" +
                    "\nwrite [index] [byte]" +
                    "\n\tWriting data" +
                    "\nread [index]" +
                    "\n\treading data");

            }
        }
        public void writeToMemory(String[] args)
        {
            if (args.Length <= 2)
            {
                Console.WriteLine("Not enough Arguments. Syntax: write index data");
                return;
            }
            uint index = Convert.ToUInt32(args[1]) * 2;
            byte data = Convert.ToByte(args[2]);
            mem.writeAt(index, data);
        }
        public ushort readFromMemory(String[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Not enough Arguments. Syntax: read index");
                return 0;
            }
            ushort index = Convert.ToUInt16(args[1]);

            return mem.readAt(index);
        }
        public void createRoot()
        {
            currentdir = new Directory(null, null, "root");
            currentdir.createSubDirectory("home");
            currentdir.createSubDirectory("users");
            currentdir.createSubDirectory("var");
        }
        public void ClearCurrentLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new string(' ', Console.WindowWidth)); // Clear the line
            Console.SetCursorPosition(0, currentLineCursor);    // Reset cursor to the beginning
        }

        public void changeDir(String[] args)
        {
            if (args.Length == 1)
            {
                Console.WriteLine("not enough arguments.");
                return;
            }
            foreach (var dir in currentdir.subdirectories)
            {
                if(dir.name == args[1])
                {
                    currentdir = dir;
                    return;
                }
            }
            if (args[1] == "..")
            {
                if(currentdir.parent != null)
                {
                    currentdir = currentdir.parent;
                }
            }
        }
        public void printCurrentDir()
        {
            Console.WriteLine(currentdir.name);
        }
        public void writeToFile(String[] args)
        {
            if (args.Length == 1) return;
            String content = "";
            for (int i = 2; i < args.Length; i++)
            {

                content += args[i]+" ";
            }
            currentdir.getFile(args[1]).changecontent(content);
        }
        public void readFile(String filename)
        {
            Console.WriteLine(currentdir.getFile(filename).name+": "+currentdir.getFile(filename).content);
        }
        public void printAllFilesAndDirs()
        {
            if (currentdir.subdirectories != null)
            {
                foreach (var dir in currentdir.subdirectories)
                {
                    Console.WriteLine($"{dir.name}");
                }
            }

            if (currentdir.files != null)
            {
                foreach (var file in currentdir.files)
                {
                    Console.WriteLine($"{file.name}");
                }
            }

        }
    }
}

