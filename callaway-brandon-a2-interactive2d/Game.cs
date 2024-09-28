// Include code libraries you need below (use the namespace).
using System;
using System.Drawing;
using System.Numerics;

// The namespace your code is in.
namespace Game10003
{
    /// <summary>
    ///     Your game code goes inside this class!
    /// </summary>
    public class Game
    {
        // Place your variables here:
        static int windowWidth = 400;
        static int windowHeight = 400;
        static int[] windowCenter = [windowWidth/2, windowHeight/2];
        float rectSize = 50;
        Color rectColor = new Color(10, 0, 155);
        float rectSpeed = 100;
        int colorChangeRate = 20;
        // Color variables

        // Rectangle Variables
        Vector4 rectangleTest = new Vector4(windowCenter[0], windowCenter[1], 0, 0);
        

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            WindowInitialization(windowWidth, windowHeight);
            rectangleTest.X = windowCenter[0];
            rectangleTest.Y = windowCenter[1];
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.DarkGray);
            Console.WriteLine($"{rectSize}");

            DrawBackground();
            HandleInput();
        }


        // Window Setup and Initialization
        public void WindowInitialization(int windowWidth, int windowHeight)
        {
            Window.SetTitle("callaway-brandon-a2-falling-game");
            Window.SetSize(windowWidth, windowHeight);
            Window.TargetFPS = 60;
        }

        public void DrawBackground()
        {
            // Set Rectangle color and place rectangle at center of screen
            Draw.FillColor = rectColor;
            Draw.Square(rectangleTest.X - rectSize / 2, rectangleTest.Y - rectSize / 2, rectSize);
        }

        public void HandleInput()
        {
            if (Input.IsKeyboardKeyDown(KeyboardInput.W))
            {
                if (rectSize > windowWidth)
                {
                    rectSize = 25;
                    rectangleTest.Z += colorChangeRate;
                    /*
                    if (rectColor.R < 255)
                    {
                        
                    }
                    else
                    {
                        rectColor.R = 95;
                    }
                    */
                    rectColor.R += (int)rectangleTest.Z;
                }

                rectSize += Time.DeltaTime * rectSpeed * 10;
            }
        }
    }
}
