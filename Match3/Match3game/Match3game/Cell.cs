using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Match3game
{
    public class TestCell
    {
        private int _x;
        private int _y;
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
        public string Icon { get; set; }
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }
        public int LocX => _x * TestBoard.CellSize;
        public int LocY => TestBoard.FormSize - (_y + 1) * TestBoard.CellSize;

        public TestCell() : this(0,0,"")
        {
        }

        public TestCell(int x, int y, string icon)
        {
            _x = x;
            _y = y;
            Icon = icon;
            BackColor = Color.AliceBlue;
            ForeColor = Color.Black;
        }

        private TestCell GetAdjacent(int dx, int dy)
        {
            int x = _x + dx;
            int y = _y + dy;

            if (x >= 0 && y >= 0 && x < TestBoard.BoardSize && y < TestBoard.BoardSize)
            {
                return TestBoard.cells[x, y];
            }
            return null;
        }
        //Вызов всех соседних ячеек
        public List<TestCell> GetAllAdjacentCells()
        {
            List<TestCell> adjcells = new List<TestCell>();
            adjcells.Add(GetAdjacent(1, 0));
            adjcells.Add(GetAdjacent(-1, 0));
            adjcells.Add(GetAdjacent(0, 1));
            adjcells.Add(GetAdjacent(0, -1));
            return adjcells;
        }
        //Поиск совпадений
        private List<TestCell> FindMatch(int dx, int dy)
        {
            List<TestCell> matchingCells = new List<TestCell>();
            TestCell adj = GetAdjacent(dx, dy);
            int i = 1;
            while (adj != null && adj.Icon == Icon)
            {
                matchingCells.Add(adj);
                ++i;
                adj = GetAdjacent(i * dx, i * dy);
            }
            return matchingCells;
        }
        bool matchFound = false;
        //Удаление совпадений в нескольких направлениях
        private void ClearMatch(int[] pathx, int[] pathy)
        {

            List<TestCell> matchingCells = new List<TestCell>();
            for (int i = 0; i < pathx.Length; ++i)
            {
                matchingCells.AddRange(FindMatch(pathx[i], pathy[i]));
            }
            if (matchingCells.Count >= 2)
            {
                foreach (TestCell match in matchingCells) {
                    TestBoard.cells[match.X, match.Y] = null;
                }
                matchFound = true;
            }
        }
        //Удаление совпадений по горизонтали и вертикали, а также текущей ячейки
        public void ClearAllMatches()
        {
            
            ClearMatch(new int[] { 1, -1, 0, 0 }, new int[] { 0, 0, 1, -1 });
            if (matchFound)
            {
                TestBoard.cells[X, Y] = null;
            }
        }

        public void SwapCells(TestCell cell2)
        {
            if (X == cell2.X && Y == cell2.Y) return;
            TestCell temp = TestBoard.cells[X, Y];
            TestBoard.cells[X, Y] = TestBoard.cells[cell2.X, cell2.Y];
            TestBoard.cells[cell2.X, cell2.Y] = temp;

            int tx = X;
            int ty = Y;
            X = cell2.X;
            Y = cell2.Y;
            cell2.X = tx;
            cell2.Y = ty;
            
        }

        
    }
    public class TestBoard {
        public const int BoardSize = 8; //Размер сетки
        public const int FormSize = BoardSize * CellSize; //Размер формы
        public const int CellSize = 64; //Размер ячейки
        public static TestCell[,] cells = new TestCell[BoardSize, BoardSize]; //Все ячейки на форме
        static Random random = new Random();
        static List<string> icons = new List<string>() //Возможные картинки на ячейках
        {
            "\u2764", "\u2720", "\u2739", "\u2726", "\u273f"
        };

        public static void SwapCells(ref TestCell tc1, ref TestCell tc2) {
            TestCell tmp = tc1;
            tc1 = tc2;
            tc2 = tmp;
        }

        public static void UpdateIndexes() {
            for (int x = 0; x < BoardSize; ++x) {
                for (int y = 0; y < BoardSize; ++y) {
                    if (cells[x, y] != null) {
                        cells[x, y].X = x;
                        cells[x, y].Y = y;
                    }
                }
            }
        }


        //Инициализация сетки
        public static void CreateBoard()
        {

            string[] prevLeft = new string[BoardSize];
            string prevBot = null;
            for (int x = 0; x < BoardSize; ++x)
            {
                for (int y = 0; y < BoardSize; ++y)
                {
                    List<string> possibleIcons = new List<string>();

                    possibleIcons.AddRange(icons);
                    possibleIcons.Remove(prevLeft[y]);
                    possibleIcons.Remove(prevBot);

                    string newIcon = possibleIcons[random.Next(possibleIcons.Count)];

                    cells[x, y] = new TestCell(x, y, newIcon);

                    prevLeft[y] = newIcon;
                    prevBot = newIcon;


                }
            }

        }
        //Падение ячеек при совпадениях
        private static int? firstEmpty;
        public static void SettleCells()
        {
            for (int x = 0; x < BoardSize; ++x)
            {
                firstEmpty = null;
                for (int y = 0; y < BoardSize; ++y)
                {
                    if (cells[x, y] == null && !firstEmpty.HasValue)
                    {
                        firstEmpty = y;
                    }
                    else if (cells[x, y] != null && firstEmpty.HasValue)
                    {
                        cells[x, firstEmpty.Value] = cells[x, y];
                        cells[x, firstEmpty.Value].Y = firstEmpty.Value;

                        cells[x, y] = null;
                        cells[x, firstEmpty.Value].ClearAllMatches();
                        firstEmpty++;
                    }
                }
            }
        }
    }
    public class Cell : Button
    {
        //Координаты ячейки в сетке
        public Point Index { get; set; }

        public Cell()
        {
           
        }
        //Преобразование координат в сетке в координаты на форме.
        public void UpdatePosition()
        {
            int x = Index.X * Board.CellSize;
            int y = Board.FormSize - (Index.Y+1) * Board.CellSize;
            this.Location = new System.Drawing.Point(x, y);

        }
        //Вызов соседней ячейки в определённом направлении
        private Cell GetAdjacent(int offsetX, int offsetY)
        {
            int x = Index.X + offsetX;
            int y = Index.Y + offsetY;
            
            if (x >= 0 && y >= 0 && x < Board.BoardSize && y < Board.BoardSize)
            {
                return Board.cells[x, y];
            }
            return null;
        }
        //Вызов всех соседних ячеек
        public List<Cell> GetAllAdjacentCells() {
            List<Cell> adjcells = new List<Cell>();
            adjcells.Add(GetAdjacent(1, 0));
            adjcells.Add(GetAdjacent(-1, 0));
            adjcells.Add(GetAdjacent(0, 1));
            adjcells.Add(GetAdjacent(0, -1));
            return adjcells;
        }
        //Поиск совпадений
        private List<Cell> FindMatch(int offsetX, int offsetY) {
            List<Cell> matchingCells = new List<Cell>();
            Cell adj = GetAdjacent(offsetX, offsetY);
            int i = 1;
            while (adj != null && adj.Text == Text) {
                matchingCells.Add(adj);
                ++i;
                adj = GetAdjacent(i*offsetX, i*offsetY);
            }
            return matchingCells;
        }
        bool matchFound = false;
        //Удаление совпадений в одном измерении
        private void ClearMatch(int[] pathx, int[] pathy) {
            List<Cell> matchingCells = new List<Cell>();
            for (int i = 0; i < pathx.Length; ++i) {
                matchingCells.AddRange(FindMatch(pathx[i], pathy[i]));
            }
            if (matchingCells.Count >= 2) {
                for (int i = 0; i < matchingCells.Count; ++i) {
                    //matchingCells[i].Text = null;
                    //matchingCells[i].Visible = false;
                    Board.cells[matchingCells[i].Index.X, matchingCells[i].Index.Y] = null;
                    //matchingCells[i].Dispose();
                    
                }
                
                matchFound = true;
            }
        }
        //Удаление совпадений в двух измерениях
        public void ClearAllMatches() {
            //if (Text == null) return;
            ClearMatch(new int[] { 1, -1}, new int[] {0,0});
            ClearMatch(new int[] { 0, 0 }, new int[] { 1, -1 });
            if (matchFound) {
                Board.cells[Index.X, Index.Y] = null;
                Dispose();
                //this.Text = null;
                //this.Visible = false;
            }
        }
        //Поменять местами ячейки
        public void SwapCells(Cell cell2)
        {
            if (Index == cell2.Index) return;
            Cell temp = Board.cells[Index.X, Index.Y];
            Board.cells[Index.X, Index.Y] = Board.cells[cell2.Index.X, cell2.Index.Y];
            Board.cells[cell2.Index.X, cell2.Index.Y] = temp;
            Point tempind = Index;
            Index = cell2.Index;
            cell2.Index = tempind;
            UpdatePosition();
            cell2.UpdatePosition();
            
        }
    }

    public class Board
    {
        public const int BoardSize = 8; //Размер сетки
        public const int FormSize = BoardSize * CellSize; //Размер формы
        public const int CellSize = 64; //Размер ячейки
        public static Cell[,] cells = new Cell[BoardSize, BoardSize]; //Все ячейки на форме
        static Random random = new Random();
        static List<string> icons = new List<string>() //Возможные картинки на ячейках
        {
            "\u2764", "\u2720", "\u2739", "\u2726", "\u273f"
        };

        public static void SwapCells(ref Cell c1, ref Cell c2)
        {
            Cell tmp = c1;
            c1 = c2;
            c2 = tmp;
            
        }

        public static void UpdateIndexes(bool upos)
        {
            for (int x = 0; x < BoardSize; ++x)
            {
                for (int y = 0; y < BoardSize; ++y)
                {
                    if (cells[x, y] != null)
                    {
                        cells[x, y].Index = new Point(x, y);
                        if (upos) {
                            cells[x, y].UpdatePosition();
                        }
                    }
                }
            }
        }

        //Инициализация сетки
        public static void CreateBoard()
        {
            
            string[] prevLeft = new string[BoardSize];
            string prevBot = null;
            for (int x = 0; x < BoardSize; ++x)
            {
                for (int y = 0; y < BoardSize; ++y)
                {
                    Cell cell = new Cell();

                    cell.Index = new Point(x, y);
                    cell.UpdatePosition();
                    cell.Name = "cell" + (x * BoardSize + y);
                    cell.Size = new System.Drawing.Size(CellSize, CellSize);
                    cell.Font = new Font("Arial", 24, FontStyle.Bold);
                    cell.BackColor = Color.AliceBlue;
                    
                    List<string> possibleIcons = new List<string>();
                    
                    possibleIcons.AddRange(icons);
                    possibleIcons.Remove(prevLeft[y]);
                    possibleIcons.Remove(prevBot);

                    string newIcon = possibleIcons[random.Next(possibleIcons.Count)];
                    cell.Text = newIcon;
                    cells[x, y] = cell;

                    prevLeft[y] = newIcon;
                    prevBot = newIcon;


                }
            }

        }
        //Падение ячеек при совпадениях
        private static int? firstEmpty;
        public static void SettleCells() {
            for (int x = 0; x < BoardSize; ++x) {
                firstEmpty = null;
                for (int y = 0; y < BoardSize; ++y) {
                    if (cells[x, y] == null && !firstEmpty.HasValue)
                    {
                        firstEmpty = y;
                    }
                    else if (cells[x, y] != null && firstEmpty.HasValue) {
                        cells[x, firstEmpty.Value] = cells[x, y];
                        cells[x, firstEmpty.Value].Index = new Point(x, firstEmpty.Value);
                        cells[x, firstEmpty.Value].UpdatePosition();
                        cells[x, y].Dispose();
                        //cells[x, y].Dispose();
                        cells[x, y] = null;
                        cells[x, firstEmpty.Value].ClearAllMatches();
                        firstEmpty++;
                    }
                }
            }
        }
 
    }
}
