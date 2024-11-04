using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flipflOS
{
    internal class FileEditor
    {
        public Directory.File editingFile;
        int cursorX = 0; //cursir Position speichern
        int cursorY = 0;
        Char[][] Inhalt;
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
            Console.WriteLine("starting Fileeditor");
            Sleep(1000);
            Inhalt = StringArrayToChar2D(editingFile.content); //konvertiert inhalt der File in das 2d char array
            Sleep(1000);
            Console.Clear();
        }
        public void run()
        {
            bool running = true;
            while (running)
            {
                printFile();
                Console.SetCursorPosition(cursorX, cursorY);
                var key = Console.ReadKey(intercept: true); //input erkennung für mögliche Eingabe von Pfeil hoch, runter und Backspace
                switch (key.Key)
                {
                    case ConsoleKey.Enter://new line
                        break;

                    case ConsoleKey.Backspace: //soll char löschen und cursor nach links
                        break;

                    case ConsoleKey.UpArrow: //soll cursor nach oben gehen lassen
                        if (cursorY > 0) cursorY--;
                        break;
                        
                    case ConsoleKey.DownArrow: //soll cursor nach unten gehen lassen
                        if (cursorY < 40) cursorY++;
                        break;

                    case ConsoleKey.RightArrow: //soll cursor nach rechts gehen lassen
                        if (cursorX < 80) cursorX++;
                        break;

                    case ConsoleKey.LeftArrow: //soll cursor nach links gehen lassen
                        if (cursorX > 0) cursorX--;
                        break;
                    case ConsoleKey.Escape:
                        running = false; //beendet Programm
                        break;
                    default:
                        writeInhalt(cursorX, cursorY, key.KeyChar);
                        break;
                }
            }
        }
        public Directory.File stop()
        {
            Console.Clear(); // clears terminal prior to returning to standard Programm
            editingFile.content = Char2DToStringArray(Inhalt);
            return editingFile; //soll am Ende fertige File zurückgeben
        }
        public void printFile()
        {
            Console.Clear();
            foreach (char[] line in Inhalt)
            {
                foreach(char c in line)
                {
                    Console.Write(c);
                }
                Console.WriteLine();
            }
        }
        public void writeInhalt(int posY, int posX, char character)
        {
            if (this.Inhalt == null || this.Inhalt.Length - 1 < posY || this.Inhalt[posX].Length - 1 < posX) return; // wenn inhalt noch nicht init, oder leer, brich ab
            this.Inhalt[posY][posX] = character;
    
        }
        public static string[] Char2DToStringArray(char[][] charArray)
        {
            // Initialize the string array with the same number of rows as charArray
            string[] stringArray = new string[charArray.Length];

            for (int i = 0; i < charArray.Length; i++)
            {
                // Convert each row (char array) to a string, trimming trailing spaces
                stringArray[i] = new string(charArray[i]).TrimEnd();
            }

            return stringArray;
        }

        public static char[][] StringArrayToChar2D(string[] stringArray)
        {
            // Initialize the 2D char array
            char[][] charArray = new char[stringArray.Length][];

            for (int i = 0; i < stringArray.Length; i++)
            {
                // Initialize each row to a length of 80
                charArray[i] = new char[80];

                // Copy characters from the string to the char array
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

            return charArray;
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
