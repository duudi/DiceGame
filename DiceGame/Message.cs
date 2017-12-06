using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DiceGame
{
    class Message
    {
        // Define a string variable called path and set it to /bin/Debug/data (where the text files are stored)
        public static string path = $@"{Environment.CurrentDirectory}\Data\";
        
        public static string GetStartMessage() // StartMessage method
        {
            // Read in all the text from welcomemessage.txt. Return the text from messagetext.txt so it can be displayed
            return File.ReadAllText(path + "welcomemessage.txt");
        }

        public static string GetDoubleDiceMessage() // DoubleDiceMessage method
        {
            string tempText = File.ReadAllText(path + "doubledicemessage.txt"); // Read in all the text from doubledicemessage.txt into a temporary variable
            if (tempText.Contains("*")) // If the text file contains an asterisk
            {
                string diceTotal = (Library.GlobalVariables.diceValue1 + Library.GlobalVariables.diceValue2).ToString(); // Calculate the total o

                /* The text in the .txt file has an asterisk where the variable should be inserted, this code replaces
                the asterisk with the total of the dicef the two dice and store it in a variable.
                Return the replaced text so it can be displayed */

                return tempText.Replace("*", diceTotal);
            }
            else // If the text file doesn't contain an asterisk
            {
                return tempText; // Return the text from doubledicemessage.txt without replacing anything
            }

        }

        public static string GetWinMessage() // WinMessage method
        {
            string tempText = File.ReadAllText(path + "winmessage.txt"); // Read in all the text from winmessage.txt into a temporary variable
            if (tempText.Contains("*")) // If the text file contains an asterisk
            {
                string totalTurns = Library.GlobalVariables.playerStats.GetValue(Library.GlobalVariables.currentPlayer, 2).ToString(); // Fetch the number of turns the current player has had from the playerStats array and store it in a variable
                string messageText = tempText.Replace("*", totalTurns); // The text in the .txt file has an asterisk where the variable should be inserted, this code replaces the asterisk with the number of turns
                return messageText; // Return the replaced text so it can be displayed
            }
            else // If the text file doesn't contain an asterisk
            {
                return tempText; // Return the text from winmessage.txt without replacing anything
            }
        }
    }
}
