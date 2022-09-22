using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace GOLStartUpTemplate1
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[30, 30];
        bool[,] scratchpad = new bool[30, 30];
        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color BackColor = Color.White;
        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        int AliveC = 0;
        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = true; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int omini = CountNeighborsFinite(x, y);
                    if (universe[x, y])
                    {
                        if (omini == 2 || omini == 3)
                        {
                            scratchpad[x, y] = true;
                        }
                        if (omini < 2 || omini > 3)
                        {
                            scratchpad[x, y] = false;
                        }
                    }
                    else
                    {
                        if (omini == 3)
                        {
                            scratchpad[x, y] = true;
                        }
                    }


                }
            }
            bool[,] temp = universe;
            universe = scratchpad;
            scratchpad = temp;

            // Increment generation count
            generations++;
            AliveC = gen();

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            AliveCells.Text = "Alive Cells = " + AliveC;
            graphicsPanel1.Invalidate();
            Array.Clear(temp, 0, temp.Length);
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush MisfitBrush = new SolidBrush(BackColor);
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    else if (universe[x, y] == false)
                    {
                        e.Graphics.FillRectangle(MisfitBrush, cellRect);
                    }
                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            MisfitBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }
                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yCheck == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yLen = yCheck = -1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Array.Clear(universe, 0, universe.Length);
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
            timer.Enabled = true;
            timer.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|.|Txt|*.txt";
            dlg.FilterIndex = 2;
            dlg.DefaultExt = "txt";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

               
                writer.WriteLine("!This is my comment.");

               
                for (int y = 0; y < universe.GetLength(0); y++)
                {
                   
                    String currentRow = string.Empty;

                    
                    for (int x = 0; x < universe.GetLength(1); x++)
                    {
                       
                        if (universe[x, y] == true)
                        {
                            currentRow += 'O';
                        }

                        
                        else if (universe[x, y] == false)
                        {
                            currentRow += '.';
                        }
                    }

                    
                    writer.WriteLine(currentRow);
                }

                
               
                writer.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string NameAdopt = AppDomain.CurrentDomain.BaseDirectory + "Ophanage.txt";
            try
            {
                using (var writer = new StreamWriter(NameAdopt))
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        for (int z = 0; z < universe.GetLength(1); z++)
                        {

                            if (universe[x, z] == true)
                            {
                                writer.Write("O");
                            }
                            else
                            {
                                writer.Write(".");
                            }
                        }

                        writer.Write("\n");
                    }

                    writer.Flush();
                    writer.Close();
                    MessageBox.Show("File saved!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem saving your file.\n" + ex.Message);
            }
        }


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void colorToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
         
        }
        private int gen()
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int y = 0; y < universe.GetLength(0); y++)
            {
                for (int x = 0; x < universe.GetLength(1); x++)
                {
                    if (universe[x, y] == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int bigshot;
            SeedDial Sauce = new SeedDial();
            Random rad = new Random(Sauce.seedDial);
            
            if (Sauce.ShowDialog() == DialogResult.OK) 
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        bigshot = rad.Next(0, 2);
                        if ( bigshot == 0)
                        {
                            universe[x, y] = false;
                        }
                        else
                        {
                            universe[x, y] = true;
                        }

                    }
                }

                gen();
                graphicsPanel1.Invalidate();

            }
            else
            {
                Sauce.Close();
            }

            graphicsPanel1.Invalidate();
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rand();
        }
        private void Rand()
        {
            Random rndom = new Random();


            int oogabooga;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {

                    // call next random
                    oogabooga = rndom.Next(2);
                    if (oogabooga == 0)
                    {
                        universe[x, y] = false;
                    }
                    else
                    {
                        universe[x, y] = true;
                    }
                }
            }
            gen();
            graphicsPanel1.Invalidate();
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog ColDial = new ColorDialog();

            ColDial.Color = cellColor;

            if (DialogResult.OK == ColDial.ShowDialog())
            {
                cellColor = ColDial.Color;

            }
            graphicsPanel1.Invalidate();
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog GridCol = new ColorDialog();

            GridCol.Color = gridColor;

            if (DialogResult.OK == GridCol.ShowDialog())
            {
                gridColor = GridCol.Color;

            }
            graphicsPanel1.Invalidate();
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
             ColorDialog GridCol = new ColorDialog();

            GridCol.Color = BackColor;

            if (DialogResult.OK == GridCol.ShowDialog())
            {
                BackColor = GridCol.Color;

            }
            graphicsPanel1.Invalidate();
        }
    }
    
    
}
