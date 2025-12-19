namespace hunglib
{
    public class ScreenWrapComponent : IComponent
    {
        private TransformComponent transform;

        public ScreenWrapComponent(TransformComponent t)
        {
            transform = t;
        }

        public void Update(float dt)
        {
            transform.WrapScreen();
        }
    }
}
