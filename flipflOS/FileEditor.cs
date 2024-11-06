using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flipflOS
{
    internal class FileEditor
    {
        public Directory.File editingFile;
        int cursorX = 0; //cursor Position speichern
        int cursorY = 0;
        Char[][] Inhalt;

        bool saved = true;
        bool confirmedescape = false;

        public Directory.File startFileeditor(Directory.File file)
        {
            editingFile = file; //objektvariable wird gesetzt
            beforeRun(); 
            run();
            return stop(); //stops and returns edited File
        }
        public void beforeRun()
        {
            Console.Clear();
            Inhalt = StringArrayToChar2D(editingFile.content); //konvertiert inhalt der File in die char Matrix
        }
        public void run()
        {
            bool running = true;
            printFile(true); // soll einmal printen, dann nurnoch die zeile ändern
            while (running)
            {
                Console.SetCursorPosition(cursorX, cursorY);
                var key = Console.ReadKey(intercept: true); // Capture key without displaying it
                switch (key.Key)
                {
                    case ConsoleKey.Enter: // new line
                        //muss noch umgesetzt werden
                        cursorX = 0;
                        if (cursorY < Inhalt.Length - 1)
                        {
                            cursorY++;
                        }
                        else
                        {
                            cursorY = 0;
                        }
                        break;

                    case ConsoleKey.Backspace: // delete character and move cursor left
                            if (cursorX > 0) // Ensure cursor is not at the start
                            {
                                cursorX--;
                                writeInhalt(cursorY, cursorX, ' '); // Replace with space
                                editDisplayedChar(cursorY, cursorX);

                            }
                            saved = false;
                            if (confirmedescape) confirmedescape = false;
                            notifyclear();
                        break;

                    case ConsoleKey.UpArrow: // Move cursor up
                        if (cursorY > 0) cursorY--;
                        break;

                    case ConsoleKey.DownArrow: // Move cursor down
                        if (cursorY < Inhalt.Length - 1) cursorY++;
                        break;

                    case ConsoleKey.RightArrow: // Move cursor right
                        if (cursorX < 79) cursorX++;
                        break;

                    case ConsoleKey.LeftArrow: // Move cursor left
                        if (cursorX > 0) cursorX--;
                        break;

                    case ConsoleKey.Tab: //saves file
                        saveChanges();
                        break;

                    case ConsoleKey.Escape: // Exit
                        if (saved || confirmedescape)
                        {
                            running = false;
                        }
                        else
                        {
                            notify("Not saved yet! Press esc to confirm to abort.");
                            confirmedescape = true;
                        }
                        break;

                    default:
                            if (!char.IsControl(key.KeyChar)) // Only write non-control characters
                            {
                                writeInhalt(cursorY, cursorX, key.KeyChar);
                                editDisplayedChar(cursorY, cursorX);

                            if (cursorX < 79) cursorX++; // Move cursor right after writing
                            }

                            if (confirmedescape) confirmedescape = false;
                            saved = false;

                            notifyclear();
                        break;
                }
            }
        }
        public Directory.File stop()
        {
            Console.Clear(); // clears terminal prior to returning to standard Programm
            return editingFile; //soll am Ende fertige File zurückgeben
        }
        public void notify(String msg)
        {
            notifyclear();
            int X = cursorX;
            int Y = cursorY; //speichert vorherige cursor pos
            Console.SetCursorPosition(0, 20); //setzt in zeile
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(msg); //schreibt
            Console.ResetColor();
            Console.SetCursorPosition(X, Y); //Setzt pos des cursors zurück
        }
        public void notifyclear()
        {
            int X = cursorX;
            int Y = cursorY; //speichert vorherige cursor pos
            Console.SetCursorPosition(0, 20); //setzt in zeile
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(X, Y); //Setzt pos des cursors zurück
        }
        public void saveChanges()
        {
            editingFile.content = Char2DToStringArray(Inhalt);
            notify("saved changes");
            saved = true;
        }
        public void printFile(bool editor)
        {
            Console.Clear();
            foreach (char[] line in Inhalt)
            {
                foreach(char c in line)
                {
                    Console.Write(c);
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 21; i < 24; i++)
            {
                Console.SetCursorPosition(0, i);
                if (editor)
                {
                    Console.Write(AsciiArt.fileEditorSettings[i - 21]);
                }else Console.Write(AsciiArt.readerSettings[i - 21]);
            }
            Console.ResetColor();
        }
        public void editDisplayedChar(int line, int posX)
        {
            for(int i = posX-1;i < posX+1; i++)
            {
                if (posX == 0) i++;
                if (posX == 80) break;
                Console.SetCursorPosition(i, line);
                Console.Write(Inhalt[line][i]);
            }

            Console.SetCursorPosition(posX, line);
        }
        public void writeInhalt(int posY, int posX, char character)
        {
            // Ensure the cursor positions are within bounds of Inhalt's dimensions
            if (Inhalt == null || posY >= Inhalt.Length || posX >= Inhalt[posY].Length)
            {
                return; // Exit if positions are out of bounds
            }

            // Assign the character to the specified position
            Inhalt[posY][posX] = character;
        }

        public static string[] Char2DToStringArray(char[][] charArray)
        {
            // Initialize a list to store non-empty lines
            List<string> stringList = new List<string>();

            for (int i = 0; i < charArray.Length; i++)
            {
                // Convert each row (char array) to a string
                string line = new string(charArray[i]).TrimEnd();
                stringList.Add(line);
               
            }

            return stringList.ToArray();
        }


        public static char[][] StringArrayToChar2D(string[] stringArray)
        {
            // Initialize the 2D char array
            char[][] charArray = new char[20][];

            for (int i = 0; i < 20; i++)
            {
                // Initialize each row to a length of 80
                charArray[i] = new char[80];

                // Copy characters from the string to the char array
                if(i < stringArray.Length)
                {
                    for (int j = 0; j < stringArray[i].Length && j < 80; j++)
                    {
                        charArray[i][j] = stringArray[i][j];
                    }

                    // Optional: fill the remaining spaces with ' ' (space) if the string is shorter than 80
                    for (int j = stringArray[i].Length; j < 80; j++)
                    {
                        charArray[i][j] = ' ';
                    }

                }
                else
                {
                    for (int j = 0; j < 80; j++)
                    {
                        charArray[i][j] = ' ';
                    }
                }
            }

            return charArray;
        }
        public void readFile(Directory.File file)
        {
            if (file == null || file.content == null || file.content.Length == 0) return;
            editingFile = file;
            this.Inhalt = StringArrayToChar2D(editingFile.content); //konvertiert inhalt der File in die char Matrix
            printFile(false); //schreibt Datei
            Console.SetCursorPosition(0, 0);

            bool running = true;
            while (running) //Laufzeit-Loop
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Escape) //Escape beendet die Schleife
                {
                    running = false;
                }
                else
                {
                    Console.SetCursorPosition(0, 0);
                }
            }

            Console.Clear(); //bei Beendingung noch das Terminal leeren.
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
    }
}
