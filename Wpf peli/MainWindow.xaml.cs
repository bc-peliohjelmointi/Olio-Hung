using System.Windows;
using System.Windows.Controls;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private bool playerXTurn = true;
        private Button[] buttons;

        public MainWindow()
        {
            InitializeComponent();
            buttons = new Button[] { B0, B1, B2, B3, B4, B5, B6, B7, B8 };
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            if (b.Content != null) return;

            b.Content = playerXTurn ? "X" : "O";
            playerXTurn = !playerXTurn;

            CheckWinner();
        }

        private void CheckWinner()
        {
            int[,] wins = new int[,]
            {
                {0,1,2},
                {3,4,5},
                {6,7,8},
                {0,3,6},
                {1,4,7},
                {2,5,8},
                {0,4,8},
                {2,4,6}
            };

            for (int i = 0; i < wins.GetLength(0); i++)
            {
                string a = buttons[wins[i, 0]].Content?.ToString();
                string b = buttons[wins[i, 1]].Content?.ToString();
                string c = buttons[wins[i, 2]].Content?.ToString();

                if (a == b && b == c && a != null)
                {
                    MessageBox.Show($"{a} wins!");
                    DisableBoard();
                    return;
                }
            }

            bool draw = true;
            foreach (Button btn in buttons)
            {
                if (btn.Content == null)
                    draw = false;
            }

            if (draw)
            {
                MessageBox.Show("Draw!");
            }
        }

        private void ResetGame(object sender, RoutedEventArgs e)
        {
            foreach (Button b in buttons)
                b.Content = null;

            playerXTurn = true;
            EnableBoard();
        }

        private void DisableBoard()
        {
            foreach (Button b in buttons)
                b.IsEnabled = false;
        }

        private void EnableBoard()
        {
            foreach (Button b in buttons)
                b.IsEnabled = true;
        }
    }
}
