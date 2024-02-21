using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Menus
{
    internal class GameOverMenu
    {
        #region Felder
        static bool isInGameOverMenu = true;
        static int highscore = 0;
        static string highscoreFilePath = "highscore.txt";
        #endregion

        #region Methoden
        static internal void InitGameOverMenu()
        {
            isInGameOverMenu = true;
            Program.previousGamestate = PreviousGameState.GameOverMenu;
            // Wenn der aktuelle Musik Pfad leer ist, bedeutet das, dass die Musik in den Settings deaktiviert wurden.
            // In diesen Fall soll auch hier keine Musik abgespielt werden
            if (Properties.Settings.Default.MusicPath != String.Empty)
                Settings.music.Play("Music/Music GameOver.mp3", true);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            CheckIfHighscore();

            RenderGameOverText();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Press 'R' for Restart or Press 'ESC' to left the game.");
            Console.WriteLine("To go to the settings menu, press 'S'.");

            while (isInGameOverMenu)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                    if (keyPressed.Key == ConsoleKey.R)              // Starte das Spiel neu, ohne das Hauptmenü zu laden
                    {
                        isInGameOverMenu = false;
                        Console.Clear();
                        Settings.music.Stop();
                        Program.game = true;
                        Program.InitGame();
                    }
                    else if (keyPressed.Key == ConsoleKey.S)
                    {
                        isInGameOverMenu = false;
                        GoToSettingsMenu();
                    }
                    else if (keyPressed.Key == ConsoleKey.Escape)    // Beende das Spiel
                    {
                        isInGameOverMenu = false;
                        Program.game = false;
                    }
                }
            }
            static void GoToSettingsMenu()
            {
                Console.Clear();
                Settings.InitSettingsMenu();
            }

            static void RenderGameOverText()
            {
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
            }
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
        #endregion
    }
}
