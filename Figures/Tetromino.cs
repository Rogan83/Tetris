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

        internal Vector2 CurrentPos1 { get; set; }  = new(0,0);
        internal Vector2 CurrentPos2 { get; set; }  = new(0,0);
        internal Vector2 CurrentPos3 { get; set; }  = new(0,0);
        internal Vector2 CurrentPos4 { get; set; }  = new(0,0);

        internal Vector2 TetroShift { get; set; } = new(0,0);       // wie weit hat sich das Tetro während dem spielen verschoben. Die einzelnen Elemente vom Tetro (StartPos1 - 4) mit ihren eigenen Positionen werden relativ zu diesen Vector ausgerichtet.

        internal int Width { get; set; }
        internal int OldRotation { get; set; }
        //internal bool IsDead { get; set; } = false;
        internal ConsoleColor tetroColor { get; set; }
        #endregion
        
        #region methods
        internal void CalculateCurrentPositions()
        {
            CurrentPos1 = Vector2.AddVector(StartPos1, TetroShift);
            CurrentPos2 = Vector2.AddVector(StartPos2, TetroShift);
            CurrentPos3 = Vector2.AddVector(StartPos3, TetroShift);
            CurrentPos4 = Vector2.AddVector(StartPos4, TetroShift);
        }

        internal void MoveTetro(Vector2 offset)
        {
            TetroShift = Vector2.AddVector(offset, TetroShift);
            CalculateCurrentPositions();
        }

        internal void MoveTetro(int x, int y)
        {
            Vector2 offset = new Vector2(x, y);
            TetroShift = Vector2.AddVector(offset, TetroShift);       // Berechnet die neue Position, von wo sich die einzelen Elemente vom Tetro orientieren.

            CalculateCurrentPositions();
        }

        //Überprüft, ob nach der Rotation eine Kollision entsteht bzw. ob sich ein Element außerhalb vom Spielfeld betrifft.
        //Falls nicht, dann rotiert sich das Element.
        protected void CheckForCollisionAfterRotation(Vector2 newStartPos1, Vector2 newStartPos2, Vector2 newStartPos3, Vector2 newStartPos4)
        {
            Vector2 newCurrentPos1 = Vector2.AddVector(newStartPos1, TetroShift);
            Vector2 newCurrentPos2 = Vector2.AddVector(newStartPos2, TetroShift);
            Vector2 newCurrentPos3 = Vector2.AddVector(newStartPos3, TetroShift);
            Vector2 newCurrentPos4 = Vector2.AddVector(newStartPos4, TetroShift);

            if (newCurrentPos1.x < 0 || newCurrentPos1.x > Program.WidthEnvironment - 1 ||
                newCurrentPos2.x < 0 || newCurrentPos2.x > Program.WidthEnvironment - 1 ||
                newCurrentPos3.x < 0 || newCurrentPos3.x > Program.WidthEnvironment - 1 ||
                newCurrentPos4.x < 0 || newCurrentPos4.x > Program.WidthEnvironment - 1)
            {
                //Falls Außerhalb vom Spielfeld, dann soll die Rotation rückgängig gemacht werden
                rotation = OldRotation;
                return;  
            }

            if (Program.tetrisBoard.Grid[newCurrentPos1.y][newCurrentPos1.x].isPositionOccupied == false &&
                Program.tetrisBoard.Grid[newCurrentPos2.y][newCurrentPos2.x].isPositionOccupied == false &&
                Program.tetrisBoard.Grid[newCurrentPos3.y][newCurrentPos3.x].isPositionOccupied == false &&
                Program.tetrisBoard.Grid[newCurrentPos4.y][newCurrentPos4.x].isPositionOccupied == false)
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

            CalculateCurrentPositions();
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
