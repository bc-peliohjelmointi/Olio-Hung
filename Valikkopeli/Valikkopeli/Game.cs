using RayGuiCreator;
using Raylib_cs;
using System;
using System.Collections.Generic;
using Asteroid;

namespace Valikkopeli
{
    enum GameState
    {
        MainMenu,
        OptionsMenu,
        DifficultyMenu,
        Asteroid,
        PauseMenu,
        LevelCompleteMenu,
        DeathScreen,
        Quit
    }

    internal class Game
    {
        Stack<GameState> stateStack = new Stack<GameState>();

        OptionsMenu optionsMenu;
        PauseMenu pauseMenu;
        LevelCompleteMenu levelCompleteMenu;
        DifficultyMenu difficultyMenu;
        DeathScreenMenu deathMenu;

        AsteroidGame asteroidGame;
        bool isPaused = false;
        GameDifficulty selectedDifficulty = GameDifficulty.Normal;

        public void Run()
        {
            Raylib.InitWindow(800, 650, "Valikkopeli Menu");
            Raylib.SetTargetFPS(60);
            Raylib.SetExitKey(KeyboardKey.Null);

            MenuCreator.SetDefaultFont("CreatoDisplay-Regular.otf");
            MenuCreator.SetTextColors(
                Raylib.ColorToInt(Color.Yellow),
                Raylib.ColorToInt(Color.Black),
                Raylib.ColorToInt(Color.DarkGray));
            MenuCreator.SetBackgroundColors(
                Raylib.ColorToInt(Color.DarkGray),
                Raylib.ColorToInt(Color.Yellow),
                Raylib.ColorToInt(Color.Yellow));

            optionsMenu = new OptionsMenu();
            optionsMenu.BackButtonPressed += OnOptionsBackPressed;

            pauseMenu = new PauseMenu();
            pauseMenu.ResumeButtonPressed += OnPauseResumePressed;
            pauseMenu.OptionsButtonPressed += OnPauseOptionsPressed;
            pauseMenu.ExitButtonPressed += OnPauseExitPressed;

            levelCompleteMenu = new LevelCompleteMenu();
            levelCompleteMenu.ContinueButtonPressed += OnLevelContinuePressed;
            levelCompleteMenu.ExitButtonPressed += OnLevelExitPressed;

            difficultyMenu = new DifficultyMenu();
            difficultyMenu.DifficultySelected += OnDifficultySelected;

            deathMenu = new DeathScreenMenu();
            deathMenu.RestartPressed += OnDeathRestartPressed;
            deathMenu.ExitPressed += OnDeathExitPressed;

            stateStack.Push(GameState.MainMenu);

            while (!Raylib.WindowShouldClose() && PeekState() != GameState.Quit)
            {
                Update();
                Draw();
            }

            Raylib.CloseWindow();
        }

        GameState PeekState() => stateStack.Count > 0 ? stateStack.Peek() : GameState.Quit;
        void PushState(GameState state) => stateStack.Push(state);
        void PopState() { if (stateStack.Count > 1) stateStack.Pop(); }
        void ClearStates() => stateStack.Clear();

        void OnOptionsBackPressed(object sender, EventArgs e) => PopState();
        void OnPauseResumePressed(object sender, EventArgs e) { isPaused = false; PopState(); }
        void OnPauseOptionsPressed(object sender, EventArgs e) => PushState(GameState.OptionsMenu);

        void OnPauseExitPressed(object sender, EventArgs e)
        {
            isPaused = false;
            ClearStates();
            PushState(GameState.MainMenu);
            asteroidGame = null;
        }

        void OnLevelContinuePressed(object sender, EventArgs e)
        {
            PopState();
            asteroidGame.ContinueNextLevel();
            PushState(GameState.Asteroid);
        }

        void OnLevelExitPressed(object sender, EventArgs e)
        {
            ClearStates();
            PushState(GameState.MainMenu);
            asteroidGame = null;
        }

        void OnDeathRestartPressed(object sender, EventArgs e)
        {
            ClearStates();
            PushState(GameState.Asteroid);
            asteroidGame = new AsteroidGame((Asteroid.Difficulty)selectedDifficulty);
            isPaused = false;
        }

        void OnDeathExitPressed(object sender, EventArgs e)
        {
            ClearStates();
            PushState(GameState.MainMenu);
            asteroidGame = null;
        }

        void OnDifficultySelected(object sender, GameDifficulty diff)
        {
            if ((int)diff == -1)
            {
                PopState();
                return;
            }

            selectedDifficulty = diff;
            ClearStates();
            PushState(GameState.Asteroid);
            asteroidGame = null;
            isPaused = false;
        }

        void Update()
        {
            switch (PeekState())
            {
                case GameState.MainMenu:
                case GameState.DifficultyMenu:
                case GameState.LevelCompleteMenu:
                case GameState.PauseMenu:
                case GameState.OptionsMenu:
                case GameState.DeathScreen:
                    break;

                case GameState.Asteroid:
                    if (asteroidGame == null)
                        asteroidGame = new AsteroidGame((Asteroid.Difficulty)selectedDifficulty);

                    if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                    {
                        isPaused = true;
                        PushState(GameState.PauseMenu);
                    }

                    asteroidGame.UpdateGameFrame(isPaused);

                    if (asteroidGame.LevelComplete)
                        PushState(GameState.LevelCompleteMenu);

                    if (asteroidGame.Lives <= 0)
                    {
                        PushState(GameState.DeathScreen);
                    }

                    break;
            }
        }

        void Draw()
        {
            Raylib.BeginDrawing();

            switch (PeekState())
            {
                case GameState.MainMenu:
                case GameState.OptionsMenu:
                case GameState.PauseMenu:
                case GameState.LevelCompleteMenu:
                case GameState.DifficultyMenu:
                    Raylib.ClearBackground(Color.DarkPurple);
                    break;
                case GameState.Asteroid:
                case GameState.DeathScreen:
                    Raylib.ClearBackground(Color.Black);
                    break;
            }

            if (asteroidGame != null && PeekState() != GameState.MainMenu && PeekState() != GameState.DifficultyMenu)
            {
                asteroidGame.DrawGameFrame();
            }

            if (isPaused && PeekState() == GameState.Asteroid)
                Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), new Color(0, 0, 0, 150));

            switch (PeekState())
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.DifficultyMenu:
                    difficultyMenu.Draw();
                    break;
                case GameState.Asteroid:
                    break;
                case GameState.PauseMenu:
                    pauseMenu.Draw();
                    break;
                case GameState.OptionsMenu:
                    optionsMenu.Draw();
                    break;
                case GameState.LevelCompleteMenu:
                    levelCompleteMenu.Draw(asteroidGame.Level, asteroidGame.Score);
                    break;
                case GameState.DeathScreen:
                    deathMenu.Draw(asteroidGame != null ? asteroidGame.Score : 0);
                    break;
            }

            Raylib.EndDrawing();
        }

        void DrawMainMenu()
        {
            int menuWidth = (int)(Raylib.GetScreenWidth() * 0.4f);
            int menuX = Raylib.GetScreenWidth() / 2 - menuWidth / 2;
            int menuY = Raylib.GetScreenHeight() / 4;
            int rowHeight = 48;

            MenuCreator mainMenu = new MenuCreator(menuX, menuY, rowHeight, menuWidth, 2);

            if (mainMenu.Button("Start Game"))
            {
                PushState(GameState.DifficultyMenu);
                asteroidGame = null;
                isPaused = false;
            }

            if (mainMenu.Button("Options"))
                PushState(GameState.OptionsMenu);

            if (mainMenu.Button("Exit"))
            {
                ClearStates();
                PushState(GameState.Quit);
            }
        }
    }
}
