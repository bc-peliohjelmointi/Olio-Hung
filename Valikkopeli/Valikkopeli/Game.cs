using RayGuiCreator;
using Raylib_cs;
using System;
using System.Collections.Generic;

namespace Valikkopeli
{
    /// <summary>
    /// Enum jossa on pelin eri tilat.
    /// </summary>
    enum GameState
    {
        MainMenu,
        GameLoop,
        PauseMenu,
        OptionsMenu,
        Quit
    }

    internal class Game
    {
        GameState currentState;
        Stack<GameState> stateStack = new Stack<GameState>();

        public OptionsMenu optionsMenu;
        public PauseMenu pauseMenu;

        public void Run()
        {
            Raylib.InitWindow(640, 480, "Valikkopeli");

            optionsMenu = new OptionsMenu();
            optionsMenu.BackButtonPressed += OnOptionsBackPressed;
            Raylib.SetMasterVolume(optionsMenu.settings.masterVolume);

            pauseMenu = new PauseMenu();
            pauseMenu.BackButtonPressed += OnPauseBackPressed;
            pauseMenu.OptionsButtonPressed += OnPauseOptionsPressed;

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

            currentState = GameState.MainMenu;

            while (!Raylib.WindowShouldClose() && currentState != GameState.Quit)
            {
                Update();
                Draw();
            }
            Raylib.CloseWindow();
        }

        void OnOptionsBackPressed(object sender, EventArgs _)
        {
            if (stateStack.Count > 0)
                ChangeState(stateStack.Pop(), pushToStack: false);
            else
                ChangeState(GameState.MainMenu, pushToStack: false);
        }

        void OnPauseBackPressed(object sender, EventArgs _)
        {
            ChangeState(GameState.GameLoop);
        }

        void OnPauseOptionsPressed(object sender, EventArgs _)
        {
            ChangeState(GameState.OptionsMenu);
        }

        void Update()
        {
            switch (currentState)
            {
                case GameState.MainMenu: break;
                case GameState.GameLoop:
                    if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                        ChangeState(GameState.PauseMenu);
                    break;
                case GameState.PauseMenu:
                    pauseMenu.Update();
                    break;
                case GameState.OptionsMenu:
                    optionsMenu.Draw();
                    break;
            }
        }

        void ChangeState(GameState nextState, bool pushToStack = true)
        {
            if (pushToStack)
                stateStack.Push(currentState);

            if (nextState == GameState.MainMenu)
                stateStack.Clear();

            currentState = nextState;
        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkPurple);
            Raylib.DrawText($"{currentState}", 200, 10, 16, Color.Yellow);

            switch (currentState)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.GameLoop:
                    // Piirrä peli
                    break;
                case GameState.OptionsMenu:
                    optionsMenu.Draw();
                    break;
                case GameState.PauseMenu:
                    pauseMenu.Draw();
                    break;
            }
            Raylib.EndDrawing();
        }

        void DrawMainMenu()
        {
            MenuCreator mainMenu = new MenuCreator(60, 60, 32, 200);
            if (mainMenu.Button("Start Game"))
                ChangeState(GameState.GameLoop);
            if (mainMenu.Button("Options"))
                ChangeState(GameState.OptionsMenu);
            if (mainMenu.Button("Exit"))
                ChangeState(GameState.Quit);
        }
    }
}
