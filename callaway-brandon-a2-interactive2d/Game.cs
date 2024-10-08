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
        // Window variables
        int windowWidth = 400;
        int windowHeight = 400;
        int[] windowCenter = [0, 0];

        // Scene 0: Main Menu, 1: Game, 2: Death, 3: Win
        int gameSceneCount = 0;

        // Player variables
        int playerClassIndex = 0;
        int playerScore;
        int playerHP = 100;
        int playerSize = 40;
        int playerAreaOffset = 25;
        float playerMovementSpeed = 2f;
        float[] playerPosition = [0, 0];
        float[] weaponPosition = [0, 0];
        bool playerAttacking = false;

        // Enemy variables
        int spriteSize = 50;
        int enemySize = 40;
        float[] enemyPosition = [90, -90];
        bool canEnemiesMove = false;
        Vector4 enemyCollisionBox = new Vector4(90, 90, 0, 0);

        // Player Collision variables
        bool isPlayerColliding = false;
        bool isWeaponColliding = false;
        Vector4 playerCollisionBox = new Vector4(0, 0, 0, 0);
        Vector4 playerWeaponCollisionBox = new Vector4(0, 0, 0, 0);

        // Background variables
        int bgColorIndex = 0;
        Vector4 warriorSelectionBox = new Vector4(100, 125, 200, 50);
        Vector4 wizardSelectionBox = new Vector4(100, 225, 200, 50);
        Vector4 wretchSelectionBox = new Vector4(100, 325, 200, 50);
        Vector4[] classSelectionBoxes = [];
        Vector4 mousePosition = new Vector4();

        // Color variables
        Color backgroundColor = new Color(0, 0, 0);
        Color wallColor = new Color(120, 120, 180);
        Color floorColor = new Color(20, 20, 60);
        Color enemyColor = new Color(85, 85, 100);
        Color playerSkinColor = new Color(207, 185, 151);
        Color wizardStarColor = new Color(255, 215, 0);
        Color winOverlayColor = new Color(235, 235, 235, 0);
        Color winTextColor = new Color(20, 20, 20, 0);

        // Text variables
        Color textColor = new Color(235, 235, 215);

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
            HandleEnemy();
            DrawPlayerSprite(playerClassIndex);
            UpdateCollisionBoxes();
            HandlePlayerScore();
            HandleGameScenes();
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
            Window.ClearBackground(backgroundColor);

            Draw.FillColor = floorColor;
            Draw.Square(0, 0, windowWidth);

            Draw.FillColor = wallColor;
            Draw.Rectangle(0, 0, 40, windowHeight);
            Draw.Rectangle(windowWidth - 40, 0, 40, windowHeight);
            Draw.Rectangle(0, windowHeight - 30, windowWidth, 30);
        }

        // Overlay specific screen based on the current scene count
        void HandleGameScenes()
        {
            // Main Menu scene, pause enemy movement, draw selection boxes
            if (gameSceneCount == 0)
            {
                // Get selection box vertex positions
                Vector4[] classSelectionBoxes = [warriorSelectionBox, wizardSelectionBox, wretchSelectionBox];

                // Get mouse position as vector4 for sprite collision check
                mousePosition.X = Input.GetMouseX();
                mousePosition.Y = Input.GetMouseY();
                mousePosition.Z = 20;
                mousePosition.W = 20;

                // Prevent enemies from moving, draw black overlay for main menu
                canEnemiesMove = false;
                Draw.FillColor = Color.Black;
                Draw.Square(0, 0, windowWidth);

                // Draw top box and text
                Draw.FillColor = new Color(225, 110, 0, 245);
                Draw.Rectangle(0, 0, windowWidth, 50);
                Text.Color = Color.Black;
                Text.Size = 35;
                Text.Draw("Dungeon of Cupidity", 25, 10);

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
            // Game scene
            else if (gameSceneCount == 1)
            {
                if (playerScore == 1000)
                {
                    gameSceneCount = 3;
                }
            }
            // Death overlay
            else if (gameSceneCount == 2)
            {
                playerHP = 1;
                Draw.Square(0, 0, windowWidth);
                Draw.FillColor = Color.Black;
                Draw.Square(25, 25, windowWidth - 50);

                Text.Color = Color.Red;
                Text.Draw("~~~YOU HAVE DIED~~~\n\n\n~~~~~PRESS ESC~~~~~", windowWidth / 2 - 125, windowHeight / 2 - 50);
            }
            else if (gameSceneCount == 3)
            {
                canEnemiesMove = false;
                Draw.FillColor = winOverlayColor;
                Draw.Square(0, 0, windowHeight);
                if (winOverlayColor.A < 255)
                {
                    winOverlayColor.A += 2;
                    winTextColor.A += 2;
                }

                Text.Color = Color.Black;
                Text.Size = 22;
                Text.Draw("~~YOU HAVE EARNED YOUR ESCAPE~~\n\n\n\n\n~~YOUR JOURNEY HAS NOT ENDED~~", windowWidth / 2 - 185, windowHeight / 2 - 75);
            }
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
            if (Input.IsKeyboardKeyDown(KeyboardInput.Space))
            {
                playerAttacking = true;
            }
            else
            {
                playerAttacking = false;
            }
        }

        // Linearly interpolate a sprite's vector to a given vector's position
        // Vector4.Lerp only accepts a minimum step value of 0.1f, which is makes the enemy too fast
        Vector4 InterpolateSpritePositions(Vector4 startVector, Vector4 endVector, float steps)
        {
            return (startVector + (endVector - startVector) * steps);
        }

        // Check different sprite collisions
        void UpdateCollisionBoxes()
        {
            // Initialize and update collision boxes 
            playerCollisionBox = GetSpriteVertexPositions(playerPosition, playerSize);
            enemyCollisionBox = GetSpriteVertexPositions(enemyPosition, enemySize);
            // Weapon position array is used for positioning weapon sprite
            if (playerAttacking)
            {
                // Change weapon attack animation depending on class index
                if (playerClassIndex == 0)
                {
                    weaponPosition = [playerPosition[0] + 8 * 4, playerPosition[1]];
                }
                else if (playerClassIndex == 1)
                {
                    weaponPosition = [playerPosition[0] + 8 * 4, playerPosition[1] + 3 * 4];
                }
                else if (playerClassIndex == 2)
                {
                    weaponPosition = [playerPosition[0] + 8 * 4, playerPosition[1] + 3 * 4];
                }

            }
            else
            {
                weaponPosition = [playerPosition[0] + 8 * 4, playerPosition[1] + 7 * 4];
            }

            // Weapon collision box is for the actual damage check
            playerWeaponCollisionBox = GetSpriteVertexPositions([playerPosition[0] + 4, playerPosition[1] - playerSize + 5], playerSize - 8);
            Draw.FillColor = Color.White;

            if (isSpriteColliding(enemyCollisionBox, playerWeaponCollisionBox) && playerAttacking)
            {
                canEnemiesMove = false;
                playerScore += 100;
            }
            else if (gameSceneCount == 1)
            {
                canEnemiesMove = true;
            }
            // First power up collision check
            if (isSpriteColliding(enemyCollisionBox, playerCollisionBox))
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
            // PlayerHP check
            if (playerHP <= 0)
            {
                gameSceneCount = 2;
            }

            Draw.LineColor = Color.Clear;
            // If player selected Warrior, draw warrior and their respective weapon
            if (playerClassIndex == 0)
            {
                // Arms
                Draw.FillColor = playerSkinColor;
                Draw.Rectangle(playerPosition[0], playerPosition[1] + 16, 8, 16);
                Draw.Rectangle(playerPosition[0] + 4, playerPosition[1] + 12, 8, 12);
                Draw.Rectangle(playerPosition[0] + 32, playerPosition[1] + 16, 8, 16);
                Draw.Rectangle(playerPosition[0] + 28, playerPosition[1] + 12, 8, 12);

                // Body
                Draw.FillColor = new Color(95, 95, 95);
                Draw.Rectangle(playerPosition[0] + 12, playerPosition[1] + 20, 16, 8);

                // Legs
                Draw.FillColor = new Color(65, 65, 65);
                Draw.Rectangle(playerPosition[0] + 12, playerPosition[1] + 28, 8, 12);
                Draw.Rectangle(playerPosition[0] + 20, playerPosition[1] + 28, 8, 12);
                Draw.Rectangle(playerPosition[0] + 8, playerPosition[1] + 36, 4, 4);
                Draw.Rectangle(playerPosition[0] + 28, playerPosition[1] + 36, 4, 4);

                // Warrior Weapon
                Draw.FillColor = new Color(109, 59, 52);
                Draw.Rectangle(weaponPosition[0] + 4, weaponPosition[1], 4, 12);
                Draw.FillColor = new Color(144, 144, 144);
                Draw.Rectangle(weaponPosition[0] - 4, weaponPosition[1] - 4, 20, 4);
                Draw.Rectangle(weaponPosition[0] + 4, weaponPosition[1] - 32, 4, 28);

                Draw.FillColor = new Color(210, 210, 210);
                Draw.Rectangle(weaponPosition[0] + 8, weaponPosition[1] - 28, 4, 20);
            }
            // If player selected Wizard, draw wizard and their respective weapon
            else if (playerClassIndex == 1)
            {
                // Arms
                Draw.FillColor = playerSkinColor;
                Draw.Rectangle(playerPosition[0] + 4, playerPosition[1] + 12, 8, 16);
                Draw.Rectangle(playerPosition[0] + 28, playerPosition[1] + 12, 8, 16);

                // Hands
                Draw.FillColor = playerSkinColor;
                Draw.Square(playerPosition[0] + 4, playerPosition[1] + 28, 4);
                Draw.Square(playerPosition[0] + 32, playerPosition[1] + 28, 4);

                // Body
                Draw.FillColor = new Color(50, 50, 80);
                Draw.Rectangle(playerPosition[0] + 12, playerPosition[1] + 20, 16, 4);
                Draw.Rectangle(playerPosition[0] + 12, playerPosition[1] + 24, 16, 4);
                Draw.Rectangle(playerPosition[0] + 8, playerPosition[1] + 28, 24, 4);
                Draw.Rectangle(playerPosition[0] + 4, playerPosition[1] + 32, 32, 4);
                Draw.Rectangle(playerPosition[0] + 4, playerPosition[1] + 36, 32, 4);

                // Robe
                Draw.FillColor = wizardStarColor;
                Draw.Square(playerPosition[0] + 4, playerPosition[1] + 36, 4);
                Draw.Square(playerPosition[0] + 12, playerPosition[1] + 36, 4);
                Draw.Square(playerPosition[0] + 24, playerPosition[1] + 36, 4);
                Draw.Square(playerPosition[0] + 32, playerPosition[1] + 36, 4);

                // Wizard Weapon
                Draw.FillColor = new Color(109, 95, 70);
                Draw.Rectangle(weaponPosition[0] + 4, weaponPosition[1] - 4, 4, 16);
                Draw.Rectangle(weaponPosition[0] + 4, weaponPosition[1] - 32, 4, 16);
                Draw.Rectangle(weaponPosition[0] + 16, weaponPosition[1] - 32, 4, 16);
                Draw.Rectangle(weaponPosition[0] - 8, weaponPosition[1] - 32, 4, 16);
                Draw.Rectangle(weaponPosition[0] - 8, weaponPosition[1] - 16, 28, 4);
                Draw.FillColor = wizardStarColor;
                Draw.Rectangle(weaponPosition[0], weaponPosition[1] - 40, 12, 12);
            }
            // If player selected Wretch, draw wretch and their respective weapon
            else if (playerClassIndex == 2)
            {
                // Arms
                Draw.FillColor = playerSkinColor;
                Draw.Rectangle(playerPosition[0] + 4, playerPosition[1] + 12, 8, 16);
                Draw.Rectangle(playerPosition[0] + 28, playerPosition[1] + 12, 8, 16);

                // Hands
                Draw.FillColor = playerSkinColor;
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

        void DrawEnemy()
        {
            enemyCollisionBox = InterpolateSpritePositions(enemyCollisionBox, playerCollisionBox, 0.023f);
            enemyPosition[0] = enemyCollisionBox.X;
            enemyPosition[1] = enemyCollisionBox.Y;

            Draw.FillColor = enemyColor;
            Draw.Rectangle(enemyPosition[0] + 4, enemyPosition[1], 8, 4);
            Draw.Rectangle(enemyPosition[0] + 28, enemyPosition[1], 8, 4);
            Draw.Rectangle(enemyPosition[0] + 8, enemyPosition[1] + 4, 24, 28);
            Draw.Rectangle(enemyPosition[0] + 4, enemyPosition[1] + 8, 32, 12);
            Draw.Rectangle(enemyPosition[0] + 4, enemyPosition[1] + 24, 32, 8);
            Draw.Rectangle(enemyPosition[0] + 4, enemyPosition[1] + 32, 12, 4);
            Draw.Rectangle(enemyPosition[0] + 24, enemyPosition[1] + 32, 12, 4);
            Draw.Square(enemyPosition[0] + 4, enemyPosition[1] + 36, 4);
            Draw.Square(enemyPosition[0] + 12, enemyPosition[1] + 36, 4);
            Draw.Square(enemyPosition[0] + 24, enemyPosition[1] + 36, 4);
            Draw.Square(enemyPosition[0] + 32, enemyPosition[1] + 36, 4);

            Draw.FillColor = Color.Red;
            Draw.Square(enemyPosition[0] + 12, enemyPosition[1] + 8, 4);
            Draw.Square(enemyPosition[0] + 24, enemyPosition[1] + 8, 4);
            Draw.Square(enemyPosition[0] + 28, enemyPosition[1] + 12, 4);
            Draw.Square(enemyPosition[0] + 8, enemyPosition[1] + 12, 4);

            Draw.FillColor = Color.OffWhite;
            Draw.Rectangle(enemyPosition[0] + 12, enemyPosition[1] + 20, 4, 8);
            Draw.Rectangle(enemyPosition[0] + 24, enemyPosition[1] + 20, 4, 8);
        }

        // If out of main menu, allow enemy movement via linear interpolation of vector4 positions
        void HandleEnemy()
        {
            if (canEnemiesMove && playerScore < 1000)
            {
                DrawEnemy();
            }
            else
            {
                enemyPosition = [Random.Integer(10, 350), -90];
            }
        }

        // Draw score text with a black background, check score for win state
        void HandlePlayerScore()
        {
            // Score box and text
            Draw.FillColor = Color.Black;
            Draw.Rectangle(13, 373, 110 + 50, 25);

            Text.Size = 25;

            if (playerScore >= 1000)
            {
                Text.Color = new Color(255, 255, 0);
            }
            else
            {
                Text.Color = textColor;
            }
            Text.Draw($"SCORE: {playerScore}", 15, 375);

            // Player HP box and text
            Draw.Rectangle(windowWidth - 117, 373, 100, 25);
            if (playerHP <= 50)
            {
                Text.Color = Color.Red;
            }
            else
            {
                Text.Color = textColor;
            }
            Text.Draw($"HP: {playerHP}", windowWidth - 115, 375);
        }
    }
}
