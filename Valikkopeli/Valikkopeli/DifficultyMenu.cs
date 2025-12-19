using System;
using RayGuiCreator;
using Raylib_cs;

namespace Valikkopeli
{
    internal class DifficultyMenu
    {
        public event EventHandler<GameDifficulty> DifficultySelected;

        public void Draw()
        {
            int menuWidth = (int)(Raylib.GetScreenWidth() * 0.4f);
            int menuX = Raylib.GetScreenWidth() / 2 - menuWidth / 2;
            int menuY = Raylib.GetScreenHeight() / 4;
            int rowHeight = 48;

            MenuCreator menu = new MenuCreator(menuX, menuY, rowHeight, menuWidth, 4);

            Raylib.DrawText("Select Difficulty", menuX + 40, menuY - 60, 32, Color.Yellow);

            if (menu.Button("Easy"))
                DifficultySelected?.Invoke(this, GameDifficulty.Easy);

            if (menu.Button("Normal"))
                DifficultySelected?.Invoke(this, GameDifficulty.Normal);

            if (menu.Button("Hard"))
                DifficultySelected?.Invoke(this, GameDifficulty.Hard);

            if (menu.Button("Back"))
                DifficultySelected?.Invoke(this, (GameDifficulty)(-1));
        }
    }
}
