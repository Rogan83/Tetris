using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tetris.Menus;


namespace Tetris.Figures
{
    internal abstract class Tetromino
    {
        protected int rotation, startRotation;
        #region properties
        internal Vector2 StartPos1 { get; set; }  = new(0,0);
        internal Vector2 StartPos2 { get; set; }  = new(0,0);
        internal Vector2 StartPos3 { get; set; }  = new(0,0);
        internal Vector2 StartPos4 { get; set; }  = new(0,0);

        internal Vector2 EndPos1 { get; set; }  = new(0,0);
        internal Vector2 EndPos2 { get; set; }  = new(0,0);
        internal Vector2 EndPos3 { get; set; }  = new(0,0);
        internal Vector2 EndPos4 { get; set; }  = new(0,0);

        internal Vector2 TetroShift { get; set; } = new(0,0);       // wie weit hat sich das Tetro während dem spielen verschoben. Die einzelnen Elemente vom Tetro (StartPos1 - 4) mit ihren eigenen Positionen werden relativ zu diesen Vector ausgerichtet.

        internal int Width { get; set; }
        internal int OldRotation { get; set; }
        //internal bool IsDead { get; set; } = false;
        internal ConsoleColor tetroColor { get; set; }
        #endregion
        
        #region methods
        internal void CalculateEndPositions()
        {
            EndPos1 = Vector2.AddVector(StartPos1, TetroShift);
            EndPos2 = Vector2.AddVector(StartPos2, TetroShift);
            EndPos3 = Vector2.AddVector(StartPos3, TetroShift);
            EndPos4 = Vector2.AddVector(StartPos4, TetroShift);
        }

        internal void MoveTetro(Vector2 offset)
        {
            TetroShift = Vector2.AddVector(offset, TetroShift);
            CalculateEndPositions();
        }

        internal void Move(int x, int y)
        {
            Vector2 offset = new Vector2(x, y);
            TetroShift = Vector2.AddVector(offset, TetroShift);       // Berechnet die neue Position, von wo sich die einzelen Elemente vom Tetro orientieren.

            CalculateEndPositions();
        }

        //Überprüft, ob nach der Rotation eine Kollision entsteht bzw. ob sich ein Element außerhalb vom Spielfeld betrifft.
        //Falls nicht, dann rotiert sich das Element.
        protected void CheckForCollision(Vector2 newStartPos1, Vector2 newStartPos2, Vector2 newStartPos3, Vector2 newStartPos4)
        {
            Vector2 newEndPos1 = Vector2.AddVector(newStartPos1, TetroShift);
            Vector2 newEndPos2 = Vector2.AddVector(newStartPos2, TetroShift);
            Vector2 newEndPos3 = Vector2.AddVector(newStartPos3, TetroShift);
            Vector2 newEndPos4 = Vector2.AddVector(newStartPos4, TetroShift);

            if (newEndPos1.x < 0 || newEndPos1.x > Program.WidthEnvironment - 1 ||
                newEndPos2.x < 0 || newEndPos2.x > Program.WidthEnvironment - 1 ||
                newEndPos3.x < 0 || newEndPos3.x > Program.WidthEnvironment - 1 ||
                newEndPos4.x < 0 || newEndPos4.x > Program.WidthEnvironment - 1)
            {
                //Falls Außerhalb vom Spielfeld, dann soll die Rotation rückgängig gemacht werden
                rotation = OldRotation;
                return;  
            }

            if (Program.tetrisBoard.Grid[newEndPos1.y][newEndPos1.x].isCollided == false &&
                Program.tetrisBoard.Grid[newEndPos2.y][newEndPos2.x].isCollided == false &&
                Program.tetrisBoard.Grid[newEndPos3.y][newEndPos3.x].isCollided == false &&
                Program.tetrisBoard.Grid[newEndPos4.y][newEndPos4.x].isCollided == false)
            {
                StartPos1 = newStartPos1;
                StartPos2 = newStartPos2;
                StartPos3 = newStartPos3;
                StartPos4 = newStartPos4;
            }
            else
            {
                //Falls Kollision, dann soll die Rotation rückgängig gemacht werden
                rotation = OldRotation;
            }
        }

        internal virtual void ResetPos() { }

        internal void Turn(int dir)
        {
            OldRotation = rotation;  // Die vorherige Rotation
            rotation += dir;            // Die neue Rotation

            if (rotation <= 0)
                rotation = 4;
            else if (rotation > 4)
                rotation = 1;

            Rotate(rotation);

            CalculateEndPositions();
        }
        internal virtual void Rotate(int rotation, bool enableSound = true)
        {
            if (enableSound)
            {
                Settings.soundtrack.Play("Sounds/Rotate.mp3");
            }
        }

        /// <summary>
        /// Eine Vorschau darstellen, welches Element als nächstes erwartet wird
        /// </summary>
        internal virtual void RenderPreview()
        {
            Console.SetCursorPosition(Program.PreviewPos.x, Program.PreviewPos.y - 2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Next:");
            Clear();
            RenderFrame();
            #region local methods
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
                            Console.SetCursorPosition(Program.PreviewPos.x + x, Program.PreviewPos.y + y);
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
                        Console.SetCursorPosition(Program.PreviewPos.x + x, Program.PreviewPos.y + y);
                        Console.WriteLine(" ");
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
