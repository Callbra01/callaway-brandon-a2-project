// Include code libraries you need below (use the namespace).
using System;
using System.Drawing;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

// The namespace your code is in.
namespace Game10003
{
    /// <summary>
    ///     Your game code goes inside this class!
    /// </summary>
    public class Game
    {
        // Place your variables here:
        int windowWidth = 400;
        int windowHeight = 400;
        static int[] windowCenter = [0, 0];

        Vector4 testRect = new Vector4(25, 25, 0, 0);

        // Player variables
        int playerScore;

        // Text var
        Color textColor = new Color(245, 245, 245);
        int scoreTextXOffset = 0;

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            WindowInitialization(windowWidth, windowHeight);
            windowCenter = [windowWidth / 2, windowHeight / 2];

        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.DarkGray);

            DrawBackground();
            HandlePlayerScore();
            HandleInput();

        }


        // Window Setup and Initialization
        void WindowInitialization(int windowWidth, int windowHeight)
        {
            Window.SetTitle("callaway-brandon-a2-falling-game");
            Window.SetSize(windowWidth, windowHeight);
            Window.TargetFPS = 30;
        }

        void DrawBackground()
        {
            
        }

        void HandleInput()
        {
            if (Input.IsKeyboardKeyDown(KeyboardInput.W))
            {
            }
        }

        // Draw score text with a black background, check score for win state
        void HandlePlayerScore()
        {
            int textX = 25;
            int textY = 350;
            

            Draw.FillColor = Color.Black;
            Draw.Rectangle(textX - 2, textY - 2, 110 + scoreTextXOffset, 25);

            Text.Size = 25;
            Text.Color = textColor;
            Text.Draw($"SCORE: {playerScore}", textX, textY);

            if (playerScore > 9 && playerScore < 11)
            {
                scoreTextXOffset += 12;
            }
            else if (playerScore > 99 && playerScore < 101)
            {
                scoreTextXOffset += 14;
            }
            else if (playerScore > 999 && playerScore < 1001)
            {
                scoreTextXOffset += 15;
            }
        }
    }
}
