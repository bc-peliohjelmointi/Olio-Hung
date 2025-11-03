using System;
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

namespace Asteroid
{
    public class Enemy
    {
        public Vector2 position;
        public Vector2 direction;
        public float speed = 100.0f;
        public Texture2D texture;
        public int hp = 3;
        public bool isAlive = true;

        private float shootCooldown = 1.0f;
        private float shootTimer = 0.0f;

        public Enemy(Vector2 startPosition, Texture2D texture)
        {
            this.position = startPosition;
            this.texture = texture;
            this.direction = new Vector2(0, 1);
        }

        public void Update(float deltaTime, Vector2 playerPosition, List<Bullet> enemyBullets, Texture2D bulletTexture)
        {
            if (!isAlive) return;

            direction = Vector2.Normalize(playerPosition - position);
            position += direction * speed * deltaTime;

            int screenW = Raylib.GetScreenWidth();
            int screenH = Raylib.GetScreenHeight();
            if (position.X < 0) position.X += screenW;
            if (position.X > screenW) position.X -= screenW;
            if (position.Y < 0) position.Y += screenH;
            if (position.Y > screenH) position.Y -= screenH;

            shootTimer -= deltaTime;
            if (shootTimer <= 0.0f)
            {
                enemyBullets.Add(new Bullet(position + direction * 20, direction, bulletTexture));
                shootTimer = shootCooldown;
            }
        }

        public void TakeDamage(int amount)
        {
            hp -= amount;
            if (hp <= 0)
                isAlive = false;
        }

        public void Draw()
        {
            if (!isAlive) return;

            float rotationDeg = MathF.Atan2(direction.Y, direction.X) * (180.0f / MathF.PI) + 90.0f;
            Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle dest = new Rectangle(position.X, position.Y, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            Raylib.DrawTexturePro(texture, source, dest, origin, rotationDeg, Color.Red);
        }
    }
}
