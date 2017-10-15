using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DiceGame
{
    public partial class Main : Form
    {

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            string welcomePath = Environment.CurrentDirectory + @"\Data\welcomemessage.txt";
            string welcomeMessage = File.ReadAllText(welcomePath);
            MessageBox.Show(welcomeMessage, "Welcome");
            string objectPath = Environment.CurrentDirectory + @"\Data\obstacles.txt";
            ReadInObstacles(objectPath);
            PlaceObstacles();
            player1.Location = new Point(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(1, 4))), Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(1, 5))));
            player2.Location = new Point(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(2, 4))), Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(2, 5))));
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            NumberOfPlayersDialog createDialog = new NumberOfPlayersDialog();
            createDialog.Show();
            createDialog.Closed += (s, args) => DialogClosed();
        }

        private void rollButton_Click(object sender, EventArgs e)
        {
            Library.GlobalVariables.totalSpaceToMove = 0;
            RollDice();
            ChangeDiceImage(Library.GlobalVariables.diceValue1, Library.GlobalVariables.diceValue2, dice1, dice2);
            Library.GlobalVariables.goingForwards = EvaluateDice(Library.GlobalVariables.diceValue1, Library.GlobalVariables.diceValue2);
            Library.GlobalVariables.totalSpaceToMove = Library.GlobalVariables.diceValue1 + Library.GlobalVariables.diceValue2;
            Library.GlobalVariables.currentSquare = SpacePossible(Library.GlobalVariables.currentSquare);
            Library.GlobalVariables.spaceLeft = Library.GlobalVariables.totalSpaceToMove;
            GameRefresh();
            IncrementScore(Library.GlobalVariables.currentPlayer);
            TurnMove(Library.GlobalVariables.currentPlayer, Library.GlobalVariables.goingForwards);
            if (Library.GlobalVariables.twoPlayers == true)
            {
                SwitchPlayer();
            }
        }
        private void TurnMove(int currentPlayer, bool goingForwards)
        {
            for (int i = 0; i < Library.GlobalVariables.totalSpaceToMove; i++)
            {
                double modSpaceUnrounded = Library.GlobalVariables.currentSquare / 7;
                int modSpace = Convert.ToInt32(Math.Floor(modSpaceUnrounded));
                int intMoveMultiplier = MoveMultiplier(modSpace, goingForwards);
                PlayerMove(currentPlayer, intMoveMultiplier);
            }
        }
        private void PlayerMove (int currentPlayer, int intMoveMultiplier)
        {
            int nextSpace = SpacePossible(NextSquare(Library.GlobalVariables.currentSquare, Library.GlobalVariables.goingForwards)); // Can I move to the next square?
            if (nextSpace != Library.GlobalVariables.currentSquare)
            {
                int yMultiplier = ChangeY(nextSpace, Library.GlobalVariables.goingForwards);
                if (yMultiplier == 0)
                {
                    int playerX = Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(currentPlayer, 4)));
                    playerX = playerX + (44 * intMoveMultiplier);
                    Library.GlobalVariables.playerStats.SetValue(playerX, currentPlayer, 4);
                }
                else
                {
                    int playerY = Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(currentPlayer, 5)));
                    playerY = playerY + (44 * yMultiplier);
                    Library.GlobalVariables.playerStats.SetValue(playerY, currentPlayer, 5);
                }
                Library.GlobalVariables.currentSquare = nextSpace;
                ApplyMove();
            }
            
        }
        private void ApplyMove()
        {
            Library.GlobalVariables.spaceLeft -= 1;
            player1.Location = new Point(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(1, 4))), Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(1, 5))));
            player2.Location = new Point(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(2, 4))), Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(2, 5))));
            if (Library.GlobalVariables.currentSquare == 49)
            {
                CompletedGame();
            }
            System.Threading.Thread.Sleep(Library.GlobalVariables.sleepTime);
            CheckForObstacles();
        }
        private void DialogClosed()
        {
            rollButton.Show();
            startButton.Hide();
            GameRefresh();
            dice1.Show();
            dice2.Show();
            player1.Show();
            speedTrackBar.Show();
            speedLabel.Show();
            speedLabel.BringToFront();
            speedLabel.Text = "Speed: " + Library.GlobalVariables.sleepTime + " ms";
            if (Library.GlobalVariables.twoPlayers == true)
            {
                player2.Show();
            }
        }
        private void ReadInObstacles(string objectPath)
        {
            Library.GlobalVariables.obstacleStats = File.ReadAllLines(objectPath)
                   .Select(l => l.Split(',').Select(i => int.Parse(i)).ToArray())
                   .ToArray();
        }
        private void PlaceObstacles()
        {
            for (int i = 0; i < Library.GlobalVariables.obstacleStats.GetLength(0); i++)
            {
                Label lbl = this.Controls.Find("label" + Library.GlobalVariables.obstacleStats[i][0], true).FirstOrDefault() as Label;
                int obstacleX = Convert.ToInt32(lbl.Location.X) - 9;
                int obstacleY = Convert.ToInt32(lbl.Location.Y) + 7;
                PictureBox obstacle = new PictureBox();
                if (Library.GlobalVariables.obstacleStats[i][1] < 0)
                {
                    obstacle.Image = Properties.Resources.badObstacle;
                }
                else
                {
                    obstacle.Image = Properties.Resources.goodObstacle;
                }
                obstacle.Location = new Point(obstacleX, obstacleY);
                obstacle.Size = new Size(17, 17);
                obstacle.SizeMode = PictureBoxSizeMode.Zoom;
                this.Controls.Add(obstacle);
                obstacle.Show();
                obstacle.BringToFront();
            }
        }
        private void CheckForObstacles()
        {
            foreach (int[] array in Library.GlobalVariables.obstacleStats)
            {
                if (Library.GlobalVariables.currentSquare == array[0] && Library.GlobalVariables.spaceLeft == 0)
                {
                    if (array[1] < 0)
                    {
                        Library.GlobalVariables.totalSpaceToMove = Math.Abs(array[1]);
                        Library.GlobalVariables.goingForwards = false;
                        MessageBox.Show("You have landed on a bad obstacle. You will now go back " + Math.Abs(array[1]) + " squares.");
                    }
                    else if (array[1] > 0)
                    {
                        Library.GlobalVariables.totalSpaceToMove = array[1];
                        MessageBox.Show("You have landed on a good obstacle. You will now advance " + array[1] + " squares.");
                    }
                    TurnMove(Library.GlobalVariables.currentPlayer, Library.GlobalVariables.goingForwards);
                }
            }
        }
        private void GameRefresh()
        {
            if (Library.GlobalVariables.twoPlayers == true)
            {
                playerturnLabel.Text = "Player " + Library.GlobalVariables.currentPlayer + "'s turn";
                playerturnLabel.Show();
            }
        }

        private void RollDice()
        {
            Random rnd = new Random();
            Library.GlobalVariables.diceValue1 = rnd.Next(1, 7);
            Library.GlobalVariables.diceValue2 = rnd.Next(1, 7);
        }

        private bool EvaluateDice(int dice1, int dice2)
        {
            if (dice1 == dice2)
            {
                MessageBox.Show("You have rolled a double! You will now go back " + (dice1 + dice2) + " squares.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private int SpacePossible(int nextSpace)
        {
            if (nextSpace < 1)
            {
                return 1;
            }
            else
            {
                return nextSpace;
            }
        }

        private void CompletedGame()
        {
            Library.GlobalVariables.totalSpaceToMove = 0;
            DialogResult dialogResult = MessageBox.Show("Congratulations! You won. It took you " + Library.GlobalVariables.playerStats.GetValue(Library.GlobalVariables.currentPlayer, 2) + " attempts. Would you like to restart?", "Dice Game", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Restart();
            }
            else if (dialogResult == DialogResult.No)
            {
                Application.Exit();
                return;
            }
        }

        private void ChangeDiceImage (int diceValue1, int diceValue2, PictureBox dice1, PictureBox dice2)
        {
            switch (diceValue1)
            {
                case 1:
                    dice1.Image = Properties.Resources.dice1;
                    break;
                case 2:
                    dice1.Image = Properties.Resources.dice2;
                    break;
                case 3:
                    dice1.Image = Properties.Resources.dice3;
                    break;
                case 4:
                    dice1.Image = Properties.Resources.dice4;
                    break;
                case 5:
                    dice1.Image = Properties.Resources.dice5;
                    break;
                case 6:
                    dice1.Image = Properties.Resources.dice6;
                    break;
            }
            switch (diceValue2)
            {
                case 1:
                    dice2.Image = Properties.Resources.dice1;
                    break;
                case 2:
                    dice2.Image = Properties.Resources.dice2;
                    break;
                case 3:
                    dice2.Image = Properties.Resources.dice3;
                    break;
                case 4:
                    dice2.Image = Properties.Resources.dice4;
                    break;
                case 5:
                    dice2.Image = Properties.Resources.dice5;
                    break;
                case 6:
                    dice2.Image = Properties.Resources.dice6;
                    break;
            }
            dice1.Refresh();
            dice2.Refresh();
        }

        private void IncrementScore(int currentPlayer)
        {
            Library.GlobalVariables.playerStats.SetValue(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(currentPlayer, 2))) + 1, currentPlayer, 2);
        }

        private void SwitchPlayer()
        {
            Library.GlobalVariables.playerStats.SetValue(Library.GlobalVariables.currentSquare, Library.GlobalVariables.currentPlayer, 1);
            if (Library.GlobalVariables.currentPlayer == 2)
            {
                Library.GlobalVariables.currentPlayer -= 1;
            }
            else
            {
                Library.GlobalVariables.currentPlayer += 1;
            }
            Library.GlobalVariables.currentSquare = Int32.Parse(Convert.ToString((Library.GlobalVariables.playerStats.GetValue((Library.GlobalVariables.currentPlayer), 1))));
        }

        private int MoveMultiplier(int modSpace, bool goingForwards)
        {
            int modspaceEven = modSpace % 2;
            if (modspaceEven == 0 && goingForwards)
            {
                return 1;
            }
            else if (modspaceEven == 0 && !goingForwards)
            {
                if (new[] { 14, 28, 42 }.Contains(Library.GlobalVariables.currentSquare))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (modspaceEven != 0 && goingForwards)
            {
                return -1;
            }
            else if (modspaceEven != 0 && !goingForwards)
            {
                if (new[] { 7, 21, 35 }.Contains(Library.GlobalVariables.currentSquare))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }

        private int ChangeY (int nextSpace, bool goingForwards)
        {
            int modChangeY = nextSpace % 7;
            if (goingForwards && modChangeY == 1)
            {
                return -1;
            }
            else if (!goingForwards && modChangeY == 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        private int NextSquare (int currentSquare, bool goingForwards)
        {
            if (goingForwards)
            {
                return currentSquare + 1;
            }
            else
            {
                return currentSquare - 1;
            }
        }

        private void speedTrackBar_scroll(object sender, EventArgs e)
        {
            Library.GlobalVariables.sleepTime = speedTrackBar.Value;
            speedLabel.Text = "Speed: " + Library.GlobalVariables.sleepTime + " ms";
        }
    }
}