using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//todo:
// mit den Pfeiltasten das Lied auswählen können
// Wenn im Gameover Menü erneut spielen gewählt wird, muss das letzte gewählte Lied wieder ausgewählt werden. 
// Im Game Over Menü sollte man auch auf Settings zugreifen können
namespace Tetris.Menus
{
    static internal class Settings
    {
        static bool isInSettingMenu = true;
        static ConsoleColor selectedColor = ConsoleColor.Red;

        

        static Vector2 posMusicA =      new Vector2(0, 5);
        static Vector2 posMusicB =      new Vector2(0, 6);
        static Vector2 posMusicC =      new Vector2(0, 7);
        static Vector2 posNoneMusic =   new Vector2(0, 8);
        static internal void InitSettingsMenu()
        {
            isInSettingMenu = true;

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Settings");
            Console.WriteLine("");
            Console.WriteLine("Use the arrow keys to select your music.");
            Console.WriteLine("Press Escape to go back to the Main Menu.");

            RenderMusicSelectionText(posMusicA, "Music A", selectedColor);
            RenderMusicSelectionText(posMusicB, "Music B");
            RenderMusicSelectionText(posMusicC, "Music C");
            RenderMusicSelectionText(posNoneMusic, "None");

            

            while (isInSettingMenu)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);

                    if (keyPressed.Key == ConsoleKey.Escape)
                    {
                        GoToMainMenu();
                    }
                }
            }

            static void GoToMainMenu()
            {
                Console.Clear();
                isInSettingMenu = false;
                MainMenu.InitMainMenu();
            }

            static void RenderMusicSelectionText(Vector2 pos, string text, ConsoleColor color = ConsoleColor.White)
            {
                Console.ForegroundColor = color;
                Console.SetCursorPosition(pos.x, pos.y);
                Console.WriteLine(text);
            }
        }
    }
}
