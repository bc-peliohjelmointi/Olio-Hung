using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Text.Json;

namespace Asteroid
{
    public enum Difficulty { Easy, Normal, Hard }

    public class HighScoreData
    {
        public int BestScore { get; set; } = 0;
        public Difficulty Difficulty { get; set; } = Difficulty.Normal;
    }

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

        public Difficulty GameDifficulty { get; private set; }

        public int Lives => lives;
        public int Level => level;
        public int Score => score;
        public bool LevelComplete { get; private set; } = false;

        bool hasPlayerActed = false;
        float spawnProtectionTimer = 3.0f;

        const string HighScoreFile = "Data/highscores.json";
        HighScoreData highScore = new HighScoreData();

        // difficulty scaling
        float scoreMultiplier = 1.0f;

        public AsteroidGame(Difficulty difficulty)
        {
            GameDifficulty = difficulty;
            InitializeCommon();
            ApplyDifficultySettings(difficulty);
            CreateLevel(level);
        }

        void InitializeCommon()
        {
            playerTexture = Raylib.LoadTexture("Data/images/playerShip2_blue.png");
            asteroidTexture = Raylib.LoadTexture("Data/images/meteorBrown_big1.png");
            bulletTexture = Raylib.LoadTexture("Data/images/laserBlue01.png");
            enemyTexture = Raylib.LoadTexture("Data/images/enemyRed1.png");
            enemyBulletTexture = Raylib.LoadTexture("Data/images/laserRed01.png");

            playerPosition = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
            playerRocket = new Rocket(playerPosition, playerTexture);
            enemy = new Enemy(new Vector2(100, 100), enemyTexture);

            LoadHighScore();
        }

        void ApplyDifficultySettings(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    scoreMultiplier = 0.8f;
                    break;
                case Difficulty.Normal:
                    scoreMultiplier = 1.0f;
                    break;
                case Difficulty.Hard:
                    scoreMultiplier = 1.5f;
                    break;
            }
        }

        void CreateLevel(int level)
        {
            asteroids.Clear();
            bullets.Clear();
            enemyBullets.Clear();

            int count = level * 2;
            if (GameDifficulty == Difficulty.Easy) count = Math.Max(1, (int)(count * 0.7f));
            if (GameDifficulty == Difficulty.Hard) count = Math.Max(1, (int)(count * 1.5f));

            for (int i = 0; i < count; i++)
            {
                Vector2 pos;
                do
                {
                    pos = new Vector2(
                        rand.Next(100, Raylib.GetScreenWidth() - 100),
                        rand.Next(100, Raylib.GetScreenHeight() - 100)
                    );
                } while (Vector2.Distance(pos, playerPosition) < 200);

                Vector2 vel = new Vector2(rand.Next(-100, 100), rand.Next(-100, 100));

                if (GameDifficulty == Difficulty.Hard) vel *= 1.3f;
                if (GameDifficulty == Difficulty.Easy) vel *= 0.8f;

                asteroids.Add(new Asteroid(pos, vel, asteroidTexture, 1.0f));
            }

            int baseHP = 3 + level;
            enemy.hp = GameDifficulty switch
            {
                Difficulty.Easy => Math.Max(1, (int)(baseHP * 0.8f)),
                Difficulty.Hard => Math.Max(1, (int)(baseHP * 1.4f)),
                _ => baseHP
            };
        }

        bool CheckCollision(Vector2 pos1, float r1, Vector2 pos2, float r2)
        {
            return Vector2.Distance(pos1, pos2) < r1 + r2;
        }

        public void UpdateGameFrame(bool isPaused)
        {
            if (isPaused) return;
            float deltaTime = Raylib.GetFrameTime();

            if (spawnProtectionTimer > 0)
                spawnProtectionTimer -= deltaTime;

            foreach (Asteroid a in asteroids) a.Update(deltaTime);
            foreach (Bullet b in bullets) b.Update(deltaTime);
            foreach (Bullet eb in enemyBullets) eb.Update(deltaTime);

            bullets.RemoveAll(b => !b.isAlive);
            enemyBullets.RemoveAll(b => !b.isAlive);

            playerRocket.Update(deltaTime);

            if (hasPlayerActed || spawnProtectionTimer <= 0)
                enemy.Update(deltaTime, playerRocket.position, enemyBullets, enemyBulletTexture);

            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                Vector2 bulletDir = playerRocket.GetDirection();
                Vector2 bulletPos = playerRocket.position + bulletDir * 30;
                bullets.Add(new Bullet(bulletPos, bulletDir, bulletTexture));
                hasPlayerActed = true;
            }

            if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.S) ||
                Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.D))
            {
                hasPlayerActed = true;
            }

            if (spawnProtectionTimer <= 0)
                CheckPlayerCollisions();

            CheckBulletCollisions();

            if (asteroids.Count == 0 && !enemy.isAlive)
            {
                LevelComplete = true;
                SaveHighScore();
            }
        }

        public void ContinueNextLevel()
        {
            LevelComplete = false;
            level++;
            lives = 5;
            enemy = new Enemy(new Vector2(rand.Next(100, 700), rand.Next(100, 500)), enemyTexture);
            CreateLevel(level);
            spawnProtectionTimer = 3f;
            hasPlayerActed = false;
        }

        public void DrawGameFrame()
        {
            Raylib.DrawText($"Level: {level}", 10, 10, 20, Color.LightGray);
            Raylib.DrawText($"Score: {score}", 10, 40, 20, Color.LightGray);
            Raylib.DrawText($"Lives: {lives}", 10, 70, 20, Color.LightGray);
            Raylib.DrawText($"Difficulty: {GameDifficulty}", 10, 100, 20, Color.LightGray);
            Raylib.DrawText($"High Score: {highScore.BestScore}", 10, 130, 20, Color.Yellow);

            foreach (Asteroid a in asteroids) a.Draw();
            foreach (Bullet b in bullets) b.Draw();
            foreach (Bullet eb in enemyBullets) eb.Draw();

            playerRocket.Draw();
            enemy.Draw();

            if (spawnProtectionTimer > 0)
            {
                Color glow = Raylib.ColorAlpha(Color.Blue, spawnProtectionTimer / 3f);
                Raylib.DrawCircleV(playerRocket.position, 45, glow);
            }
        }

        void CheckBulletCollisions()
        {
            List<Asteroid> newAsteroids = new List<Asteroid>();

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    if (CheckCollision(bullets[i].position, bullets[i].Radius, asteroids[j].position, asteroids[j].Radius))
                    {
                        bullets[i].isAlive = false;

                        int points = (int)(100 * asteroids[j].size * level * scoreMultiplier);
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

            if (enemy.isAlive)
            {
                foreach (Bullet b in bullets)
                {
                    if (CheckCollision(b.position, b.Radius, enemy.position, enemy.texture.Width / 2f))
                    {
                        b.isAlive = false;
                        enemy.TakeDamage(1);
                        if (!enemy.isAlive)
                        {
                            int bonus = (int)(500 * level * scoreMultiplier);
                            score += bonus;
                            SaveHighScore();
                        }
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
                    PlayerHit();
                    break;
                }
            }

            foreach (Bullet eb in enemyBullets)
            {
                if (CheckCollision(eb.position, eb.Radius, playerRocket.position, playerRadius))
                {
                    eb.isAlive = false;
                    PlayerHit();
                    break;
                }
            }

            if (lives < 0) lives = 0;
        }

        void PlayerHit()
        {
            lives--;
            playerRocket.position = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
            playerRocket.velocity = Vector2.Zero;
            spawnProtectionTimer = 3f;
            hasPlayerActed = false;
        }

        void LoadHighScore()
        {
            try
            {
                if (File.Exists(HighScoreFile))
                {
                    string json = File.ReadAllText(HighScoreFile);
                    highScore = JsonSerializer.Deserialize<HighScoreData>(json) ?? new HighScoreData();
                }
            }
            catch
            {
                highScore = new HighScoreData();
            }
        }

        void SaveHighScore()
        {
            if (score > highScore.BestScore)
            {
                highScore.BestScore = score;
                highScore.Difficulty = GameDifficulty;
                string json = JsonSerializer.Serialize(highScore, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory("Data");
                File.WriteAllText(HighScoreFile, json);
            }
        }
    }
}
