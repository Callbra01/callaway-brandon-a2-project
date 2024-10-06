// Include code libraries you need below (use the namespace).
using Raylib_cs;
using System;
using System.Data;
using System.Drawing;
using System.IO;
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
        int windowWidth = 400;
        int windowHeight = 400;
        int[] windowCenter = [0, 0];
        float spriteSizeScalar = 4f;

        // Scene 0: Main Menu, 1: Game, 2: Death, 3: Win
        int gameSceneCount = 0;

        // Player variables
        int playerClassIndex = 0;
        int playerScore;
        int playerHP = 100;
        float playerMovementSpeed = 2f;
        int playerSize = 40;
        int playerAreaOffset = 25;
        float[] playerPosition = [0, 0];
        float[] weaponPosition = [0, 0];
        bool playerAttacking = false;

        // Player Collision variables
        Vector4 playerCollisionBox = new Vector4(0, 0, 0, 0);
        bool isPlayerColliding = false;

        // Background variables
        int bgColorIndex = 0;
        Color backgroundColor = new Color(0, 0, 0);
        Color backgroundColor2 = new Color(255, 255, 255);
        Color backgroundColor3;
        Color wallColor = new Color(120, 120, 180);
        Vector4 warriorSelectionBox = new Vector4(100, 125, 200, 50);
        Vector4 wizardSelectionBox = new Vector4(100, 225, 200, 50);
        Vector4 wretchSelectionBox = new Vector4(100, 325, 200, 50);
        Vector4[] classSelectionBoxes = [];
        Vector4 mousePosition = new Vector4();

        // Enemy Variables
        float[] enemy1Position = [90, -90];
        Vector4 enemy1CollisionBox = new Vector4(90, 90, 0, 0);
        Color enemy1Color = new Color(50, 50, 255);
        int spriteSize = 50;
        int enemy1Size = 40;
        bool canEnemiesMove = false;

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
            UpdateCollisionBoxes();
            HandlePlayerHP();
            HandleEnemy1();
            DrawPlayerSprite(playerClassIndex);
            HandlePlayerScore();
            HandleGameScenes();
        }

        void HandleGameScenes()
        {
            if (gameSceneCount == 0)
            {
                // Get selection box vertex positions
                Vector4[] classSelectionBoxes = [warriorSelectionBox, wizardSelectionBox, wretchSelectionBox];

                // Get mouse position as vector4 for sprite collision check
                mousePosition.X = Input.GetMouseX();
                mousePosition.Y = Input.GetMouseY();
                mousePosition.Z = 20;
                mousePosition.W = 20;

                // Prevent enemies from moving, draw black overlay over screen
                canEnemiesMove = false;
                Draw.FillColor = Color.Black;
                Draw.Square(0, 0, windowWidth);

                // Draw top box and text
                Draw.FillColor = new Color(225, 110, 0, 245);
                Draw.Rectangle(0, 0, windowWidth, 50);
                Text.Color = Color.Black;
                Text.Size = 35;
                Text.Draw("Dungeon of Cupidity", 25, 0);


                // For every selection box, create a W and Z value based off size
                for (int i = 0; i < 3; i++)
                {
                    Draw.FillColor = new Color(200, 85, 0);
                    Draw.Rectangle(classSelectionBoxes[i].X, classSelectionBoxes[i].Y, 200, 50);
                    Text.Color = Color.Black;
                    Text.Draw("Warrior", classSelectionBoxes[0].X + 35, classSelectionBoxes[0].Y + 10);
                    Text.Draw("Wizard", classSelectionBoxes[1].X + 38, classSelectionBoxes[1].Y + 10);
                    Text.Draw("Wretch", classSelectionBoxes[2].X + 40, classSelectionBoxes[2].Y + 10);

                    classSelectionBoxes[i].Z = classSelectionBoxes[i].X + 200;
                    classSelectionBoxes[i].W = classSelectionBoxes[i].Y + 50;
                }

                // Check if the mouse is positioned and clicks over a given selectionbox
                for (int i = 0; i < 3; i++)
                {
                    if (Input.IsMouseButtonDown(MouseInput.Left) && isSpriteColliding(mousePosition, classSelectionBoxes[i]))
                    {
                        playerClassIndex = i;
                        gameSceneCount = 1;
                        canEnemiesMove = true;
                    }
                }
            }
            else if (gameSceneCount == 1)
            {

            }
            else if (gameSceneCount == 2)
            {
                Color redBackground = new Color(255, 0, 0, 255);
                playerHP = 1;
                
                Draw.Square(0, 0, windowWidth);
                Draw.FillColor = Color.Black;
                Draw.Square(25, 25, windowWidth - 50);

                Text.Color = Color.Red;
                Text.Draw("~~~YOU HAVE DIED~~~\n\n\n~~~~~PRESS ESC~~~~~", windowWidth / 2 - 125, windowHeight / 2);
            }
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
            // Change room color when player reaches a specific score
            if (bgColorIndex == 0)
            {
                backgroundColor = new Color(50, 50, 50);
            }
            else if (bgColorIndex == 1)
            {
                backgroundColor = new Color(0, 50, 0);
            }
            else if (bgColorIndex == 2)
            {
                backgroundColor = new Color(0, 0, 50);
            }

            Draw.FillColor = wallColor;
            Draw.Rectangle(0, 0, 40, windowHeight);
            Draw.Rectangle(windowWidth - 40, 0, 40, windowHeight);
            Draw.Rectangle(0, windowHeight - 30, windowWidth, 30);

            Window.ClearBackground(backgroundColor);
        }


        // Check for player input and change player position accordingly, and store last position
        // Only allow movement when in bounds defined by window size and custom offset
        void HandleInput()
        {
            // Player vertical movement 
            if (Input.IsKeyboardKeyDown(KeyboardInput.W) && playerPosition[1] > 0 + playerAreaOffset)
            {
                playerPosition[1] -= Time.DeltaTime * playerMovementSpeed * 100;
            }
            else if (Input.IsKeyboardKeyDown(KeyboardInput.S) && playerPosition[1] < windowHeight - playerSize - playerAreaOffset)
            {
                playerPosition[1] += Time.DeltaTime * playerMovementSpeed * 100;
            }

            // Player horizontal movement
            if (Input.IsKeyboardKeyDown(KeyboardInput.A) && playerPosition[0] > 0 + playerAreaOffset)
            {
                playerPosition[0] -= Time.DeltaTime * playerMovementSpeed * 100;
            }
            else if (Input.IsKeyboardKeyDown(KeyboardInput.D) && playerPosition[0] < windowWidth - playerSize - playerAreaOffset)
            {
                playerPosition[0] += Time.DeltaTime * playerMovementSpeed * 100;
            }

            // Player attack check
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Space))
            {
                playerAttacking = true;
            }

        }

        Vector4 InterpolateSpritePositions(Vector4 startVector, Vector4 endVector, float steps)
        {
            return (startVector + (endVector - startVector) * steps);
        }

        // Check different sprite collisions
        void UpdateCollisionBoxes()
        {
            playerCollisionBox = GetSpriteVertexPositions(playerPosition, playerSize);
            enemy1CollisionBox = GetSpriteVertexPositions(enemy1Position, enemy1Size);
            weaponPosition = [playerPosition[0] + 8 * spriteSizeScalar, playerPosition[1] + 7 * spriteSizeScalar];

            // First power up collision check
            if (isSpriteColliding(enemy1CollisionBox, playerCollisionBox))
            {
                playerHP -= 1;
            }
        }

        // Returns the 4 points of a sprite rectangle, given the position array and a size scalar
        Vector4 GetSpriteVertexPositions(float[] spritePos, int spriteSize)
        {
            Vector4 spriteVertexPositions;

            spriteVertexPositions.X = spritePos[0];
            spriteVertexPositions.Y = spritePos[1];
            spriteVertexPositions.Z = spritePos[0] + spriteSize;
            spriteVertexPositions.W = spritePos[1] + spriteSize;
            return spriteVertexPositions;
        }

        // Returns true if a given sprites vertex positions intersects the players vertex positions
        bool isSpriteColliding(Vector4 spritePos, Vector4 playerPos)
        {
            isPlayerColliding = false;
            if (spritePos.X >= playerPos.X && spritePos.X <= playerPos.Z && spritePos.Y >= playerPos.Y && spritePos.Y <= playerPos.W)
            {
                isPlayerColliding = true;
                return true;
            }
            else if (spritePos.X >= playerPos.X && spritePos.X <= playerPos.Z && spritePos.W >= playerPos.Y && spritePos.W <= playerPos.W)
            {
                isPlayerColliding = true;
                return true;
            }
            else if (spritePos.Z >= playerPos.X && spritePos.Z <= playerPos.Z && spritePos.Y >= playerPos.Y && spritePos.Y <= playerPos.W)
            {
                isPlayerColliding = true;
                return true;
            }
            else if (spritePos.Z >= playerPos.X && spritePos.Z <= playerPos.Z && spritePos.W >= playerPos.Y && spritePos.W <= playerPos.W)
            {
                isPlayerColliding = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        // Draw square  at player position
        void DrawPlayerSprite(int classIndex)
        {
            // If player selected Warrior, draw warrior and their respective weapon
            if (playerClassIndex == 0)
            {

            }
            // If player selected Wizard, draw wizard and their respective weapon
            else if (playerClassIndex == 1)
            {

            }
            // If player selected Wretch, draw wretch and their respective weapon
            else if (playerClassIndex == 2)
            {
                Draw.LineColor = Color.Clear;

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

                // Legs
                Draw.FillColor = new Color(65, 65, 65);
                Draw.Rectangle(playerPosition[0] + 12, playerPosition[1] + 28, 8, 12);
                Draw.Rectangle(playerPosition[0] + 20, playerPosition[1] + 28, 8, 12);

                // Wretch Weapon
                Draw.FillColor = new Color(89, 39, 32);
                Draw.Square(weaponPosition[0], weaponPosition[1], 4);
                Draw.Square(weaponPosition[0] + 4, weaponPosition[1] - 4, 4);
                Draw.FillColor = new Color(109, 59, 52);
                Draw.Rectangle(weaponPosition[0] + 8, weaponPosition[1] - 28, 8, 24);
                Draw.FillColor = new Color(129, 79, 72);
                Draw.Rectangle(weaponPosition[0] + 12, weaponPosition[1] - 32, 8, 12);
            }

            // Draw non-specific class shapes
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
        }

        void HandlePlayerHP()
        {
            if (playerHP <= 0)
            {
                gameSceneCount = 2;
            }
        }

        // Draw and manage power up sprites and position
        void HandleEnemy1()
        {
            if (canEnemiesMove)
            {
                enemy1CollisionBox = InterpolateSpritePositions(enemy1CollisionBox, playerCollisionBox, 0.023f);

                enemy1Position[0] = enemy1CollisionBox.X;
                enemy1Position[1] = enemy1CollisionBox.Y;
            }
            Draw.FillColor = enemy1Color;
            Draw.Square(enemy1CollisionBox.X, enemy1CollisionBox.Y, enemy1Size);

            Draw.FillColor = Color.White;


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

            // Draw player HP box and text
            Draw.Rectangle(windowWidth - 117, textY - 2, 100, 25);
            Text.Draw($"HP: {playerHP}", windowWidth - 115, textY);

        }
    }
}
