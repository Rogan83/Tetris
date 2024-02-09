using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Menus
{
    internal class GameOverMenu
    {
        private static Vector2 offset = new(10, 3);

        static internal void ShowGameOverScreen()
        {
            #region Alternate GameOverScreen
            //Console.SetCursorPosition(offset.x, offset.y);

            //Console.WriteLine("  ____    ___      __  __  ______       ___    \
            //Console.WriteLine(" / ___|  / _ \\   |  \\/ ||  ____|     / _ \\
            //Console.WriteLine("| |  _  / /_\\ \\ | |\\/ || |___      | | | |
            //Console.WriteLine("| | | | |  _  | | | |  | ||  ___|     | | | |
            //Console.WriteLine("| |_| | | | | | | | |  |_|| |____     | |_| |
            //Console.WriteLine("\\____| \\_| |_/  |_|  |_||______|     \\___/


            //Console.WriteLine("  ___  ");
            //Console.WriteLine(" / _ \\ ");
            //Console.WriteLine("/ /_\\ \\");
            //Console.WriteLine("|  _  |");
            //Console.WriteLine("| | | |");
            //Console.WriteLine("\\_| |_/");

            //Console.WriteLine(" __  __ ");
            //Console.WriteLine("|  \\/  |");
            //Console.WriteLine("| |\\/| |");
            //Console.WriteLine("| |  | |");
            //Console.WriteLine("|_|  |_|");

            //Console.WriteLine(" ______ ");
            //Console.WriteLine("|  ____|");
            //Console.WriteLine("| |___  ");
            //Console.WriteLine("|  ___| ");
            //Console.WriteLine("| |____ ");
            //Console.WriteLine("|______|");

            //Console.WriteLine("  ___  ");
            //Console.WriteLine(" / _ \\ ");
            //Console.WriteLine("| | | |");
            //Console.WriteLine("| | | |");
            //Console.WriteLine(" \\___/ ");
            #endregion

            Console.ForegroundColor = ConsoleColor.DarkRed;
            

            Console.WriteLine();
            Console.WriteLine("███▀▀▀██ ███▀▀▀███ ███▀█▄█▀███ ██▀▀▀");
            Console.WriteLine("██    ██ ██     ██ ██   █   ██ ██   ");
            Console.WriteLine("██   ▄▄▄ ██▄▄▄▄▄██ ██   ▀   ██ ██▀▀▀");
            Console.WriteLine("██    ██ ██     ██ ██       ██ ██   ");
            Console.WriteLine("███▄▄▄██ ██     ██ ██       ██ ██▄▄▄");
            Console.WriteLine("                                    "); 
            Console.WriteLine("███▀▀▀███ ▀███  ██▀ ██▀▀▀ ██▀▀▀▀██▄ ");
            Console.WriteLine("██     ██   ██  ██  ██    ██     ██ ");
            Console.WriteLine("██     ██   ██  ██  ██▀▀▀ ██▄▄▄▄▄▀▀ ");
            Console.WriteLine("██     ██   ██  █▀  ██    ██     ██ ");
            Console.WriteLine("███▄▄▄███   ─▀█▀    ██▄▄▄ ██     ██▄");

            Console.WriteLine("");
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Press 'R' for Restart or Press 'ESC' to left the game.");
        }
    }
}
