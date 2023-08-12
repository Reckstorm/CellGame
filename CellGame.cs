using System.Diagnostics.Metrics;
using Timer = System.Windows.Forms.Timer;

namespace CellGame
{
    public partial class CellGame : Form
    {
        public static int maxAge { get; set; }
        public List<Point> Points { get; set; }
        public List<Cell> Cells { get; set; }
        private object locker { get; set; }
        public bool isRunning { get; set; }
        public CellGame()
        {
            InitializeComponent();
            maxAge = 1000;
            this.Points = new List<Point>();
            this.Cells = new List<Cell>();
            this.FieldSize_num.Value = 50;
            this.TicInterval_num.Minimum = 0;
            this.TicInterval_num.Maximum = 9999;
            this.TicInterval_num.Value = 1000;
            isRunning = false;
            locker = new object();
            this.PicBox.Paint += DrawGrid;
        }

        public void DrawGrid(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black, 1f);
            Brush brush = pen.Brush;
            GetPoints();
            e.Graphics.Clear(Color.White);
            try
            {
                Points.ForEach(p =>
                {
                    e.Graphics.DrawRectangle(pen, p.X, p.Y, (int)this.FieldSize_num.Value, (int)this.FieldSize_num.Value);
                });
                Cells.ForEach(c =>
                {
                    e.Graphics.FillRectangle(brush, c.point.X, c.point.Y, c.size, c.size);
                });
            }
            catch (InvalidOperationException) { }
        }

        public void GetPoints()
        {
            this.Points.Clear();
            for (int i = 0; i < this.PicBox.Width; i += (int)this.FieldSize_num.Value)
            {
                for (int j = 0; j < this.PicBox.Height; j += (int)this.FieldSize_num.Value)
                {
                    Point t = new Point(i, j);
                    lock (locker)
                    {
                        try
                        {
                            if (Cells.Count > 0 && Cells.Any(c => c.point.Equals(t))) continue;
                        }
                        catch (InvalidOperationException) { }
                        Points.Add(t);
                    }
                }
            }
        }

        public void ChangeFieldSize(object sender, EventArgs e)
        {
            Cells.Clear();
            Points.Clear();
            this.PicBox.Invalidate();
        }

        public void CreateCell(Point p)
        {
            if (p.X == -1) return;
            lock (locker)
            {
                if (Cells.Any(c => c.point.Equals(p))) return;
                Cell c = new Cell((int)this.FieldSize_num.Value, maxAge, p);
                Cells.Add(c);
                Points.Remove(c.point);
            }
        }

        public void CreateCellOnClick(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            Point p = Points.FirstOrDefault(p => (p.X <= args.Location.X && (p.X + (int)this.FieldSize_num.Value) >= args.Location.X) && (p.Y <= args.Location.Y && (p.Y + (int)this.FieldSize_num.Value) >= args.Location.Y));
            CreateCell(p);
            this.PicBox.Invalidate();
        }

        public void CreateCellOnCondition(Point p)
        {
            int counter = 0;
            for (int i = p.X - (int)this.FieldSize_num.Value; i < p.X + (int)this.FieldSize_num.Value * 2; i += (int)this.FieldSize_num.Value)
            {
                for (int j = p.Y - (int)this.FieldSize_num.Value; j < p.Y + (int)this.FieldSize_num.Value * 2; j += (int)this.FieldSize_num.Value)
                {
                    if (i == p.X && j == p.Y) continue;
                    lock (locker)
                    {
                        if (Cells.Any(c => c.point.Equals(new Point(i, j)))) counter++;
                    }
                }
            }
            if (counter == 3) CreateCell(p);
            this.PicBox.Invalidate();
        }

        public void DeleteCellOnCondition(Cell c)
        {
            int counter = 0;
            for (int i = c.point.X - (int)this.FieldSize_num.Value; i < c.point.X + (int)this.FieldSize_num.Value * 2; i += (int)this.FieldSize_num.Value)
            {
                for (int j = c.point.Y - (int)this.FieldSize_num.Value; j < c.point.Y + (int)this.FieldSize_num.Value * 2; j += (int)this.FieldSize_num.Value)
                {
                    if (i == c.point.X && j == c.point.Y) continue;
                    lock (locker)
                    {
                        if (Cells.Any(c => c.point.Equals(new Point(i, j)))) counter++;
                    }
                }
            }
            if (counter < 2 || counter > 3)
            {
                Points.Add(c.point);
                Cells.Remove(c);
            }
            this.PicBox.Invalidate();
        }

        public void StepClick(object sender, EventArgs e)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                CreateCellOnCondition(Points[i]);
            }
            for (int i = 0; i < Cells.Count; i++)
            {
                DeleteCellOnCondition(Cells[i]);
            }
        }

        public void StartClick(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
                {
                    while (isRunning)
                    {
                        for (int i = 0; i < Points.Count; i++)
                        {
                            if (!isRunning) break;
                            CreateCellOnCondition(Points[i]);
                        }
                        for (int i = 0; i < Cells.Count; i++)
                        {
                            if (!isRunning) break;
                            DeleteCellOnCondition(Cells[i]);
                        }
                        Thread.Sleep((int)this.TicInterval_num.Value);
                    }
                });
            if (!isRunning)
            {
                this.Start_btn.Text = "Stop";
                thread.Start();
            }
            else
            {
                this.Start_btn.Text = "Start";
            }
            isRunning = !isRunning;
        }
    }
}