using RayGuiCreator;
using Raylib_cs;
using System;

namespace Valikkopeli
{
    internal class OptionsMenu
    {
        public event EventHandler BackButtonPressed;
        public GameSettings settings;
        private bool asteroidSpinnerActive = false;

        public OptionsMenu()
        {
            settings = new GameSettings();
            settings.LoadFromDisk();
        }

        public void Draw()
        {
            int menuWidth = (int)(Raylib.GetScreenWidth() * 0.5f);
            int menuX = Raylib.GetScreenWidth() / 2 - menuWidth / 2;
            int menuY = Raylib.GetScreenHeight() / 4;
            int rowHeight = 48;

            MenuCreator options = new MenuCreator(menuX, menuY, rowHeight, menuWidth);
            options.Label("Options Menu");

            options.Slider("Volume", $"{settings.masterVolume,3:F2}", ref settings.masterVolume, 0.0f, 1.0f);
            Raylib.SetMasterVolume(settings.masterVolume);

            options.Spinner("Asteroids", ref settings.asteroidAmount, 1, 10, ref asteroidSpinnerActive);

            if (options.Button("Save"))
                settings.SaveToDisk();

            if (options.Button("Reset"))
                settings = new GameSettings();

            if (options.Button("Back"))
                BackButtonPressed?.Invoke(this, EventArgs.Empty);
        }
    }
}
