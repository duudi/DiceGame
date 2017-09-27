using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DiceGame
{
    public partial class NumberOfPlayersDialog : Form
    {
        public NumberOfPlayersDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Library.GlobalVariables.twoPlayers = false;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Library.GlobalVariables.twoPlayers = true;
            this.Close();
        }
    }
}
