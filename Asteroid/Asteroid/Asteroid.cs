using System;
using System.Numerics;
using Raylib_cs;

namespace Asteroid
{
    class Asteroid
    {
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public Texture2D texture;
        public float size;
        public float Radius => (texture.Width * size) / 2.0f;

        public Asteroid(Vector2 position, Vector2 velocity, Texture2D texture, float size)
        {
            this.position = position;
            this.velocity = velocity;
            this.texture = texture;
            this.size = size;
        }

        public void Update(float deltaTime)
        {
            position += velocity * deltaTime;

            int screenW = Raylib.GetScreenWidth();
            int screenH = Raylib.GetScreenHeight();

            if (position.X < 0) position.X += screenW;
            if (position.X > screenW) position.X -= screenW;
            if (position.Y < 0) position.Y += screenH;
            if (position.Y > screenH) position.Y -= screenH;
        }

        public void Draw()
        {
            float rotationDeg = rotation * (180.0f / MathF.PI);
            Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
            float scale = size;
            Rectangle dest = new Rectangle(position.X, position.Y, texture.Width * scale, texture.Height * scale);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Raylib.DrawTexturePro(texture, source, dest, origin, rotationDeg, Color.White);
        }
    }
}
