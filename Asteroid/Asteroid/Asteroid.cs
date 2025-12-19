using System.Numerics;
using Raylib_cs;
using hunglib;

namespace Asteroid
{
    class Asteroid
    {
        public TransformComponent Transform;
        public PhysicsComponent Physics;
        public SpriteRendererComponent Renderer;
        public ColliderComponent Collider;

        private RotationComponent _rotator;
        private ScreenWrapComponent _wrapper;

        public float Size;

        public Asteroid(Vector2 position, Vector2 velocity, Texture2D texture, float size)
        {
            Size = size;

            Transform = new TransformComponent(position, 0f, size);
            Physics = new PhysicsComponent(velocity);
            Renderer = new SpriteRendererComponent(texture);

            float rotSpeed = (velocity.X + velocity.Y) * 0.01f;
            _rotator = new RotationComponent(rotSpeed);
            _wrapper = new ScreenWrapComponent(Transform);

            float radius = ((texture.Width * size) / 2.0f) + 2.0f;
            Collider = new ColliderComponent(radius);
        }

        public void Update(float deltaTime)
        {
            Physics.Update(deltaTime, Transform);
            _rotator.Update(deltaTime, Transform);
            _wrapper.Update(deltaTime);
        }

        public void Draw()
        {
            Renderer.Draw(Transform);
        }
    }
}