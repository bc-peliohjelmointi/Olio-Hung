using RayGuiCreator;
using Raylib_cs;
using System;

namespace Valikkopeli
{
    public class PauseMenu
    {
        public event EventHandler OptionsButtonPressed;
        public event EventHandler BackButtonPressed;

        public void Draw()
        {
            MenuCreator pause = new MenuCreator(40, 40, 32, 200, 2);
            if (pause.Button("Options"))
                OptionsButtonPressed?.Invoke(this, EventArgs.Empty);
            if (pause.Button("Back"))
                BackButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        public void Update()
        {
            Draw();
        }
    }
}
