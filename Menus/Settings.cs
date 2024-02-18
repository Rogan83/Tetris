using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Im Game Over Menü sollte man auch auf Settings zugreifen können
namespace Tetris.Menus
{
    //Todo: 
    //Eventuell noch die Option hinzufügen, wie die Soundeffekt- und die Hintergrundmusiklautstärke zu ändern
    static internal class Settings
    {
        static bool isInSettingMenu = true;
        static bool isSelectionChanged = false;
        static bool isSelectionConfirmed = false;

        static bool isConfirmedSound = false;
        static bool isConfirmedMusic = false;

        static float volumeStep = .1f;                // Der Schritt, mit dem die Lautstärke erhöht werden soll
        private static float _currentSoundVolume;

        static internal Audio soundtrack { get; set; }  = new Audio();
        static internal Audio music { get; set; } = new Audio();

        static internal float currentSoundVolume
        {
            get => _currentSoundVolume; 
            set
            {
                float volume = value;
                if (volume > 1)
                    volume = 1;
                else if (volume < 0)
                    volume = 0;
                _currentSoundVolume = volume;
            }
        }
        private static float _currentMusicVolume;

        static internal float currentMusicVolume
        {
            get => _currentMusicVolume; 
            set
            {
                float volume = value;
                if (volume > 1)
                    volume = 1;
                else if (volume < 0)
                    volume = 0;
                _currentMusicVolume = volume;
            }
        }

        static ConsoleColor selectedColor = ConsoleColor.Yellow;
        static ConsoleColor confirmedColor = ConsoleColor.Red;
        static ConsoleColor volumeColor = ConsoleColor.Green;

        static Vector2 posSelection     = new Vector2(0, 0);              //Die Position von der Auswahl des Textes, welcher mit einer anderen Schriftfarbe gehighlightet werden soll
        static Vector2 posSelectionEnd  = new Vector2(1, 3);                   // Die letzte Position

        static Vector2 posMusicA        = new Vector2(0, 5);
        static Vector2 posMusicB        = new Vector2(0, 6);
        static Vector2 posMusicC        = new Vector2(0, 7);
        static Vector2 posNoneMusic     = new Vector2(0, 8);

        static Vector2 posVolumeSoundText  = new Vector2(20, 5);
        static Vector2 posVolumeMusicText  = new Vector2(20, 6);
        static Vector2 posVolumeSound      = new Vector2(30, 5);
        static Vector2 posVolumeMusic      = new Vector2(30, 6);

        static internal void InitSettingsMenu()
        {
            isInSettingMenu = true;
            
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Settings");
            Console.WriteLine(String.Empty);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Use the arrow keys to select your music.");

            if (Program.gamestate == GameState.MainMenu)                // Wenn man sich vorher im Main Menü befand
                Console.WriteLine("Press Backspace to go back to the Main Menu.");
            else if (Program.gamestate == GameState.GameOverMenu)       // Wenn man sich vorher im Game Over Menü befand
                Console.WriteLine("Press Backspace to go back to the Game Over Menu.");

            // Init
            ReRender();
            isSelectionChanged = true;
            HighlightSelectedMusic(posSelection);
            isSelectionChanged = false;


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
                        posSelection.y++;
                        if (posSelection.y > posSelectionEnd.y)
                            posSelection.y = 0;

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.UpArrow)
                    {
                        posSelection.y--;
                        if (posSelection.y < 0)
                            posSelection.y = posSelectionEnd.y;

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.RightArrow)
                    {
                        // Wurde der Sound- oder die Musiklautstärke ausgewählt, dann erhöhe die Lautstärke mit der rechten Pfeiltaste
                        if (isConfirmedSound)
                        {
                            currentSoundVolume += volumeStep;
                            soundtrack.ChangeVolume(currentSoundVolume);
                        }
                        else if (isConfirmedMusic)
                        {
                            currentMusicVolume += volumeStep;
                            music.ChangeVolume(currentMusicVolume);
                        }
                        else
                        {
                            posSelection.x++;
                            if (posSelection.x > posSelectionEnd.x)
                                posSelection.x = 0;

                            posSelection.y = 0;
                        }

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.LeftArrow)
                    {
                        // Wurde der Sound- oder die Musiklautstärke ausgewählt, dann verringer die Lautstärke mit der linken Pfeiltaste
                        if (isConfirmedSound)
                        {
                            currentSoundVolume -= volumeStep;
                            soundtrack?.ChangeVolume(currentSoundVolume);
                        }
                        else if (isConfirmedMusic)
                        {
                            currentMusicVolume -= volumeStep;
                            music.ChangeVolume(currentMusicVolume);
                        }
                        else
                        {
                            posSelection.x--;
                            if (posSelection.x < 0)
                                posSelection.x = posSelectionEnd.x;

                            posSelection.y = 0;
                        }

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.Enter)
                    {
                        isSelectionConfirmed = true;

                        if (posSelection.Equals(new Vector2(1, 0))) //Wenn der Sound selektiert wurde, dann bestätige dies
                        {
                            isConfirmedSound = true;
                            isConfirmedMusic = false;
                        }
                        else if (posSelection.Equals(new Vector2(1, 1))) //Wenn die Musik selektiert wurde
                        //else if (posSelection == new Vector2(1, 1)) //Wenn die Musik selektiert wurde
                        {
                            isConfirmedSound = false;
                            isConfirmedMusic = true;
                        }
                        else
                        {
                            isConfirmedSound = false;
                            isConfirmedMusic = false;
                        }
                    }
                }
                if (isSelectionChanged)
                {
                    HighlightSelectedMusic(posSelection);
                    isSelectionChanged = false;
                }
                else if (isSelectionConfirmed)
                {
                    HighlightSelectedMusic(posSelection);
                    isSelectionConfirmed = false;
                }
            }

            static void HighlightSelectedMusic(Vector2 cursorPos)
            {
                if (cursorPos.x == 0)   // Wenn man sich im Menü ganz links befindet
                {
                    switch (cursorPos.y)
                    {
                        case 0:
                            ReRender();
                            if (isSelectionChanged)
                                RenderText(posMusicA, "Music A", selectedColor);
                            if (isSelectionConfirmed)
                            {
                                RenderText(posMusicA, "Music A", confirmedColor);
                                PlayMusic(Program.pathMusicA);
                            }
                            break;
                        case 1:
                            ReRender();
                            if (isSelectionChanged)
                                RenderText(posMusicB, "Music B", selectedColor);
                            if (isSelectionConfirmed)
                            { 
                                RenderText(posMusicB, "Music B", confirmedColor);
                                PlayMusic(Program.pathMusicB);
                            }
                            break;
                        case 2:
                            ReRender();
                            if (isSelectionChanged)
                                RenderText(posMusicC, "Music C", selectedColor);
                            if (isSelectionConfirmed)
                            {
                                RenderText(posMusicC, "Music C", confirmedColor);
                                PlayMusic(Program.pathMusicC);
                            }
                            break;
                        case 3:
                            ReRender();
                            RenderAndPlayMusicC();
                            break;
                        default:
                            ReRender();
                            RenderAndPlayMusicC();
                            break;
                    }
                }

                if (cursorPos.x == 1)
                {
                    switch (cursorPos.y)
                    {
                        case 0:
                            ReRender();
                            if (isSelectionChanged)
                                RenderText(posVolumeSoundText, "Sound:", selectedColor);
                            else if (isSelectionConfirmed)
                            {
                                RenderText(posVolumeSoundText, "Sound:", confirmedColor);
                            }
                            break;
                        case 1:
                            ReRender();
                            if (isSelectionChanged)
                                RenderText(posVolumeMusicText, "Music:", selectedColor);
                            else if (isSelectionConfirmed)
                            {
                                RenderText(posVolumeMusicText, "Music:", confirmedColor);
                            }
                            break;
                    }
                }

                

                static void RenderAndPlayMusicC()
                {
                    RenderText(posMusicA, "Music A");
                    RenderText(posMusicB, "Music B");
                    RenderText(posMusicC, "Music C");

                    if (isSelectionChanged)
                    {
                        Program.currentPathMusic = string.Empty;
                        RenderText(posNoneMusic, "None", selectedColor);
                    }
                    else if (isSelectionConfirmed)
                    {
                        music.Stop();
                        RenderText(posNoneMusic, "None", confirmedColor);
                    }
                }

                
            }

            static void ReRender()          // Render das ganze Menü neu
            {
                RenderText(posMusicA, "Music A");
                RenderText(posMusicB, "Music B");
                RenderText(posMusicC, "Music C");
                RenderText(posNoneMusic, "None");

                RenderText(posVolumeSoundText, "Sound:");
                RenderText(posVolumeMusicText, "Music:");

                RenderVolume();
            }

            static void RenderVolume()
            {
                float volume = currentSoundVolume * 10;
                for (int i = 0; i < 10; i++)
                {
                    if (i < volume)
                    {
                        RenderText(new Vector2(posVolumeSound.x + i, posVolumeSound.y), "#", volumeColor);
                    }
                    else
                    {
                        RenderText(new Vector2(posVolumeSound.x + i, posVolumeSound.y), "#");
                    }
                }

                volume = currentMusicVolume * 10;
                for (int i = 0; i < 10; i++)
                {
                    if (i < volume)
                    {
                        RenderText(new Vector2(posVolumeMusic.x + i, posVolumeMusic.y), "#", volumeColor);
                    }
                    else
                    {
                        RenderText(new Vector2(posVolumeMusic.x + i, posVolumeMusic.y), "#");
                    }
                }
            }

            static void PlayMusic(string pathMusic)
            {
                if (Program.gamestate == GameState.MainMenu)
                    music.Play(pathMusic, currentMusicVolume);
                Program.currentPathMusic = pathMusic;
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

            static void RenderText(Vector2 pos, string text, ConsoleColor color = ConsoleColor.White)
            {
                Console.ForegroundColor = color;
                Console.SetCursorPosition(pos.x, pos.y);
                Console.WriteLine(text);
            }
        }
    }
}
