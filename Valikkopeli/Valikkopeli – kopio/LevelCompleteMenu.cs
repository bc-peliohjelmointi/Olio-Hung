using RayGuiCreator;
using Raylib_cs;
using System;

namespace Valikkopeli
{
    internal class LevelCompleteMenu
    {
        public event EventHandler ContinueButtonPressed;
        public event EventHandler ExitButtonPressed;

        public void Draw(int currentLevel, int score)
        {
            int menuWidth = (int)(Raylib.GetScreenWidth() * 0.4f);
            int menuX = Raylib.GetScreenWidth() / 2 - menuWidth / 2;
            int menuY = Raylib.GetScreenHeight() / 3;
            int rowHeight = 48;

            MenuCreator completeMenu = new MenuCreator(menuX, menuY, rowHeight, menuWidth, 2);

            completeMenu.Label($"Level {currentLevel} Complete!");
            completeMenu.Label($"Score: {score}");

            if (completeMenu.Button("Continue to Next Level"))
                ContinueButtonPressed?.Invoke(this, EventArgs.Empty);

            if (completeMenu.Button("Quit to Main Menu"))
                ExitButtonPressed?.Invoke(this, EventArgs.Empty);
        }
    }
}
