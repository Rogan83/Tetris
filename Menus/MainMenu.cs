using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Menus
{
    static internal class MainMenu
    {
        private static Vector2 offset = new(10, 3);
        private static int pulseSpeed = 1;
        static Timer pulse;
        static object lockObject = new object();
        static bool toggleColor = false;
        internal static void ShowStartScreen()
        {
            pulse = new Timer(_ => OnTimerPulseElapsed(), null, 0, 1000 / pulseSpeed);

            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            Console.Clear();
            pulse.Dispose();
        }

        static void OnTimerPulseElapsed()
        {
            lock (lockObject)
            {
                if (toggleColor)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                toggleColor = !toggleColor;

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
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine("Made by Daniel Rothweiler");
            }
        }
    }
}
