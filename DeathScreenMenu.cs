using System;
using RayGuiCreator;
using Raylib_cs;

namespace Valikkopeli
{
    internal class DeathScreenMenu
    {
        public event EventHandler RestartPressed;
        public event EventHandler ExitPressed;

        public void Draw(int finalScore)
        {
            int menuWidth = (int)(Raylib.GetScreenWidth() * 0.4f);
            int menuX = Raylib.GetScreenWidth() / 2 - menuWidth / 2;
            int menuY = Raylib.GetScreenHeight() / 3;
            int rowHeight = 48;

            Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), new Color(0, 0, 0, 200));

            string title = "GAME OVER";
            int titleWidth = Raylib.MeasureText(title, 40);
            Raylib.DrawText(title, Raylib.GetScreenWidth() / 2 - titleWidth / 2, menuY - 100, 40, Color.Red);

            string scoreText = $"Final Score: {finalScore}";
            int scoreWidth = Raylib.MeasureText(scoreText, 30);
            Raylib.DrawText(scoreText, Raylib.GetScreenWidth() / 2 - scoreWidth / 2, menuY - 50, 30, Color.White);

            MenuCreator menu = new MenuCreator(menuX, menuY, rowHeight, menuWidth, 2);

            if (menu.Button("Restart"))
            {
                RestartPressed?.Invoke(this, EventArgs.Empty);
            }

            if (menu.Button("Main Menu"))
            {
                ExitPressed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}