using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    /// <summary>
    /// Die Klasse zum Speichern von den Positionen der Kollider und dessen Farbe
    /// </summary>
    internal class Vector2
    {
        public int x, y;

        public Vector2(int x, int y)
        {
            this.x = x; this.y = y;
        }

        public static Vector2 AddVector(Vector2 vec1, Vector2 vec2)
        {
            if (vec1 != null && vec2 != null)
            {
                Vector2 newPos = new Vector2(0, 0);
                newPos.x = vec1.x + vec2.x;
                newPos.y = vec1.y + vec2.y;
                return newPos;
            }
            else
            {
                //Console.WriteLine("Bei der Methode AddVector wurde ein NULL Wert übergeben");
                return null;
            }
        }
    }
}
