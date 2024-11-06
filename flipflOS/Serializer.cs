﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;

namespace flipflOS
{
    public class Serializer
    {

        public static void saveFile(String filePath, String content)
        {
            try
            {
                filePath = @"0:\" + filePath;

                createDirectoryFromFilepath(filePath);

                File.WriteAllText(filePath, content);
                Console.WriteLine("File written successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to file: " + e.Message);
            }
        }
        public String readFile(String filePath)
        {
            filePath = @"0:\"+ filePath;  // Ensure this path is correct
            try
            {
                // Read the content of the file
                return File.ReadAllText(filePath);            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading file: " + e.Message);
                return "";
            }
        }
        public static void createDirectoryFromFilepath(string filePath)
        {
           string directoryPath = RemoveLastDirectory(filePath);
           createDirectory(directoryPath);
        }
        public static void createDirectory(string directoryPath)
        {
            try
            {
                // Create directory if it doesn't exist
                if (!System.IO.Directory.Exists(directoryPath))
                {
                    System.IO.Directory.CreateDirectory(directoryPath);
                    Console.WriteLine("Directory created: " + directoryPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
        public static string RemoveLastDirectory(string filePath)
        {
            // Find the last occurrence of the backslash
            int lastBackslashIndex = filePath.LastIndexOf('\\');

            // If there is no backslash, return the path as is (or empty)
            if (lastBackslashIndex == -1)
            {
                return filePath; // Or you could return "" depending on your need
            }

            // Return the substring before the last backslash
            return filePath.Substring(0, lastBackslashIndex);
        }
        public static Directory createRoot()
        {
            Directory root = new Directory(null, null, "root");
            string rootDir = @"0:\";  // Starting directory
            try
            {
                // Call the function to list subdirectories
                String[] subdirectories = GetSubdirectories(rootDir);

                //Console.WriteLine("Subdirectories in " + rootDir + ":");
                foreach (String subdir in subdirectories)
                {
                    root.createSubDirectory(subdir);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return root;
        }
        public static string[] GetSubdirectories(string path)
        {
            try
            {
                if (System.IO.Directory.Exists(path))
                {
                    String subdirs = "";
                    //   var entries = System.IO.Directory.GetFileSystemEntries(path);
                    String[] entries = { "home","directory"};
                    if (entries.Length == 0)
                    {
                        Console.WriteLine("Verzeichnis ist leer.");
                        return null;
                    }
                    else
                    {
                        foreach (var entry in entries)
                        {
                            subdirs += "%" + entry;
                        }
                        String[] subdirectories = subdirs.Split("%");
                        return subdirectories;
                    }
                }
                else
                {
                    Console.WriteLine($"Verzeichnis '{path}' existiert nicht.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Auflisten des Verzeichnisses: {ex.Message}");
                return null;
            }
        }


    }
}
