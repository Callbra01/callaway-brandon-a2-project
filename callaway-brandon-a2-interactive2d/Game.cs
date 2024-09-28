// Include code libraries you need below (use the namespace).
using System;
using System.Data;
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
        int[] windowCenter = [0, 0];

        // Player variables
        int playerScore;
        int playerMovementSpeed = 4;
        int playerSize = 40;
        float[] playerPosition = [0, 0];
        int playerAreaOffset = 45;


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
            playerPosition = [windowWidth / 2 - playerSize / 2, windowHeight / 2 - playerSize / 2];
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.DarkGray);

            DrawBackground();
            HandleInput();
            DrawPlayer();

            HandlePlayerScore();
        }

        // Window Setup and Initialization
        void WindowInitialization(int windowWidth, int windowHeight)
        {
            Window.SetTitle("callaway-brandon-a2-falling-game");
            Window.SetSize(windowWidth, windowHeight);
            Window.TargetFPS = 60;
        }

        void DrawBackground()
        {

        }

        // Draw square  at player position
        // TODO: CHANGE PLAYER SQUARE TO COMPOUND GRAPHICS
        void DrawPlayer()
        {
            Draw.FillColor = new Color(255, 255, 0);
            Draw.Square(playerPosition[0], playerPosition[1], playerSize);
        }

        // Check for player input and change player position accordingly
        // Only allow movement when in bounds defined by window size and custom offset
        void HandleInput()
        {
            if (Input.IsKeyboardKeyDown(KeyboardInput.W) && playerPosition[1] > 0 + playerAreaOffset)
            {
                playerPosition[1] -= Time.DeltaTime * playerMovementSpeed * 100;
            }
            else if (Input.IsKeyboardKeyDown(KeyboardInput.S) && playerPosition[1] < windowHeight - playerSize - playerAreaOffset)
            {
                playerPosition[1] += Time.DeltaTime * playerMovementSpeed * 100;
            }

            if (Input.IsKeyboardKeyDown(KeyboardInput.A) && playerPosition[0] > 0 + playerAreaOffset)
            {
                playerPosition[0] -= Time.DeltaTime * playerMovementSpeed * 100;
            }
            else if (Input.IsKeyboardKeyDown(KeyboardInput.D) && playerPosition[0] < windowWidth - playerSize - playerAreaOffset)
            {
                playerPosition[0] += Time.DeltaTime * playerMovementSpeed * 100;
            }
        }

        // Draw score text with a black background, check score for win state
        void HandlePlayerScore()
        {
            int textX = 15;
            int textY = 375;


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
