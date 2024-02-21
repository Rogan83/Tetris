using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Figures
{
    /// <summary>
    /// #
    /// #
    /// ##
    /// </summary>
    internal class L : Tetromino
    {
        internal L()
        {
            startRotation = rotation = 1;
            Width = 3;
            tetroColor = ConsoleColor.Red;
            StartPos();
        }

        void StartPos()
        {
            Rotate(startRotation, false);
        }

        internal override void ResetPos()
        {
            Rotate(startRotation);
        }

        internal override void Rotate(int rot, bool enableSound = true)
        {
            base.Rotate(rot, enableSound);
            switch (rot)
            {
                case 1:
                    Vector2 newStartPos1 = new Vector2(1, 1);
                    Vector2 newStartPos2 = new Vector2(2, 1);
                    Vector2 newStartPos3 = new Vector2(3, 1);
                    Vector2 newStartPos4 = new Vector2(3, 0);

                    CheckForCollisionAfterRotation(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                case 2:
                    newStartPos1 = new Vector2(2, 0);
                    newStartPos2 = new Vector2(2, 1);
                    newStartPos3 = new Vector2(2, 2);
                    newStartPos4 = new Vector2(3, 2);

                    CheckForCollisionAfterRotation(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                case 3:
                    newStartPos1 = new Vector2(1, 1);
                    newStartPos2 = new Vector2(1, 2);
                    newStartPos3 = new Vector2(2, 1);
                    newStartPos4 = new Vector2(3, 1);

                    CheckForCollisionAfterRotation(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                case 4:
                    newStartPos1 = new Vector2(1, 0);
                    newStartPos2 = new Vector2(2, 0);
                    newStartPos3 = new Vector2(2, 1);
                    newStartPos4 = new Vector2(2, 2);

                    CheckForCollisionAfterRotation(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                default:
                    break;
            }
        }

        internal override void RenderPreview()
        {
            base.RenderPreview();

            RenderTetroPreview();

            void RenderTetroPreview()
            {
                /// #
                /// #
                /// ##

                Console.ForegroundColor = tetroColor;
                Vector2 startPos = new Vector2(Program.PreviewPos.x + 2, Program.PreviewPos.y + 2);

                Console.SetCursorPosition(startPos.x, startPos.y);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x, startPos.y + 1);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x, startPos.y + 2);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x + 1, startPos.y + 2);
                Console.WriteLine("#");
            }
        }
    }
}
