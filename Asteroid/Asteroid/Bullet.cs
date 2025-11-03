using System;
using System.Numerics;
using Raylib_cs;

namespace Asteroid
{
    public class Bullet
    {
        public Vector2 position;
        public Vector2 direction;
        public float speed = 400.0f;
        public float lifetime = 2.0f;
        public bool isAlive = true;
        public Texture2D texture;
        public float Radius => texture.Width / 2.0f;

        public Bullet(Vector2 position, Vector2 direction, Texture2D texture)
        {
            this.position = position;
            this.direction = Vector2.Normalize(direction);
            this.texture = texture;
        }

        public void Update(float deltaTime)
        {
            position += direction * speed * deltaTime;
            lifetime -= deltaTime;
            if (lifetime <= 0.0f)
                isAlive = false;

            int screenW = Raylib.GetScreenWidth();
            int screenH = Raylib.GetScreenHeight();

            if (position.X < 0) position.X += screenW;
            if (position.X > screenW) position.X -= screenW;
            if (position.Y < 0) position.Y += screenH;
            if (position.Y > screenH) position.Y -= screenH;
        }

        public void Draw()
        {
            float rotationDeg = MathF.Atan2(direction.Y, direction.X) * (180.0f / MathF.PI) + 90.0f;

            Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle dest = new Rectangle(position.X, position.Y, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            Raylib.DrawTexturePro(texture, source, dest, origin, rotationDeg, Color.White);
        }
    }
}
