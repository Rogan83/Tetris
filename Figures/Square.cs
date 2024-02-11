using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Figures
{
    /// <summary>
    ///  ##
    ///  ##
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

        public override void RenderPreview()
        {
            base.RenderPreview();

            RenderTetroPreview();

            void RenderTetroPreview()
            {
                //  ##
                //  ##

                Console.ForegroundColor = tetroColor;
                Vector2 startPos = new Vector2(Program.PrevievPos.x + 2, Program.PrevievPos.y + 2);

                Console.SetCursorPosition(startPos.x, startPos.y);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x + 1, startPos.y);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x, startPos.y + 1);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x + 1, startPos.y + 1);
                Console.WriteLine("#");
            }


        }

        public override void Rotate(int rot, bool enableSound = true)
        {
            base.Rotate(rot, enableSound);
        }
    }
}
