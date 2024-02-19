using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;   
using System.Media;
using NAudio.Wave;

namespace Tetris.Menus
{
    static internal class MainMenu
    {
        static bool isMainMenu = true;
        private static Vector2 offset = new(10, 3);
        private static int pulseSpeed = 1;
        static Random random = new Random();
      
        static Timer pulse, timerMusic, timerHandleInput;
        private static bool stopPlaying = false;
        static object lockObject = new object();

        //Color
        static ConsoleColor textColorMusic = ConsoleColor.DarkBlue;
        static ConsoleColor textColorMusic2 = ConsoleColor.DarkRed;

        internal static void InitMainMenu()
        {
            Program.gamestate = GameState.MainMenu;
            pulse = new Timer(_ => OnTimerPulseElapsed(), null, 0, 1000 / pulseSpeed);
            //timerMusic = new Timer(_ => OnTimerMusicElapsed(), null, 0, Timeout.Infinite);
            timerHandleInput = new Timer(_ => OnTimerHandleInputElapsed(), null, 0, 20);
        }
        /// <summary>
        /// wählt eine zufällige Farbe für das große Wort "Tetris" aus
        /// </summary>
        static void OnTimerPulseElapsed()
        {
            lock (lockObject)
            {
                int randomColorIndex = new Random().Next(0, 7);

                switch (randomColorIndex)
                {
                    case 0:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case 6:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                }

                Console.SetCursorPosition(offset.x, offset.y);
                Console.WriteLine(" _______   ______   _______   _____      _____    _____ ");
                Console.SetCursorPosition(offset.x, offset.y + 1);
                Console.WriteLine("|__   __| |  ____| |__   __| |  __ \\   |_   _|  / ____|");
                Console.SetCursorPosition(offset.x, offset.y + 2);
                Console.WriteLine("   | |    | |___      | |    | |__) |    | |   | (___  ");
                Console.SetCursorPosition(offset.x, offset.y + 3);
                Console.WriteLine("   | |    |  ___|     | |    |  _  /     | |    \\___ \\");
                Console.SetCursorPosition(offset.x, offset.y + 4);
                Console.WriteLine("   | |    | |____     | |    | | \\ \\    _| |_   ____) |");
                Console.SetCursorPosition(offset.x, offset.y + 5);
                Console.WriteLine("   |_|    |______|    |_|    |_|  \\_\\  |_____| |_____/ ");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(offset.x + 12, offset.y + 8);
                Console.WriteLine("Press Enter to start the game.");

                int highscore = GameOverMenu.LoadHighscore();

                if (highscore > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(offset.x + 12, offset.y + 10);
                    Console.WriteLine($"Your Current Highscore is {highscore}.");
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.WriteLine("Controls: ");
                Console.WriteLine("");
                Console.WriteLine("To Move, press  A, S, D  OR  the Arrow Keys");
                Console.WriteLine("To Rotate, press W  OR the Arrow up Key");
                Console.WriteLine("To Pause, press the spacebar. Press the spacebar again, to continue the game.");
                Console.WriteLine("To go to the settings menu, press 'S'.");
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine("Made by Daniel Rothweiler");
            }
        }

        static void OnTimerMusicElapsed()
        {
            while (!stopPlaying)
            {
                int randomMusicIndex = random.Next(0, 3);
                int speed = random.Next(0,10);
                int speedFactor = 1;

                if (speed > 0)
                    speed = 1 * speedFactor;
                else
                    speed = 2 * speedFactor;

                if (randomMusicIndex > 1)
                {
                    SongTetris(speed);
                    Thread.Sleep(2000);
                }
                else //Es wird selten mal der Song von Super Mario abgespielt (als Easter Egg)
                {
                    SongMario(speed);
                    Thread.Sleep(2000);
                }

                //SongBeverlyHillsCops();
                //Thread.Sleep(2000);
                //SongDeutscheNationalhymne();
                //Thread.Sleep(2000);

                static void SongTetris(double speed = 1)
                {
                    List<(int frequency, int duration)> songTetris = new List<(int frequency, int duration)>
                    {
                        (1320, 500), (990, 250), (1056, 250), (1188, 250), (1320, 125),
                        (1188, 125), (1056, 250), (990, 250), (880, 500), (880, 250),
                        (1056, 250), (1320, 500), (1188, 250), (1056, 250), (990, 750),
                        (1056, 250), (1188, 500), (1320, 500), (1056, 500), (880, 500),
                        (880, 500), /*(250),   count 21, */(1188, 500), (1408, 250), (1760, 500), (1584, 250),
                        (1408, 250), (1320, 750), (1056, 250), (1320, 500),(1188, 250),
                        (1056, 250), (990, 500), (990, 250), (1056, 250), (1188, 500),
                        (1320, 500), (1056, 500),(880, 500), (880, 500), /*(250), 39*/(1320, 500),
                        (990, 250), (1056, 250), (1188, 250), (1320, 125), (1188, 125),
                        (1056, 250), (990, 250), (880, 500), (880, 250), (1056, 250),
                        (1320, 500), (1188, 250), (1056, 250), (990, 750), (1056, 250),
                        (1188, 500), (1320, 500), (1056, 500), (880, 500), (880, 500), //(500),  tupel 60
                        (660, 1000), (528, 1000), (594, 1000), (495, 1000), (528, 1000), (440, 1000), (419, 1000), (495, 1000),
                        (660, 1000), (528, 1000), (594, 1000), (495, 1000), (528, 500), (660, 500), (880, 1000), (838, 2000),
                        (660, 1000), (528, 1000), (594, 1000), (495, 1000), (528, 1000), (440, 1000), (419, 1000), (495, 1000),
                        (660, 1000), (528, 1000), (594, 1000), (495, 1000), (528, 500), (660, 500), (880, 1000), (838, 2000)
                    };

                    for (int i = 0; i < songTetris.Count; i++)
                    {
                        songTetris[i] = (songTetris[i].frequency, (int)(songTetris[i].duration / speed));
                    }

                    int count = 0;
                    foreach (var tone in songTetris)
                    {
                        Beep(tone.frequency, tone.duration);
                        count++;

                        if (count == 21 || count == 39)
                        {
                            Thread.Sleep((int)(250 / speed));
                        }
                        if (count == 60)
                            Thread.Sleep((int)(500 / speed));

                        if (stopPlaying) return;
                    }
                }

                static void SongMario(double speed = 1)
                {
                    List<(int frequency, int duration)> songMario = new List<(int frequency, int duration)>
                    {
                        (480, 200), (1568, 200), (1568, 200), (1568, 200),
                        (740, 200), (784, 200), (784, 200), (784, 200),
                        (370, 200), (392, 200), (370, 200), (392, 200),
                        (392, 400), (196, 400),(740, 200), (784, 200),
                        (784, 200), (740, 200),(784, 200), (784, 200),
                        (734, 200), (84, 200),      //22
                        (880, 200), (831, 200), (880, 200), (988, 400),
                        (880, 200), (784, 200), (698, 200), (740, 200),
                        (784, 200), (784, 200), (734, 200), (784, 200),
                        (784, 200), (734, 200), (784, 200), (880, 200),
                        (831, 200), (880, 200), (988, 400),
                        (1108, 10), (1175, 200), (1480, 10), (1568, 200),
                        (740, 200), (784, 200), (784, 200), (740, 200),
                        (784, 200), (784, 200), (734, 200), (784, 200),
                        (880, 200), (831, 200), (880, 200), (988, 400),
                        (880, 200), (784, 200), (698, 200), (659, 200),
                        (698, 200), (784, 200), (880, 400), (784, 200),
                        (698, 200), (659, 200),
                        (587, 200), (659, 200), (698, 200), (784, 400),
                        (698, 200), (659, 200), (587, 200), (523, 200),
                        (587, 200), (659, 200), (698, 400), (659, 200),
                        (587, 200), (493, 200), (523, 200),
                        (349, 400), (392, 200), (330, 200), (523, 200),
                        (493, 200), (466, 200), (440, 200), (493, 200),
                        (523, 200), (880, 200), (493, 200), (880, 200),
                        (1760, 200), (440, 200),
                        (392, 200), (440, 200), (494, 200), (784, 200),
                        (440, 200), (784, 200), (1568, 200), (392, 200),
                        (349, 200), (392, 200), (440, 200), (698, 200),
                        (415, 200), (698, 200), (1396, 200), (349, 200),
                        (330, 200), (311, 200), (330, 200), (659, 200),
                        (698, 400), (784, 400),
                        (440, 200), (494, 200), (523, 200), (880, 200),
                        (494, 200), (880, 200), (1760, 200), (440, 200),
                        (392, 200), (440, 200), (494, 200), (784, 200),
                        (440, 200), (784, 200), (1568, 200), (392, 200),
                        (349, 200), (392, 200), (440, 200), (698, 200),
                        (659, 200), (698, 200), (740, 200), (784, 200),
                        (392, 200), (392, 200), (392, 200), (392, 200),
                        (196, 200), (196, 200), (196, 200),
                        (185, 200), (196, 200), (185, 200), (196, 200),
                        (208, 200), (220, 200), (233, 200), (247, 200)
                    };

                    for (int i = 0; i < songMario.Count; i++)
                    {
                        songMario[i] = (songMario[i].frequency, (int)(songMario[i].duration / speed));
                    }

                    int count = 0;
                    foreach (var tone in songMario)
                    {
                        if (stopPlaying) return;
                        Beep(tone.frequency, tone.duration);
                        count++;

                        if (count == 41 || count == 45)
                        {
                            Thread.Sleep((int)(200 / speed));
                        }
                        if (count == 82)
                            Thread.Sleep((int)(400 / speed));

                    }
                }
            }
        }

        //static bool toggleIsMusicOn = true;

        static Vector2 pos = new(0,0);
        static void OnTimerHandleInputElapsed()
        {
            lock (lockObject)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);

                    if (keyPressed.Key == ConsoleKey.Enter)
                    {
                        GoAndRestartPlayingState();
                    }
                    else if (keyPressed.Key == ConsoleKey.S)
                    {
                        GoToSettingsMenu();
                    }
                }
            }
            
            static void GoAndRestartPlayingState()
            {
                Console.Clear();

                pulse?.Dispose();

                isMainMenu = false;
                timerHandleInput?.Dispose();

                Program.InitGame();
            }

            static void GoToSettingsMenu()
            {
                Console.Clear();
                pulse?.Dispose();

                isMainMenu = false;
                timerHandleInput?.Dispose();

                Settings.InitSettingsMenu();
            }
        }
    }
}
