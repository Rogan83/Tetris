using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Figures
{
    internal class J : Tetromino
    {
        /// <summary>
        ///   #
        ///   #
        ///  ##
        /// </summary>
        public J()
        {
            startRotation = rotation = 1;
            width = 3;
            tetroColor = ConsoleColor.DarkBlue;
            StartPos();
        }

        private void StartPos()
        {
            Rotate(startRotation);
        }

        public override void ResetPos()
        {
            Rotate(startRotation);
        }

        public override void Rotate(int rot)
        {
            switch (rot)
            {
                case 1:
                    Vector2 newStartPos1 = new Vector2(1, 0);
                    Vector2 newStartPos2 = new Vector2(1, 1);
                    Vector2 newStartPos3 = new Vector2(2, 1);
                    Vector2 newStartPos4 = new Vector2(3, 1);

                    CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                case 2:
                    newStartPos1 = new Vector2(2, 0);
                    newStartPos2 = new Vector2(3, 0);
                    newStartPos3 = new Vector2(2, 1);
                    newStartPos4 = new Vector2(2, 2);

                    CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                case 3:
                    newStartPos1 = new Vector2(1, 1);
                    newStartPos2 = new Vector2(2, 1);
                    newStartPos3 = new Vector2(3, 1);
                    newStartPos4 = new Vector2(3, 2);

                    CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                case 4:
                    newStartPos1 = new Vector2(2, 0);
                    newStartPos2 = new Vector2(2, 1);
                    newStartPos3 = new Vector2(1, 2);
                    newStartPos4 = new Vector2(2, 2);

                    CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                default:
                    break;
            }
        }

        public override void RenderPreview()
        {
            base.RenderPreview();

            // Die eig. Vorschau vom Tetro rendern
            RenderTetroPreview();

            void RenderTetroPreview()
            {
                ///   #
                ///   #
                ///  ##

                Console.ForegroundColor = tetroColor;
                Vector2 startPos = new Vector2(Program.PrevievPos.x + 2, Program.PrevievPos.y + 2);

                Console.SetCursorPosition(startPos.x + 2, startPos.y);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x + 2, startPos.y + 1);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x + 2, startPos.y + 2);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x + 1, startPos.y + 2);
                Console.WriteLine("#");
            }
        }
    }
}
