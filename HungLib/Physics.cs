using System.Numerics;

namespace hunglib
{
    public class PhysicsComponent : IComponent
    {
        public Vector2 Velocity;

        public PhysicsComponent(Vector2 velocity)
        {
            Velocity = velocity;
        }

        public void Update(float dt)
        {
            // Physics component only works when paired with a transform
        }

        public void Update(float dt, TransformComponent transform)
        {
            transform.Position += Velocity * dt;
        }
    }
}
