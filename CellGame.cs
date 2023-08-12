using System.Diagnostics.Metrics;
using System.Drawing;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace CellGame
{
    public partial class CellGame : Form
    {
        public static int maxAge { get; set; }
        public List<Cell> Cells { get; set; }
        public static Semaphore semaphore { get; set; }
        private object locker { get; set; }
        public bool isRunning { get; set; }
        public CellGame()
        {
            InitializeComponent();
            maxAge = 1000;
            this.Cells = new List<Cell>();
            this.FieldSize_num.Minimum = 10;
            this.FieldSize_num.Maximum = 200;
            this.FieldSize_num.Value = 50;
            this.TicInterval_num.Minimum = 500;
            this.TicInterval_num.Maximum = 9999;
            this.TicInterval_num.Value = 1000;
            isRunning = false;
            locker = new object();
            this.PicBox.Paint += DrawGrid;
        }

        public void DrawGrid(object sender, PaintEventArgs e)
        {
            if (Cells.Count == 0) GetPoints();
            e.Graphics.Clear(Color.White);
            foreach (Cell c in Cells)
            {
                DrawCell(c, e);
            }
        }

        public void DrawCell(Cell c, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black, 1f);
            Brush brush = pen.Brush;
            if (c.isAlive)
            {
                e.Graphics.FillRectangle(brush, c.point.X, c.point.Y, c.size, c.size);
            }
            else
            {
                e.Graphics.DrawRectangle(pen, c.point.X, c.point.Y, (int)this.FieldSize_num.Value, (int)this.FieldSize_num.Value);
            }
        }

        public void GetPoints()
        {
            for (int i = 0; i < this.PicBox.Width; i += (int)this.FieldSize_num.Value)
            {
                for (int j = 0; j < this.PicBox.Height; j += (int)this.FieldSize_num.Value)
                {
                    Point t = new Point(i, j);
                    Cells.Add(new Cell((int)this.FieldSize_num.Value, maxAge, t));
                }
            }
            semaphore = new Semaphore(Cells.Count, Cells.Count);
        }

        public void ChangeFieldSize(object sender, EventArgs e)
        {
            if (isRunning) isRunning = !isRunning;
            Cells.Clear();
            this.PicBox.Invalidate();
        }

        public void CreateCellOnClick(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            try
            {
                Cell c = Cells.FirstOrDefault(c => (c.point.X <= args.Location.X && (c.point.X + (int)this.FieldSize_num.Value) >= args.Location.X) && (c.point.Y <= args.Location.Y && (c.point.Y + (int)this.FieldSize_num.Value) >= args.Location.Y));
                c.isAlive = true;
            }
            catch (Exception ex) { }
            this.PicBox.Invalidate();
        }

        public void StepClick(object sender, EventArgs e)
        {
            LifeIteration();
        }

        public void StartClick(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                isRunning = !isRunning;
                this.Start_btn.Text = "Stop";
                Task.Factory.StartNew(() =>
                {
                    Live();
                });
            }
            else
            {
                isRunning = !isRunning;
                this.Start_btn.Text = "Start";
            }
        }

        public void Live()
        {
            semaphore.WaitOne();
            while (isRunning)
            {
                Cells.ForEach(c =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        OneCellLifeIteration(c);
                    });
                });
                Thread.Sleep((int)this.TicInterval_num.Value);
            }
            try
            {
                semaphore.Release();
            }
            catch (Exception ex) { }
        }

        public void LifeIteration()
        {
            Cells.ForEach(cell => OneCellLifeIteration(cell));
        }

        public void OneCellLifeIteration(Cell c)
        {
            int counter = CountCells(c);
            if (!c.isAlive && counter == 3) c.isAlive = true;
            if (c.isAlive && (counter < 2 || counter > 3)) c.isAlive = false;
            this.PicBox.Invalidate();
        }

        public int CountCells(Cell c)
        {
            int counter = 0;
            for (int i = c.point.X - (int)this.FieldSize_num.Value; i < c.point.X + (int)this.FieldSize_num.Value * 2; i += (int)this.FieldSize_num.Value)
            {
                for (int j = c.point.Y - (int)this.FieldSize_num.Value; j < c.point.Y + (int)this.FieldSize_num.Value * 2; j += (int)this.FieldSize_num.Value)
                {
                    if (i == c.point.X && j == c.point.Y) continue;
                    lock (locker)
                    {
                        if (i < 0 || j < 0 || i > PicBox.Width || j > PicBox.Height) continue;
                        if (Cells.Find(c => c.point.Equals(new Point(i, j))).isAlive) counter++;
                    }
                }
            }
            return counter;
        }
    }
}