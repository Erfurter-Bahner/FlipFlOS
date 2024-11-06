using System;
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

        public void saveFile(String filePath, String content)
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
        public void createDirectoryFromFilepath(string filePath)
        {
           string directoryPath = RemoveLastDirectory(filePath);
           createDirectory(directoryPath);
        }
        public void createDirectory(string directoryPath)
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
    }
}
