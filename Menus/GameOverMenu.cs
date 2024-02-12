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
        private static int highscore = 0;
        static string highscoreFilePath = "highscore.txt";

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
            Program.music.Play("Music/Music GameOver.mp3");

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            

            Console.WriteLine("");
            Console.WriteLine("");
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

            CheckIfHighscore();
        }
        /// <summary>
        /// Überprüft, ob der Highscore geknackt wurde und überschreibt ihn, wenn dies der Fall ist.
        /// </summary>
        static internal void CheckIfHighscore()
        {
            highscore = LoadHighscore();
            int oldHighscore = highscore;

            if (Program.Score > highscore)
            {
                highscore = Program.Score;
                SaveHighscore(highscore);

                if (oldHighscore != 0)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("New Highscrore!: " + highscore);
                }
            }
        }

        internal static int LoadHighscore()
        {
            int highscore = 0;
            if (File.Exists(highscoreFilePath))
            {
                string[] lines = File.ReadAllLines(highscoreFilePath);
                if (lines.Length > 0)
                {
                    int.TryParse(lines[0], out highscore);
                }
            }
            return highscore;
        }

        static void SaveHighscore(int highscore)
        {
            File.WriteAllText(highscoreFilePath, highscore.ToString());
        }
    }
}
