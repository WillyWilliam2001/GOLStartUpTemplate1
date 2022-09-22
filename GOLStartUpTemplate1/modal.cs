using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLStartUpTemplate1
{
    public partial class SeedDial : Form
    {
        public int seedDial;
        public SeedDial()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           Random random = new Random();
            numericUpDown1.Value = random.Next(-10000000, 100000000);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button2Accept_Click(object sender, EventArgs e)
        {
            int seed = (int)numericUpDown1.Value;
            seedDial = seed;
        }
        public void setmyAccept(int pix)
        {
            seedDial = pix;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
