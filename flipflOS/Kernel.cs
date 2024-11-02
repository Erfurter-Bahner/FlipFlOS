using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            "       ,@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#                                          ",
            "    @@@@%       &@@@@&@&          *@@@@@@@@@@@.                                 ",
            "   @@@      .&@@@@@@                       @@@@@@@@                             ",
            "  @@@    .@@@@@@#                              @@@@@@@,                         ",
            "  @@@@@@@@@(@                                      &@@@@@                       ",
            "  @@     @@@@@@@@%@                                  @@@@@                      ",
            "   @*       %@@@@@@@@@%                              @@@@#                      ",
            "    @@@@          @@@@@@@@@@@@@@@@                 #@@@@@                       ",
            "      @@@@@@@@@@@@@@@(      *@@@@@@@@@@@@@@@@@@#@@@@@@                          ",
            "                                                                                ",
            "",
            "",
            "",
            "",
            "",
            "                           *@@@@@@#@@(@@@*         @@@@@@@@@@@@@@@@@@@@@@@      ",
            "                         %@@&@        @@@@@@@@@@@@@@@@&@.               @@@@@@  ",
            "                        @@,       (@@@@@@@@@(                             @@@@& ",
            "                       &@     /&@@@@@@@%%                                 &@@@@.",
            "                       @@@@@@@@@%.                                      @@@@@*  ",
            "                       (@@    &&@@@@@@                               &@@@@@@    ",
            "                        @@@      &&@@@@@#                       .(@@@@@*%       ",
            "                         /@@&        @@@@@@#             #%%@@@@@@(@            ",
            "                            @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@                    ",
        };
        String[] logo2 =
        {
            "                              /@@@@@/@@@@@@@@@@@@@@@%                           ",
            "                           %@@&@/      (@@@@@@@@#@/@@#@@@@@@@@&@(               ",
            "                        #@@#        &@@@@@                    @@@@@@@@.         ",
            "                       @@@      &@@@@@@                            @@@@@@@%     ",
            "                       @@&@@@@@@@@@@*                                  @@@@@&   ",
            "                       @@    #@@@@@%                                     (@@@@& ",
            "                       %@@      &@@@@@@@@@%                                @@@@ ",
            "                        @@@&        #@@@@@@@@@@@@@                       &@@@@, ",
            "                          %@@@@@@@@@@@@@@@@@@@@@&@@@@@@@@@@@          @@@@@%.   ",
            "                                                            /@@@@%@@@@@@        ",
            "",
            "",
            "",
            "",
            "                                          *@@&@@@@@@&.                          ",
            "       .@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@          @@@@@@                      ",
            "      @@&        @@@@@@@@(@@@@@@.                      &@@@/                    ",
            "     @@       @@@@@@@@@@@                               @@@@&                   ",
            "     @@   (&@@@@@.                                     @@@@@@                   ",
            "     @@@@@@@@@@@@@                                  .#@@@@@                     ",
            "     %@@      #@@@@@&                            /@@@@@(                        ",
            "      @@@@       (@@@@@,                   /%@@@@@&@                            ",
            "        *@@@&(.     &@@@@@@@@#@,/@@#@@@@@@@@@@,                                 ",
            "            @@@@@@&@@@@@@@@@@@@@@@,                                             ",

        };
        protected override void BeforeRun()
        {
            createRoot(); //erstellt für DateiSystem das Root verzeichnis, sowie weitere
            Console.Clear();
            loadingScreen(5);
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
                    case "removeFile":
                        removeFile(args[1]);
                    break;
                    case "moveFile":
                        moveFile(args);
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
        public void removeFile(String path)
        {
            Directory startingdir = currentdir; //speichert startverzeichnis um später zurückzukommen
            String[] seperatedbyslash = path.Split("/"); //teilt pfad in unterOrdner
            String file = seperatedbyslash[seperatedbyslash.Length - 1]; //speichert Dateiname
            for(int i = 0;i < seperatedbyslash.Length - 1; i++)
            {
                changeDir(seperatedbyslash[i]); //geht zum Pfad wo die Datei ist
            }
            currentdir.removeFile(file); //löscht File
            currentdir = startingdir; //geht wieder zum Startverzeichnis
        }
        public void moveFile(String[] args)
        {
            String path = args[1];        //nimmt die argumente
            String destination = args[2];

            Directory startingdir = currentdir;
            String[] seperatedbyslash = path.Split("/"); //teilt den ersten path mit den Slashs
            String file = seperatedbyslash[seperatedbyslash.Length - 1];

            for (int i = 0; i < seperatedbyslash.Length - 1; i++)
            {
                changeDir(seperatedbyslash[i]); //bewegt currentdir zur path von der Datei
            }
            if(currentdir.getFile(file) == null)
            {
                return;
            }
            Directory.File movingFile = currentdir.getFile(file); //speichert Datei in einer temporären Variable
            currentdir.removeFile(file); //löscht im ersten Verzeichnis
            currentdir = startingdir; //fängt von vorne an

            seperatedbyslash = destination.Split("/"); //teilt zielPfad 
            for (int i = 0; i < seperatedbyslash.Length; i++)
            {
                changeDir(seperatedbyslash[i]); //bewegt currentdir zum Zielpfad
            }
            currentdir.addFile(movingFile); //speichert Datei dort
            currentdir = startingdir; //geht wieder zum Startverzeichnis
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
        public void writeToFile(String[] args)
        {
            if (args.Length == 1) return;
            String content = "";

            for (int i = 2; i < args.Length; i++) //nimmt alle argumente nach "writeFile [file]" und schreibt sie in die datei hinein.
            {
                String argument = args[i];
                content += argument+" ";
            }
            //now content has the whole line
            content = AddSeparator(content);
            String[] splitbynewlines = content.Split('/');

            currentdir.getFile(args[1]).changecontent(splitbynewlines);
        }
        public void readFile(String filename)
        { //liest den dateiinhalt und gibt ihn aus
            if(currentdir.getFile(filename) == null)
            {
                return; //bricht direkt ab, wenn Datei nicht gefunden wird
            }
            Console.WriteLine(currentdir.getFile(filename).name + ": ");
            for (int i = 0; i < currentdir.getFile(filename).content.Length; i++)
            {
                Console.WriteLine(currentdir.getFile(filename).content[i]);
            }
        }
        public string AddSeparator(string input)
        {
            StringBuilder result = new StringBuilder();
            int countSinceLastSlash = 0; // Track the number of characters since the last "/"

            for (int i = 0; i < input.Length; i++)
            {
                // Add current character to result
                result.Append(input[i]);

                // Increase the counter if the character is not '/'
                if (input[i] != '/')
                {
                    countSinceLastSlash++;
                }
                else
                {
                    // Reset the counter when a '/' is encountered
                    countSinceLastSlash = 0;
                }

                // If we've reached 50 characters without a '/', add "-/"
                if (countSinceLastSlash == 50)
                {
                    result.Append("-/");
                    countSinceLastSlash = 0; // Reset the counter after adding "-/"
                }
            }

            return result.ToString();
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
            while(seconds > 8)
            {
                loadingScreen(8);
                seconds -= 8;
            }
            double startingTime = 0;
            int spinnerIndex = 0;

            while (startingTime < seconds)
            {
                Console.Clear();
                drawLogo(0, 0, spinnerIndex%2);

                Sleep(5);
                // Increment the time counter
                startingTime += 0.1 * 10;
                spinnerIndex++;
            }
            Console.Clear();
            ClearCurrentLine();
        }
        public void drawLogo(int X, int Y, int type)
        {
            for (int i = 0; i < logo.Length; i++)
            {
                Console.SetCursorPosition(X, Y);
                if(type == 0)
                {
                    Console.Write(logo[i]);
                }else Console.Write(logo2[i]);
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
    }
}

