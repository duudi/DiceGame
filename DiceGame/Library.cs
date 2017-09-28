using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DiceGame
{
    public static class Library
    {
        public static class GlobalVariables
        {
            public static bool twoPlayers = false;
            public static int currentPlayer = 1;
            public static int dice1;
            public static int dice2;
            public static int totalSpaceToMove;
            public static int currentSquare = 1;
            public static int player1x = 16;
            public static int player1y = 286;
            public static string playerDirection = "Right";
            public static int player1score = 0;
            public static int sleepTime = 0;
        }
    }
}
