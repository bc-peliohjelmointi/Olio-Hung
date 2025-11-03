using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Asteroid
{
    public class AsteroidGame
    {
        Texture2D playerTexture;
        Rocket playerRocket;
        Vector2 playerPosition;

        List<Asteroid> asteroids = new List<Asteroid>();
        Texture2D asteroidTexture;

        List<Bullet> bullets = new List<Bullet>();
        Texture2D bulletTexture;

        Enemy enemy;
        List<Bullet> enemyBullets = new List<Bullet>();
        Texture2D enemyTexture;
        Texture2D enemyBulletTexture;

        Random rand = new Random();

        int level = 1;
        int score = 0;
        int lives = 5;

        // 🆕 Added properties for menu access
        public int Level => level;
        public int Score => score;
        public int Lives => lives;
        public bool LevelComplete { get; private set; } = false;

        public AsteroidGame()
        {
            // Load textures
            playerTexture = Raylib.LoadTexture("Data/images/playerShip2_blue.png");
            asteroidTexture = Raylib.LoadTexture("Data/images/meteorBrown_big1.png");
            bulletTexture = Raylib.LoadTexture("Data/images/laserBlue01.png");
            enemyTexture = Raylib.LoadTexture("Data/images/enemyRed1.png");
            enemyBulletTexture = Raylib.LoadTexture("Data/images/laserRed01.png");

            playerPosition = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
            playerRocket = new Rocket(playerPosition, playerTexture);

            enemy = new Enemy(new Vector2(100, 100), enemyTexture);

            CreateLevel(level);
        }

        void CreateLevel(int level)
        {
            asteroids.Clear();
            int count = level * 2;
            for (int i = 0; i < count; i++)
            {
                Vector2 pos;
                do
                {
                    pos = new Vector2(rand.Next(100, 700), rand.Next(100, 500));
                } while (Vector2.Distance(pos, playerPosition) < 100);

                Vector2 vel = new Vector2(rand.Next(-100, 100), rand.Next(-100, 100));
                asteroids.Add(new Asteroid(pos, vel, asteroidTexture, 1.0f));
            }

            // Reset flags each level
            LevelComplete = false;
        }

        bool CheckCollision(Vector2 pos1, float r1, Vector2 pos2, float r2)
        {
            return Vector2.Distance(pos1, pos2) < r1 + r2;
        }

        public void UpdateGameFrame(bool isPaused)
        {
            if (isPaused || LevelComplete) return; // 🆕 pause updating if level complete

            float deltaTime = Raylib.GetFrameTime();

            foreach (Asteroid a in asteroids) a.Update(deltaTime);
            foreach (Bullet b in bullets) b.Update(deltaTime);
            foreach (Bullet eb in enemyBullets) eb.Update(deltaTime);

            bullets.RemoveAll(b => !b.isAlive);
            enemyBullets.RemoveAll(b => !b.isAlive);

            playerRocket.Update(deltaTime);
            enemy.Update(deltaTime, playerRocket.position, enemyBullets, enemyBulletTexture);

            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                Vector2 bulletDir = playerRocket.GetDirection();
                Vector2 bulletPos = playerRocket.position + bulletDir * 30;
                bullets.Add(new Bullet(bulletPos, bulletDir, bulletTexture));
            }

            // Collisions
            CheckBulletCollisions();
            CheckPlayerCollisions();

            // 🆕 Check if level is cleared
            if (asteroids.Count == 0 && !LevelComplete)
            {
                LevelComplete = true; // 🔔 notify Game.cs to show LevelCompleteMenu
            }
        }

        public void ContinueNextLevel()
        {
            // 🆕 Called from LevelCompleteMenu when player clicks "Continue"
            level++;
            enemy = new Enemy(new Vector2(rand.Next(100, 700), rand.Next(100, 500)), enemyTexture);
            enemy.hp = 3 + level; // stronger each level
            CreateLevel(level);
            LevelComplete = false;
        }

        public void DrawGameFrame()
        {
            Raylib.DrawText($"Level: {level}", 10, 10, 20, Color.LightGray);
            Raylib.DrawText($"Score: {score}", 10, 40, 20, Color.LightGray);
            Raylib.DrawText($"Lives: {lives}", 10, 70, 20, Color.LightGray);

            foreach (Asteroid a in asteroids) a.Draw();
            foreach (Bullet b in bullets) b.Draw();
            foreach (Bullet eb in enemyBullets) eb.Draw();

            playerRocket.Draw();
            enemy.Draw();

            // 🆕 Overlay message if level complete
            if (LevelComplete)
            {
                Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), new Color(0, 0, 0, 150));
                Raylib.DrawText("LEVEL COMPLETE!", Raylib.GetScreenWidth() / 2 - 160, Raylib.GetScreenHeight() / 2 - 30, 32, Color.Yellow);
            }
        }

        void CheckBulletCollisions()
        {
            List<Asteroid> newAsteroids = new List<Asteroid>();

            // Player bullets -> asteroids
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    if (CheckCollision(bullets[i].position, bullets[i].Radius, asteroids[j].position, asteroids[j].Radius))
                    {
                        bullets[i].isAlive = false;

                        int points = (int)(100 * asteroids[j].size);
                        score += points;

                        if (asteroids[j].size > 0.25f)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                float angle = (float)(rand.NextDouble() * Math.PI * 2);
                                Vector2 dir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                                newAsteroids.Add(new Asteroid(asteroids[j].position, dir * 80, asteroidTexture, asteroids[j].size / 2f));
                            }
                        }

                        asteroids.RemoveAt(j);
                        break;
                    }
                }
            }
            asteroids.AddRange(newAsteroids);

            // Player bullets -> enemy
            if (enemy.isAlive)
            {
                foreach (Bullet b in bullets)
                {
                    if (CheckCollision(b.position, b.Radius, enemy.position, enemy.texture.Width / 2f))
                    {
                        b.isAlive = false;
                        enemy.TakeDamage(1);

                        if (!enemy.isAlive)
                            score += 500 * level;

                        break;
                    }
                }
            }
        }

        void CheckPlayerCollisions()
        {
            float playerRadius = playerTexture.Width / 2f;

            foreach (Asteroid a in asteroids)
            {
                if (CheckCollision(playerRocket.position, playerRadius, a.position, a.Radius))
                {
                    lives--;
                    playerRocket.position = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
                    playerRocket.velocity = Vector2.Zero;
                    break;
                }
            }

            foreach (Bullet eb in enemyBullets)
            {
                if (CheckCollision(eb.position, eb.Radius, playerRocket.position, playerRadius))
                {
                    lives--;
                    eb.isAlive = false;
                    playerRocket.position = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
                    playerRocket.velocity = Vector2.Zero;
                    break;
                }
            }

            if (lives < 0) lives = 0;
        }
    }
}
