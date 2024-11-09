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
       
        public char[][] canvas = new char[20][];
        private int lastMouseX = -1;
        private int lastMouseY = -1;
        public char[][] paint(char[][] inhalt)
        {
            canvas = inhalt;

            beforeRun();
            run();
            stop(); //stops and returns edited File
            return canvas;
        }

        public void beforeRun()
        {
            // Initialize mouse settings (optional but recommended)
            MouseManager.ScreenWidth = 800;   // Set the screen width
            MouseManager.ScreenHeight = 200;  // Set the screen height
            MouseManager.X = 400;             // Start mouse in the middle of the screen width
            MouseManager.Y = 100;             // Start mouse in the middle of the screen height

        }
        public void run()
        {
            while (true)
            {
                // Get the current mouse X and Y position
                int mouseX = (int)MouseManager.X;
                int mouseY = (int)MouseManager.Y;

                // Detect mouse movement
                if (mouseX != lastMouseX || mouseY != lastMouseY)
                {
                    System.Console.SetCursorPosition(mouseX / 10, mouseY / 10);

                    if (MouseManager.LastMouseState == MouseState.Middle)
                    {
                        break;
                    }
                    if (MouseManager.LastMouseState == MouseState.Right)
                    {
                        System.Console.Write(" ");
                        canvas[mouseY / 10][mouseX / 10] = ' ';
                    }
                    if (MouseManager.LastMouseState == MouseState.Left)
                    {
                        System.Console.Write("@");
                        canvas[mouseY / 10][mouseX / 10] = '@';
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
          
        }
    }
}
