using System.Diagnostics.Metrics;
using System.Drawing;
using System.Threading;

namespace CellGame
{
    public partial class CellGame : Form
    {
        public static int maxAge { get; set; }
        public List<Cell> Cells { get; set; }
        public static Semaphore semaphore { get; set; }
        private object locker { get; set; }
        public bool isRunning { get; set; }
        public List<Color> Colors { get; set; }
        public CellGame()
        {
            InitializeComponent();
            maxAge = 99;
            InitColors();
            this.Cells = new List<Cell>();
            this.FieldSize_num.Minimum = 10;
            this.FieldSize_num.Maximum = 200;
            this.FieldSize_num.Value = 100;
            this.TicInterval_num.Minimum = 100;
            this.TicInterval_num.Maximum = 9999;
            this.TicInterval_num.Value = 100;
            isRunning = false;
            locker = new object();
            this.PicBox.Paint += DrawGrid;
        }

        private void InitColors()
        {
            Colors = new List<Color>()
            {
                Color.FromArgb(22, 114, 136),
                Color.FromArgb(140, 218, 236),
                Color.FromArgb(180, 82, 72),
                Color.FromArgb(212, 140, 132),
                Color.FromArgb(168, 154, 73),
                Color.FromArgb(214, 207, 162),
                Color.FromArgb(60, 180, 100),
                Color.FromArgb(155, 221, 177),
                Color.FromArgb(100, 60, 106),
                Color.FromArgb(131, 99, 148),
            };
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
            if (c.isAlive)
            {
                Brush brush = new SolidBrush(Colors[c.color]);
                e.Graphics.FillRectangle(brush, c.point.X + 1, c.point.Y + 1, c.size - 1, c.size - 1);
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
            if (isRunning) Stop();
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
            if (isRunning) Stop();
            LifeIteration();
        }

        public void StartClick(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                lock (locker) isRunning = !isRunning;
                this.Start_btn.Text = "Stop";
                Task.Factory.StartNew(() =>
                {
                    Live();
                });
            }
            else
            {
                Stop();
            }
        }

        private void Stop()
        {
            lock (locker)
            {
                isRunning = !isRunning;
                this.Start_btn.Invoke(() => this.Start_btn.Text = "Start");
            }
            MessageBox.Show("Game stopped");
        }

        public void Live()
        {
            while (isRunning)
            {
                if (Cells.Any(c => c.age == maxAge)) Stop();
                LifeIteration();
                Thread.Sleep((int)this.TicInterval_num.Value);
            }
        }

        public void LifeIteration()
        {
            Cells.ForEach(cell =>
            {
                Task.Factory.StartNew(() =>
                {
                    OneCellLifeIteration(cell);
                });
            });
        }

        public void OneCellLifeIteration(Cell c)
        {
            semaphore.WaitOne();
            int counter = CountCells(c);
            if (!c.isAlive && counter == 3) c.isAlive = true;
            if (c.isAlive && (counter < 2 || counter > 3))
            {
                c.age = 0;
                c.isAlive = false;
            }
            if (c.isAlive)
            {
                c.color = c.age < maxAge ? c.age / 10 : 9;
                c.age++;
            }
            this.PicBox.Invalidate();
            try
            {
                semaphore.Release();
            }
            catch (Exception ex) { }
        }

        public int CountCells(Cell c)
        {
            int counter = 0;
            for (int i = c.point.X - (int)this.FieldSize_num.Value; i < c.point.X + (int)this.FieldSize_num.Value * 2; i += (int)this.FieldSize_num.Value)
            {
                for (int j = c.point.Y - (int)this.FieldSize_num.Value; j < c.point.Y + (int)this.FieldSize_num.Value * 2; j += (int)this.FieldSize_num.Value)
                {
                    if (i == c.point.X && j == c.point.Y) continue;
                    if (i < 0 || j < 0 || i > PicBox.Width || j > PicBox.Height) continue;
                    if (Cells.Find(c => c.point.Equals(new Point(i, j))).isAlive) counter++;
                }
            }
            return counter;
        }
    }
}