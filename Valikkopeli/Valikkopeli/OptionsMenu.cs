using RayGuiCreator;
using Raylib_cs;
using System;

namespace Valikkopeli
{
    public class OptionsMenu
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
            int menuWidth = (int)(Raylib.GetScreenWidth() * (3.0f / 4.0f));
            int menuX = Raylib.GetScreenWidth() / 2 - menuWidth / 2;
            MenuCreator options = new MenuCreator(menuX, 64, 32, menuWidth);
            options.Label("Options menu");

            options.Slider("Volume", $"{settings.masterVolume,3:F2}",
                ref settings.masterVolume, 0.0f, 1.0f);
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
