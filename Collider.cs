using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Collider
    {
        public bool isCollided;
        public ConsoleColor color;

        public Collider(bool isCollided, ConsoleColor color)
        {
            this.isCollided = isCollided;
            this.color = color;
        }
    }
}
