using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
using System.Data.Common;
using Tetris.Figures;
using Tetris.Menus;
using NAudio.Wave;

namespace Tetris
{
    // Bug: Wenn ein Element über die höchstmögliche grenze hinaus geht, dann wird nicht vorher überprüft, ob zeilen gelöscht werden könnten
    public class Program
    {
        #region fields
        const ConsoleColor enviromentColor = ConsoleColor.Gray;
        static Tetromino? tetro;
        static Tetromino? nextTetro;
        static Timer? timerTetroMoveDown, timerCheckInput;

        static Random random = new Random();

        static Vector2 offsetEnvironment = new Vector2(20, 5);
        static Vector2 offSetTetro = new Vector2(0, 5);
        static Vector2 offsetScore = new Vector2(0, 0);

        static object lockObject = new object();

        static float speed = 1f;
        static float originSpeed = 1f;
        static int level = 1;
        static int speedThreshold = 10;          //10 ist der Standardwert
        static int smallestNumb, biggestNumb, xPos;

        static bool isCollide = false;
        static bool isGameOver = false;

        //Variablen fürs blinken, wenn Reihe(n) gelöscht wurden
        static int blinkCount = 3;
        static int blinkSpeed = 200;
        #endregion

        #region Properties
        internal static TetrisBoard? tetrisBoard;
        internal static PreviousGameState previousGamestate { get; set; }          // Dient dazu, um zu schauen, in welchen State (Spiel, Menü) man sich momentan befindet
        internal static ConsoleColor PreviewFrameColor { get; set; } = ConsoleColor.DarkYellow;

        internal static string PathMusicA { get; set; } = "Music/Music Tetris Type A.mp3";
        internal static string PathMusicB { get; set; } = "Music/Music Tetris Type B.mp3";
        internal static string PathMusicC { get; set; } = "Music/Music Tetris Type C.mp3";

        internal static bool game { get; set; } = true;

        internal static int HeightEnvironment { get; private set; } = 4 + offSetTetro.y + 1;              //standard Höhe 18

        internal static int WidthEnvironment { get; private set; } = 6;         //mindestbreite = 6. 12 ist ein guter wert (10 spielbreite + 2 für die wände sind beim Originaltetris der Fall)

        internal static int Score { get; set; }
        internal static int DeletedRowsTotal { get; set; }

        internal static Vector2 PreviewPos { get; set; } = new Vector2(offsetEnvironment.x + WidthEnvironment + 5, offsetEnvironment.y + offSetTetro.y);
        internal static Vector2 PreviewFrameSize { get; set; } = new(7, 8);
        #endregion


        static void Main()
        {
            Settings.soundtrack = new();
            Settings.soundtrack.Volume = Properties.Settings.Default.VolumeSoundtrack;

            Settings.music = new();
            Settings.music.Volume = Properties.Settings.Default.VolumeMusic;

            if (Properties.Settings.Default.MusicPath != String.Empty)
            {
                Settings.music.Play(Properties.Settings.Default.MusicPath, true);
            }

            MainMenu.InitMainMenu();

            while (game) { }
        }
        static void GoToGameOverMenu()
        {
            timerTetroMoveDown?.Dispose();
            timerCheckInput?.Dispose();

            Settings.music.Stop();
            Settings.soundtrack.Play("Sounds/Dead.mp3");
            Thread.Sleep(1000);
            Settings.soundtrack.Play("Sounds/GameOver.mp3");
            Thread.Sleep(2000);
            GameOverMenu.InitGameOverMenu();
        }
        /// <summary>
        /// Initialsiert die Startwerte, rendert den Hintergrund und wählt ein zufälliges start Tetro und ein zufälliges 
        /// Tetro aus, welches als nächstes erscheinen wird.
        /// </summary>
        internal static void InitGame()
        {
            Reset();

            if (Settings.music.waveOut.PlaybackState == PlaybackState.Stopped)
            {
                if (Properties.Settings.Default.MusicPath != String.Empty)
                    Settings.music.Play(Properties.Settings.Default.MusicPath, true);
                else
                    Settings.music.Stop();
            }

            Console.BufferHeight = 70;
            Console.CursorVisible = false;
            tetrisBoard = new(HeightEnvironment, WidthEnvironment, enviromentColor);
            
            InitAndRenderEnvironment();
            RenderInfos();

            tetro = RandomTetro();
            nextTetro = RandomTetro();
            // nächstes Tetro als Vorschau darstellen
            nextTetro.RenderPreview();

            RandomPosition();   // Verschiebt das neue generierte Tetro zufällig in der X-Achse. Dabei darf das Tetro nicht weiter rechts spawnen als die breite zulässt

            if (speed != 0)
                timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, (int)(1000 / speed));
            else
                timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, 0);

            timerCheckInput = new Timer(_ => OnTimerCheckInputElapsed(), null, 0, 10);
            
        }
        /// <summary>
        /// Resettet alle Werte
        /// </summary>
        internal static void Reset()
        {
            isGameOver = false;
            speed = originSpeed;
            level = 1;
            Score = 0;
            DeletedRowsTotal = 0;
        }

        static void RandomPosition()
        {
            smallestNumb = 0;
            if (tetro != null)
                biggestNumb = WidthEnvironment - tetro.Width - 1;
            xPos = 1;
            if (biggestNumb >= smallestNumb)
                xPos = random.Next(smallestNumb, biggestNumb);
            else
            {
                Console.WriteLine("Spielbreite ist zu klein gewählt");
                return;
            }
            if (tetro != null)
                tetro.MoveTetro(new Vector2(xPos, 0));
        }

        static void Move(int x, int y)
        {
            if (isPaused) { return; }       //Wenn das Spiel pausiert ist, dann führe die Bewegung nicht aus

            DeleteTetroGrafic();
            isCollide = UpdateGame(new Vector2(x, y));

            // Wenn das Tetro mit dem Boden kollidiert ist, dann Spawne ein neues zufälliges Tetro Element
            if (isCollide && !isGameOver)
            {
                isCollide = SpawnNewRandomTetro();
            }

            RenderElement();
        }

        static void OnTimerTetroMoveDownElapsed()
        {
            lock (lockObject)
            {
                if (!isGameOver)
                    Move(0, 1);
                else
                    GoToGameOverMenu();
            }
        }

        static void OnTimerCheckInputElapsed()
        {
            lock (lockObject)
            {
                if (!isGameOver)
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
        static bool isPaused;
        //static bool keyPressed = false;
        
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

            void Turn(int dir)
            {
                if (isPaused) { return; }       //Wenn das Spiel pausiert ist, dann führe die Bewegungn nicht aus

                DeleteTetroGrafic();
                if (tetro != null)
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
                }
                else
                {
                    // Wenn fortgesetzt wird, Timer wieder starten
                    InitAndRenderEnvironment();
                    Console.SetCursorPosition(pos.x, pos.y);
                    Console.WriteLine("          ");
                }
            }
        }

        static bool SpawnNewRandomTetro()
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
            int randomTetro = new Random().Next(0, 7);  //7 vers. Tetros gibt es
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

        /// <summary>
        /// Überprüft, ob das Feld, wo sich das Tetro hinbewegt, frei ist. Wenn nicht, dann kollidiert es. Wenn es mit dem Boden kollidiert, dann wird
        /// in Abhängigkeit, ob das Tetro über das Tetrisfeld hinaus ragt, das Spiel beendet oder die Position von diesen Tetro als Hindernis gespeichert. 
        /// Zusätzlich wird noch überprüft, ob eine oder mehrere Reihen komplett sind und diese werden, wenn dies der Fall ist, gelöscht.
        /// Gibt true zurück, wenn es mit dem Boden kollidiert ist.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        static bool UpdateGame(Vector2 dir)
        {
            if (tetro == null || tetrisBoard == null) return false;

            Vector2 targetPos1 = Vector2.AddVector(tetro.CurrentPos1, dir);
            Vector2 targetPos2 = Vector2.AddVector(tetro.CurrentPos2, dir);
            Vector2 targetPos3 = Vector2.AddVector(tetro.CurrentPos3, dir);
            Vector2 targetPos4 = Vector2.AddVector(tetro.CurrentPos4, dir);
           
            if (tetrisBoard.Grid[targetPos1.y][targetPos1.x].isPositionOccupied == false &&
                tetrisBoard.Grid[targetPos2.y][targetPos2.x].isPositionOccupied == false &&
                tetrisBoard.Grid[targetPos3.y][targetPos3.x].isPositionOccupied == false &&
                tetrisBoard.Grid[targetPos4.y][targetPos4.x].isPositionOccupied == false)
            {
                tetro.MoveTetro(dir);
                return false;
            }
            else if (dir.y == 1)
            {
                // Wenn das Tetro mit dem Boden kollidiert ist und die Position vom ersten Element vom Tetro über das Spielfeld befindet, dann soll
                // erstmal überprüft werden, wie viele Zeilen gelöscht werden können. Wenn nach Abzug der Linien immer noch der Rest über den 
                // Spielrand hinaus ragt, dann soll das Spiel beendet werden.


                /// Überprüfe, welches Element vom Tetro sich am weitesten oben befindet. 
                /// Dieses hat die kleinste zahl in der y-Koordinate, weil der Ursprung von der Y-Achse oben anfängt
                List<Vector2> positions = new List<Vector2>();
                positions.Add(tetro.CurrentPos1);
                positions.Add(tetro.CurrentPos2);
                positions.Add(tetro.CurrentPos3);
                positions.Add(tetro.CurrentPos4);

                Vector2 highestElement = new(0, 0);
                double lowestYCoordinate = double.PositiveInfinity;
                foreach (var pos in positions)
                {
                    if (pos.y < lowestYCoordinate)
                    {
                        lowestYCoordinate = pos.y;
                        highestElement = pos;
                    }
                }
                ///

                float tetroSpawnPosY = offsetEnvironment.y - offSetTetro.y;
                float tetroSpawnPointGroundPosY = tetroSpawnPosY + tetro.Width;

                SaveNewCollider();                      //Speicher die Pos vom neuen Tetro ab

                int completeLines = CountCompleteLines(false);

                // Das Spielfeld ist um den Betrag größer, der die größe von der verschiebung vom gespawnten Tetro nach oben entspricht, aber diesen sieht man nicht.
                // Dieser Bereich dient nur dazu, dass dort Tetros spawnen können und diese sich über den Spielfeld nicht
                // in der X-Achse außerhalb vom Spielfeld positionieren können, weil sie dort auch auf Kollisionen überprüft werden.
                // Somit können sie sich nicht zu weit nach links oder rechts bewegen lassen.
                int upperEdgePlayingField = offsetEnvironment.y + offSetTetro.y;

                // Bestimmt, welche Reihe sich am weitesten oben befindet, wenn die reihen, die komplett sind, abgezogen werden
                int upperEdgeHighestRow = highestElement.y + completeLines + offsetEnvironment.y;

                if (upperEdgeHighestRow < upperEdgePlayingField)         
                {
                    isGameOver = true;
                    return true;
                }
                //Wenn das Tetro mit dem Boden kollidiert ist und nicht über das Spielfeld hinaus ragt
                else
                {
                    Settings.soundtrack.Play("Sounds/Drop.mp3");
                    
                    int deletedRows = CountCompleteLines();    //Überprüft, ob eine Linie vollständig ist und löscht diese dann standardmäßig, wenn dies der Fall ist. Wenn False übergeben wird, dann löscht es sie nicht
                                                            //Gibt die Anzahl der gelöschten Zeilen zurück, welche benötigt wird, um die Punkte zu berechnen
                    Score += CalculateScore(deletedRows);
                    speed = IncreaseSpeed(deletedRows, speed);
                    //Damit die neue geschwindigkeit gleich übernommen wird
                    timerTetroMoveDown?.Dispose();
                    timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, (int)(1000 / speed));
                    RenderAll();                            // Rendert alles neu

                    return true;
                }
            }
            else
            {
                return false;
            }

            // Speichert die Position vom neuen Hindernis

            #region local methods
            static void SaveNewCollider()
            {
                if (tetro == null || tetrisBoard == null) return;

                tetrisBoard.Grid[tetro.CurrentPos1.y][tetro.CurrentPos1.x] = new Collider(true, tetro.tetroColor);
                tetrisBoard.Grid[tetro.CurrentPos2.y][tetro.CurrentPos2.x] = new Collider(true, tetro.tetroColor);
                tetrisBoard.Grid[tetro.CurrentPos3.y][tetro.CurrentPos3.x] = new Collider(true, tetro.tetroColor);
                tetrisBoard.Grid[tetro.CurrentPos4.y][tetro.CurrentPos4.x] = new Collider(true, tetro.tetroColor);

                RenderElement();
            }
            //Überprüft, wie viele Linien vollständig sind und löscht diese anschließend optional
            static int CountCompleteLines(bool delete = true)
            {
                if (tetrisBoard == null) { return 0; }

                // Überprüfe, ob die Zeile komplett ist. 
                List<int> linesToClear = new List<int>();

                // Durchlaufe jede Zeile von unten nach oben außer die letzte, da dies der Boden ist
                for (int row = HeightEnvironment - 2; row >= 0; row--)
                {
                    // Überprüfe, ob alle Zellen in der Zeile gefüllt sind
                    if (tetrisBoard.Grid[row].All(cell => cell.isPositionOccupied == true))
                    {
                        // Füge die zu löschende Zeile zur Liste hinzu
                        linesToClear.Add(row);
                    }
                }

                if (!delete) return linesToClear.Count;         // Gebe nur die Anzahl der zu löschenden Zeilen zurück


                if (linesToClear.Count > 0 && linesToClear.Count < 4)
                {
                    Settings.soundtrack.Play("Sounds/DeleteRow.mp3");
                }
                else if (linesToClear.Count == 4)
                {
                    Settings.soundtrack.Play("Sounds/Delete4Rows.mp3");
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
                TetrisBoard newTetrisBoard = new(HeightEnvironment, WidthEnvironment, enviromentColor);
                InitNewTetrisBoard();

                TetrisBoard oldTetrisBoard = CopyFrom(tetrisBoard);           // eine Sicherung vom alten Board, damit ich noch auf die Farben, wie sie vorher waren, zugreifen kann, da einzelne Zeilen gelöscht werden und das tetrisBoard überschrieben wird. Wird für das aufblinken von den Zeilen, die gelöscht wurden benötigt, damit diese mit den ursprünglichen Farben aufblinken

                for (int row = 0; row < HeightEnvironment; row++)
                {
                    if (!linesToClear.Contains(row))
                    {
                        for (int column = 0; column < WidthEnvironment; column++)
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
                                for (int column = 1; column < WidthEnvironment - 1; column++)
                                {
                                    Console.SetCursorPosition(column + offsetEnvironment.x, rowToClear + offsetEnvironment.y);
                                    Console.Write(" ");
                                }
                            }

                            Thread.Sleep(blinkSpeed);

                            foreach (var rowToClear in linesToClear)
                            {
                                for (int column = 1; column < WidthEnvironment - 1; column++)
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

                void InitNewTetrisBoard()
                {
                    for (int row = 0; row < HeightEnvironment; row++)
                    {
                        for (int column = 0; column < WidthEnvironment; column++)
                        {
                            if (row == HeightEnvironment - 1)
                            {
                                newTetrisBoard.Grid[row][column] = new Collider(true, enviromentColor);
                            }
                            else if (column == 0 || column == WidthEnvironment - 1)
                            {
                                newTetrisBoard.Grid[row][column] = new Collider(true, enviromentColor);
                            }
                        }
                    }
                }

                static TetrisBoard CopyFrom(TetrisBoard tetrisBoard)
                {
                    TetrisBoard temp = new(HeightEnvironment, WidthEnvironment, enviromentColor);

                    for (int r = 0; r < HeightEnvironment; r++)
                    {
                        for (int c = 0; c < WidthEnvironment; c++)
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

            static int CalculateScore(int deletedRowAmount)
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
                return pointFactor * level;
            }
            
            static float IncreaseSpeed(int deletedRows, float speed)
            {
                DeletedRowsTotal += deletedRows;

                if (DeletedRowsTotal >= speedThreshold)
                {
                    DeletedRowsTotal = 0;
                    speed += .5f;
                    level++;
                }
                return speed;
            }

            static void RenderAll()
            {
                if (tetrisBoard == null) return;

                // Render die Umgebung inkl. Tetros (mit den richtigen Farben) erneut
                for (int y = 0; y < HeightEnvironment; y++)
                {
                    for (int x = 0; x < WidthEnvironment; x++)
                    {
                        //Lösche die Elemente an dieser Stelle
                        Console.SetCursorPosition(x + offsetEnvironment.x, y + offsetEnvironment.y);
                        Console.WriteLine(" ");

                        if (tetrisBoard.Grid[y][x].isPositionOccupied == true)
                        {
                            Console.SetCursorPosition(x + offsetEnvironment.x, y + offsetEnvironment.y);
                            Console.ForegroundColor = tetrisBoard.Grid[y][x].color;
                            if (y == HeightEnvironment - 1)
                                Console.WriteLine("-");
                            else if (y >= offSetTetro.y)
                            {
                                if (x == 0 || x == WidthEnvironment - 1)
                                    Console.WriteLine("|");
                                else
                                    Console.WriteLine("#");
                            }
                        }
                    }
                }

                RenderInfos();
            }
            #endregion
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
                Console.SetCursorPosition(offsetScore.x, offsetScore.y);
                Console.WriteLine("                                                                                       ");
                Console.WriteLine("                                                                                       ");
                Console.WriteLine("                                                                                       ");
            }

            static void RenderNewInfos()
            {
                Console.SetCursorPosition(offsetScore.x, offsetScore.y);
                Console.WriteLine($"Score: {Score}");
                Console.WriteLine($"Level: {level}");
                Console.WriteLine($"Rows left until level up: {speedThreshold - DeletedRowsTotal}");
            }
        }

        static void RenderElement()
        {
            if (tetro == null) return;

            Console.ForegroundColor = tetro.tetroColor;

            Console.SetCursorPosition(tetro.CurrentPos1.x + offsetEnvironment.x, tetro.CurrentPos1.y + offsetEnvironment.y);
            Console.Write("#");
            Console.SetCursorPosition(tetro.CurrentPos2.x + offsetEnvironment.x, tetro.CurrentPos2.y + offsetEnvironment.y);
            Console.Write("#");
            Console.SetCursorPosition(tetro.CurrentPos3.x + offsetEnvironment.x, tetro.CurrentPos3.y + offsetEnvironment.y);
            Console.Write("#");
            Console.SetCursorPosition(tetro.CurrentPos4.x + offsetEnvironment.x, tetro.CurrentPos4.y + offsetEnvironment.y);
            Console.Write("#");
        }
        /// <summary>
        /// Löscht das Tetro nur optisch
        /// </summary>
        static void DeleteTetroGrafic()
        {
            if (tetro == null) return;

            if (tetro.CurrentPos1 != null)
            {
                Console.SetCursorPosition(tetro.CurrentPos1.x + offsetEnvironment.x, tetro.CurrentPos1.y + offsetEnvironment.y);
                Console.Write(" ");
            }
            if (tetro.CurrentPos2 != null)
            {
                Console.SetCursorPosition(tetro.CurrentPos2.x + offsetEnvironment.x, tetro.CurrentPos2.y + offsetEnvironment.y);
                Console.Write(" ");
            }
            if (tetro.CurrentPos3 != null)
            {
                Console.SetCursorPosition(tetro.CurrentPos3.x + offsetEnvironment.x, tetro.CurrentPos3.y + offsetEnvironment.y);
                Console.Write(" ");
            }
            if (tetro.CurrentPos4 != null)
            {
                Console.SetCursorPosition(tetro.CurrentPos4.x + offsetEnvironment.x, tetro.CurrentPos4.y + offsetEnvironment.y);
                Console.Write(" ");
            }
        }
        /// <summary>
        ///  Speichert die Umgebung und rendert diese
        /// </summary>
        static void InitAndRenderEnvironment(ConsoleColor color = enviromentColor)
        {
            if (tetrisBoard == null) return;

            Console.ForegroundColor = color;
            for (int i = 0; i < HeightEnvironment; i++)
            {
                Console.SetCursorPosition(offsetEnvironment.x, i + offsetEnvironment.y);
                //collider[0, i] = new Collider(true, ConsoleColor.White);
                tetrisBoard.Grid[i][0] = new Collider(true, color);
                if (i >= offSetTetro.y)
                    Console.Write("|");

                Console.SetCursorPosition(WidthEnvironment - 1 + offsetEnvironment.x, i + offsetEnvironment.y);
                //collider[widthEnvironment - 1, i] = new Collider(true, ConsoleColor.White);
                tetrisBoard.Grid[i][WidthEnvironment - 1] = new Collider(true, color);
                if (i >= offSetTetro.y)
                    Console.Write("|");
            }

            for (int i = 0; i < WidthEnvironment; i++)
            {
                Console.SetCursorPosition(i + offsetEnvironment.x, HeightEnvironment - 1 + offsetEnvironment.y);
                //collider[i, heightEnvironment - 1] = new Collider(true, ConsoleColor.White);
                tetrisBoard.Grid[HeightEnvironment - 1][i] = new Collider(true, color);
                Console.Write("-");
            }
            Console.SetCursorPosition(0, 0);
        }
    }
}



