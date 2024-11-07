using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem.Listing;
using System.Reflection.Metadata;

namespace flipflOS
{
    public class Serializer
    {

        public static bool saveFile(Directory dir, String filename, String[] content)
        {
            try
            {
                String filePath = generateDirectoryPath(dir);
                if (filePath == @"0:\") filePath += filename;
                else filePath += @"\" + filename;

                createDirectoryFromFilepath(dir, filePath);

                File.WriteAllText(filePath, string.Join("\n",content));
                Console.WriteLine("File written successfully!");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to file: " + e.Message);
                return false;
            }
        }
        public static String[] readFile(Directory dir, String filename)
        {
            String filePath = generateDirectoryPath(dir);
            if (filePath == @"0:\") filePath += filename;
            else filePath += @"\" + filename;
            try
            {
                // Read the content of the file
                if (File.ReadAllText(filePath) != null) return File.ReadAllText(filePath).Split("\n");
                else return new string[0];
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading file: " + e.Message);
                return new string[0];
            }
        }
        public static bool deleteFile(Directory dir, String filename)
        {
            try
            {
                String filePath = @"0:\" + generateDirectoryPath(dir) + @"\" + filename;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine("File deleted successfully!");
                    return true;
                }
                else
                {
                    Console.WriteLine("File does not exist.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting file: " + e.Message);
                return false;
            }
        }
        public static void createDirectoryFromFilepath(Directory dir, string filePath)
        {
           string directoryPath = RemoveLastDirectory(filePath);
           createDirectory(dir, directoryPath);
        }
        public static void createDirectory(Directory dir,string directoryname)
        {
            try
            {
                String directoryPath = generateDirectoryPath(dir);
                if (directoryPath == @"0:\") directoryPath += directoryname;
                else directoryPath += @"\" + directoryname;
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
        public static bool deleteDirectory(Directory dir,string directoryname)
        {
            try
            {
                String directoryPath = generateDirectoryPath(dir);
                if (directoryPath == @"0:\") directoryPath += directoryname;
                else directoryPath += @"\" + directoryname;                // Create directory if it doesn't exist
                if (System.IO.Directory.Exists(directoryPath))
                {
                    System.IO.Directory.Delete(directoryPath);
                    Console.WriteLine("Directory deleted: " + directoryPath);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return false;
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
                for(int i = 0; i<root.subdirectories.Length; i++)
                {
                    root.subdirectories[i] = createSubDirectories(root.subdirectories[i]);  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return root;
        }
        public static Directory createSubDirectories(Directory directory)
        {
            String directoryPath = generateDirectoryPath(directory);

            String[] subdirectories = GetSubdirectories(directoryPath);
            String[] files = GetFiles(directoryPath);

            if ((subdirectories == null && files == null) || (subdirectories.Length == 0 && files.Length == 0))
            {
                return directory; // No subdirectories to process.
            }

            foreach (var subdir in subdirectories)
            {
                if (!string.IsNullOrWhiteSpace(subdir))
                {
                    directory.createSubDirectory(subdir);
                }
            }

            foreach (var subdir in directory.subdirectories)
            {
                if (subdir != null)
                {
                    createSubDirectories(subdir);
                }
            }
            if(directory.files == null)
            {
                directory.files = new Directory.File[0];
            }
            foreach (var file in files)
            {
                if (!string.IsNullOrWhiteSpace(file))
                {
                    directory.createFile(file);
                }
            }
            foreach(var file in directory.files)
            {
                if(file != null)
                {
                    file.content = readFile(directory, file.name);
                }
            }

            return directory;
        }

        public static String generateDirectoryPath(Directory directory)
        {
            String path = directory.name;
            if (path == "root") path = "";
            Directory parentdirectory = directory.parent;
            while (parentdirectory != null)
            {
                if(parentdirectory.name!="root") path = parentdirectory.name + "\\"+path;
                parentdirectory = parentdirectory.parent;
            }
            path = @"0:\" + path;
            return path;

        }
        public static string[] GetSubdirectories(string path)
        {
            try
            {
                if (System.IO.Directory.Exists(path))
                {
                    List<string> subdirectories = new List<string>();
                    var entries = Cosmos.System.FileSystem.VFS.VFSManager.GetDirectoryListing(path);

                    foreach (var entry in entries)
                    {
                        if (entry.mEntryType == DirectoryEntryTypeEnum.Directory && !string.IsNullOrWhiteSpace(entry.mName))
                        {
                            subdirectories.Add(entry.mName);
                            Console.WriteLine("Subdirectory found: " + entry.mName);
                        }
                    }

                    return subdirectories.ToArray();
                }
                else
                {
                    Console.WriteLine($"Directory '{path}' does not exist.");
                    return new string[0]; // Return an empty array instead of null to prevent null reference issues.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing directory: {ex.Message}");
                return new string[0]; // Return an empty array to avoid null checks.
            }
        }
        public static string[] GetFiles(string path)
        {
            try
            {
                if (System.IO.Directory.Exists(path))
                {
                    List<string> files = new List<string>();
                    var entries = Cosmos.System.FileSystem.VFS.VFSManager.GetDirectoryListing(path);

                    foreach (var entry in entries)
                    {
                        if (entry.mEntryType == DirectoryEntryTypeEnum.File && !string.IsNullOrWhiteSpace(entry.mName))
                        {
                            files.Add(entry.mName);
                            Console.WriteLine("File found: " + entry.mName);
                        }
                    }

                    return files.ToArray();
                }
                else
                {
                    Console.WriteLine($"File '{path}' does not exist.");
                    return new string[0]; // Return an empty array instead of null to prevent null reference issues.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing File: {ex.Message}");
                return new string[0]; // Return an empty array to avoid null checks.
            }
        }



    }
}
