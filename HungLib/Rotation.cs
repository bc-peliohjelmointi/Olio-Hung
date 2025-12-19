namespace hunglib
{
    public class RotationComponent : IComponent
    {
        public float RotationSpeed;

        public RotationComponent(float speed)
        {
            RotationSpeed = speed;
        }

        public void Update(float dt, TransformComponent transform)
        {
            transform.Rotation += RotationSpeed * dt;
        }

        public void Update(float dt) { }
    }
}
