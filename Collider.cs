using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Collider
    {
        internal bool isPositionOccupied;
        internal ConsoleColor color;

        internal Collider(bool isFieldOccupied, ConsoleColor color)
        {
            this.isPositionOccupied = isFieldOccupied;
            this.color = color;
        }
    }
}
