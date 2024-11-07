using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.HAL;
using Cosmos.System;

namespace flipflOS
{
    public class Painter
    {
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

            // Initialize mouse settings (optional but recommended)
            MouseManager.ScreenWidth = 800;   // Set the screen width
            MouseManager.ScreenHeight = 250;  // Set the screen height
            MouseManager.X = 400;             // Start mouse in the middle of the screen width
            MouseManager.Y = 150;             // Start mouse in the middle of the screen height
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
                    System.Console.Clear();
                    System.Console.WriteLine($"Mouse moved to: X = {mouseX}, Y = {mouseY}");

                    System.Console.SetCursorPosition( mouseX/10, mouseY/10 );
                    System.Console.Write("0");

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
