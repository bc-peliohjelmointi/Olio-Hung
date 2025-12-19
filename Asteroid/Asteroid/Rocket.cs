using System;
using System.Numerics;
using Raylib_cs;
using hunglib;

namespace Asteroid
{
    public class Rocket
    {
        public TransformComponent Transform;
        public PhysicsComponent Physics;
        public SpriteRendererComponent Renderer;
        public ColliderComponent Collider;
        private ScreenWrapComponent _wrapper;

        public float TurnSpeedRadians = MathF.PI;
        public float Acceleration = 300.0f;
        public float MaxSpeed = 400.0f;

        private Vector2 _direction;

        public Rocket(Vector2 startPosition, Texture2D texture)
        {
            Transform = new TransformComponent(startPosition, 0f, 1f);
            Physics = new PhysicsComponent(Vector2.Zero);
            Renderer = new SpriteRendererComponent(texture);
            Collider = new ColliderComponent(texture.Width / 2.0f);
            _wrapper = new ScreenWrapComponent(Transform);

            _direction = new Vector2(0.0f, -1.0f);
        }

        public void Update(float deltaTime)
        {
            if (Raylib.IsKeyDown(KeyboardKey.A))
                Transform.Rotation -= TurnSpeedRadians * deltaTime;
            if (Raylib.IsKeyDown(KeyboardKey.D))
                Transform.Rotation += TurnSpeedRadians * deltaTime;

            Matrix4x4 rotMatrix = Matrix4x4.CreateRotationZ(Transform.Rotation);
            _direction = Vector2.Transform(new Vector2(0.0f, -1.0f), rotMatrix);

            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                Physics.Velocity += _direction * Acceleration * deltaTime;
            }

            if (Physics.Velocity.Length() > MaxSpeed)
                Physics.Velocity = Vector2.Normalize(Physics.Velocity) * MaxSpeed;

            Physics.Update(deltaTime, Transform);
            _wrapper.Update(deltaTime);
        }

        public void Draw()
        {
            Renderer.Draw(Transform);
        }

        public Vector2 GetDirection()
        {
            return _direction;
        }
    }
}