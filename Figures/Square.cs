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
        internal Square()
        {
            StartPos();
            Width = 2;
            tetroColor = ConsoleColor.Yellow;
        }

        private void StartPos()
        {
            StartPos1 = new Vector2(1, 0);
            StartPos2 = new Vector2(1, 1);
            StartPos3 = new Vector2(2, 0);
            StartPos4 = new Vector2(2, 1);
        }

        internal override void ResetPos()
        {
            StartPos();
        }

        internal override void RenderPreview()
        {
            base.RenderPreview();

            RenderTetroPreview();

            void RenderTetroPreview()
            {
                //  ##
                //  ##

                Console.ForegroundColor = tetroColor;
                Vector2 startPos = new Vector2(Program.PreviewPos.x + 2, Program.PreviewPos.y + 2);

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

        internal override void Rotate(int rot, bool enableSound = true)
        {
            base.Rotate(rot, enableSound);
        }
    }
}
