using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cosmos.HAL;
using Cosmos.System;

namespace flipflOS
{
    public class Painter
    {
       
        public char[][] canvas = new char[80][];
        private int lastMouseX = -1;
        private int lastMouseY = -1;
        public void startPainter()
        {
            beforeRun();
            run();
            stop(); //stops and returns edited File
        }

        public void beforeRun()
        {
            System.Console.WriteLine("Starting Cosmos OS with mouse tracking.");

            for(int i = 0; i < 80; i++)
            {
                canvas[i] = new char[20];
            }
            // Initialize mouse settings (optional but recommended)
            MouseManager.ScreenWidth = 800;   // Set the screen width
            MouseManager.ScreenHeight = 200;  // Set the screen height
            MouseManager.X = 400;             // Start mouse in the middle of the screen width
            MouseManager.Y = 100;             // Start mouse in the middle of the screen height

        }
        public void run()
        {
            System.Console.Clear();
            while (true)
            {
                // Get the current mouse X and Y position
                int mouseX = (int)MouseManager.X;
                int mouseY = (int)MouseManager.Y;

                // Detect mouse movement
                if (mouseX != lastMouseX || mouseY != lastMouseY)
                {
                    System.Console.SetCursorPosition(mouseX / 10, mouseY / 10);

                    if (MouseManager.LastMouseState == MouseState.Right)
                    {
                        System.Console.Clear();
                        break;

                    }

                    if (MouseManager.LastMouseState == MouseState.Left)
                    {
                        System.Console.Write("0");
                    }
                    else
                    {
                        System.Console.SetCursorPosition(lastMouseX/10, lastMouseY/10);
                        int X = mouseX/10; int Y = mouseY/10;
                        System.Console.Write(" ");
                        System.Console.SetCursorPosition(X, Y);
                    }
                    lastMouseX = mouseX;
                    lastMouseY = mouseY;
                }


                // Small delay to reduce CPU usage and make output readable
                System.Threading.Thread.Sleep(1000/60);
            }
        }
        public void stop()
        {
            Random rnd = new Random();
            int randomnumber = rnd.Next(10000);     // creates a number between 0 and 10000

            Directory.File file = new Directory.File("AsciiArt." + randomnumber, CanvasToStringArray(canvas));
            if (Serializer.saveFile(Kernel.currentdir, file.name, file.content)) Kernel.currentdir.addFile(file);
        }
        public static string[] CanvasToStringArray(char[][] charArray)
        {
            int numRows = charArray[0].Length;
            int numCols = charArray.Length;

            string[] result = new string[numRows];

            for (int y = 0; y < numRows; y++)
            {
                char[] row = new char[numCols];
                for (int x = 0; x < numCols; x++)
                {
                    row[x] = charArray[x][y];
                }
                result[y] = new string(row);
            }

            return result;
        }
    }
}
