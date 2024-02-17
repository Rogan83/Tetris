using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tetris.Menus
{
    static internal class Settings
    {
        static bool isInSettingMenu = true;
        static internal void InitSettingsMenu()
        {
            isInSettingMenu = true;

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Settings");
            Console.WriteLine("");
            Console.WriteLine("Press Escape to go back to the Main Menu.");

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
        }
    }
}
