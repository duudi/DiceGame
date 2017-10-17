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
            public static int diceValue1;
            public static int diceValue2;
            public static int totalSpaceToMove;
            public static int spaceLeft;
            public static int currentSquare = 1;
            public static int sleepTime = 0;
            public static bool goingForwards;
            public static int[][] obstacleStats;
            public static object[,] playerStats = new object[,] {
                                                                  { 0, 0, 0, 0, 0, 0, }, // Dummy Record
                                                                  {  1, 1, 0, false, 16, 286 }, // 1st Element: Player Number, 2nd Element: CurrentSquare, 3rd Element: Score,
                                                                  { 2, 1, 0, false, 37, 286 } }; // 4th Element: Going Backwards? 5th Element: Player X, 6th Element: Player Y
        
        }
    }
}
