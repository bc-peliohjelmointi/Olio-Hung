using RayGuiCreator;
using Raylib_cs;
using System;

namespace Valikkopeli
{
    internal class PauseMenu
    {
        public event EventHandler ResumeButtonPressed;
        public event EventHandler OptionsButtonPressed;
        public event EventHandler ExitButtonPressed;

        public void Draw()
        {
            int menuWidth = (int)(Raylib.GetScreenWidth() * 0.4f);
            int menuX = Raylib.GetScreenWidth() / 2 - menuWidth / 2;
            int menuY = Raylib.GetScreenHeight() / 3;
            int rowHeight = 48;

            MenuCreator pause = new MenuCreator(menuX, menuY, rowHeight, menuWidth, 2);

            if (pause.Button("Resume"))
                ResumeButtonPressed?.Invoke(this, EventArgs.Empty);

            if (pause.Button("Options"))
                OptionsButtonPressed?.Invoke(this, EventArgs.Empty);

            if (pause.Button("Exit to Main Menu"))
                ExitButtonPressed?.Invoke(this, EventArgs.Empty);
        }
    }
}
