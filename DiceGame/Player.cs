using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DiceGame
{
    class Player
    {
        public int Number { get; set; }
        public int CurrentSquare { get; set; }
        public int Score { get; set; }
        public bool GoingBackwards { get; set; }
        public Point Coords = new Point();
    }
}
