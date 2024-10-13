using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Sys = Cosmos.System;

namespace flipflOS
{
    public class Kernel : Sys.Kernel
    {
        DateTime start;
        Memory mem = new Memory();

        protected override void BeforeRun()
        {
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
            start = DateTime.Now;
            
        }

        protected override void Run()
        {
            Console.Write("Input: ");
            var input = Console.ReadLine();

            Console.WriteLine(input);
            String[] args = input.Split(' ');
            switch (args[0])
            {
                case "help":
                    {
                        help(args);
                    }
                    break;
                case "time":
                    {
                        TimeSpan runtime = DateTime.Now - start;
                        Console.WriteLine("running for: " + runtime.TotalSeconds + " seconds");
                    }
                    break;
                case "write":
                    {
                        writeToMemory(args);
                    }
                    break;
                case "read":
                    {
                        Console.WriteLine(readFromMemory(args));
                    }
                    break;
                default:
                    {
                        Console.WriteLine("command not known. Please use 'help' for help.");
                    }
                    break;

            }
        }
        public void help(String[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("need help with: "+args[1]);
            }
            else
            {
                Console.WriteLine("time" +
                    "\n\tfor getting current runtime" +
                    "\nhelp" +
                    "\n\tfor getting help :D" +
                    "\nwrite [index] [byte]" +
                    "\n\tWriting data" +
                    "\nread [index]" +
                    "\n\treading data");

            }
        }
        public void writeToMemory(String[] args)
        {
            if(args.Length <= 2)
            {
                Console.WriteLine("Not enough Arguments. Syntax: write index data");
                return;
            }
            uint index = Convert.ToUInt32(args[1])*2;
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
    }
}

