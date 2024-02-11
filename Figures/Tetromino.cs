using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Tetris.Figures
{
    internal abstract class Tetromino
    {
        public Vector2 startPos1;
        public Vector2 startPos2;
        public Vector2 startPos3;
        public Vector2 startPos4;

        public Vector2 endPos1;
        public Vector2 endPos2;
        public Vector2 endPos3;
        public Vector2 endPos4;

        public Vector2 shift = new Vector2(0, 0);       // wie weit hat sich das Tetro während dem spielen verschoben

        public int width;
        protected int rotation, startRotation;
        public int oldRotation;
        public bool isDead = false;
        public ConsoleColor tetroColor;

        public void CalculateEndPositions()
        {
            endPos1 = Vector2.AddVector(startPos1, shift);
            endPos2 = Vector2.AddVector(startPos2, shift);
            endPos3 = Vector2.AddVector(startPos3, shift);
            endPos4 = Vector2.AddVector(startPos4, shift);
        }

        public void Move(Vector2 offset)
        {
            shift = Vector2.AddVector(offset, shift);
            CalculateEndPositions();
        }

        public void Move(int x, int y)
        {
            Vector2 offset = new Vector2(x, y);
            shift = Vector2.AddVector(offset, shift);

            CalculateEndPositions();
        }

        //Überprüft, ob nach der Rotation eine Kollision entsteht bzw. ob sich ein Element außerhalb vom Spielfeld betrifft. Falls nicht, dann rotiert sich das Element, ansonsten nicht
        protected void CheckForCollision(Vector2 newStartPos1, Vector2 newStartPos2, Vector2 newStartPos3, Vector2 newStartPos4)
        {
            Vector2 newEndPos1 = Vector2.AddVector(newStartPos1, shift);
            Vector2 newEndPos2 = Vector2.AddVector(newStartPos2, shift);
            Vector2 newEndPos3 = Vector2.AddVector(newStartPos3, shift);
            Vector2 newEndPos4 = Vector2.AddVector(newStartPos4, shift);

            if (newEndPos1.x < 0 || newEndPos1.x > Program.widthEnvironment - 1 ||
                newEndPos2.x < 0 || newEndPos2.x > Program.widthEnvironment - 1 ||
                newEndPos3.x < 0 || newEndPos3.x > Program.widthEnvironment - 1 ||
                newEndPos4.x < 0 || newEndPos4.x > Program.widthEnvironment - 1)
            {
                //Falls Außerhalb vom Spielfeld, dann soll die Rotation rückgängig gemacht werden
                rotation = oldRotation;
                return;
            }

            //if (collider[newEndPos1.x, newEndPos1.y].isCollided == false &&
            //    collider[newEndPos2.x, newEndPos2.y].isCollided == false &&
            //    collider[newEndPos3.x, newEndPos3.y].isCollided == false &&
            //    collider[newEndPos4.x, newEndPos4.y].isCollided == false)
            if (Program.tetrisBoard.Grid[newEndPos1.y][newEndPos1.x].isCollided == false &&
                Program.tetrisBoard.Grid[newEndPos2.y][newEndPos2.x].isCollided == false &&
                Program.tetrisBoard.Grid[newEndPos3.y][newEndPos3.x].isCollided == false &&
                Program.tetrisBoard.Grid[newEndPos4.y][newEndPos4.x].isCollided == false)
            {
                startPos1 = newStartPos1;
                startPos2 = newStartPos2;
                startPos3 = newStartPos3;
                startPos4 = newStartPos4;
            }
            else
            {
                //Falls Kollision, dann soll die Rotation rückgängig gemacht werden
                rotation = oldRotation;
            }
        }

        public virtual void ResetPos() { }

        public void Turn(int dir)
        {
            oldRotation = rotation;  // Die vorherige Rotation
            rotation += dir;            // Die neue Rotation

            if (rotation <= 0)
                rotation = 4;
            else if (rotation > 4)
                rotation = 1;

            Rotate(rotation);

            CalculateEndPositions();
        }
        public virtual void Rotate(int rotation, bool enableSound = true)
        {
            if (enableSound)
            {
                Audio.Play("Sounds/Rotate.mp3");
            }
        }

        
        /// <summary>
        /// Eine Vorschau darstellen, welches Element als nächstes erwartet wird
        /// </summary>
        public virtual void RenderPreview()
        {
            Console.SetCursorPosition(Program.PrevievPos.x, Program.PrevievPos.y - 2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Next:");
            Clear();
            RenderFrame();

            void RenderFrame()
            {
                // Rahmen von der Vorschau Rendern
                for (int y = 0; y < Program.PreviewFrameSize.y; y++)
                {
                    for (int x = 0; x < Program.PreviewFrameSize.x; x++)
                    {

                        if ((y == 0 || y == Program.PreviewFrameSize.y - 1) ||
                            (x == 0 || x == Program.PreviewFrameSize.x - 1))
                        {
                            Console.SetCursorPosition(Program.PrevievPos.x + x, Program.PrevievPos.y + y);
                            Console.ForegroundColor = Program.PreviewFrameColor;
                            Console.WriteLine("*");
                        }
                    }
                }
            }

            static void Clear()
            {
                for (int y = 0; y < Program.PreviewFrameSize.y; y++)
                {
                    for (int x = 0; x < Program.PreviewFrameSize.x; x++)
                    {
                        Console.SetCursorPosition(Program.PrevievPos.x + x, Program.PrevievPos.y + y);
                        Console.WriteLine(" ");
                    }
                }
            }
        }
    }
}
