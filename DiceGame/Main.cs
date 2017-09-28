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
            player1.Location = new Point(Library.GlobalVariables.player1x, Library.GlobalVariables.player1y);
            player2.Location = new Point(37, 170);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            NumberOfPlayersDialog createDialog = new NumberOfPlayersDialog();
            createDialog.Show();
            createDialog.Closed += (s, args) => DialogClosed();
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

        private void GameRefresh()
        {
            if (Library.GlobalVariables.twoPlayers == true)
            {
                playerturnLabel.Text = "Player " + Library.GlobalVariables.currentPlayer + "'s turn";
                playerturnLabel.Show();
            }
        }

        private void rollButton_Click(object sender, EventArgs e)
        {
            Library.GlobalVariables.totalSpaceToMove = 0;
            Random rnd = new Random();
            Library.GlobalVariables.dice1 = rnd.Next(1, 7);
            Library.GlobalVariables.dice2 = rnd.Next(1, 7);
            switch (Library.GlobalVariables.dice1)
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
            switch (Library.GlobalVariables.dice2)
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
            if (Library.GlobalVariables.dice1 == Library.GlobalVariables.dice2)
            {
                MessageBox.Show("You have rolled a double! You will now go back " + (Library.GlobalVariables.dice1 + Library.GlobalVariables.dice2) + " squares.");
            }
            Library.GlobalVariables.totalSpaceToMove = Library.GlobalVariables.dice1 + Library.GlobalVariables.dice2;
            if (Library.GlobalVariables.currentSquare < 1)
            {
                Library.GlobalVariables.currentSquare = 1;
            }
            GameRefresh();
            PlayerMove();
            Library.GlobalVariables.player1score += 1;
            if (Library.GlobalVariables.twoPlayers == true && Library.GlobalVariables.currentPlayer == 2)
            {
                Library.GlobalVariables.currentPlayer -= 1;
            }
            else if (Library.GlobalVariables.twoPlayers == true && Library.GlobalVariables.currentPlayer == 1)
            {
                Library.GlobalVariables.currentPlayer += 1;
            }
        }
        private void PlayerMove()
        {
            if (Library.GlobalVariables.twoPlayers == false)
            {
                for (int i = 0; i < Library.GlobalVariables.totalSpaceToMove; i++)
                {
                    if (Library.GlobalVariables.currentSquare == 7 || Library.GlobalVariables.currentSquare == 21 || Library.GlobalVariables.currentSquare == 35)
                    {
                        Library.GlobalVariables.playerDirection = "Left";
                        Library.GlobalVariables.player1y -= 44;
                        Library.GlobalVariables.currentSquare += 1;
                    }
                    else if (Library.GlobalVariables.currentSquare == 14 || Library.GlobalVariables.currentSquare == 28 || Library.GlobalVariables.currentSquare == 42)
                    {
                        Library.GlobalVariables.playerDirection = "Right";
                        Library.GlobalVariables.player1y -= 44;
                        Library.GlobalVariables.currentSquare += 1;
                    }
                    else if (Library.GlobalVariables.currentSquare >= 49)
                    {
                        DialogResult dialogResult = MessageBox.Show("Congratulations! You won. It took you " + Library.GlobalVariables.player1score + " attempts. Would you like to restart?", "Dice Game", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            Application.Restart();
                            break;
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            Environment.Exit(9);
                            break;
                        }
                    }
                    else if (Library.GlobalVariables.playerDirection == "Right")
                    {
                        Library.GlobalVariables.player1x += 44;
                        Library.GlobalVariables.currentSquare += 1;
                    }
                    else if (Library.GlobalVariables.playerDirection == "Left")
                    {
                        Library.GlobalVariables.player1x -= 44;
                        Library.GlobalVariables.currentSquare += 1;
                    }
                    player1.Location = new Point(Library.GlobalVariables.player1x, Library.GlobalVariables.player1y);
                    System.Threading.Thread.Sleep(Library.GlobalVariables.sleepTime);
                }

            }

        }

        private void speedTrackBar_scroll(object sender, EventArgs e)
        {
            Library.GlobalVariables.sleepTime = speedTrackBar.Value;
            speedLabel.Text = "Speed: " + Library.GlobalVariables.sleepTime + " ms";
        }
    }
}
