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
                if (subdirectories[i].name == name)
                {
                    Console.WriteLine("Directory already exists.");
                    return;
                }
                newSubDirectories[i] = subdirectories[i];
            }
            newSubDirectories[newSubDirectories.Length - 1] = new Directory(null, this, name);
            subdirectories = newSubDirectories;
        }
        public void createFile(string name)
        {
            // Check if files is null and initialize it if necessary
            if (files == null)
            {
                files = new File[0]; // Initialize to an empty array if null
            }

            File[] newFiles = new File[files.Length + 1];
            // Copy existing files to the new array
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].name == name)
                {
                    Console.WriteLine("File already exists.");
                    return;
                }
                newFiles[i] = files[i];
            }

            // Add the new file to the last position
            newFiles[newFiles.Length - 1] = new File(name, new string[]{ });
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
        public void removeFile(String name)
        {
            if (name == null) return;
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].name == name)
                { //if FIle has the same name delete it
                    //go ahead and shorten the array by 1
                    File[] newfiles = new File[files.Length - 1];
                    for (int j = 0; j < i; j++)
                    {
                        newfiles[j] = files[j];
                    }
                    for(int j = i; j<files.Length - 1; j++)
                    {
                        newfiles[j] = files[j + 1];
                    }
                    files = newfiles;
                    break;
                }
            }
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
            public String[] content;
            public File(String name, String[] content)
            {
                this.name = name;
                this.content = content;
            }
            public void changecontent(String[] content)
            {
                this.content = content;
            }
        }
    }
}
