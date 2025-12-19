using System.Numerics;

namespace hunglib
{
    public class TransformComponent : IComponent
    {
        public Vector2 Position;
        public float Rotation;   // Radians
        public float Scale;

        public TransformComponent(Vector2 position, float rotation = 0f, float scale = 1f)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public void Update(float dt) { }

        // Wrap object around screen edges
        public void WrapScreen()
        {
            int w = Raylib_cs.Raylib.GetScreenWidth();
            int h = Raylib_cs.Raylib.GetScreenHeight();

            if (Position.X < 0) Position.X += w;
            if (Position.X > w) Position.X -= w;
            if (Position.Y < 0) Position.Y += h;
            if (Position.Y > h) Position.Y -= h;
        }
    }
}
