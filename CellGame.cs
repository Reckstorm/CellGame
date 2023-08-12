using System.Diagnostics.Metrics;

namespace CellGame
{
    public partial class CellGame : Form
    {
        public static int maxAge { get; set; }
        public static Semaphore semaphore { get; set; }
        public List<Task> tasks { get; set; }
        public List<Point> Points { get; set; }
        public List<Cell> Cells { get; set; }
        private object locker { get; set; }
        public CellGame()
        {
            InitializeComponent();
            maxAge = 1000;
            this.Points = new List<Point>();
            this.Cells = new List<Cell>();
            this.FieldSize_num.Value = 50;
            this.TicInterval_num.Value = 1000;
            locker = new object();
            this.PicBox.Paint += DrawGrid;
        }

        public void DrawGrid(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black, 1f);
            Brush brush = pen.Brush;
            GetPoints();
            e.Graphics.Clear(Color.White);
            Points.ForEach(p =>
            {
                e.Graphics.DrawRectangle(pen, p.X, p.Y, (int)this.FieldSize_num.Value, (int)this.FieldSize_num.Value);
            });
            Cells.ForEach(c =>
            {
                e.Graphics.FillRectangle(brush, c.point.X, c.point.Y, c.size, c.size);
            });
        }

        public void GetPoints()
        {
            this.Points.Clear();
            for (int i = 0; i < this.PicBox.Width; i += (int)this.FieldSize_num.Value)
            {
                for (int j = 0; j < this.PicBox.Height; j += (int)this.FieldSize_num.Value)
                {
                    Points.Add(new Point(i, j));
                }
            }
            semaphore = new Semaphore(Points.Count, Points.Count);
            tasks = new List<Task>(Points.Count);
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
            try
            {
                semaphore.WaitOne();
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
            }
            finally
            {
                semaphore.Release();
            }
            this.PicBox.Invalidate();
        }

        public void DeleteCellOnCondition(Cell c)
        {
            try
            {
                semaphore.WaitOne();
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
            }
            finally
            {
                semaphore.Release();
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
    }
}