using System.Numerics;
using Raylib_cs;
using hunglib;

namespace Asteroid
{
    public class Bullet
    {
        public TransformComponent Transform;
        public PhysicsComponent Physics;
        public SpriteRendererComponent Renderer;
        public ColliderComponent Collider;
        private ScreenWrapComponent _wrapper;

        public bool IsAlive = true;
        public float Lifetime = 2.0f;

        public Bullet(Vector2 position, Vector2 direction, Texture2D texture)
        {
            float speed = 400.0f;
            Vector2 velocity = Vector2.Normalize(direction) * speed;

            float rotation = MathF.Atan2(direction.Y, direction.X) + (MathF.PI / 2f);

            Transform = new TransformComponent(position, rotation, 1.0f);
            Physics = new PhysicsComponent(velocity);
            Renderer = new SpriteRendererComponent(texture);
            _wrapper = new ScreenWrapComponent(Transform);

            float maxDimension = MathF.Max(texture.Width, texture.Height);
            Collider = new ColliderComponent(maxDimension * 0.4f);
        }

        public void Update(float deltaTime)
        {
            Physics.Update(deltaTime, Transform);
            _wrapper.Update(deltaTime);

            Lifetime -= deltaTime;
            if (Lifetime <= 0.0f)
                IsAlive = false;
        }

        public void Draw()
        {
            Renderer.Draw(Transform);
        }
    }
}