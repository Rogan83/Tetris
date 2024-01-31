using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    static object lockObject = new object();
    static Tetromino t;
    static bool game = true;
    static float speed = 2;
    static int heightEnvironment = 14;
    static int widthEnvironment = 7;        //mindestbreite = 6
    static Vector2 offsetEnvironment = new Vector2(20, 3);
    static Vector2 offSetTetro = new Vector2(0, 10);
    static Collider[,] collider = new Collider[widthEnvironment, heightEnvironment];
    static Timer timerTetroMoveDown, timerCheckInput;
    static bool isCollide = false;

    // braucht man nur für die alternative input variante
    //[DllImport("user32.dll")]
    //public static extern short GetKeyState(ConsoleKey vKey);
    //

    static void Main()
    {
        Console.BufferHeight = 70;
        Console.CursorVisible = false;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;

        InitCollider();

        RenderEnvironment();

        Random random = new Random();
        RandomTetro();
        // Verschiebt das neue generierte Tetro zufällig in der X-Achse. Dabei darf das Tetro nicht weiter rechts spawnen als die breite zulässt
        int smallestNumb = 1;
        int biggestNumb = widthEnvironment - t.width-1;
        int xPos = 1;
        if (biggestNumb >= smallestNumb)
            xPos = random.Next(1, widthEnvironment - t.width);
        else
        {
            Console.WriteLine("Spielbreite ist zu klein gewählt");
            return;
        }

        t.Move(new Vector2(xPos, 0));

        if (speed != 0)
            timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, (int)(1000 / speed));
        else
            timerTetroMoveDown = new Timer(_ => OnTimerTetroMoveDownElapsed(), null, 0, 0);


        timerCheckInput = new Timer(_ => OnTimerCheckInputElapsed(), null, 0, 100);



        while (game)
        {
            //HandleInput();
        }

        if (!game)
        {
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
        

        static void InitCollider()
        {
            for (int x = 0; x < widthEnvironment; x++)
            {
                for (int y = 0; y < heightEnvironment; y++)
                {
                    collider[x, y] = new Collider(false, ConsoleColor.White);
                }
            }
        }
    }

    static void OnTimerTetroMoveDownElapsed()
    {
        lock (lockObject)
        {
            DeleteTetro();
            Vector2 moveDown = new Vector2(0, 1);
            isCollide = UpdateGame(moveDown);
            RenderElement();

            if (isCollide)
            {
                RandomTetro();
                int r = new Random().Next(1, widthEnvironment - t.width);
                t.Move(new Vector2(r, 0));
            }
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
            UpdateGame(new Vector2(x, y));
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
        int randomTetro = new Random().Next(0, 0);

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
        Vector2 targetPos1 = Vector2.AddVector(t.Pos1, move);
        Vector2 targetPos2 = Vector2.AddVector(t.Pos2, move);
        Vector2 targetPos3 = Vector2.AddVector(t.Pos3, move);
        Vector2 targetPos4 = Vector2.AddVector(t.Pos4, move);

        if (collider[targetPos1.x, targetPos1.y].isCollided == false && 
            collider[targetPos2.x, targetPos2.y].isCollided == false &&
            collider[targetPos3.x, targetPos3.y].isCollided == false &&
            collider[targetPos4.x, targetPos4.y].isCollided == false)
        {
            t.Move(move);
            return false;
        }
        else if (move.y == 1)
        {
            // Wenn das Tetro kollidiert ist und die Position vom ersten Element vom Tetro über das Spielfeld befindet, dann soll das Spiel beendet werden.
            if (t.Pos1.y < offSetTetro.y)
            {
                game = false;
                timerTetroMoveDown.Dispose(); 
                timerCheckInput.Dispose();
                GameOverText();
                return false;
            }
            else
            {
                SaveNewCollider();
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
            collider[t.Pos1.x, t.Pos1.y] = new Collider(true, t.tetroColor); 
            collider[t.Pos2.x, t.Pos2.y] = new Collider(true, t.tetroColor);
            collider[t.Pos3.x, t.Pos3.y] = new Collider(true, t.tetroColor);
            collider[t.Pos4.x, t.Pos4.y] = new Collider(true, t.tetroColor);

            RenderElement();
        }
    }
    
    static void RenderElement()
    {
        if (t.isDead)
        {
            return;
        }

        Console.ForegroundColor = t.tetroColor;

        Console.SetCursorPosition(t.Pos1.x + offsetEnvironment.x, t.Pos1.y + offsetEnvironment.y);
        Console.Write("#");
        Console.SetCursorPosition(t.Pos2.x + offsetEnvironment.x, t.Pos2.y + offsetEnvironment.y);
        Console.Write("#");
        Console.SetCursorPosition(t.Pos3.x + offsetEnvironment.x, t.Pos3.y + offsetEnvironment.y);
        Console.Write("#");
        Console.SetCursorPosition(t.Pos4.x + offsetEnvironment.x, t.Pos4.y + offsetEnvironment.y);
        Console.Write("#");
    }

    static void DeleteTetro()
    {
        Console.SetCursorPosition(t.Pos1.x + offsetEnvironment.x, t.Pos1.y + offsetEnvironment.y);
        Console.Write(" ");
        Console.SetCursorPosition(t.Pos2.x + offsetEnvironment.x, t.Pos2.y + offsetEnvironment.y);
        Console.Write(" ");
        Console.SetCursorPosition(t.Pos3.x + offsetEnvironment.x, t.Pos3.y + offsetEnvironment.y);
        Console.Write(" ");
        Console.SetCursorPosition(t.Pos4.x + offsetEnvironment.x, t.Pos4.y + offsetEnvironment.y);
        Console.Write(" ");
    }

    static void RenderEnvironment()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        for (int i = 0; i < heightEnvironment; i++)
        {
            Console.SetCursorPosition(offsetEnvironment.x, i + offsetEnvironment.y);
            collider[0, i] = new Collider(true, ConsoleColor.White);
            if (i >= offSetTetro.y)
                Console.Write("|");

            Console.SetCursorPosition(widthEnvironment - 1 + offsetEnvironment.x, i + offsetEnvironment.y);
            collider[widthEnvironment - 1, i] = new Collider(true, ConsoleColor.White);
            if (i >= offSetTetro.y)
                Console.Write("|");
        }

        for (int i = 0; i < widthEnvironment; i++)
        {
            Console.SetCursorPosition(i + offsetEnvironment.x, heightEnvironment - 1 + offsetEnvironment.y);
            collider[i, heightEnvironment - 1] = new Collider(true, ConsoleColor.White);
            Console.Write("-");
        }

        //for (int x = 0; x < widthEnvironment; x++)
        //{
        //    for (int y = 0; y < heightEnvironment; y++)
        //    {
        //        if (collider[x,y].isCollided == true)
        //        {
        //            Console.SetCursorPosition(x, y);
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
        public Vector2 Pos1;
        public Vector2 Pos2;
        public Vector2 Pos3;
        public Vector2 Pos4;

        public int width;
        protected int rotation;
        public bool isDead = false;
        public ConsoleColor tetroColor;

        public void Move(Vector2 offset)
        {
            Pos1 = Vector2.AddVector(offset, Pos1);
            Pos2 = Vector2.AddVector(offset, Pos2);
            Pos3 = Vector2.AddVector(offset, Pos3);
            Pos4 = Vector2.AddVector(offset, Pos4);
        }

        public void Move(int x, int y)
        {
            Vector2 offset = new Vector2(x, y);
            Pos1 = Vector2.AddVector(offset, Pos1);
            Pos2 = Vector2.AddVector(offset, Pos2);
            Pos3 = Vector2.AddVector(offset, Pos3);
            Pos4 = Vector2.AddVector(offset, Pos4);
        }

        public virtual void ResetPos() { }

        public virtual void Turn(int dir) { }
    }

    class I : Tetromino
    {
        
        public I()
        {
            StartPos();
            width = 4;
            rotation = 1;
            tetroColor = ConsoleColor.Blue;
        }

        private void StartPos()
        {
            //Pos1 = new Vector2(0, 0);
            //Pos2 = new Vector2(0, 1);
            //Pos3 = new Vector2(0, 2);
            //Pos4 = new Vector2(0, 3);

            Pos1 = new Vector2(0, 0);
            Pos2 = new Vector2(1, 0);
            Pos3 = new Vector2(2, 0);
            Pos4 = new Vector2(3, 0);
        }

        public override void ResetPos()
        {
            StartPos();
        }

        public override void Turn(int dir)
        {
            int oldRoation = rotation;  // Die vorherige Rotation
            rotation += dir;            // Die neue Rotation

            if (rotation <= 0)
                rotation = 4;
            else if (rotation > 4)
                rotation = 1;
           
            switch (rotation)
            {
                case 1:
                    if (oldRoation == 4)
                    {
                        Pos1 = Vector2.AddVector(Pos1, new Vector2(-1, 1));
                        Pos2 = Vector2.AddVector(Pos2, new Vector2(0, 0));
                        Pos3 = Vector2.AddVector(Pos3, new Vector2(1, -1));
                        Pos4 = Vector2.AddVector(Pos4, new Vector2(2, -2));
                    }
                    else
                    {
                        Pos1 = Vector2.AddVector(Pos1, new Vector2(-2, 1));
                        Pos2 = Vector2.AddVector(Pos2, new Vector2(-1, 0));
                        Pos3 = Vector2.AddVector(Pos3, new Vector2(0, -1));
                        Pos4 = Vector2.AddVector(Pos4, new Vector2(1, -2));
                    }
                    break;

                case 2:
                    if (oldRoation == 1)
                    {
                        Pos1 = Vector2.AddVector(Pos1, new Vector2(2, -1));
                        Pos2 = Vector2.AddVector(Pos2, new Vector2(1, 0));
                        Pos3 = Vector2.AddVector(Pos3, new Vector2(0, 1));
                        Pos4 = Vector2.AddVector(Pos4, new Vector2(-1, 2));
                    }
                    else
                    {
                        Pos1 = Vector2.AddVector(Pos1, new Vector2(2, -2));
                        Pos2 = Vector2.AddVector(Pos2, new Vector2(1, -1));
                        Pos3 = Vector2.AddVector(Pos3, new Vector2(0, 0));
                        Pos4 = Vector2.AddVector(Pos4, new Vector2(-1, 1));
                    }
                    break;
                case 3:
                    if (oldRoation == 2)
                    {
                        Pos1 = Vector2.AddVector(Pos1, new Vector2(-2, 2));
                        Pos2 = Vector2.AddVector(Pos2, new Vector2(-1, 1));
                        Pos3 = Vector2.AddVector(Pos3, new Vector2(0, 0));
                        Pos4 = Vector2.AddVector(Pos4, new Vector2(1, -1));
                    }
                    else
                    {
                        Pos1 = Vector2.AddVector(Pos1, new Vector2(-1, 2));
                        Pos2 = Vector2.AddVector(Pos2, new Vector2(0, 1));
                        Pos3 = Vector2.AddVector(Pos3, new Vector2(1, 0));
                        Pos4 = Vector2.AddVector(Pos4, new Vector2(2, -1));
                    }
                    break;
                case 4:
                    if (oldRoation == 3)
                    {
                        Pos1 = Vector2.AddVector(Pos1, new Vector2(1, -2));
                        Pos2 = Vector2.AddVector(Pos2, new Vector2(0, -1));
                        Pos3 = Vector2.AddVector(Pos3, new Vector2(-1, 0));
                        Pos4 = Vector2.AddVector(Pos4, new Vector2(-2, 1));
                    }
                    else
                    {
                        Pos1 = Vector2.AddVector(Pos1, new Vector2(1, -1));
                        Pos2 = Vector2.AddVector(Pos2, new Vector2(0, 0));
                        Pos3 = Vector2.AddVector(Pos3, new Vector2(-1, 1));
                        Pos4 = Vector2.AddVector(Pos4, new Vector2(-2, 2));
                    }
                    break;
                default:
                    break;
            }
        }
    }

    class J : Tetromino
    {
        public J()
        {
            StartPos();
            width = 2;
            tetroColor = ConsoleColor.DarkBlue;
        }

        private void StartPos()
        {
            Pos1 = new Vector2(1, 0);
            Pos2 = new Vector2(1, 1);
            Pos3 = new Vector2(1, 2);
            Pos4 = new Vector2(0, 2);
        }

        public override void ResetPos()
        {
            StartPos();
        }

        
    }

    class L : Tetromino
    {
        public L()
        {
            StartPos();
            width = 2;
            tetroColor = ConsoleColor.Red;
        }

        public override void ResetPos()
        {
            StartPos();
        }

        void StartPos()
        {
            Pos1 = new Vector2(0, 0);
            Pos2 = new Vector2(0, 1);
            Pos3 = new Vector2(0, 2);
            Pos4 = new Vector2(1, 2);
        }
    }

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
            Pos1 = new Vector2(0, 0);
            Pos2 = new Vector2(0, 1);
            Pos3 = new Vector2(1, 0);
            Pos4 = new Vector2(1, 1);
        }

        public override void ResetPos()
        {
            StartPos();
        }
    }

    class Z : Tetromino
    {
        public Z()
        {
            StartPos();
            width = 3;
            tetroColor = ConsoleColor.DarkRed;
        }

        private void StartPos()
        {
            Pos1 = new Vector2(0, 0);
            Pos2 = new Vector2(1, 0);
            Pos3 = new Vector2(1, 1);
            Pos4 = new Vector2(2, 1);
        }

        public override void ResetPos()
        {
            StartPos();
        }
    }

    class T : Tetromino
    {
        public T()
        {
            StartPos();
            width = 3;
            tetroColor = ConsoleColor.DarkMagenta;
        }

        private void StartPos()
        {
            Pos1 = new Vector2(0, 0);
            Pos2 = new Vector2(1, 0);
            Pos3 = new Vector2(2, 0);
            Pos4 = new Vector2(1, 1);
        }

        public override void ResetPos()
        {
            StartPos();
        }
    }

    class S : Tetromino
    {
        public S()
        {
            StartPos();
            width = 3;
            tetroColor = ConsoleColor.Green;
        }

        private void StartPos()
        {
            Pos1 = new Vector2(1, 0);
            Pos2 = new Vector2(2, 0);
            Pos3 = new Vector2(0, 1);
            Pos4 = new Vector2(1, 1);
        }

        public override void ResetPos()
        {
            StartPos();
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
            Vector2 newPos = new Vector2(0, 0);
            newPos.x = vec1.x + vec2.x;
            newPos.y = vec1.y + vec2.y;
            return newPos;
        }
    }

    /// <summary>
    /// Die Klasse zum Speichern von den Positionen der Kollider und dessen Farbe
    /// </summary>
    class Collider
    {
        public bool isCollided;
        public ConsoleColor color; 

        public Collider(bool isCollided, ConsoleColor color)
        {
            this.isCollided = isCollided; 
            this.color = color;
        }
    }
}
