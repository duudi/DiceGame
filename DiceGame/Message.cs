using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DiceGame
{
    class Message
    {
        public static string path = Environment.CurrentDirectory + @"\Data\"; // Define a string variable called path and set it to /bin/Debug/data (where the text files are stored)
        public static string StartMessage() // StartMessage method
        {
            string messageText = File.ReadAllText(path + "welcomemessage.txt"); // Read in all the text from welcomemessage.txt
            return messageText; // Return the text from messagetext.txt so it can be displayed
        }
        public static string DoubleDiceMessage() // DoubleDiceMessage method
        {
            string tempText = File.ReadAllText(path + "doubledicemessage.txt"); // Read in all the text from doubledicemessage.txt into a temporary variable
            string diceTotal = (Library.GlobalVariables.diceValue1 + Library.GlobalVariables.diceValue2).ToString(); // Calculate the total of the two dice and store it in a variable
            string messageText = tempText.Replace("*", diceTotal); // The text in the .txt file has an asterisk where the variable should be inserted, this code replaces the asterisk with the total of the dice
            return messageText; // Return the replaced text so it can be displayed
        }
        public static string WinMessage() // WinMessage method
        {
            return "";
        }
    }
}
