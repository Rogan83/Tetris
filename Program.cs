using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
using System.Data.Common;
using Tetris.Figures;
using Tetris.Menus;

namespace Tetris
{
    public class Program
    {
        #region PropertiesAndFields
        static object lockObject = new object();
        static Tetromino tetro;
        static Tetromino nextTetro;
        static bool game = true;
        private static float speed = .5f;
        private static int speedShreshold = 10;

        static int heightEnvironment = 20;
        public static int widthEnvironment = 6;        //mindestbreite = 6. 12 ist ein guter wert (10 spielbreite + 2 für die wände sind beim Originaltetris der Fall)
        static Vector2 offsetEnvironment = new Vector2(20, 3);
        static Vector2 offSetTetro = new Vector2(0, 5);
        internal static TetrisBoard tetrisBoard;

        static Timer timerTetroMoveDown, timerCheckInput;
        static bool isCollide = false;

        static int smallestNumb, biggestNumb, xPos;
        static Random random = new Random();

        const ConsoleColor enviromentColor = ConsoleColor.Gray;

        //Eigenschaften fürs blinken, wenn Reihe(n) gelöscht wurden
        static int blinkCount = 3;
        static int blinkSpeed = 200;

        //Eigenschaften von der Tetro Vorschau
        internal static Vector2 PrevievPos { get; set; } = new Vector2(offsetEnvironment.x + widthEnvironment + 5, offsetEnvironment.y + offSetTetro.y);
        internal static Vector2 PreviewFrameSize { get; set; } = new(7, 8);
        internal static ConsoleColor PreviewFrameColor { get; set; } = ConsoleColor.DarkYellow;


        public static int Score { get; set; }
        public static int DeletedRowsTotal { get; set; }

        private static Vector2 offsetPoints = new Vector2(0, 0);

        // braucht man nur für die alternative input variante
        //[DllImport("user32.dll")]
        //public static extern short GetKeyState(ConsoleKey vKey);
        //
        #endregion
        static void Main()
        {
            Init();
            
            while (true)
            {
                if (!game)                                                      // Wenn das Spiel vorbei ist
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.R)              // Starte das Spiel neu, ohne das Hauptmenü zu laden
                    {
                        game = true;
                        Console.Clear();
                        Reset();
                        Init(false);
                        continue;
                    }
                    else if (Console.ReadKey(true).Key == ConsoleKey.Escape)    // Beende das Spiel
                    {
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// Initialsiert die Startwerte, rendert den Hintergrund und wählt ein zufälliges start Tetro und ein zufälliges 
        /// Tetro aus, welches als nächstes erscheinen wird.
        /// </summary>
        private static void Init(bool showStartScreen = true)
        {
            Console.BufferHeight = 70;
            Console.CursorVisible = false;
            tetrisBoard = new(heightEnvironment, widthEnvironment, enviromentColor);
            if (showStartScreen)
                MainMenu.ShowStartScreen();
            InitAndRenderEnvironment();
            RenderInfos();

            tetro = RandomTetro();
            nextTetro = RandomTetro();
            // Todo: nächstes Tetro als Vorschau darstellen
            nextTetro.RenderPreview();

            RandomPosition();   // Verschiebt das neue generierte Tetro zufällig in der X-Achse. Dabei darf das Tetro nicht weiter rechts spawnen als die breite zulässt

            if (speed != 0)
                timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, (int)(500 / speed));
            else
                timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, 0);

            timerCheckInput = new Timer(_ => OnTimerCheckInputElapsed(), null, 0, 10);
        }
        /// <summary>
        /// Resettet alle Werte
        /// </summary>
        private static void Reset()
        {
            speed = .5f;
            Score = 0;
            DeletedRowsTotal = 0;
        }

        static void RandomPosition()
        {
            smallestNumb = 0;
            biggestNumb = widthEnvironment - tetro.width - 1;
            xPos = 1;
            if (biggestNumb >= smallestNumb)
                xPos = random.Next(smallestNumb, biggestNumb);
            else
            {
                Console.WriteLine("Spielbreite ist zu klein gewählt");
                return;
            }

            tetro.Move(new Vector2(xPos, 0));
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
                    isCollide = SpawnNewTetro(isCollide);
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
        static bool isPaused = false;
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
                    case ConsoleKey.Spacebar:
                        TogglePause();
                        break;
                    default:
                        break;
                }
            }

            void Move(int x, int y)
            {
                if (isPaused) { return; }       //Wenn das Spiel pausiert ist, dann führe die Bewegungn nicht aus

                DeleteTetro();
                isCollide = UpdateGame(new Vector2(x, y));

                if (isCollide)
                {
                    isCollide = SpawnNewTetro(isCollide);
                }

                RenderElement();
            }

            void Turn(int dir)
            {
                if (isPaused) { return; }       //Wenn das Spiel pausiert ist, dann führe die Bewegungn nicht aus

                DeleteTetro();
                tetro.Turn(dir);
                RenderElement();
            }
            // wenn die space Taste gedrückt wird, wird pausiert und wenn diese erneut gedrückt wird, wird das Spiel fortgesetzt.
            static void TogglePause()
            {
                isPaused = !isPaused;
                Vector2 pos = new Vector2(0, 4);

                if (isPaused)
                {
                    // Wenn pausiert, Timer deaktivieren
                    InitAndRenderEnvironment(ConsoleColor.Cyan);
                    Console.SetCursorPosition(pos.x, pos.y);
                    Console.WriteLine("Pause");

                    timerTetroMoveDown.Change(Timeout.Infinite, Timeout.Infinite);
                }
                else
                {
                    // Wenn fortgesetzt wird, Timer wieder starten
                    InitAndRenderEnvironment();
                    Console.SetCursorPosition(pos.x, pos.y);
                    Console.WriteLine("          ");
                    timerTetroMoveDown.Change(0, (int)(500 / speed));
                }
            }

        }

        static bool SpawnNewTetro(bool isCollide)
        {
            isCollide = false;
            tetro = nextTetro;
            nextTetro = RandomTetro();
            nextTetro.RenderPreview();
            RandomPosition();

            return isCollide;
        }

        static Tetromino RandomTetro()
        {
            //int randomTetro = new Random().Next(0, 7);
            int randomTetro = new Random().Next(0, 1);
            Tetromino t;

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
            return t;
        }

        static bool UpdateGame(Vector2 move)
        {
            Vector2 targetPos1 = Vector2.AddVector(tetro.endPos1, move);
            Vector2 targetPos2 = Vector2.AddVector(tetro.endPos2, move);
            Vector2 targetPos3 = Vector2.AddVector(tetro.endPos3, move);
            Vector2 targetPos4 = Vector2.AddVector(tetro.endPos4, move);

           
            if (tetrisBoard.Grid[targetPos1.y][targetPos1.x].isCollided == false &&
                 tetrisBoard.Grid[targetPos2.y][targetPos2.x].isCollided == false &&
                 tetrisBoard.Grid[targetPos3.y][targetPos3.x].isCollided == false &&
                 tetrisBoard.Grid[targetPos4.y][targetPos4.x].isCollided == false)
            {
                tetro.Move(move);
                return false;
            }
            else if (move.y == 1)
            {
                // Wenn das Tetro mit dem Boden kollidiert ist und die Position vom ersten Element vom Tetro über das Spielfeld befindet, dann soll das Spiel beendet werden.
                if (tetro.endPos1.y < offSetTetro.y)
                {
                    game = false;
                    timerTetroMoveDown.Dispose();
                    timerCheckInput.Dispose();
                    GameOver();
                    return false;
                }//Wenn das Tetro mit dem Boden kollidiert ist und nicht über das Spielfeld hinaus ragt
                else
                {
                    SaveNewCollider();                      //Speicher die Pos vom neuen Tetro ab
                    int deletedRows = CheckAndClearLines();    //Überprüft, ob eine Linie vollständig ist und löscht diese dann, wenn dies der Fall ist.
                                                            //Gibt die Anzahl der gelöschten Zeilen zurück, welche benötigt wird, um die Punkte zu berechnen
                    Score += CalculatePoints(deletedRows);
                    IncreaseSpeed(deletedRows);
                    timerTetroMoveDown.Dispose();
                    timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, (int)(500 / speed));
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
                tetrisBoard.Grid[tetro.endPos1.y][tetro.endPos1.x] = new Collider(true, tetro.tetroColor);
                tetrisBoard.Grid[tetro.endPos2.y][tetro.endPos2.x] = new Collider(true, tetro.tetroColor);
                tetrisBoard.Grid[tetro.endPos3.y][tetro.endPos3.x] = new Collider(true, tetro.tetroColor);
                tetrisBoard.Grid[tetro.endPos4.y][tetro.endPos4.x] = new Collider(true, tetro.tetroColor);

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
                

                if (linesToClear.Count > 0 && linesToClear.Count < 4)
                {
                    Audio.Play("Sounds/DeleteRow.mp3");
                }
                else if (linesToClear.Count == 4)
                {
                    Audio.Play("Sounds/Delete4Rows.mp3");
                }


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

                TetrisBoard oldTetrisBoard = CopyFrom(tetrisBoard);           // eine Sicherung vom alten Board, damit ich noch auf die Farben, wie sie vorher waren, zugreifen kann, da einzelne Zeilen gelöscht werden und das tetrisBoard überschrieben wird. Wird für das aufblinken von den Zeilen, die gelöscht wurden benötigt, damit diese mit den ursprünglichen Farben aufblinken

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

                static TetrisBoard CopyFrom(TetrisBoard tetrisBoard)
                {
                    TetrisBoard temp = new(heightEnvironment, widthEnvironment, enviromentColor);

                    for (int r = 0; r < heightEnvironment; r++)
                    {
                        for (int c = 0; c < widthEnvironment; c++)
                        {
                            temp.Grid[r][c] = tetrisBoard.Grid[r][c];
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

            static int CalculatePoints(int deletedRowAmount)
            {
                int pointFactor = 0;
                switch (deletedRowAmount)
                {
                    case 1:
                        pointFactor = 40;
                        break;
                    case 2:
                        pointFactor = 100;
                        break;
                    case 3:
                        pointFactor = 300;
                        break;
                    case 4:
                        pointFactor = 1200;
                        break;
                    default:
                        break;
                }
                return pointFactor * (int)(speed * 2);
            }
            
            static void IncreaseSpeed(int deletedRows)
            {
                DeletedRowsTotal += deletedRows;

                if (DeletedRowsTotal >= speedShreshold)
                {
                    DeletedRowsTotal = 0;
                    speed += .5f;
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

                RenderInfos();
            }
        }
        /// <summary>
        /// Zeigt alle Infos an wie Punkte, Level etc. 
        /// </summary>
        static void RenderInfos()
        {
            ClearOldInfos();
            RenderNewInfos();

            static void ClearOldInfos()
            {
                Console.SetCursorPosition(offsetPoints.x, offsetPoints.y);
                Console.WriteLine("                                                                                       ");
                Console.WriteLine("                                                                                       ");
                Console.WriteLine("                                                                                       ");
            }

            static void RenderNewInfos()
            {
                Console.SetCursorPosition(offsetPoints.x, offsetPoints.y);
                Console.WriteLine($"Points: {Score}");
                Console.WriteLine($"Level: {speed * 2}");
                Console.WriteLine($"Rows left until level up: {speedShreshold - DeletedRowsTotal}");
            }
        }

        static void RenderElement()
        {
            if (tetro.isDead)
            {
                return;
            }

            Console.ForegroundColor = tetro.tetroColor;

            Console.SetCursorPosition(tetro.endPos1.x + offsetEnvironment.x, tetro.endPos1.y + offsetEnvironment.y);
            Console.Write("#");
            Console.SetCursorPosition(tetro.endPos2.x + offsetEnvironment.x, tetro.endPos2.y + offsetEnvironment.y);
            Console.Write("#");
            Console.SetCursorPosition(tetro.endPos3.x + offsetEnvironment.x, tetro.endPos3.y + offsetEnvironment.y);
            Console.Write("#");
            Console.SetCursorPosition(tetro.endPos4.x + offsetEnvironment.x, tetro.endPos4.y + offsetEnvironment.y);
            Console.Write("#");
        }

        static void DeleteTetro()
        {
            if (tetro.endPos1 != null)
            {
                Console.SetCursorPosition(tetro.endPos1.x + offsetEnvironment.x, tetro.endPos1.y + offsetEnvironment.y);
                Console.Write(" ");
            }
            if (tetro.endPos2 != null)
            {
                Console.SetCursorPosition(tetro.endPos2.x + offsetEnvironment.x, tetro.endPos2.y + offsetEnvironment.y);
                Console.Write(" ");
            }
            if (tetro.endPos3 != null)
            {
                Console.SetCursorPosition(tetro.endPos3.x + offsetEnvironment.x, tetro.endPos3.y + offsetEnvironment.y);
                Console.Write(" ");
            }
            if (tetro.endPos4 != null)
            {
                Console.SetCursorPosition(tetro.endPos4.x + offsetEnvironment.x, tetro.endPos4.y + offsetEnvironment.y);
                Console.Write(" ");
            }
        }
        /// <summary>
        ///  Speichert die Umgebung und rendert diese
        /// </summary>
        static void InitAndRenderEnvironment(ConsoleColor color = enviromentColor)
        {
            Console.ForegroundColor = color;
            for (int i = 0; i < heightEnvironment; i++)
            {
                Console.SetCursorPosition(offsetEnvironment.x, i + offsetEnvironment.y);
                //collider[0, i] = new Collider(true, ConsoleColor.White);
                tetrisBoard.Grid[i][0] = new Collider(true, color);
                if (i >= offSetTetro.y)
                    Console.Write("|");

                Console.SetCursorPosition(widthEnvironment - 1 + offsetEnvironment.x, i + offsetEnvironment.y);
                //collider[widthEnvironment - 1, i] = new Collider(true, ConsoleColor.White);
                tetrisBoard.Grid[i][widthEnvironment - 1] = new Collider(true, color);
                if (i >= offSetTetro.y)
                    Console.Write("|");
            }

            for (int i = 0; i < widthEnvironment; i++)
            {
                Console.SetCursorPosition(i + offsetEnvironment.x, heightEnvironment - 1 + offsetEnvironment.y);
                //collider[i, heightEnvironment - 1] = new Collider(true, ConsoleColor.White);
                tetrisBoard.Grid[heightEnvironment - 1][i] = new Collider(true, color);
                Console.Write("-");
            }
            Console.SetCursorPosition(0, 0);
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

        static void GameOver()
        {
            //Console.SetCursorPosition(offsetEnvironment.x, 0);
            //RenderElement();
            //Console.ForegroundColor = ConsoleColor.DarkRed;
            //Console.Write("GAME OVER");

            //int space = 5;
            //for (int i = 0; i < space; i++)
            //{
            //    Console.WriteLine("");
            //}

            GameOverMenu.ShowGameOverScreen();
        }
    }
}



