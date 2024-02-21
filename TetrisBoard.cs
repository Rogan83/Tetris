using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class TetrisBoard
    {
        internal int Rows { get; private set; }
        internal int Cols { get; private set; }

        internal ConsoleColor EnviromentColor { get; set; }
        internal List<Collider[]> Grid { get; private set; }

        internal TetrisBoard(int rows, int cols, ConsoleColor enviromentColor)
        {
            Rows = rows;
            Cols = cols;
            this.EnviromentColor = enviromentColor;
            InitializeGrid(enviromentColor);
        }

        void InitializeGrid(ConsoleColor enviromentColor)
        {
            Grid = new List<Collider[]>();
            for (int r = 0; r < Rows; r++)
            {
                Grid.Add(new Collider[Cols]);
                for (int c = 0; c < Cols; c++)
                {
                    Grid[r][c] = new Collider(false, enviromentColor);
                }
            }
        }
    }
}
