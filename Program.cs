using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
using System.Data.Common;
using Tetris;

namespace Tetris
{
    public class Program
    {
        static object lockObject = new object();
        static Tetromino t;
        static bool game = true;
        static float speed = 1;
        static int heightEnvironment = 14;
        static int widthEnvironment = 6;        //mindestbreite = 6
        static Vector2 offsetEnvironment = new Vector2(20, 3);
        static Vector2 offSetTetro = new Vector2(0, 5);
        static TetrisBoard tetrisBoard;

        static Timer timerTetroMoveDown, timerCheckInput;
        static bool isCollide = false;

        static int smallestNumb, biggestNumb, xPos;
        static Random random = new Random();

        static ConsoleColor enviromentColor = ConsoleColor.Gray;

        //Eigenschaften fürs blinken, wenn Reihe(n) gelöscht wurden
        static int blinkCount = 3;
        static int blinkSpeed = 200;

        // braucht man nur für die alternative input variante
        //[DllImport("user32.dll")]
        //public static extern short GetKeyState(ConsoleKey vKey);
        //

        static void Main()
        {
            Console.BufferHeight = 70;
            Console.CursorVisible = false;
            //Console.BackgroundColor = ConsoleColor.Black;
            //Console.ForegroundColor = ConsoleColor.White;

            tetrisBoard = new(heightEnvironment, widthEnvironment, enviromentColor);

            InitAndRenderEnvironment();

            RandomTetro();
            // Verschiebt das neue generierte Tetro zufällig in der X-Achse. Dabei darf das Tetro nicht weiter rechts spawnen als die breite zulässt
            RandomPosition();

            if (speed != 0)
                timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, (int)(1000 / speed));
            else
                timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, 0);


            timerCheckInput = new Timer(_ => OnTimerCheckInputElapsed(), null, 0, 10);

            while (game)
            {
                //HandleInput();
            }

            if (!game)
            {
                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            }

            //static void InitCollider()
            //{
            //    for (int x = 0; x < widthEnvironment; x++)
            //    {
            //        for (int y = 0; y < heightEnvironment; y++)
            //        {
            //            collider[x, y] = new Collider(false, ConsoleColor.White);
            //        }
            //    }
            //}
        }

        static void RandomPosition()
        {
            smallestNumb = 0;
            biggestNumb = widthEnvironment - t.width - 1;
            xPos = 1;
            if (biggestNumb >= smallestNumb)
                xPos = random.Next(smallestNumb, biggestNumb);
            else
            {
                Console.WriteLine("Spielbreite ist zu klein gewählt");
                return;
            }

            t.Move(new Vector2(xPos, 0));
        }

        static void OnTimerTetroMoveDownElapsed()
        {
            lock (lockObject)
            {
                DeleteTetro();
                Vector2 moveDown = new Vector2(0, 1);
                isCollide = UpdateGame(moveDown);

                if (isCollide)
                {
                    isCollide = false;
                    RandomTetro();
                    //int r = new Random().Next(1, widthEnvironment - t.width);
                    //t.Move(new Vector2(r, 0));
                    RandomPosition();
                }

                RenderElement();

            }
        }

        static void OnTimerCheckInputElapsed()
        {
            lock (lockObject)
            {
                HandleInput();
            }
        }

        /// <summary>
        /// alte Implementierung
        /// </summary>
        //static void HandleInput()
        //{
        //    ConsoleKeyInfo key = new();
        //    //Wenn keine Taste gedrückt wurde, dann springe raus, damit nicht auf einen Input gewartet wird
        //    if (!Console.KeyAvailable)
        //    {
        //        return;
        //    }

        //    key = Console.ReadKey(true);

        //    switch (key.Key)
        //    {
        //        case ConsoleKey.LeftArrow:
        //        case ConsoleKey.A:
        //            Move(-1, 0);
        //            break;
        //        case ConsoleKey.RightArrow:
        //        case ConsoleKey.D:
        //            Move(1, 0);
        //            break;
        //        case ConsoleKey.DownArrow:
        //        case ConsoleKey.S:
        //            Move(0, 1);
        //            break;
        //        default:
        //            break;
        //    }

        //    ///Der Code löst zwar das Problem, dass das Tetro sich nicht noch bewegt, obwohl man schon die Taste fürs bewegen losgelassen hat, aber dafür muss die Taste in dem Augenblick gedrückt werden, wenn diese Methode
        //    ///ausgeführt wird, was bei "Console.ReadKey" merkwürdigerweise nicht der Fall ist. 
        //    //if ((GetKeyState(ConsoleKey.LeftArrow) & 0x8000) != 0)
        //    //{
        //    //    Move(-1, 0);
        //    //}
        //    //else if ((GetKeyState(ConsoleKey.RightArrow) & 0x8000) != 0)
        //    //{
        //    //    Move(1, 0);
        //    //}
        //    //else if ((GetKeyState(ConsoleKey.DownArrow) & 0x8000) != 0)
        //    //{
        //    //    Move(0, 1);
        //    //}

        //    void Move(int x, int y)
        //    {
        //        DeleteTetro();
        //        UpdateGame(new Vector2(x, y));
        //        RenderElement();
        //    }
        //}

        static void HandleInput()
        {
            // Es gibt einen Mechanismus, bei dem die Betriebssystemtastaturpuffer abgefragt werden, um zu sehen, welche Tasten gedrückt sind. Console.KeyAvailable prüft, ob es Tasten im Puffer gibt,
            // die abgerufen werden können, und wenn ja, wird Console.ReadKey() verwendet, um die nächste verfügbare Taste zu lesen.
            // Das bedeutet, wenn du eine Taste drückst, wird sie im Tastaturpuffer gespeichert. Wenn du dann Console.KeyAvailable abfragst, bevor du tatsächlich Console.ReadKey() aufrufst, wird die
            // vorherige Eingabe, die im Puffer ist, abgerufen, auch wenn die Taste bereits losgelassen wurde.
            // Um dies zu umgehen und eine genauere Kontrolle über den Tastenzustand zu haben, wurde die System.Windows.Forms.SendKeys - Klasse verwendet, die direkten Zugriff auf den aktuellen Zustand
            // der Tastatur ermöglicht. Diese Klasse stützt sich nicht auf den Puffermechanismus von Console.KeyAvailable.

            // Durch die While Schleife werden nacheinander alle Elemente von der Queue zugewiesen und am Ende bleibt nur das letzte Element übrig (deswegen der Name "lagestKeyPress")


            ConsoleKeyInfo latestKeyPress = new ConsoleKeyInfo();

            while (Console.KeyAvailable)
            {
                latestKeyPress = Console.ReadKey(true); // true, um die Eingabe nicht anzuzeigen
            }

            if (latestKeyPress.Key != 0) // Überprüfe, ob eine Taste gedrückt wurde
            {
                switch (latestKeyPress.Key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        Move(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        Move(1, 0);
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        Move(0, 1);
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        Turn(1);
                        break;
                    default:
                        break;
                }
            }

            void Move(int x, int y)
            {
                DeleteTetro();
                isCollide = UpdateGame(new Vector2(x, y));

                if (isCollide)
                {
                    isCollide = false;
                    RandomTetro();
                    //int r = new Random().Next(1, widthEnvironment - t.width);
                    //t.Move(new Vector2(r, 0));
                    RandomPosition();
                }

                RenderElement();
            }

            void Turn(int dir)
            {
                DeleteTetro();
                t.Turn(dir);
                RenderElement();
            }
        }

        static void RandomTetro()
        {
            int randomTetro = new Random().Next(0, 7);
            //int randomTetro = new Random().Next(0, 2);

            switch (randomTetro)
            {
                case 0:
                    t = new I();
                    break;
                case 1:
                    t = new J();
                    break;
                case 2:
                    t = new L();
                    break;
                case 3:
                    t = new Square();
                    break;
                case 4:
                    t = new Z();
                    break;
                case 5:
                    t = new T();
                    break;
                case 6:
                    t = new S();
                    break;
                default:
                    t = new S();
                    break;
            }
        }

        static bool UpdateGame(Vector2 move)
        {
            Vector2 targetPos1 = Vector2.AddVector(t.endPos1, move);
            Vector2 targetPos2 = Vector2.AddVector(t.endPos2, move);
            Vector2 targetPos3 = Vector2.AddVector(t.endPos3, move);
            Vector2 targetPos4 = Vector2.AddVector(t.endPos4, move);

            //if (collider[targetPos1.x, targetPos1.y].isCollided == false &&
            //    collider[targetPos2.x, targetPos2.y].isCollided == false &&
            //    collider[targetPos3.x, targetPos3.y].isCollided == false &&
            //    collider[targetPos4.x, targetPos4.y].isCollided == false)
            if (tetrisBoard.Grid[targetPos1.y][targetPos1.x].isCollided == false &&
                 tetrisBoard.Grid[targetPos2.y][targetPos2.x].isCollided == false &&
                 tetrisBoard.Grid[targetPos3.y][targetPos3.x].isCollided == false &&
                 tetrisBoard.Grid[targetPos4.y][targetPos4.x].isCollided == false)
            {
                t.Move(move);
                return false;
            }
            else if (move.y == 1)
            {
                // Wenn das Tetro mit dem Boden kollidiert ist und die Position vom ersten Element vom Tetro über das Spielfeld befindet, dann soll das Spiel beendet werden.
                if (t.endPos1.y < offSetTetro.y)
                {
                    game = false;
                    timerTetroMoveDown.Dispose();
                    timerCheckInput.Dispose();
                    GameOverText();
                    return false;
                }//Wenn das Tetro mit dem Boden kollidiert ist und nicht über das Spielfeld hinaus ragt
                else
                {
                    SaveNewCollider();                      //Speicher die Pos vom neuen Tetro ab
                    int rowCount = CheckAndClearLines();    //Überprüft, ob eine Linie vollständig ist und löscht diese dann, wenn dies der Fall ist.
                                                            //Gibt die Anzahl der gelöschten Zeilen zurück, welche benötigt wird, um die Punkte zu berechnen

                    RenderAll();                            // Rendert alles neu

                    return true;
                }
            }
            else
            {
                return false;
            }

            // Speichert die Position vom neuen Hindernis


            static void SaveNewCollider()
            {
                //collider[t.endPos1.x, t.endPos1.y] = new Collider(true, t.tetroColor);
                //collider[t.endPos2.x, t.endPos2.y] = new Collider(true, t.tetroColor);
                //collider[t.endPos3.x, t.endPos3.y] = new Collider(true, t.tetroColor);
                //collider[t.endPos4.x, t.endPos4.y] = new Collider(true, t.tetroColor);

                tetrisBoard.Grid[t.endPos1.y][t.endPos1.x] = new Collider(true, t.tetroColor);
                tetrisBoard.Grid[t.endPos2.y][t.endPos2.x] = new Collider(true, t.tetroColor);
                tetrisBoard.Grid[t.endPos3.y][t.endPos3.x] = new Collider(true, t.tetroColor);
                tetrisBoard.Grid[t.endPos4.y][t.endPos4.x] = new Collider(true, t.tetroColor);

                RenderElement();
            }

            static int CheckAndClearLines()
            {
                // Überprüfe, ob die Zeile komplett ist. 
                List<int> linesToClear = new List<int>();

                // Durchlaufe jede Zeile von unten nach oben außer die letzte, da dies der Boden ist
                for (int row = heightEnvironment - 2; row >= 0; row--)
                {
                    // Überprüfe, ob alle Zellen in der Zeile gefüllt sind
                    if (tetrisBoard.Grid[row].All(cell => cell.isCollided == true))
                    {
                        // Füge die zu löschende Zeile zur Liste hinzu
                        linesToClear.Add(row);
                    }
                }
                //bool firstRowRemoved = false;
                #region alternativesLöschen
                // Lösche die vollständigen Zeilen und füge neue leere Zeilen oben hinzu
                //foreach (int row in linesToClear)
                //{
                //    if (!firstRowRemoved)
                //        tetrisBoard.Grid.RemoveAt(row);
                //    else
                //        tetrisBoard.Grid.RemoveAt(row + 1);

                //    firstRowRemoved = true;
                //    tetrisBoard.Grid.Insert(0, new Collider[widthEnvironment]);

                //    for (int c = 0; c < widthEnvironment; c++)
                //    {
                //        if (c == 0 || c == widthEnvironment - 1)
                //            tetrisBoard.Grid[0][c] = new Collider(true, ConsoleColor.White);
                //        else
                //            tetrisBoard.Grid[0][c] = new Collider(false, ConsoleColor.White);
                //    }
                //}
                #endregion

                #region zweiteVarianteZumLöschen
                TetrisBoard newTetrisBoard = new(heightEnvironment, widthEnvironment, enviromentColor);
                InitEnviroment();

                var oldTetrisBoard = CopyFrom(tetrisBoard);           // eine Sicherung vom alten Board, damit ich noch auf die Farben, wie sie vorher waren, zugreifen kann, da einzelne Zeilen gelöscht werden und das tetrisBoard überschrieben wird. Wird für das aufblinken von den Zeilen, die gelöscht wurden benötigt, damit diese mit den ursprünglichen Farben aufblinken

                for (int row = 0; row < heightEnvironment; row++)
                {
                    if (!linesToClear.Contains(row))
                    {
                        for (int column = 0; column < widthEnvironment; column++)
                        {
                            newTetrisBoard.Grid[row][column] = tetrisBoard.Grid[row][column];
                        }
                    }
                    else        //Wenn die Zeile nicht hinzugefügt werden soll, dann sollen alle anderen Zeilen um 1 Einheit nach unten verschoben werden
                    {
                        TetrisBoard temp = CopyFrom(newTetrisBoard);    // Eine Kopie vom neuen Board anlegen
                        MoveAllElementsDown(newTetrisBoard, row, temp); // Eine Einheit nach unten verschieben
                    }
                }

                tetrisBoard = newTetrisBoard;
                #endregion

                Blink();
                
                // blinken
                void Blink()
                {
                    timerTetroMoveDown.Change(Timeout.Infinite, Timeout.Infinite);   // Timer anhalten

                    if (linesToClear.Count > 0)
                    {
                        for (int i = 0; i < blinkCount; i++)
                        {
                            foreach (var rowToClear in linesToClear)
                            {
                                for (int column = 1; column < widthEnvironment - 1; column++)
                                {
                                    Console.SetCursorPosition(column + offsetEnvironment.x, rowToClear + offsetEnvironment.y);
                                    Console.Write(" ");
                                }
                            }

                            Thread.Sleep(blinkSpeed);

                            foreach (var rowToClear in linesToClear)
                            {
                                for (int column = 1; column < widthEnvironment - 1; column++)
                                {
                                    Console.SetCursorPosition(column + offsetEnvironment.x, rowToClear + offsetEnvironment.y);
                                    Console.ForegroundColor = oldTetrisBoard.Grid[rowToClear][column].color;
                                    Console.Write("#");
                                }
                            }

                            Thread.Sleep(blinkSpeed);
                        }
                    }

                    timerTetroMoveDown.Change(0, (int)(1000 / speed));  // timer wieder starten
                }



                return linesToClear.Count;

                void InitEnviroment()
                {
                    for (int row = 0; row < heightEnvironment; row++)
                    {
                        for (int column = 0; column < widthEnvironment; column++)
                        {
                            if (row == heightEnvironment - 1)
                            {
                                newTetrisBoard.Grid[row][column] = new Collider(true, enviromentColor);
                            }
                            else if (column == 0 || column == widthEnvironment - 1)
                            {
                                newTetrisBoard.Grid[row][column] = new Collider(true, enviromentColor);
                            }
                        }
                    }
                }

                static TetrisBoard CopyFrom(TetrisBoard newTetrisBoard)
                {
                    TetrisBoard temp = new(heightEnvironment, widthEnvironment, enviromentColor);

                    for (int r = 0; r < heightEnvironment; r++)
                    {
                        for (int c = 0; c < widthEnvironment; c++)
                        {
                            temp.Grid[r][c] = newTetrisBoard.Grid[r][c];
                        }
                    }

                    return temp;
                }

                static void MoveAllElementsDown(TetrisBoard newTetrisBoard, int row, TetrisBoard temp)
                {
                    for (int i = 0; i < row; i++)
                    {
                        newTetrisBoard.Grid[i + 1] = temp.Grid[i];
                    }
                }
            }

            static void RenderAll()
            {
                // Render die Umgebung inkl. Tetros (mit den richtigen Farben) erneut
                for (int y = 0; y < heightEnvironment; y++)
                {
                    for (int x = 0; x < widthEnvironment; x++)
                    {
                        //Lösche die Elemente an dieser Stelle
                        Console.SetCursorPosition(x + offsetEnvironment.x, y + offsetEnvironment.y);
                        Console.WriteLine(" ");

                        if (tetrisBoard.Grid[y][x].isCollided == true)
                        {
                            Console.SetCursorPosition(x + offsetEnvironment.x, y + offsetEnvironment.y);
                            Console.ForegroundColor = tetrisBoard.Grid[y][x].color;
                            if (y == heightEnvironment - 1)
                                Console.WriteLine("-");
                            else if (y >= offSetTetro.y)
                            {
                                if (x == 0 || x == widthEnvironment - 1)
                                    Console.WriteLine("|");
                                else
                                    Console.WriteLine("#");
                            }
                        }

                    }
                }
            }
        }

        static void RenderElement()
        {
            if (t.isDead)
            {
                return;
            }

            Console.ForegroundColor = t.tetroColor;

            Console.SetCursorPosition(t.endPos1.x + offsetEnvironment.x, t.endPos1.y + offsetEnvironment.y);
            Console.Write("#");
            Console.SetCursorPosition(t.endPos2.x + offsetEnvironment.x, t.endPos2.y + offsetEnvironment.y);
            Console.Write("#");
            Console.SetCursorPosition(t.endPos3.x + offsetEnvironment.x, t.endPos3.y + offsetEnvironment.y);
            Console.Write("#");
            Console.SetCursorPosition(t.endPos4.x + offsetEnvironment.x, t.endPos4.y + offsetEnvironment.y);
            Console.Write("#");
        }

        static void DeleteTetro()
        {
            if (t.endPos1 != null)
            {
                Console.SetCursorPosition(t.endPos1.x + offsetEnvironment.x, t.endPos1.y + offsetEnvironment.y);
                Console.Write(" ");
            }
            if (t.endPos2 != null)
            {
                Console.SetCursorPosition(t.endPos2.x + offsetEnvironment.x, t.endPos2.y + offsetEnvironment.y);
                Console.Write(" ");
            }
            if (t.endPos3 != null)
            {
                Console.SetCursorPosition(t.endPos3.x + offsetEnvironment.x, t.endPos3.y + offsetEnvironment.y);
                Console.Write(" ");
            }
            if (t.endPos4 != null)
            {
                Console.SetCursorPosition(t.endPos4.x + offsetEnvironment.x, t.endPos4.y + offsetEnvironment.y);
                Console.Write(" ");
            }
        }
        /// <summary>
        ///  Speichert die Umgebung und rendert diese
        /// </summary>
        static void InitAndRenderEnvironment()
        {
            Console.ForegroundColor = enviromentColor;
            for (int i = 0; i < heightEnvironment; i++)
            {
                Console.SetCursorPosition(offsetEnvironment.x, i + offsetEnvironment.y);
                //collider[0, i] = new Collider(true, ConsoleColor.White);
                tetrisBoard.Grid[i][0] = new Collider(true, enviromentColor);
                if (i >= offSetTetro.y)
                    Console.Write("|");

                Console.SetCursorPosition(widthEnvironment - 1 + offsetEnvironment.x, i + offsetEnvironment.y);
                //collider[widthEnvironment - 1, i] = new Collider(true, ConsoleColor.White);
                tetrisBoard.Grid[i][widthEnvironment - 1] = new Collider(true, enviromentColor);
                if (i >= offSetTetro.y)
                    Console.Write("|");
            }

            for (int i = 0; i < widthEnvironment; i++)
            {
                Console.SetCursorPosition(i + offsetEnvironment.x, heightEnvironment - 1 + offsetEnvironment.y);
                //collider[i, heightEnvironment - 1] = new Collider(true, ConsoleColor.White);
                tetrisBoard.Grid[heightEnvironment - 1][i] = new Collider(true, enviromentColor);
                Console.Write("-");
            }
            //Debug
            //for (int x = 0; x < widthEnvironment; x++)
            //{
            //    for (int y = 0; y < heightEnvironment; y++)
            //    {
            //        if (collider[x, y].isCollided == true)
            //        {
            //            Console.SetCursorPosition(x + offsetEnvironment.x, y + offsetEnvironment.y);
            //            Console.ForegroundColor = ConsoleColor.Red;
            //            Console.Write("a");
            //        }
            //    }
            //}
        }

        static void GameOverText()
        {
            Console.SetCursorPosition(offsetEnvironment.x, 0);
            RenderElement();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("GAME OVER");

            int space = 5;
            for (int i = 0; i < space; i++)
            {
                Console.WriteLine("");
            }
            //Console.ReadLine();
            //while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        abstract class Tetromino
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

                if (newEndPos1.x < 0 || newEndPos1.x > (widthEnvironment - 1) ||
                    newEndPos2.x < 0 || newEndPos2.x > (widthEnvironment - 1) ||
                    newEndPos3.x < 0 || newEndPos3.x > (widthEnvironment - 1) ||
                    newEndPos4.x < 0 || newEndPos4.x > (widthEnvironment - 1))
                {
                    //Falls Außerhalb vom Spielfeld, dann soll die Rotation rückgängig gemacht werden
                    rotation = oldRotation;
                    return;
                }

                //if (collider[newEndPos1.x, newEndPos1.y].isCollided == false &&
                //    collider[newEndPos2.x, newEndPos2.y].isCollided == false &&
                //    collider[newEndPos3.x, newEndPos3.y].isCollided == false &&
                //    collider[newEndPos4.x, newEndPos4.y].isCollided == false)
                if (tetrisBoard.Grid[newEndPos1.y][newEndPos1.x].isCollided == false &&
                    tetrisBoard.Grid[newEndPos2.y][newEndPos2.x].isCollided == false &&
                    tetrisBoard.Grid[newEndPos3.y][newEndPos3.x].isCollided == false &&
                    tetrisBoard.Grid[newEndPos4.y][newEndPos4.x].isCollided == false)
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

            public virtual void Rotate(int rotation) { }
        }
        /// <summary>
        ///   #
        ///   #
        ///   #
        ///   #
        /// </summary>
        class I : Tetromino
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
                Rotate(startRotation);
            }

            public override void ResetPos()
            {
                //StartPos();
                Rotate(startRotation);
            }

            public override void Rotate(int rot)
            {
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
        }
        /// <summary>
        ///   #
        ///   #
        ///   #
        /// ###
        /// </summary>
        class J : Tetromino
        {
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
        }
        /// <summary>
        /// #
        /// #
        /// #
        /// ###
        /// </summary>
        class L : Tetromino
        {
            public L()
            {
                startRotation = rotation = 1;
                width = 3;
                tetroColor = ConsoleColor.Red;
                StartPos();
            }

            void StartPos()
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
                        Vector2 newStartPos1 = new Vector2(1, 1);
                        Vector2 newStartPos2 = new Vector2(2, 1);
                        Vector2 newStartPos3 = new Vector2(3, 1);
                        Vector2 newStartPos4 = new Vector2(3, 0);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 2:
                        newStartPos1 = new Vector2(2, 0);
                        newStartPos2 = new Vector2(2, 1);
                        newStartPos3 = new Vector2(2, 2);
                        newStartPos4 = new Vector2(3, 2);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 3:
                        newStartPos1 = new Vector2(1, 1);
                        newStartPos2 = new Vector2(1, 2);
                        newStartPos3 = new Vector2(2, 1);
                        newStartPos4 = new Vector2(3, 1);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 4:
                        newStartPos1 = new Vector2(1, 0);
                        newStartPos2 = new Vector2(2, 0);
                        newStartPos3 = new Vector2(2, 1);
                        newStartPos4 = new Vector2(2, 2);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 
        ///  ##
        ///  ##
        ///  
        /// </summary>
        class Square : Tetromino
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
        /// <summary>
        /// 
        ///  ##
        ///   ##
        ///   
        /// </summary>
        class Z : Tetromino
        {
            public Z()
            {
                startRotation = rotation = 1;
                width = 3;
                tetroColor = ConsoleColor.DarkRed;
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
                        Vector2 newStartPos2 = new Vector2(2, 0);
                        Vector2 newStartPos3 = new Vector2(2, 1);
                        Vector2 newStartPos4 = new Vector2(3, 1);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 2:
                        newStartPos1 = new Vector2(2, 0);
                        newStartPos2 = new Vector2(1, 1);
                        newStartPos3 = new Vector2(2, 1);
                        newStartPos4 = new Vector2(1, 2);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 3:
                        newStartPos1 = new Vector2(1, 1);
                        newStartPos2 = new Vector2(2, 1);
                        newStartPos3 = new Vector2(2, 2);
                        newStartPos4 = new Vector2(3, 2);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 4:
                        newStartPos1 = new Vector2(2, 0);
                        newStartPos2 = new Vector2(1, 1);
                        newStartPos3 = new Vector2(2, 1);
                        newStartPos4 = new Vector2(1, 2);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// ###
        ///  #
        ///  #
        /// </summary>
        class T : Tetromino
        {
            public T()
            {
                startRotation = rotation = 1;
                width = 3;
                tetroColor = ConsoleColor.DarkMagenta;
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
                        Vector2 newStartPos1 = new Vector2(1, 1);
                        Vector2 newStartPos2 = new Vector2(2, 0);
                        Vector2 newStartPos3 = new Vector2(2, 1);
                        Vector2 newStartPos4 = new Vector2(3, 1);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 2:
                        newStartPos1 = new Vector2(1, 0);
                        newStartPos2 = new Vector2(1, 1);
                        newStartPos3 = new Vector2(2, 1);
                        newStartPos4 = new Vector2(1, 2);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 3:
                        newStartPos1 = new Vector2(1, 1);
                        newStartPos2 = new Vector2(2, 1);
                        newStartPos3 = new Vector2(2, 2);
                        newStartPos4 = new Vector2(3, 1);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 4:
                        newStartPos1 = new Vector2(2, 0);
                        newStartPos2 = new Vector2(1, 1);
                        newStartPos3 = new Vector2(2, 1);
                        newStartPos4 = new Vector2(2, 2);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// #
        /// ##
        ///  #
        /// </summary>
        class S : Tetromino
        {
            public S()
            {
                startRotation = rotation = 1;
                width = 3;
                tetroColor = ConsoleColor.Green;
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
                        Vector2 newStartPos1 = new Vector2(1, 1);
                        Vector2 newStartPos2 = new Vector2(2, 0);
                        Vector2 newStartPos3 = new Vector2(2, 1);
                        Vector2 newStartPos4 = new Vector2(3, 0);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 2:
                        newStartPos1 = new Vector2(1, 0);
                        newStartPos2 = new Vector2(1, 1);
                        newStartPos3 = new Vector2(2, 1);
                        newStartPos4 = new Vector2(2, 2);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 3:
                        newStartPos1 = new Vector2(1, 2);
                        newStartPos2 = new Vector2(2, 1);
                        newStartPos3 = new Vector2(2, 2);
                        newStartPos4 = new Vector2(3, 1);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    case 4:
                        newStartPos1 = new Vector2(1, 0);
                        newStartPos2 = new Vector2(1, 1);
                        newStartPos3 = new Vector2(2, 1);
                        newStartPos4 = new Vector2(2, 2);

                        CheckForCollision(newStartPos1, newStartPos2, newStartPos3, newStartPos4);
                        break;
                    default:
                        break;
                }
            }
        }

        class Vector2
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

        /// <summary>
        /// Die Klasse zum Speichern von den Positionen der Kollider und dessen Farbe
        /// </summary>

    }
}

public class Collider
{
    public bool isCollided;
    public ConsoleColor color;

    public Collider(bool isCollided, ConsoleColor color)
    {
        this.isCollided = isCollided;
        this.color = color;
    }
}

public class TetrisBoard
{
    public int Rows { get; private set; }
    public int Cols { get; private set; }

    public ConsoleColor EnviromentColor { get; set; }
    public List<Collider[]> Grid { get; private set; }

    public TetrisBoard(int rows, int cols, ConsoleColor enviromentColor)
    {
        Rows = rows;
        Cols = cols;
        this.EnviromentColor = enviromentColor;
        InitializeGrid(enviromentColor);
    }

    private void InitializeGrid(ConsoleColor enviromentColor)
    {
        Grid = new List<Collider[]>();
        for (int r = 0; r < Rows; r++)
        {
            Grid.Add(new Collider[Cols]);
            for (int c = 0; c < Cols; c++)
            {
                Grid[r][c] = new Collider(false, enviromentColor);  
            }
        }
    }

    // Weitere Methoden für das Hinzufügen von Tetrominos, Überprüfen und Löschen von Zeilen, usw.
}