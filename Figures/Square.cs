using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Figures
{
    /// <summary>
    /// 
    ///  ##
    ///  ##
    ///  
    /// </summary>
    internal class Square : Tetromino
    {
        public Square()
        {
            StartPos();
            width = 2;
            tetroColor = ConsoleColor.Yellow;
        }

        private void StartPos()
        {
            startPos1 = new Vector2(1, 0);
            startPos2 = new Vector2(1, 1);
            startPos3 = new Vector2(2, 0);
            startPos4 = new Vector2(2, 1);
        }

        public override void ResetPos()
        {
            StartPos();
        }
    }
}
