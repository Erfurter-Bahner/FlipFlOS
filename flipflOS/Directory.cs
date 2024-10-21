using System;

namespace flipflOS
{
    public class Directory
    {
        public File[] files = { };
        public Directory parent;
        public Directory[] subdirectories = { };
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
    }
}
