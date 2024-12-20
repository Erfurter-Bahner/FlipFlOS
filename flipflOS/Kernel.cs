using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.FileSystem.VFS;
using System.Drawing;
using Cosmos.HAL;

namespace flipflOS
{
    public class Kernel : Sys.Kernel
    {
        public static Sys.FileSystem.CosmosVFS fs1;
        DateTime start;
        Memory mem = new Memory(); //initialisieren aller Variablen ofc
        public static Directory currentdir;

        public static ConsoleColor textColor = ConsoleColor.White;
        public static ConsoleColor bgColor = ConsoleColor.Black;

        protected override void BeforeRun()
        {
            fs1 = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs1);

            createRoot(); //erstellt für DateiSystem das Root verzeichnis, sowie weitere
            Console.Clear();
            loadingScreen(6);
            Console.Clear();
            Console.WriteLine("Willkommen im FlipFlOS- nutze help fuer Hilfe!");
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
                ResetColor(); //textfarbe zurück zu schwarz

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
                                ResetColor();
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
                                ResetColor();
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
                                ResetColor();
                                Console.Write(" : " + currentInput);
                            }
                            else
                            {
                                currentInput = ""; // No more commands, clear input
                                ClearCurrentLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(currentdir.getPath() + " : ");
                                ResetColor();
                            }
                            break;
                        case ConsoleKey.Tab:
                            string autoCompletePart = HandleTabCompletion(currentInput);
                            if (!string.IsNullOrEmpty(autoCompletePart))
                            {
                                // Vervollständigen und auf der Konsole anzeigen
                                Console.Write(autoCompletePart);
                                currentInput += autoCompletePart; // Update userInput mit dem vervollständigten Teil
                            }
                            break;
                        default:

                            if (key.KeyChar > 31)
                            {
                                currentInput += key.KeyChar;
                                Console.Write(key.KeyChar);
                            }
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
                    case "cd":
                        changeDirIterative(args);
                        break;
                    case "gcd":
                        printCurrentDir();
                        break;
                    case "ls":
                            FileManager.printAllFilesAndDirs();
                        break;
                    case "mkdir":
                            FileManager.makeDirectory(args);
                        break;
                    case "removeDir":
                            FileManager.removeDirectory(args);
                        break;
                    case "touch":
                            FileManager.makeFile(args);
                        break;
                    case "readFile":
                            FileManager.read(args);
                        break;
                    case "read":
                            FileManager.read(args);
                        break;
                    case "removeFile":
                            FileManager.removeFile(args);
                        break;
                    case "moveFile":
                            FileManager.moveFile(args);
                        break;
                    case "copyFile":
                            FileManager.copyFile(args);
                        break;
                    case "edit":
                            FileManager.editFile(args);
                        break;
                    case "rename":
                            FileManager.rename(args);
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "loadingScreen":
                        if (args.Length == 1) break;
                        loadingScreen(int.Parse(args[1]));
                        break;
                    case "commands":
                            commands();
                        break;
                    case "commands+":
                            commandsFull();
                        break;
                    case "colorchange":
                            changeFormat();
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
                //schleife durch array aller commands, mit angabe der usage und info, und verwendung von Farbe
                for (int i = 0; i < CommandManager.commands.Length; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta; //magenta für die usage
                    Console.WriteLine(CommandManager.commands[i].usage);

                    ResetColor(); //weiß für die beschreibung
                    Console.WriteLine("\t" + CommandManager.commands[i].info);
                }
        }
       
        public void createRoot()
        { //erstellt alle nötigen verzeichnisse für das dateisystem
            currentdir = Serializer.createRoot();
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
            if (args.Length <= 1)
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
        public static void changeDir(String directory)
        {
            if (directory == "..")
            {
                if (currentdir.parent != null)
                {
                    currentdir = currentdir.parent; //wenn .. dann ins parentdir
                }
                return;
            }
            if( directory == ".")
            {
                return;
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
        
        public static void loadingScreen(int seconds)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            if (seconds > 8) seconds = 8;
            int startingTime = 0;
            int spinnerIndex = 0;

            while (startingTime < 3) //3 seconds the logo
            {
                if(spinnerIndex%2 == 0) drawLogo(0, 0, AsciiArt.logo);
                else                  drawLogo(0, 0, AsciiArt.logo2);
                Sleep(1000);
                // Increment the time counter
                startingTime++;
                spinnerIndex++;
            }
            drawLogo(0, 0, AsciiArt.logo3);
            Sleep((seconds - 3)*1000);
            Console.Clear();
            ResetColor();
        }
        public static void drawLogo(int X, int Y, String[] logo)
        {
            Console.Clear();
            for (int i = 0; i < AsciiArt.logo.Length; i++)
            {
                Console.SetCursorPosition(X, Y);
                    Console.Write(logo[i]);
                Y++;
            }

        }
        public static void Sleep(int milliseconds)
        {
            // Simple delay using a busy loop
            var ticks = DateTime.Now.Ticks + (milliseconds * 10000); // Convert milliseconds to ticks (1 ms = 10000 ticks)
            while (DateTime.Now.Ticks < ticks)
            {
                // Do nothing, just wait
            }
        }
        public void commands()
        {
            //schleife durch array aller commands, mit angabe nur vom Namen
            for (int i = 0; i < CommandManager.commands.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Red; //red für den Namen
                Console.WriteLine(CommandManager.commands[i].name);
            }
        }
        public void commandsFull()
        {
            //schleife durch array aller commands, mit angabe nur vom Namen
            for (int i = 0; i < CommandManager.commands.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Red; //red für den Namen
                Console.WriteLine(CommandManager.commands[i].usage);
            }
        }
        public string HandleTabCompletion(string userInput)
        {
            foreach (var command in CommandManager.commands) //Suchschleife durch die Commands
            {
                if (command.name.StartsWith(userInput))
                {
                    // Ergänzt die Benutzereingabe um den fehlenden Teil des Befehls
                    return command.name.Substring(userInput.Length);
                }
            }
            // Gibt eine leere Zeichenfolge zurück, wenn keine Übereinstimmung gefunden wird
            return string.Empty;
        }

        public static void ResetColor()
        {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = bgColor;
        }
        public static void changeFormat()
        {
            if(textColor == ConsoleColor.White)
            {
                textColor = ConsoleColor.Black;
                bgColor = ConsoleColor.White;
            }
            else
            {
                textColor = ConsoleColor.White;
                bgColor = ConsoleColor.Black;
            }
            ResetColor();
            Console.Clear();
        }
    }
}

