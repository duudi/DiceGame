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

        private void Main_Load(object sender, EventArgs e) // When the program has loaded
        {
            MessageBox.Show(Message.StartMessage(), "Welcome"); // Shows a welcome message (the text is read from a text file in the Message class)
            string obstaclePath = Environment.CurrentDirectory + @"\Data\obstacles.txt"; // Sets the location of the text file containing the obstacles to a variable
            ReadInObstacles(obstaclePath); // Calls the ReadInObstacles method and passes in the path of the text file
            PlaceObstacles(); // Calls the PlaceObstacles method
            player1.Location = new Point(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(1, 4))), Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(1, 5)))); // Sets player one's location to the default location in the array
            player2.Location = new Point(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(2, 4))), Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(2, 5)))); // Sets player two's location to the default location in the array
        }



        /// <summary>
        /// Methods that place/check obstacles
        /// </summary>
        /// <param name="objectPath"></param>
        private void ReadInObstacles(string objectPath) // ReadInObstacles method
        {
            Library.GlobalVariables.obstacleStats = File.ReadAllLines(objectPath) // Reads in the text file
                   .Select(l => l.Split(',').Select(i => int.Parse(i)).ToArray()) // Splits the text by commas
                   .ToArray(); // Converts everything to a jagged array
        }
        private void PlaceObstacles() // PlaceObstacles method
        {
            for (int i = 0; i < Library.GlobalVariables.obstacleStats.GetLength(0); i++) // For every obstacle in the obstacleStats array
            {
                Label lbl = this.Controls.Find("label" + Library.GlobalVariables.obstacleStats[i][0], true).FirstOrDefault() as Label; // Find the label of the square the obstacle should be placed at
                int obstacleX = Convert.ToInt32(lbl.Location.X) - 9; // Finds the x of the label and subtracts 9 to space it out from the label
                int obstacleY = Convert.ToInt32(lbl.Location.Y) + 7; // Finds the y of the label and adds 7 to space it out from the label
                PictureBox obstacle = new PictureBox(); // Creates a new PictureBox
                if (Library.GlobalVariables.obstacleStats[i][1] < 0) // If the obstacle moves the player back
                {
                    obstacle.Image = Properties.Resources.badObstacle; // Set the image to a stop sign
                }
                else // If the obstacle moves the player forward
                {
                    obstacle.Image = Properties.Resources.goodObstacle; // Set the image to a forwards arrow
                }
                obstacle.Location = new Point(obstacleX, obstacleY); // Set the location of the obstacle to the two variables created above
                obstacle.Size = new Size(17, 17); // Set the obstacle size to 17x17 (like all the other sprites)
                obstacle.SizeMode = PictureBoxSizeMode.Zoom; // Set the obstacle's scaling mode to zoom
                this.Controls.Add(obstacle); // Add the obstacle to the form
                obstacle.Show(); // Show the obstacle
                obstacle.BringToFront(); // Bring the obstacle to the front
            }
        }
        private void CheckForObstacles() // CheckForObstacles method
        {
            foreach (int[] array in Library.GlobalVariables.obstacleStats) // For every item in the array
            {
                if (Library.GlobalVariables.currentSquare == array[0] && Library.GlobalVariables.spaceLeft == 0) // If the current square is an obstacle and the player has no more places to move
                {
                    if (array[1] < 0) // If the obstacle sends the player backwards
                    {
                        Library.GlobalVariables.totalSpaceToMove = Math.Abs(array[1]); // Set the number of spaces the player needs to move and use the absolute value to remove the minus sign
                        Library.GlobalVariables.goingForwards = false; // Set the goingForwards variable to false (the player is going backwards)
                        MessageBox.Show("You have landed on a bad obstacle. You will now go back " + Math.Abs(array[1]) + " squares."); // Show a message to the player with the number of spaces they are moving back by
                    }
                    else if (array[1] > 0) // If the obstacle sends the player forwards
                    {
                        Library.GlobalVariables.totalSpaceToMove = array[1]; // Sets the number of spaces the player needs to move
                        Library.GlobalVariables.goingForwards = true; // Set the goingForwards variable to true (the player is going forwards)
                        MessageBox.Show("You have landed on a good obstacle. You will now advance " + array[1] + " squares."); // Show a message to the player with the number of spaces they are moving forwards by
                    }
                    TurnMove(Library.GlobalVariables.currentPlayer, Library.GlobalVariables.goingForwards); // Call the TurnMove method to move the player
                }
            }
        }



        /// <summary>
        /// User Controls
        /// </summary>=
        private void startButton_Click(object sender, EventArgs e) // When the start button is clicked
        {
            NumberOfPlayersDialog createDialog = new NumberOfPlayersDialog(); // Create an instance of the dialog asking the user how many players they would like
            createDialog.Show(); // Show the dialog
            createDialog.Closed += (s, args) => DialogClosed(); // When the dialog box is closed call the DialogClosed method
        }
        private void DialogClosed() // When the number of players has been selected
        {
            rollButton.Show(); // Show the roll button
            startButton.Hide(); // Hide the start button
            GameRefresh(); // Call the GameRefresh method
            dice1.Show(); // Show the first dice image
            dice2.Show(); // Show the second dice image
            player1.Show(); // Show player 1
            speedTrackBar.Show(); // Show the speed slider
            speedLabel.Show(); // Show the label on the speed slider
            speedLabel.BringToFront(); // Bring the label to the front
            speedLabel.Text = "Speed: " + Library.GlobalVariables.sleepTime + " ms"; // Set the text of the label
            if (Library.GlobalVariables.twoPlayers == true) // If there are two players playing
            {
                player2.Show(); // Show player 2
            }
        }
        private void rollButton_Click(object sender, EventArgs e) // When the roll button is clicked
        {
            Library.GlobalVariables.totalSpaceToMove = 0; // Reset the number of spaces the player needs to move
            RollDice(); // Call the RollDice method
            ChangeDiceImage(Library.GlobalVariables.diceValue1, Library.GlobalVariables.diceValue2, dice1, dice2); // Call the change dice method and pass in the values of the dice
            Library.GlobalVariables.goingForwards = EvaluateDice(Library.GlobalVariables.diceValue1, Library.GlobalVariables.diceValue2);
            Library.GlobalVariables.totalSpaceToMove = Library.GlobalVariables.diceValue1 + Library.GlobalVariables.diceValue2; // Set the totalSpaceToMove to the total of dice 1 and dice 2
            Library.GlobalVariables.currentSquare = SpacePossible(Library.GlobalVariables.currentSquare); // Call the SpacePossible method to check if the next square is a possible move
            Library.GlobalVariables.spaceLeft = Library.GlobalVariables.totalSpaceToMove; // Set the number of spaces left to the total spaces to move
            IncrementScore(Library.GlobalVariables.currentPlayer); // Call the IncrementScore method to increment the score of the current player
            TurnMove(Library.GlobalVariables.currentPlayer, Library.GlobalVariables.goingForwards); // Call the TurnMove method and pass in the currentPlayer and whether the player is moving forwards
            if (Library.GlobalVariables.twoPlayers == true) // If there are two players
            {
                SwitchPlayer(); // Call the SwitchPlayer method
                GameRefresh(); // Call the GameRefresh method
            }
        }
        private void speedTrackBar_scroll(object sender, EventArgs e) // When the speed slider is changed
        {
            Library.GlobalVariables.sleepTime = speedTrackBar.Value; // Set the sleep time variable to the value selected by the user
            speedLabel.Text = "Speed: " + Library.GlobalVariables.sleepTime + " ms"; // Change the text to the value the user has selected
        }



        /// <summary>
        /// Methods that roll/check the dice
        /// </summary>
        private void RollDice() // RollDice method
        {
            Random rnd = new Random(); // Create a new instance of the Random class
            Library.GlobalVariables.diceValue1 = rnd.Next(1, 7); // Generate a random value from 1 to 6 for dice 1
            Library.GlobalVariables.diceValue2 = rnd.Next(1, 7); // Generate a random value from 1 to 6 for dice 2
        }
        private bool EvaluateDice(int dice1, int dice2) // EvaluateDice method
        {
            if (dice1 == dice2) // If both dice have the same value
            {
                MessageBox.Show("You have rolled a double! You will now go back " + (dice1 + dice2) + " squares."); // Tell the user they have rolled a double
                return false; // Return false
            }
            else // If the dice have different values
            {
                return true; // Return true
            }
        }
        private void ChangeDiceImage(int diceValue1, int diceValue2, PictureBox dice1, PictureBox dice2) // ChangeDiceImage method
        {
            switch (diceValue1) // Creates a switch based on the value of dice 1
            {
                case 1: // If dice 1 is 1
                    dice1.Image = Properties.Resources.dice1; // Set the image to a dice with 1 dot
                    break; // Break out of the switch statement
                case 2: // If dice 1 is 2
                    dice1.Image = Properties.Resources.dice2; // Set the image to a dice with 2 dots
                    break; // Break out of the switch statement
                case 3: // If dice 1 is 3
                    dice1.Image = Properties.Resources.dice3; // Set the image to a dice with 3 dots
                    break; // Break out of the switch statement
                case 4: // If dice 1 is 4
                    dice1.Image = Properties.Resources.dice4; // Set the image to a dice with 4 dots
                    break; // Break out of the switch statement
                case 5: // If dice 1 is 5
                    dice1.Image = Properties.Resources.dice5; // Set the image to a dice with 5 dots
                    break; // Break out of the switch statement
                case 6: // If dice 1 is 6
                    dice1.Image = Properties.Resources.dice6; // Set the image to a dice with 6 dots
                    break; // Break out of the switch statement
            }
            switch (diceValue2) // Creates a switch based on the value of dice 2
            {
                case 1: // If dice 2 is 1
                    dice2.Image = Properties.Resources.dice1; // Set the image to a dice with 1 dot
                    break; // Break out of the switch statement
                case 2: // If dice 2 is 2
                    dice2.Image = Properties.Resources.dice2; // Set the image to a dice with 2 dots
                    break; // Break out of the switch statement
                case 3: // If dice 2 is 3
                    dice2.Image = Properties.Resources.dice3; // Set the image to a dice with 3 dots
                    break; // Break out of the switch statement
                case 4: // If dice 2 is 4
                    dice2.Image = Properties.Resources.dice4; // Set the image to a dice with 4 dots
                    break; // Break out of the switch statement
                case 5: // If dice 2 is 5
                    dice2.Image = Properties.Resources.dice5; // Set the image to a dice with 5 dots
                    break; // Break out of the switch statement
                case 6: // If dice 2 is 6
                    dice2.Image = Properties.Resources.dice6; // Set the image to a dice with 6 dots
                    break; // Break out of the switch statement
            }
            dice1.Refresh(); // Refresh the image of dice 1
            dice2.Refresh(); // Refresh the image of dice 2
        }



        /// <summary>
        /// Methods that move the player
        /// </summary>
        private void TurnMove(int currentPlayer, bool goingForwards) // TurnMove method
        {
            for (int i = 0; i < Library.GlobalVariables.totalSpaceToMove; i++) // For every square the player needs to move 
            {
                double playerRow = Library.GlobalVariables.currentSquare / 7; // Determines which row the player is on by dividing the current square by 7
                int roundedplayerRow = Convert.ToInt32(Math.Floor(playerRow)); // Round this figure
                int intMoveMultiplier = MoveMultiplier(roundedplayerRow, goingForwards); // Call the MoveMultiplier method to work out if the player should move left or right
                PlayerMove(currentPlayer, intMoveMultiplier); // Call the PlayerMove mnethod to move the player
            }
        }
        private void PlayerMove (int currentPlayer, int intMoveMultiplier) // PlayerMove method
        {
            int nextSpace = SpacePossible(NextSquare(Library.GlobalVariables.currentSquare, Library.GlobalVariables.goingForwards)); // Can I move to the next square?
            if (nextSpace != Library.GlobalVariables.currentSquare) // If the player can move
            {
                int yMultiplier = ChangeY(nextSpace, Library.GlobalVariables.goingForwards); // Call the ChangeY method to determine if the player needs to move up, down or carry on
                if (yMultiplier == 0) // If the player doesn't need to move up or down
                {
                    int playerX = Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(currentPlayer, 4))); // Fetch the player's x value from the array and store it temporarily in a variable
                    playerX = playerX + (44 * intMoveMultiplier); // Multiply the current x by 44 times the move multiplier
                    Library.GlobalVariables.playerStats.SetValue(playerX, currentPlayer, 4); // Store the new x value back into the array
                }
                else // If the player needs to move up or down
                {
                    int playerY = Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(currentPlayer, 5))); // Fetch the player's y value from the array and store it temporarily in a variable
                    playerY = playerY + (44 * yMultiplier); // Multiply the current y by 44 times the y multiplier
                    Library.GlobalVariables.playerStats.SetValue(playerY, currentPlayer, 5); // Store the new y value back into the array
                }
                Library.GlobalVariables.currentSquare = nextSpace; // Set the current square to the next place
                ApplyMove(); // Call the ApplyMove method
            }
            
        }
        private void ApplyMove() // ApplyMove method
        {
            Library.GlobalVariables.spaceLeft -= 1; // Reduce the spaces left by 1
            player1.Location = new Point(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(1, 4))), Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(1, 5)))); // Set player 1's location to the x and y in the array
            player2.Location = new Point(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(2, 4))), Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(2, 5)))); // Set player 2's location to the x and y in the array
            if (Library.GlobalVariables.currentSquare == 49) // If the player has reached square 49 
            {
                CompletedGame(); // Call the CompletedGame method
            }
            System.Threading.Thread.Sleep(Library.GlobalVariables.sleepTime); // Sleep for the amount of time specified by the user on the slider
            CheckForObstacles(); // Call the CheckForObstacles method

        }



        /// <summary>
        /// Methods that calculate game logic
        /// </summary>
        private void GameRefresh() // GameRefresh method
        {
            if (Library.GlobalVariables.twoPlayers == true) // If there are two players
            {
                playerturnLabel.Text = "Player " + Library.GlobalVariables.currentPlayer + "'s turn"; // Change the playerturnLabel to the current player
                playerturnLabel.Show(); // Show the playerturnLabel
            }
        }
        private int SpacePossible(int nextSpace) // SpacePossible method
        {
            if (nextSpace < 1) // If the next space is smaller than 1
            {
                return 1; // Return 1 (stops the player going lower than square 1)
            }
            else // If the square is larger than 1
            {
                return nextSpace; // Return the next space
            }
        }
        private void IncrementScore(int currentPlayer) // IncrementScore method
        {
            Library.GlobalVariables.playerStats.SetValue(Int32.Parse(Convert.ToString(Library.GlobalVariables.playerStats.GetValue(currentPlayer, 2))) + 1, currentPlayer, 2); // Get the score of the player from the array, add 1 to it and then put it back in the array
        }
        private void SwitchPlayer() // SwitchPlayer method
        {
            Library.GlobalVariables.playerStats.SetValue(Library.GlobalVariables.currentSquare, Library.GlobalVariables.currentPlayer, 1); // Update the current square in the array with the player's new current square
            if (Library.GlobalVariables.currentPlayer == 2) // If the current player is player 2
            {
                Library.GlobalVariables.currentPlayer -= 1; // Take one away from the current player, making the current player 1
            }
            else // If the current player is player 2
            {
                Library.GlobalVariables.currentPlayer += 1; // Add one to the current player, making the current player 2
            }
            Library.GlobalVariables.currentSquare = Int32.Parse(Convert.ToString((Library.GlobalVariables.playerStats.GetValue((Library.GlobalVariables.currentPlayer), 1)))); // Set the global variable currentSquare to the value in the array for the next player
        }
        private int MoveMultiplier(int playerRow, bool goingForwards) // MoveMultiplier method
        {
            int playerRowEven = playerRow % 2; // Find the remainder of player row divided by 2
            if (playerRowEven == 0 && goingForwards) // If the player is on an even row and the player is going forwards
            {
                return 1; // Return a multiplier of 1 (go right)
            }
            else if (playerRowEven == 0 && !goingForwards) // If the player is on an even row and the player is going backwards
            {
                if (new[] { 14, 28, 42 }.Contains(Library.GlobalVariables.currentSquare)) // If the current square is 14, 28 or 42
                {
                    return 1; // Return a multiplier of 1 (go right)
                }
                else // If the current square is anything else
                {
                    return -1; // Return a multiplier of -1 (go left)
                }
            }
            else if (playerRowEven != 0 && goingForwards) // If the player is on an odd row and the player is going forwards
            {
                return -1; // Return a multiplier of -1 (go left)
            }
            else if (playerRowEven != 0 && !goingForwards) // If the player is on an odd row and the player is going backwards
            {
                if (new[] { 7, 21, 35 }.Contains(Library.GlobalVariables.currentSquare)) // If the current square is 7, 21 or 35
                {
                    return -1; // Return a multiplier of -1 (go left)
                }
                else // If the current square is anything else
                {
                    return 1; // Return a multiplier of 1 (go right)
                }
            }
            else // Failsafe
            {
                return 1; // Return a multiplier of 1 (go right)
            }
        }
        private int ChangeY (int nextSpace, bool goingForwards) // ChangeY Method
        {
            int modChangeY = nextSpace % 7; // Determines whether the player's next square will move them up or down
            if (goingForwards && modChangeY == 1) // If the player is going forwards and modChangeY is 1 (the player needs to go up)
            {
                return -1; // Return a y multiplier of -1 (going up)
            }
            else if (!goingForwards && modChangeY == 0) // If the player is going backwards and modChangeY is 0 (the player needs to go down)
            {
                return 1; // Return a y multiplier of 1 (going down)
            }
            else // If the player is going forwards and does not need to move up or down
            {
                return 0; // Return a y multiplier of 0 (no change in height)
            }
        }
        private int NextSquare (int currentSquare, bool goingForwards) // NextSquare method
        {
            if (goingForwards) // If the player is going forwards
            {
                return currentSquare + 1; // Return the current square plus 1
            }
            else // If the player is going backwards
            {
                return currentSquare - 1; // Return the current square minus 1
            }
        }


        /// <summary>
        /// When the player reaches tile 49
        /// </summary>
        private void CompletedGame() // CompletedGame method
        {
            Library.GlobalVariables.totalSpaceToMove = 0; // Set the total space left to move to 0
            DialogResult dialogResult = MessageBox.Show("Congratulations! You won. It took you " + Library.GlobalVariables.playerStats.GetValue(Library.GlobalVariables.currentPlayer, 2) + " attempts. Would you like to restart?", "Dice Game", MessageBoxButtons.YesNo); // Show a dialog with the winning player's score and the option to restart or quit the game
            if (dialogResult == DialogResult.Yes) // If the user wants to restart the game
            {
                Application.Restart(); // Restart the entire application
            }
            else if (dialogResult == DialogResult.No) // If the user wants to quit the game
            {
                Application.Exit(); // Quit the application
                return;
            }
        }


    }
}