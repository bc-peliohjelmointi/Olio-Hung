using System.Numerics;

namespace hunglib
{
    public class ColliderComponent : IComponent
    {
        public float Radius;

        public ColliderComponent(float radius)
        {
            Radius = radius;
        }

        public void Update(float dt) { }

        public bool CollidesWith(TransformComponent a, TransformComponent b, ColliderComponent other)
        {
            float dist = Vector2.Distance(a.Position, b.Position);
            return dist < (Radius + other.Radius);
        }
    }
}
