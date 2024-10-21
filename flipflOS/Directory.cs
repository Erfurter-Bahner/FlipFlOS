using System;

namespace flipflOS
{
    public class Directory
    {
        public File[] files = new File[0];
        public Directory parent;
        public Directory[] subdirectories = new Directory[0];
        public string name;

        public Directory(File[] files, Directory parent, string name)
        {
            this.files = files;
            this.parent = parent;
            this.name = name;
        }

        public void createSubDirectory(string name)
        {
            Directory[] newSubDirectories = new Directory[subdirectories.Length + 1];
            for (int i = 0; i < subdirectories.Length; i++)
            {
                newSubDirectories[i] = subdirectories[i];
            }
            newSubDirectories[newSubDirectories.Length - 1] = new Directory(null, this, name);
            subdirectories = newSubDirectories;
        }
        public void createFile(string name)
        {
            Console.WriteLine("Creating File: " + name);

            // Check if files is null and initialize it if necessary
            if (files == null)
            {
                files = new File[0]; // Initialize to an empty array if null
            }

            File[] newFiles = new File[files.Length + 1];
            Console.WriteLine("Created new Files array");

            // Copy existing files to the new array
            for (int i = 0; i < files.Length; i++)
            {
                newFiles[i] = files[i];
            }

            // Add the new file to the last position
            newFiles[newFiles.Length - 1] = new File(name, "");
            files = newFiles;
        }
        public String getPath()
        {
            String path = "";
            Directory currentdir = this;
            while (currentdir != null)
            {
                path = currentdir.name +"/"+ path;
                currentdir = currentdir.parent;
            }
            return path;
        }
        public File getFile(String filename)
        {
            if (filename == null || this.files == null)
            {
                return null;
            }
            foreach (File f in this.files)
            {
                if (f.name == filename) return f;
            }
            return null;
        }

        public class File
        {
            public String name;
            public String content;
            public File(String name, String content)
            {
                this.name = name;
                this.content = content;
            }
            public void changecontent(String content)
            {
                this.content = content;
            }
        }
    }
}
