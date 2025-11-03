using System;
using System.Numerics;
using Raylib_cs;

namespace ASTEROIDS
{
    class Rocket
    {
        public Vector2 position;
        public Vector2 direction;
        public float rotationRadians;
        public float turnSpeedRadians = MathF.PI;
        public float acceleration = 200.0f;
        public float maxSpeed = 300.0f;
        public Vector2 velocity = Vector2.Zero;

        public Texture2D texture;
        private bool engineOn = false;

        public Rocket(Vector2 startPosition, Texture2D texture)
        {
            this.position = startPosition;
            this.texture = texture;
            this.direction = new Vector2(0.0f, -1.0f);
        }

        public void Update(float deltaTime)
        {
            if (Raylib.IsKeyDown(KeyboardKey.A))
                rotationRadians -= turnSpeedRadians * deltaTime;
            if (Raylib.IsKeyDown(KeyboardKey.D))
                rotationRadians += turnSpeedRadians * deltaTime;

            Matrix4x4 rotMatrix = Matrix4x4.CreateRotationZ(rotationRadians);
            direction = Vector2.Transform(new Vector2(0.0f, -1.0f), rotMatrix);

            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                velocity += direction * acceleration * deltaTime;
                engineOn = true;
            }
            else
            {
                engineOn = false;
            }

            if (velocity.Length() > maxSpeed)
                velocity = Vector2.Normalize(velocity) * maxSpeed;

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
            float rotationDeg = rotationRadians * (180.0f / MathF.PI);
            Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle dest = new Rectangle(position.X, position.Y, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            Raylib.DrawTexturePro(texture, source, dest, origin, rotationDeg, Color.White);
        }

        public Vector2 GetDirection()
        {
            return direction;
        }
    }
}