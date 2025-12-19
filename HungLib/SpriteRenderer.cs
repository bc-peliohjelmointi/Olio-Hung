using System.Numerics;
using Raylib_cs;
using System;

namespace hunglib
{
    public class SpriteRendererComponent : IComponent
    {
        public Texture2D Texture;
        public Color Tint = Color.White;

        public SpriteRendererComponent(Texture2D tex)
        {
            Texture = tex;
        }

        public void Update(float dt) { }

        public void Draw(TransformComponent transform)
        {
            float rotationDeg = transform.Rotation * (180f / MathF.PI);

            float width = Texture.Width * transform.Scale;
            float height = Texture.Height * transform.Scale;

            Rectangle src = new Rectangle(0, 0, Texture.Width, Texture.Height);

            Rectangle dest = new Rectangle(
                transform.Position.X,
                transform.Position.Y,
                width,
                height
            );

            Vector2 origin = new Vector2(width / 2, height / 2);

            Raylib.DrawTexturePro(Texture, src, dest, origin, rotationDeg, Tint);
        }
    }
}