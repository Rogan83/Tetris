using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Im Game Over Menü sollte man auch auf Settings zugreifen können
namespace Tetris.Menus
{
    static internal class Settings
    {
        static bool isInSettingMenu = true;
        static ConsoleColor selectedColor = ConsoleColor.Red;

        static int selectionPos = 1;              //Die Position von der Auswahl des Textes, welcher mit einer anderen Schriftfarbe gehighlightet werden soll

        static Vector2 posMusicA =      new Vector2(0, 5);
        static Vector2 posMusicB =      new Vector2(0, 6);
        static Vector2 posMusicC =      new Vector2(0, 7);
        static Vector2 posNoneMusic =   new Vector2(0, 8);
        static internal void InitSettingsMenu()
        {
            isInSettingMenu = true;
            
            bool isSelectionChanged = false;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Settings");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Use the arrow keys to select your music.");

            if (Program.gamestate == GameState.MainMenu) 
                Console.WriteLine("Press Backspace to go back to the Main Menu.");
            else if (Program.gamestate == GameState.GameOverMenu)
                Console.WriteLine("Press Backspace to go back to the Game Over Menu.");

            HighlightSelectedMusic(selectionPos, false);

            while (isInSettingMenu)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);

                    if (keyPressed.Key == ConsoleKey.Backspace)
                    {
                        GoBack();
                    }
                    else if (keyPressed.Key == ConsoleKey.DownArrow)
                    {
                        selectionPos++;
                        if (selectionPos > 4)
                            selectionPos = 1;

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.UpArrow)
                    {
                        selectionPos--;
                        if (selectionPos < 1)
                            selectionPos = 4;

                        isSelectionChanged = true;
                    }
                }
                if (isSelectionChanged)
                {
                    HighlightSelectedMusic(selectionPos);
                    isSelectionChanged = false;
                }
            }

            static void HighlightSelectedMusic(int cursorPos, bool isPlayMusic = true)
            {
                switch (cursorPos)
                {
                    case 1:
                        RenderMusicSelectionText(posMusicA, "Music A", selectedColor);
                        RenderMusicSelectionText(posMusicB, "Music B");
                        RenderMusicSelectionText(posMusicC, "Music C");
                        RenderMusicSelectionText(posNoneMusic, "None");
                        if (isPlayMusic)
                        {
                            if (Program.gamestate == GameState.MainMenu)
                                Program.music.Play(Program.pathMusicA);
                            Program.currentPath = Program.pathMusicA;
                        }
                        break;
                    case 2:
                        RenderMusicSelectionText(posMusicA, "Music A");
                        RenderMusicSelectionText(posMusicB, "Music B", selectedColor);
                        RenderMusicSelectionText(posMusicC, "Music C");
                        RenderMusicSelectionText(posNoneMusic, "None");
                        if (isPlayMusic)
                        {
                            if (Program.gamestate == GameState.MainMenu)
                                Program.music.Play(Program.pathMusicB);
                            Program.currentPath = Program.pathMusicB;
                        }
                        break;
                    case 3:
                        RenderMusicSelectionText(posMusicA, "Music A");
                        RenderMusicSelectionText(posMusicB, "Music B");
                        RenderMusicSelectionText(posMusicC, "Music C", selectedColor);
                        RenderMusicSelectionText(posNoneMusic, "None");
                        if (isPlayMusic)
                        {
                            if (Program.gamestate == GameState.MainMenu)
                                Program.music.Play(Program.pathMusicC);
                            Program.currentPath = Program.pathMusicC;
                        }
                        break;
                    case 4:
                        RenderMusicSelectionText(posMusicA, "Music A");
                        RenderMusicSelectionText(posMusicB, "Music B");
                        RenderMusicSelectionText(posMusicC, "Music C");
                        RenderMusicSelectionText(posNoneMusic, "None", selectedColor);
                        if (isPlayMusic)
                        {
                            if (Program.gamestate == GameState.MainMenu)
                                Program.music.Stop();
                            Program.currentPath = String.Empty;
                        }
                        break;
                    default:
                        RenderMusicSelectionText(posMusicA, "Music A");
                        RenderMusicSelectionText(posMusicB, "Music B");
                        RenderMusicSelectionText(posMusicC, "Music C");
                        RenderMusicSelectionText(posNoneMusic, "None", selectedColor);
                        if (isPlayMusic)
                        {
                            if (Program.gamestate == GameState.MainMenu)
                                Program.music.Stop();
                            Program.currentPath = String.Empty;
                        }
                        break;
                }
                
            }

            static void GoBack()
            {
                Console.Clear();
                
                isInSettingMenu = false;

                if (Program.gamestate == GameState.MainMenu) 
                    MainMenu.InitMainMenu();
                else if (Program.gamestate == GameState.GameOverMenu)
                    GameOverMenu.InitGameOverMenu();
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
