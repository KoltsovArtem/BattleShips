using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleship
{
    public partial class PlayVSComp : UserControl
    {
        public event EventHandler replay;

        public string playerName;
        public int highScore;
        public Grid[] playerGrid;
        public Grid[] compGrid;
        public List<int> hitList;
        int turnCount = 0;
        public Random random = new Random();

        int pCarrierCount = 5, cCarrierCount = 5;
        int pBattleshipCount = 4, cBattleshipCount = 4;
        int pSubmarineCount = 3, cSubmarineCount = 3;
        int pCruiserCount = 3, cCruiserCount = 3;
        int pDestroyerCount = 2, cDestroyerCount = 2;

        public PlayVSComp(Grid[] playerGrid, string playerName)
        {
            InitializeComponent();

            this.playerName = playerName;
            initiateSetup(playerGrid);
            hitList = new List<int>();
            displayHighScores(loadHighScores());

        }

        private void initiateSetup(Grid[] userGrid)
        {
            compGrid = new Grid[100];
            CompGrid.Children.CopyTo(compGrid, 0);
            for (int i = 0; i < 100; i++)
            {
                compGrid[i].Tag = "water";
            }
            setupCompGrid();
            playerGrid = new Grid[100];
            PlayerGrid.Children.CopyTo(playerGrid, 0);

            for (int i = 0; i < 100; i++)
            {
                playerGrid[i].Background = userGrid[i].Background;
                playerGrid[i].Tag = userGrid[i].Tag;
            }
        }


        private void setupCompGrid()
        {
            Random random = new Random();
            int[] shipSizes = new int[] { 2, 3, 3, 4, 5 };
            string[] ships = new string[] { "destroyer", "cruiser", "submarine", "battleship", "carrier" };
            int size, index;
            string ship;
            Orientation orientation;
            bool unavailableIndex = true;

            for (int i = 0; i < shipSizes.Length; i++)
            {
                size = shipSizes[i];
                ship = ships[i];
                unavailableIndex = true;

                if (random.Next(0, 2) == 0)
                    orientation = Orientation.Horizontal;
                else
                    orientation = Orientation.Vertical;

                if (orientation.Equals(Orientation.Horizontal))
                {
                    index = random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while ((index + size - 1) % 10 < size - 1)
                        {
                            index = random.Next(0, 100);
                        }

                        for (int j = 0; j < size; j++)
                        {
                            if (index + j > 99 || !compGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < size; j++)
                    {
                        compGrid[index + j].Tag = ship;
                    }
                }
                else
                {
                    index = random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while (index / 10 + size * 10 > 100)
                        {
                            index = random.Next(0, 100);
                        }

                        for (int j = 0; j < size * 10; j += 10)
                        {
                            if (index + j > 99 || !compGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < size * 10; j += 10)
                    {
                        compGrid[index + j].Tag = ship;
                    }
                }

            }


        }

        private void gridMouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid square = (Grid)sender;

            if (turnCount % 2 != 0)
            {
                return;
            }

            switch (square.Tag.ToString())
            {
                case "water":
                    square.Tag = "miss";
                    square.Background = new SolidColorBrush(Colors.LightGray);
                    turnCount++;
                    compTurn();
                    return;
                case "miss":
                case "hit":
                    return;
                case "destroyer":
                    cDestroyerCount--;
                    break;
                case "cruiser":
                    cCruiserCount--;
                    break;
                case "submarine":
                    cSubmarineCount--;
                    break;
                case "battleship":
                    cBattleshipCount--;
                    break;
                case "carrier":
                    cCarrierCount--;
                    break;
            }
            square.Tag = "hit";
            square.Background = new SolidColorBrush(Colors.Red);
            turnCount++;
            checkPlayerWin();
            compTurn();

        }

        private void compTurn()
        {
            hunterMode();
            turnCount++;
            checkComputerWin();
        }
        private void checkPlayerWin()
        {
            if (cCarrierCount == 0)
            {
                cCarrierCount = -1;
                MessageBox.Show("Вы потопили мой авианосец!");
            }
            if (cCruiserCount == 0)
            {
                cCruiserCount = -1;
                MessageBox.Show("Вы потопили мой крейсер!");
            }
            if (cDestroyerCount == 0)
            {
                cDestroyerCount = -1;
                MessageBox.Show("Вы потопили мой эсминец!");
            }
            if (cBattleshipCount == 0)
            {
                cBattleshipCount = -1;
                MessageBox.Show("Вы потопили мой линкор!");
            }
            if (cSubmarineCount == 0)
            {
                cSubmarineCount = -1;
                MessageBox.Show("Вы потопили мою подводную лодку!");
            }

            if (cCarrierCount == -1 && cBattleshipCount == -1 && cSubmarineCount == -1 &&
                cCruiserCount == -1 && cDestroyerCount == -1)
            {
                MessageBox.Show("Вы выиграли!");
                disableGrids();
                displayHighScores(saveHighScores(true));
            }
        }



        private void checkComputerWin()
        {
            if (pCarrierCount == 0)
            {
                pCarrierCount = -1;
                MessageBox.Show("Адмирал Кузнецов опять в ремонте!");
            }
            if (pCruiserCount == 0)
            {
                pCruiserCount = -1;
                MessageBox.Show("Ваш крейсер уничтожен!");
            }
            if (pDestroyerCount == 0)
            {
                pDestroyerCount = -1;
                MessageBox.Show("Ваш эсминец уничтожен!");
            }
            if (pBattleshipCount == 0)
            {
                pBattleshipCount = -1;
                MessageBox.Show("Ваш линкор уничтожен!");
            }
            if (pSubmarineCount == 0)
            {
                pSubmarineCount = -1;
                MessageBox.Show("Ваша подводная лодка уничтожена!");
            }

            if (pCarrierCount == -1 && pBattleshipCount == -1 && pSubmarineCount == -1 &&
                pCruiserCount == -1 && pDestroyerCount == -1)
            {
                MessageBox.Show("Вы проиграли!");
                disableGrids();
                displayHighScores(saveHighScores(false));
            }
        }
        private void disableGrids()
        {
            foreach (var element in compGrid)
            {
                if (element.Tag.Equals("water"))
                {
                    element.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (element.Tag.Equals("carrier") || element.Tag.Equals("cruiser") ||
                  element.Tag.Equals("destroyer") || element.Tag.Equals("battleship") || element.Tag.Equals("submarine"))
                {
                    element.Background = new SolidColorBrush(Colors.LightGreen);
                }
                element.IsEnabled = false;
            }
            foreach (var element in playerGrid)
            {
                if (element.Tag.Equals("water"))
                {
                    element.Background = new SolidColorBrush(Colors.LightGray);
                }
                element.IsEnabled = false;
            }

        }
        private string validateXCoordinate(string X)
        {
            if (X.Length != 1)
            {
                return "";
            }

            if (Char.IsLetter(X[0]))
            {
                return X;
            }
            return "";
        }

        private string validateYCoordinate(string Y)
        {
            if (Y.Length > 2 || Y == "")
            {
                return "";
            }

            if (int.Parse(Y) > 0 || int.Parse(Y) <= 10)
            {
                return Y;
            }
            return "";
        }

        
        private void btnStartOver_Click(object sender, RoutedEventArgs e)
        {
            replay(this, e);
        }

        private void intelligentMoves()
        {
            if (hitList.Count == 0)
            {
                hunterMode();
            }
            else
                killerMode();
        }

        private void hunterMode()
        {
            int position;
            do
            {
                position = random.Next(100);
            } while ((playerGrid[position].Tag.Equals("miss")) || (playerGrid[position].Tag.Equals("hit")));


            simpleMode(position);
        }

        private void simpleMode(int position)
        {
            if (!(playerGrid[position].Tag.Equals("water")))
            {
                switch (playerGrid[position].Tag.ToString())
                {
                    case "destroyer":
                        pDestroyerCount--;
                        break;
                    case "cruiser":
                        pCruiserCount--;
                        break;
                    case "submarine":
                        pSubmarineCount--;
                        break;
                    case "battleship":
                        pBattleshipCount--;
                        break;
                    case "carrier":
                        pCarrierCount--;
                        break;
                }
                playerGrid[position].Tag = "hit";
                playerGrid[position].Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                playerGrid[position].Tag = "miss";
                playerGrid[position].Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            string path = @"../../scores.txt";
            File.Delete(path);
            FileStream stream = File.Create(path);
            stream.Close();

            txtBlockNames.Text = "Имя";
            txtBlockWins.Text = "Победы";
            txtBlockLosses.Text = "Проигрыши";
        }

        private void killerMode()
        {
            int position;
            do
            {
                position = random.Next(hitList.Count);
            } while (playerGrid[hitList[position]].Tag.Equals("miss") || playerGrid[hitList[position]].Tag.Equals("hit"));


        }

        private List<string> saveHighScores(bool playerWins)
        {
            String filename = @"../../scores.txt";
            string[] user = { playerName, "0", "0" };
            string[] playerNames;
            int index;
            int wins = 0;
            int losses = 0;

            if (!File.Exists(filename))
            {
                FileStream stream = File.Create(filename);
                stream.Close();
            }

            List<string> players = new List<string>(File.ReadAllLines(filename));

            playerNames = new string[players.Count];

            for (index = 0; index < players.Count; index++)
            {
                playerNames[index] = players[index].Split(' ')[0];
            }
            index = binarySearch(playerNames, playerName);

            if (index > -1)
            {
                user = players[index].Split();
                players.RemoveAt(index);
            }
            else
            {
                index = -(index + 1);
            }
            if (playerWins == true)
            {
                wins = int.Parse(user[1]) + 1;
            }
            else
            {
                losses = int.Parse(user[2]) + 1;
            }
            players.Insert(index, playerName + " " + wins + " " + losses);

            File.WriteAllLines(filename, players);
            return players;
        }
        private int binarySearch(string[] players, string value)
        {

            int low = 0;
            int high = players.Length - 1;

            while (high >= low)
            {
                int middle = (low + high) / 2;

                if (players[middle].CompareTo(value) == 0)
                {
                    return middle;
                }
                if (players[middle].CompareTo(value) < 0)
                {
                    low = middle + 1;
                }
                if (players[middle].CompareTo(value) > 0)
                {
                    high = middle - 1;
                }
            }
            return -(low + 1);
        }

        private List<string> loadHighScores()
        {
            String filename = @"../../scores.txt";
            string[] playerNames;
            int index;

            if (!File.Exists(filename))
            {
                FileStream stream = File.Create(filename);
                stream.Close();
            }

            List<string> players = new List<string>(File.ReadAllLines(filename));

            playerNames = new string[players.Count];

            for (index = 0; index < players.Count; index++)
            {
                playerNames[index] = players[index].Split(' ')[0];
            }

            File.WriteAllLines(filename, players);
            return players;
        }

        private void displayHighScores(List<string> players)
        {
            string[] player;
            string names = "Имя" + Environment.NewLine;
            string wins = "Победы" + Environment.NewLine;
            string losses = "Проигрыши" + Environment.NewLine;

            for (int i = 0; i < players.Count; i++)
            {
                player = players[i].Split(' ');
                names += player[0] + Environment.NewLine;
                wins += player[1] + Environment.NewLine;
                losses += player[2] + Environment.NewLine;
            }
            txtBlockNames.Text = names;
            txtBlockWins.Text = wins;
            txtBlockLosses.Text = losses;

        }
    }
}
