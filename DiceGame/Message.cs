using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DiceGame
{
    class Message
    {
        public static string path = Environment.CurrentDirectory + @"\Data\";
        public static string StartMessage()
        {
            string messageText = File.ReadAllText(path + "welcomemessage.txt");
            return messageText;
        }
        public static class DoubleDiceMessage
        {

        }
        public static class WinMessage
        {

        }
    }
}
