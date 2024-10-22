using System;
using System.Collections.Generic;
using Sys = Cosmos.System;

namespace flipflOS
{
    public class Kernel : Sys.Kernel
    {
        DateTime start;
        Memory mem = new Memory(); //initialisieren aller Variablen ofc
        public Directory currentdir;
        protected override void BeforeRun()
        {
            createRoot(); //erstellt für DateiSystem das Root verzeichnis, sowie weitere
            Console.WriteLine("Willkommen im FlipFlOS- nutze help für Hilfe!");
            start = DateTime.Now;
        }

        protected override void Run()
        {
            List<string> commandHistory = new List<string>();
            int historyIndex = -1;  //commandHistory für pfeiltasten-navigation
            string currentInput = "";

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green; //ändert Farbe zu Grün
                Console.Write(currentdir.getPath() + " : ");
                Console.ResetColor(); //textfarbe zurück zu schwarz

                currentInput = "";

                while (true)
                {
                    var key = Console.ReadKey(intercept: true); //input erkennung für mögliche Eingabe von Pfeil hoch, runter und Backspace
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            Console.WriteLine();
                            if (!string.IsNullOrWhiteSpace(currentInput))
                            {
                                commandHistory.Add(currentInput);
                                historyIndex = commandHistory.Count;
                            }
                            break;

                        case ConsoleKey.Backspace:
                            if (currentInput.Length > 0)
                            {
                                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                                ClearCurrentLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(currentdir.getPath());
                                Console.ResetColor();
                                Console.Write(" : " + currentInput);
                            }
                            break;

                        case ConsoleKey.UpArrow: //schreibt letzten command 
                            if (historyIndex > 0)
                            { 
                                historyIndex--;
                                currentInput = commandHistory[historyIndex]; // Fetch command from history
                                ClearCurrentLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(currentdir.getPath());
                                Console.ResetColor();
                                Console.Write(" : " + currentInput);
                            }
                            break;

                        case ConsoleKey.DownArrow: //schreibt nächsten command
                            if (historyIndex < commandHistory.Count - 1)
                            {
                                historyIndex++;
                                currentInput = commandHistory[historyIndex]; // Fetch next command from history
                                ClearCurrentLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(currentdir.getPath());
                                Console.ResetColor();
                                Console.Write(" : " + currentInput);
                            }
                            else
                            {
                                currentInput = ""; // No more commands, clear input
                                ClearCurrentLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(currentdir.getPath() + " : ");
                                Console.ResetColor();
                            }
                            break;

                        default:
                            currentInput += key.KeyChar;
                            Console.Write(key.KeyChar);
                            break;
                    }

                    if (key.Key == ConsoleKey.Enter) // End the loop after an "Enter" press
                    {
                        break;
                    }
                }


                string[] args = currentInput.Split(' ');
                ClearCurrentLine();
                if (args.Length>=2 && args[1] == "-help" && CommandManager.getCommand(args[0])!=null)
                {
                    Console.WriteLine(CommandManager.getCommand(args[0]).usage);
                    Console.WriteLine("\t"+CommandManager.getCommand(args[0]).info);
                    ClearCurrentLine();
                }
                else
                {
                    switch (args[0]) //commands werden überprüft
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
                        changeDirIterative(args);
                        break;
                    case "gcd":
                        printCurrentDir();
                        break;
                    case "ls":
                        printAllFilesAndDirs();
                        break;
                    case "mkdir":
                        currentdir.createSubDirectory(args[1]);
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
                // Loop through both arrays and print them with appropriate colors
                for (int i = 0; i < CommandManager.commands.Length; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(CommandManager.commands[i].usage);

                    Console.ResetColor();
                    Console.WriteLine("\t" + CommandManager.commands[i].info);
                }
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
        public static void ClearCurrentLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, currentLineCursor);  
        }

        public void changeDirIterative(String[] args)
        {
            if (args.Length == 1)
            {
                Console.WriteLine("not enough arguments.");
                return;
            }
            String[] directories = args[1].Split("/");
            foreach (String directory in directories)
            {
                changeDir(directory);
            }
        }
        public void changeDir(String directory)
        {
            if (directory == "..")
            {
                if (currentdir.parent != null)
                {
                    currentdir = currentdir.parent;
                }
            }
            foreach (var dir in currentdir.subdirectories)
            {
                if(dir.name == directory)
                {
                    currentdir = dir;
                    return;
                }
            }

        }
        public void printCurrentDir()
        {
            Console.WriteLine("  "+currentdir.name);
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
                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var dir in currentdir.subdirectories)
                {
                    Console.WriteLine($"  {dir.name}");
                }
            }

            if (currentdir.files != null)
            {
                Console.ForegroundColor = ConsoleColor.Blue;

                foreach (var file in currentdir.files)
                {
                    Console.WriteLine($"  {file.name}");
                }
            }
            Console.ForegroundColor = ConsoleColor.Black;

        }
    }
}

