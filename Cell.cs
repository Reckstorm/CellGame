using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace CellGame
{
    public class Cell
    {
        public int size { get; set; }
        public int age { get; set; }
        public bool isAlive { get; set; }
        public Point point { get; set; }
        public Cell()
        {
            this.size = -1;
            this.age = -1;
            this.point = new Point();
        }

        public Cell(int size, int maxAge, Point point)
        {
            this.size = size;
            this.age = 0;
            this.point = point;
            this.isAlive = false;
        }
    }
}
