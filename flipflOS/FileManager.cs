using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flipflOS
{
    public class FileManager
    {
        public static void makeDirectory(String[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Not enough Arguments. Syntax: mkdir dir");
                return;
            }
            Kernel.currentdir.createSubDirectory(args[1]);

            Serializer.createDirectory(Kernel.currentdir, args[1]);
        }
        public static void removeDirectory(String[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Not enough Arguments. Syntax: mkdir dir");
                return;
            }
            if (Serializer.deleteDirectory(Kernel.currentdir, args[1]))
            {
                Kernel.currentdir.removeSubDirectory(args[1]);
            }

        }
        public static void makeFile(String[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Not enough Arguments. Syntax: touch file");
                return;
            }

            String path = args[1];        //nimmt die argumente
            Directory startingdir = Kernel.currentdir; //speichert ursprüngliches directory

            String[] seperatedbyslash = path.Split("/"); //teilt den ersten path mit den Slashs
            String fileString = seperatedbyslash[seperatedbyslash.Length - 1];

            if (seperatedbyslash.Length == 0 || fileString == "")
            {
                Console.WriteLine("No name given");
                return;
            }
            for (int i = 0; i < seperatedbyslash.Length - 1; i++)
            {
                Kernel.changeDir(seperatedbyslash[i]); //bewegt Kernel.currentdir zur path von der Datei
            }
            if (Serializer.saveFile(Kernel.currentdir, fileString, new string[] { "", "" })) Kernel.currentdir.addFile(new Directory.File(fileString, new string[0]));
            Kernel.currentdir = startingdir;
        }
        public static void removeFile(String[] args)
        {
            if (args.Length <= 1) return;

            String path = args[1];
            Directory startingdir = Kernel.currentdir; //speichert startverzeichnis um später zurückzukommen
            String[] seperatedbyslash = path.Split("/"); //teilt pfad in unterOrdner
            String file = seperatedbyslash[seperatedbyslash.Length - 1]; //speichert Dateiname
            for (int i = 0; i < seperatedbyslash.Length - 1; i++)
            {
                Kernel.changeDir(seperatedbyslash[i]); //geht zum Pfad wo die Datei ist
            }
            if (Serializer.deleteFile(Kernel.currentdir, file)) Kernel.currentdir.deleteFile(file); //löscht File
            Kernel.currentdir = startingdir; //geht wieder zum Startverzeichnis
        }
        public static void moveFile(String[] args)
        {
            copyFile(args);

            String path = args[1];        //nimmt die argumente

            String[] seperatedbyslash = path.Split("/"); //teilt den ersten path mit den Slashs
            String file = seperatedbyslash[seperatedbyslash.Length - 1];

            Kernel.currentdir.deleteFile(file); //löscht File
        }
        public static void copyFile(String[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Not enough arguments.");
                return;
            }
            String path = args[1];        //nimmt die argumente
            String destination = args[2];

            Directory startingdir = Kernel.currentdir;
            String[] seperatedbyslash = path.Split("/"); //teilt den ersten path mit den Slashs
            String file = seperatedbyslash[seperatedbyslash.Length - 1];

            for (int i = 0; i < seperatedbyslash.Length - 1; i++)
            {
                Kernel.changeDir(seperatedbyslash[i]); //bewegt Kernel.currentdir zur path von der Datei
            }
            if (Kernel.currentdir.getFile(file) == null)
            {
                return;
            }
            Directory.File movingFile = Kernel.currentdir.getFile(file); //speichert Datei in einer temporären Variable
            Directory firstFileDirectory = Kernel.currentdir; //speichert directory von File ab, falls Zieldir bereits Datei mit namen beinhaltet.
            Kernel.currentdir = startingdir; //fängt von vorne an

            seperatedbyslash = destination.Split("/"); //teilt zielPfad 
            for (int i = 0; i < seperatedbyslash.Length; i++)
            {
                Kernel.changeDir(seperatedbyslash[i]); //bewegt Kernel.currentdir zum Zielpfad
            }
            Kernel.currentdir.addFile(new Directory.File(movingFile.name, movingFile.content)); //speichert Datei ab, wenn möglich.
            Kernel.currentdir = startingdir; //geht wieder zum Startverzeichnis
        }
        public static void editFile(String[] args)
        {
            if (args.Length <= 1) return;

            String path = args[1];        //nimmt die argumente

            Directory startingdir = Kernel.currentdir; //speichert ursprüngliches directory
            String[] seperatedbyslash = path.Split("/"); //teilt den ersten path mit den Slashs

            String fileString = seperatedbyslash[seperatedbyslash.Length - 1];

            for (int i = 0; i < seperatedbyslash.Length - 1; i++)
            {
                Kernel.changeDir(seperatedbyslash[i]); //bewegt Kernel.currentdir zur path von der Datei
            }
            if (Kernel.currentdir.getFile(fileString) == null) //wenn FIle nicht existiert, returne
            {
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Kernel.drawLogo(0, 0, AsciiArt.Fileeditor);
            Kernel.ResetColor();
            Kernel.Sleep(2000);
            Directory.File file = Kernel.currentdir.getFile(fileString);
            Directory.File newfile = new FileEditor().startFileeditor(file);
            if (Serializer.deleteFile(Kernel.currentdir, file.name)) Kernel.currentdir.deleteFile(file.name);
            if (Serializer.saveFile(Kernel.currentdir, file.name, newfile.content)) Kernel.currentdir.addFile(newfile);

            Kernel.currentdir = startingdir; // zurück zum ersten Directory
        }
        public static void read(String[] args)
        {
            if (args.Length <= 1) return;
            String path = args[1];        //nimmt die argumente

            Directory startingdir = Kernel.currentdir; //speichert ursprüngliches directory
            String[] seperatedbyslash = path.Split("/"); //teilt den ersten path mit den Slashs

            String fileString = seperatedbyslash[seperatedbyslash.Length - 1];

            for (int i = 0; i < seperatedbyslash.Length - 1; i++)
            {
                Kernel.changeDir(seperatedbyslash[i]); //bewegt Kernel.currentdir zur path von der Datei
            }
            if (Kernel.currentdir.getFile(fileString) == null) //wenn FIle nicht existiert, returne
            {
                return;
            }

            new FileEditor().readFile(Kernel.currentdir.getFile(fileString));

            Kernel.currentdir = startingdir; // zurück zum ersten Directory
        }
        public static void rename(String[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Not enough Arguments. Syntax: rename [file] [newname]");
                return;
            }
            String path = args[1];        //nimmt die argumente

            Directory startingdir = Kernel.currentdir; //speichert ursprüngliches directory
            String[] seperatedbyslash = path.Split("/"); //teilt den ersten path mit den Slashs

            String fileString = seperatedbyslash[seperatedbyslash.Length - 1];

            for (int i = 0; i < seperatedbyslash.Length - 1; i++)
            {
                Kernel.changeDir(seperatedbyslash[i]); //bewegt Kernel.currentdir zur path von der Datei
            }
            if (Kernel.currentdir.getFile(fileString) == null) //wenn FIle nicht existiert, returne
            {
                return;
            }
            Directory.File file = Kernel.currentdir.getFile(fileString);
            file.name = args[2];
            Kernel.currentdir = startingdir;
        }
        public static void printAllFilesAndDirs()
        {

            if (Kernel.currentdir.subdirectories != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var dir in Kernel.currentdir.subdirectories) //iteriert durch alle subdirs und printet sie blau
                {
                    Console.WriteLine($"  {dir.name}");
                }
            }

            if (Kernel.currentdir.files != null)
            {
                Console.ForegroundColor = ConsoleColor.Blue;

                foreach (var file in Kernel.currentdir.files) //iteriert durch alle dateiein im directory und printet sie blau
                {
                    Console.WriteLine($"  {file.name}");
                }
            }
            Kernel.ResetColor();
            Kernel.ClearCurrentLine();
        }
    }
}
