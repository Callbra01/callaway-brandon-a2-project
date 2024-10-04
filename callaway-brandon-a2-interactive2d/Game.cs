// Include code libraries you need below (use the namespace).
using Raylib_cs;
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
        float spriteSizeScalar = 4f;

        // Player variables
        int playerClassIndex = 0;
        int playerScore;
        int playerMovementSpeed = 2;
        int playerSize = 40;
        int playerAreaOffset = 25;
        float[] playerPosition = [0, 0];
        Vector4 playerCollisionBox = new Vector4(0, 0, 0, 0);
        Color playerColor = new Color(255, 255, 0, 10);

        // Background variables
        int bgColorIndex = 0;
        Color backgroundColor = new Color(0, 0, 0);
        Color backgroundColor2 = new Color(255, 255, 255);
        Color backgroundColor3;

        Vector4 powerUp1Position = new Vector4(90, 90, 0, 0);
        Color powerUpColor = new Color(50, 50, 255);

        // Text var
        Color textColor = new Color(235, 235, 215);
        int scoreTextXOffset = 0;

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        
        // Setup window, Set player to center of screen
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
            

            DrawBackground();
            HandleInput();

            HandlePowerUpSprites();
            isSpriteColliding(powerUp1Position, playerCollisionBox);
            HandleSpriteCollision(powerUp1Position, playerCollisionBox);
            HandlePlayerSprite(playerClassIndex);
            HandlePlayerScore();
            //Console.WriteLine($"X: {playerCollisionBox.X}, Y: {playerCollisionBox.Y}, X+S: {playerCollisionBox.Z}, Y+S: {playerCollisionBox.W}");
        }

        // Window Setup and Initialization
        void WindowInitialization(int windowWidth, int windowHeight)
        {
            Window.SetTitle("callaway-brandon-a2-game");
            Window.SetSize(windowWidth, windowHeight);
            Window.TargetFPS = 60;
        }

        void DrawBackground()
        {
            Window.ClearBackground(Color.Black);
            // Change tunnel color when player reaches a specific score
            if (bgColorIndex == 0)
            {
                backgroundColor = new Color(50, 0, 0);
            }
            else if (bgColorIndex == 1)
            {
                backgroundColor = new Color(0, 50, 0);
            }
            else if (bgColorIndex == 2)
            {
                backgroundColor = new Color(0, 0, 50);
            }
        }

        // Check for player input and change player position accordingly, and store last position
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

        void HandleSpriteCollision(Vector4 spritePos, Vector4 playerPos)
        {
            playerCollisionBox.X = playerPosition[0];
            playerCollisionBox.Y = playerPosition[1];
            playerCollisionBox.Z = playerPosition[0] + playerSize;
            playerCollisionBox.W = playerPosition[1] + playerSize;

            /* TODO: FIX THIS LATER
            if (playerPosition == powerUp1Position)
            {
                testPowerUpPos = [800, 800];
                powerUpColor = Color.Clear;
            }
            */
        }

        float[] getSpriteCenter(Vector4 spritePos, int spriteSize)
        {
            float[] spriteCenterPosition = [0, 0, 0, 0];

            spriteCenterPosition = [spritePos.X, spritePos.Y, spritePos.X + spriteSize, spritePos.Y + spriteSize];

            return spriteCenterPosition;
        }

        void isSpriteColliding(Vector4 spritePos, Vector4 playerPos)
        {

            
            if (spritePos.X >= playerPos.X && spritePos.X <= playerPos.Z && spritePos.Y >= playerPos.Y && spritePos.Y <= playerPos.W)
            {
                // Console.WriteLine("COLLISION  IS WORKING LMAO")
                Console.WriteLine($"SPRITE_X: {spritePos.X}, Y: {spritePos.Y}");
            }
            Draw.Circle(spritePos.X, spritePos.Y, 15);
            
        }

        // Draw square  at player position
        // TODO: CHANGE PLAYER SQUARE TO COMPOUND GRAPHICS
        void HandlePlayerSprite(int classIndex)
        {
            if (playerClassIndex == 0)
            {
                Draw.LineColor = Color.Clear;
                Draw.FillColor = playerColor;
                Draw.Square(playerPosition[0], playerPosition[1], playerSize);


                // Arms
                Draw.FillColor = new Color(207, 185, 151);
                Draw.Rectangle(playerPosition[0] + 4, playerPosition[1] + 12, 8, 16);
                Draw.Rectangle(playerPosition[0] + 28, playerPosition[1] + 12, 8, 16);

                // Hands
                Draw.FillColor = new Color(207, 185, 151);
                Draw.Square(playerPosition[0] + 4, playerPosition[1] + 28, 4);
                Draw.Square(playerPosition[0] + 32, playerPosition[1] + 28, 4);

                // Body
                Draw.FillColor = new Color(95, 95, 95);
                Draw.Rectangle(playerPosition[0] + 12, playerPosition[1] + 20, 16, 8);

                // Head
                Draw.FillColor = new Color(227, 205, 171);
                Draw.Rectangle(playerPosition[0] + 8, playerPosition[1] + 4, 24, 12);
                Draw.Rectangle(playerPosition[0] + 12, playerPosition[1] + 16, 16, 4);

                // Hair
                Draw.FillColor = new Color(144, 144, 144);
                Draw.Rectangle(playerPosition[0] + 12, playerPosition[1], 16, 4);
                

                // Eyes
                Draw.FillColor = Color.Black;
                Draw.Square(playerPosition[0] + 12, playerPosition[1] + 8, 4);
                Draw.Square(playerPosition[0] + 24, playerPosition[1] + 8, 4);

                // Mouth
                Draw.FillColor = new Color(180, 120, 120);
                Draw.Rectangle(playerPosition[0] + 16, playerPosition[1] + 16, 8, 4);

                // Legs
                Draw.FillColor = new Color(65, 65, 65);
                Draw.Rectangle(playerPosition[0] + 12, playerPosition[1] + 28, 8, 12);
                Draw.Rectangle(playerPosition[0] + 20, playerPosition[1] + 28, 8, 12);

                Draw.FillColor = Color.White;
                Draw.Circle(playerCollisionBox.X, playerCollisionBox.Y, 5);
                Draw.Circle(playerCollisionBox.X, playerCollisionBox.W, 5);
                Draw.Circle(playerCollisionBox.Z, playerCollisionBox.Y, 5);
                Draw.Circle(playerCollisionBox.Z, playerCollisionBox.W, 5);
            }
        }

        // Draw and manage power up sprites and position
        void HandlePowerUpSprites()
        {
            Draw.FillColor = powerUpColor;
            Draw.Square(powerUp1Position.X, powerUp1Position.Y, 50);
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
