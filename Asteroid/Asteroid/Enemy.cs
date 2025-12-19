using System;
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;
using hunglib;

namespace Asteroid
{
    public class Enemy
    {
        public TransformComponent Transform;
        public PhysicsComponent Physics;
        public SpriteRendererComponent Renderer;
        public ColliderComponent Collider;
        private ScreenWrapComponent _wrapper;

        public int Hp = 3;
        public bool IsAlive = true;

        public float Speed = 90.0f;
        private float TurnSpeed = 3.0f;

        private float shootCooldown = 1.5f;
        private float shootTimer = 0.0f;
        private float stopDistance = 180.0f;

        public Enemy(Vector2 startPosition, Texture2D texture)
        {
            Transform = new TransformComponent(startPosition);
            Physics = new PhysicsComponent(Vector2.Zero);
            Renderer = new SpriteRendererComponent(texture) { Tint = Color.Red };
            Collider = new ColliderComponent(texture.Width / 2.0f);
            _wrapper = new ScreenWrapComponent(Transform);
        }

        public void Update(float deltaTime, Vector2 playerPosition, List<Bullet> enemyBullets, Texture2D bulletTexture)
        {
            if (!IsAlive) return;

            Vector2 offset = playerPosition - Transform.Position;
            float distanceToPlayer = offset.Length();

            float targetAngle = MathF.Atan2(offset.Y, offset.X) + (MathF.PI / 2f);
            float angleDifference = targetAngle - Transform.Rotation;

            while (angleDifference < -MathF.PI) angleDifference += 2 * MathF.PI;
            while (angleDifference > MathF.PI) angleDifference -= 2 * MathF.PI;

            float maxTurn = TurnSpeed * deltaTime;

            if (MathF.Abs(angleDifference) < maxTurn)
            {
                Transform.Rotation = targetAngle;
            }
            else
            {
                Transform.Rotation += MathF.Sign(angleDifference) * maxTurn;
            }

            float rotationRad = Transform.Rotation - (MathF.PI / 2f);
            Vector2 facingDirection = new Vector2(MathF.Cos(rotationRad), MathF.Sin(rotationRad));

            if (distanceToPlayer > stopDistance)
            {
                Physics.Velocity = facingDirection * Speed;
            }
            else
            {
                Physics.Velocity = Vector2.Zero;
            }

            Physics.Update(deltaTime, Transform);
            _wrapper.Update(deltaTime);

            shootTimer -= deltaTime;

            bool isFacingPlayer = MathF.Abs(angleDifference) < 0.5f;

            if (shootTimer <= 0.0f && isFacingPlayer)
            {
                Vector2 spawnPos = Transform.Position + facingDirection * 20;

                float inaccuracy = 0.2f;
                float rndX = (Raylib.GetRandomValue(-100, 100) / 100.0f) * inaccuracy;
                float rndY = (Raylib.GetRandomValue(-100, 100) / 100.0f) * inaccuracy;

                Vector2 bulletDir = Vector2.Normalize(facingDirection + new Vector2(rndX, rndY));

                enemyBullets.Add(new Bullet(spawnPos, bulletDir, bulletTexture));
                shootTimer = shootCooldown;
            }
        }

        public void TakeDamage(int amount)
        {
            Hp -= amount;
            if (Hp <= 0)
                IsAlive = false;
        }

        public void Draw()
        {
            if (!IsAlive) return;
            Renderer.Draw(Transform);
        }
    }
}