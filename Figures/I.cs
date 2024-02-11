﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Figures
{
    /// <summary>
    ///   #
    ///   #
    ///   #
    ///   #
    /// </summary>
    internal class I : Tetromino
    {
        public I()
        {
            startRotation = rotation = 1;
            width = 4;
            tetroColor = ConsoleColor.Blue;
            StartPos();
        }

        private void StartPos()
        {
            Rotate(startRotation, false);
        }

        public override void ResetPos()
        {
            //StartPos();
            Rotate(startRotation);
        }

        public override void Rotate(int rot, bool enableSound = true)
        {
            base.Rotate(rot, enableSound);
            switch (rot)
            {
                case 1:
                    Vector2 newStartPos1 = new Vector2(1, 1);
                    Vector2 newStartPos2 = new Vector2(2, 1);
                    Vector2 newStartPos3 = new Vector2(3, 1);
                    Vector2 newStartPos4 = new Vector2(4, 1);

                    CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                case 2:
                    newStartPos1 = new Vector2(2, 0);
                    newStartPos2 = new Vector2(2, 1);
                    newStartPos3 = new Vector2(2, 2);
                    newStartPos4 = new Vector2(2, 3);

                    CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                case 3:
                    newStartPos1 = new Vector2(0, 2);
                    newStartPos2 = new Vector2(1, 2);
                    newStartPos3 = new Vector2(2, 2);
                    newStartPos4 = new Vector2(3, 2);

                    CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                case 4:
                    newStartPos1 = new Vector2(1, 0);
                    newStartPos2 = new Vector2(1, 1);
                    newStartPos3 = new Vector2(1, 2);
                    newStartPos4 = new Vector2(1, 3);

                    CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                    break;
                default:
                    break;
            }
        }

        public override void RenderPreview()
        {
            base.RenderPreview();

            RenderTetroPreview();

            void RenderTetroPreview()
            {
                //   #
                //   #
                //   #
                //   #

                Console.ForegroundColor = tetroColor;
                Vector2 startPos = new Vector2(Program.PrevievPos.x + 2, Program.PrevievPos.y + 2);

                Console.SetCursorPosition(startPos.x, startPos.y);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x, startPos.y + 1);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x, startPos.y + 2);
                Console.WriteLine("#");
                Console.SetCursorPosition(startPos.x, startPos.y + 3);
                Console.WriteLine("#");
            }
        }
    }
}
