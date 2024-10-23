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
        String[] logo =
        {
            "--------------------------------",
            "OOO O   OOO OOO OOO O    OOO OOO",
            "O   O    O  O O O   O    O O O  ",
            "OOO O    O  OOO OOO O    O O OOO",
            "O   O    O  O   O   O    O O   O",
            "O   OOO OOO O   O   OOO  OOO OOO",
            "--------------------------------",
        };
        protected override void BeforeRun()
        {
            createRoot(); //erstellt für DateiSystem das Root verzeichnis, sowie weitere
            Console.Clear();
            loadingScreen(5);
            Console.Clear();
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
                {                                   // diese abfrage guckt, ob nach dem command das -help attribut steht, 
                                                    // um statt dem befehl die informationen dazu auszugeben
                    Console.WriteLine(CommandManager.getCommand(args[0]).usage);
                    Console.WriteLine("\t"+CommandManager.getCommand(args[0]).info);
                    ClearCurrentLine();
                }
                else
                {
                    switch (args[0]) // switch case der möglichen commands
                    {
                    case "help":
                        help(args);
                        break;
                    case "time":
                        TimeSpan runtime = DateTime.Now - start;
                        Console.WriteLine("running for: " + runtime.TotalSeconds + " seconds"); // gibt sekunden seit systemstart aus
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
                    case "clear":
                            Console.Clear();
                        break;
                        case "loadingScreen":
                            if (args.Length == 1) break;
                            loadingScreen(int.Parse(args[1]));
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
                //schleife durch array aller commands, mit angabe der usage und info, und verwendung von Farbe
                for (int i = 0; i < CommandManager.commands.Length; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta; //magenta für die usage
                    Console.WriteLine(CommandManager.commands[i].usage);

                    Console.ResetColor(); //weiß für die beschreibung
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
        { //erstellt alle nötigen verzeichnisse für das dateisystem
            currentdir = new Directory(null, null, "root");
            currentdir.createSubDirectory("home");
            currentdir.createSubDirectory("users");
            currentdir.createSubDirectory("var");
        }
        public static void ClearCurrentLine()
        { //stellt sicher, dass während der navigation mit pfeiltasten keine neue Zeile begonnen wird.
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, currentLineCursor);  
        }

        public void changeDirIterative(String[] args)
        {
            if (args.Length == 1)
            {
                Console.WriteLine("not enough arguments."); //wenn nur "cd" geschrieben wird, argslength = 1, deswegen funktionsabbruch
                return;
            }
            String[] directories = args[1].Split("/");  //string wird in alle subdirectories aufgeteilt, welche durch "/" getrennt werden.
            foreach (String directory in directories)
            {
                changeDir(directory); // abarbeitung der directories von links nach rechts
            }
        }
        public void changeDir(String directory)
        {
            if (directory == "..")
            {
                if (currentdir.parent != null)
                {
                    currentdir = currentdir.parent; //wenn .. dann ins parentdir
                }
            }
            foreach (var dir in currentdir.subdirectories)
            {
                if(dir.name == directory)
                {
                    currentdir = dir; //sucht nach dem subdirectory und setzt pointer von currentdir auf ihn.
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
            for (int i = 2; i < args.Length; i++) //nimmt alle argumente nach "writeFile [file]" und schreibt sie in die datei hinein.
            {
                content += args[i]+" ";
            }
            currentdir.getFile(args[1]).changecontent(content);
        }
        public void readFile(String filename)
        { //liest den dateiinhalt und gibt ihn aus
            Console.WriteLine(currentdir.getFile(filename).name+": "+currentdir.getFile(filename).content);
        }
        public void printAllFilesAndDirs()
        {

            if (currentdir.subdirectories != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var dir in currentdir.subdirectories) //iteriert durch alle subdirs und printet sie blau
                {
                    Console.WriteLine($"  {dir.name}");
                }
            }

            if (currentdir.files != null)
            {
                Console.ForegroundColor = ConsoleColor.Blue;

                foreach (var file in currentdir.files) //iteriert durch alle dateiein im directory und printet sie blau
                {
                    Console.WriteLine($"  {file.name}");
                }
            }
            Console.ForegroundColor = ConsoleColor.Black;

        }
        public void loadingScreen(int seconds)
        {
            Console.Clear();

            if(seconds > 5) seconds = 5;

            int centerX = Console.WindowWidth / 2;
            int centerY = Console.WindowHeight / 2;

            int[] posX = new int[] { centerX - 1, centerX-1,centerX,centerX+1,centerX+1,centerX+1,centerX,centerX-1};
            int[] posY = new int[] { centerY, centerY + 1, centerY + 1, centerY + 1, centerY, centerY - 1, centerY -1, centerY - 1 };
            drawLogo(centerX - 16, centerY - 9);
            double startingTime = 0;
            int spinnerIndex = 0;

            for(int i = 0; i<posX.Length; i++)
            {
                Console.SetCursorPosition(posX[i], posY[i]);
                Console.Write("O");

            }
            while (startingTime < seconds)
            {

                if (spinnerIndex > posX.Length)
                {
                    spinnerIndex = 0;
                }
                // Set cursor to the middle of the screen
                Console.SetCursorPosition(posX[spinnerIndex], posY[spinnerIndex]);

                // Draw the current spinner character
                Console.Write(" ");

                // Sleep for 100 milliseconds (20 frames per second)
                Sleep(5);
                Console.SetCursorPosition(posX[spinnerIndex], posY[spinnerIndex]);
                // Increment the time counter
                startingTime += 0.1 * 10;
                spinnerIndex++;
                // Optional: Clear the spinner before redrawing
                Console.Write('O'); // Erase the previous spinner before the next frame
            }
            Console.Clear();
            ClearCurrentLine();
        }
        public void drawLogo(int X, int Y)
        {
            for (int i = 0; i < logo.Length; i++)
            {
                Console.SetCursorPosition(X, Y);
                Console.Write(logo[i]);
                Y++;
            }
        }
        public void Sleep(int milliseconds)
        {
            // Simple delay using a busy loop
            var ticks = DateTime.Now.Ticks + (milliseconds * 10000); // Convert milliseconds to ticks (1 ms = 10000 ticks)
            while (DateTime.Now.Ticks < ticks)
            {
                // Do nothing, just wait
            }
        }

    }
}

