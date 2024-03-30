using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Match3game
{
    public partial class Form1 : Form
    {
        Cell firstClicked = null;
        Cell secondClicked = null;
        Button fClicked = null;
        Button sClicked = null;
        List<Button> butts = new List<Button>();
        Cell[,] testcell = new Cell[TestBoard.BoardSize, TestBoard.BoardSize];
        public Form1()
        {
            InitializeComponent();
            this.SuspendLayout();
            /*buttons = new List<Cell>();
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; ++j)
                {
                    //Cell cell;
                    int index = i * N + j;
                    Cell but = new Cell();
                    but.Size = new Size(ClientSize.Width / N, ClientSize.Height / N);
                    but.Location = new System.Drawing.Point(j*but.Size.Width, i*but.Size.Height);
                    but.Name = "button" + (i*N+j).ToString();
                    but.Font = new Font("Arial", 24, FontStyle.Bold);
                    int rand = random.Next(icons.Count);
                    but.Text = icons[rand];
                    //but.Text = "button" + index.ToString();
                    but.UseVisualStyleBackColor = true;
                    //but.Font = new Font("Webdings", 48, FontStyle.Bold);
                    but.BackColor = Color.CornflowerBlue;
                    but.ForeColor = Color.Black;
                    
                    but.Click += new EventHandler(button1_Click);
                    
                    Controls.Add(but);
                    buttons.Add(but);
                }
            }*/
            this.ClientSize = new Size(Board.FormSize, Board.FormSize);
            Board.CreateBoard();
            foreach (Cell cell in Board.cells) {
                cell.Click += new EventHandler(button1_Click);
                this.Controls.Add(cell);
            }
            /*this.ClientSize = new Size(Board.FormSize, Board.FormSize);
            TestBoard.CreateBoard();
            foreach (TestCell cell in TestBoard.cells)
            {
                Cell but = new Cell();
                but.Location = new Point(cell.LocX, cell.LocY);
                but.Size = new Size(TestBoard.CellSize, TestBoard.CellSize);
                but.Font = new Font("Arial", 24, FontStyle.Bold);
                but.Text = cell.Icon;
                but.Click += new EventHandler(btn_Click);
                but.Index = new Point(cell.X, cell.Y);
                but.ForeColor = cell.ForeColor;
                but.BackColor = cell.BackColor;
                testcell[cell.X, cell.Y] = but;
               // butts.Add(but);
                this.Controls.Add(but);
            }*/
            this.ResumeLayout(false);
        }

        private void updateCells() {
            
            for (int x = 0; x < TestBoard.BoardSize; x++) {
                for (int y = 0; y < TestBoard.BoardSize; ++y) {
                    if (Board.cells[x, y] == null) {
                        Controls.Remove(Board.cells[x, y]);

                        
                    }
                }
            }
          
        }

        private void btn_Click(object sender, EventArgs e) {
            Cell but = sender as Cell;
            if (but != null) {
                if (but.BackColor == Color.Aqua)
                    return;

                if (firstClicked == null)
                {
                    firstClicked = but;
                    firstClicked.BackColor = Color.Aqua;

                    return;
                }

                secondClicked = but;
                secondClicked.BackColor = Color.Aqua;

                ref TestCell fcell = ref TestBoard.cells[firstClicked.Index.X, firstClicked.Index.Y];
                ref TestCell scell = ref TestBoard.cells[secondClicked.Index.X, secondClicked.Index.Y];
                if (fcell.GetAllAdjacentCells().Contains(scell)) {
                    TestBoard.SwapCells(ref fcell, ref scell);
                    //updateCells();
                    firstClicked.Location = new Point(fcell.LocX, fcell.LocY);
                    secondClicked.Location = new Point(scell.LocX, scell.LocY);
                    firstClicked.Index = new Point(fcell.X, fcell.Y);
                    secondClicked.Index = new Point(scell.X, scell.Y);
                    fcell.ClearAllMatches();
                    scell.ClearAllMatches();
                    TestBoard.UpdateIndexes();
                    updateCells();
                }

                secondClicked.BackColor = Color.AliceBlue;
                firstClicked.BackColor = Color.AliceBlue;
                firstClicked = null;
                secondClicked = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ref object obj = ref sender;
            Cell but = sender as Cell;
            if (but != null)
            {
                if (but.BackColor == Color.Aqua)
                    return;

                if (firstClicked == null)
                {
                    firstClicked = but;
                    firstClicked.BackColor = Color.Aqua;

                    return;
                }
               
                secondClicked = but;
                secondClicked.BackColor = Color.Aqua;
                
                var adjcells = firstClicked.GetAllAdjacentCells();
                if (adjcells.Contains(secondClicked)) {
                    firstClicked.SwapCells(secondClicked);
                    //Board.SwapCells(ref firstClicked, ref secondClicked);
                    //Board.UpdateIndexes(false);
                    firstClicked.ClearAllMatches();
                    secondClicked.ClearAllMatches();
                    //updateCells();
                    Board.SettleCells();
                    //if(firstClicked != null && secondClicked != null)
                    //    Board.SwapCells(firstClicked, secondClicked);
                    // MessageBox.Show("Swaped");
                }
                secondClicked.BackColor = Color.AliceBlue;
                firstClicked.BackColor = Color.AliceBlue;
                firstClicked = null;
                secondClicked = null;
                
            }
            //MessageBox.Show(but.id.ToString());*/
        }

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            /*this.SuspendLayout();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; ++j)
                {
                    buttons[i*N+j].Size = new Size(ClientSize.Width / N, ClientSize.Height / N);
                    buttons[i*N+j].Location = new System.Drawing.Point(i * buttons[i * N + j].Size.Width, j * buttons[i * N + j].Size.Height);
                }
            }
            this.ResumeLayout(false);*/
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            secondClicked.BackColor = Color.AliceBlue;
            firstClicked.BackColor = Color.AliceBlue;
            firstClicked = null;
            secondClicked = null;
        }
    }
}
