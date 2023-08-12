using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellGame
{
    public class Cell
    {
        public int size { get; set; }
        public int age { get; set; }
        public static int maxAge { get; set; }
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
            Cell.maxAge = maxAge;
            this.point = point;
        }
    }
}
