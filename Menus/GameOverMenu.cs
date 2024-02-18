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
        private static Vector2 offset = new(10, 3);
        private static int highscore = 0;
        static string highscoreFilePath = "highscore.txt";
        static Timer timerHandleInput;


        static internal void InitGameOverMenu()
        {
            Program.gamestate = GameState.GameOverMenu;
            timerHandleInput = new Timer(_ => OnTimerHandleInputElapsed(), null, 0, 20);
            Settings.music.Play("Music/Music GameOver.mp3",Settings.currentMusicVolume, true);

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
            Console.WriteLine("To go to the settings menu, press 'S'.");

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

        static void OnTimerHandleInputElapsed()
        {
            lock (Program.lockObject)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                    if (keyPressed.Key == ConsoleKey.R)              // Starte das Spiel neu, ohne das Hauptmenü zu laden
                    {
                        Settings.music.Stop();
                        Program.game = true;
                        Console.Clear();
                        //Program.gameState = GameState.Playing;
                        //Program.isInit = false;
                        Program.InitGame();
                        timerHandleInput?.Dispose();
                    }
                    else if (keyPressed.Key == ConsoleKey.S)
                    {
                        GoToSettingsMenu();
                    }
                    else if (keyPressed.Key == ConsoleKey.Escape)    // Beende das Spiel
                    {
                        Program.game = false;
                    }
                }
            }
            static void GoToSettingsMenu()
            {
                Console.Clear();
                timerHandleInput?.Dispose();

                Settings.InitSettingsMenu();
            }
        }
    }
}
