using Asteroid; // Make sure you added this!
using Valikkopeli;

namespace Valikkopeli
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Start the menu, which now controls the Asteroid game
            Game menu = new Game();
            menu.Run();

            // DO NOT call asteroidGame.Run() here!
            // The Asteroid game is now run through the menu's state system
        }
    }
}
