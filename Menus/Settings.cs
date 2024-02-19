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

        static bool isInVolumeSetting = false;

        static bool isConfirmedSound = false;
        static bool isConfirmedMusic = false;

        static float volumeStep = .1f;                // Der Schritt, mit dem die Lautstärke erhöht werden soll
        private static float _currentSoundVolume;

        static internal Audio soundtrack { get; set; }  = new Audio();
        static internal Audio music { get; set; } = new Audio();

        //static internal float currentSoundVolume
        //{
        //    get => _currentSoundVolume; 
        //    set
        //    {
        //        float volume = value;
        //        if (volume > 1)
        //            volume = 1;
        //        else if (volume < 0)
        //            volume = 0;
        //        _currentSoundVolume = volume;
        //    }
        //}
        //private static float _currentMusicVolume;

        //static internal float currentMusicVolume
        //{
        //    get => _currentMusicVolume; 
        //    set
        //    {
        //        float volume = value;
        //        if (volume > 1)
        //            volume = 1;
        //        else if (volume < 0)
        //            volume = 0;
        //        _currentMusicVolume = volume;
        //    }
        //}

        static ConsoleColor selectedColor = ConsoleColor.Yellow;
        static ConsoleColor confirmedColor = ConsoleColor.Red;
        const ConsoleColor volumeColor = ConsoleColor.Green;
        private static Vector2 _posSelection = new Vector2(0, 0);

        static Vector2 posSelection
        {
            get => _posSelection; 
            set
            {
                int endPosY;
                int endPosX;

                _posSelection = value;

                if (_posSelection.x == 0)
                    endPosY = posSelectionEndColumn1;
                else
                    endPosY = posSelectionEndColumn2;
                endPosX = posSelectionEndRow;


                if (_posSelection.y > endPosY)
                    _posSelection.y = 0;
                else if (_posSelection.y < 0)
                    _posSelection.y = endPosY;

                if (_posSelection.x > endPosX)
                    _posSelection.x = 0;
                else if (_posSelection.x < 0)
                    _posSelection.x = endPosX;
            }
        }

        static int posSelectionEndColumn1 = 3;              // Definiert, wie groß die max. Größe von der 1, der 2. Spalte und der Reihe ist. (inkl. der Zahl 0)
        static int posSelectionEndColumn2 = 1;

        static int posSelectionEndRow = 1;


        static Vector2 posMusicA            = new Vector2(0, 8);
        static Vector2 posMusicB            = new Vector2(0, 9);
        static Vector2 posMusicC            = new Vector2(0, 10);
        static Vector2 posNoneMusic         = new Vector2(0, 11);

        static Vector2 posVolumeSoundText   = new Vector2(20, 9);
        static Vector2 posVolumeMusicText   = new Vector2(20, 10);
        static Vector2 posVolumeSound       = new Vector2(30, 9);
        static Vector2 posVolumeMusic       = new Vector2(30, 10);

        static internal void InitSettingsMenu()
        {
            isInSettingMenu = true;
            
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Settings");
            Console.WriteLine(String.Empty);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Use the down- and up arrow keys to navigate down and up.");
            Console.WriteLine("Press tab to navigate to the left and right side.");
            Console.WriteLine("Use the enter key to confirm your choice. ");
            Console.WriteLine("When you confirm the volume from the sound or music, you can change it with the left- or right arrow.");

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
                        if (isInVolumeSetting)      //Solange man sich in den Einstellungen befindet, wo man die Lautstärke verändert, dann ignoriere diese Taste
                            continue;

                        posSelection = new Vector2(posSelection.x, ++posSelection.y);

                        isConfirmedSound = false;
                        isConfirmedMusic = false;

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.UpArrow)
                    {
                        if (isInVolumeSetting)      //Solange man sich in den Einstellungen befindet, wo man die Lautstärke verändert, dann ignoriere diese Taste
                            continue;

                        posSelection = new Vector2(posSelection.x, --posSelection.y);

                        isConfirmedSound = false;
                        isConfirmedMusic = false;

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.Tab)
                    {
                        if (isInVolumeSetting)          //Solange man sich in den Einstellungen befindet, wo man die Lautstärke verändert, dann ignoriere diese Taste
                            continue;

                        posSelection = new Vector2(++posSelection.x, posSelection.y);
                        //Resette die Y-Achse
                        posSelection.y = 0;

                        isConfirmedSound = false;
                        isConfirmedMusic = false;

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.RightArrow)
                    {
                        // Wurde der Sound- oder die Musiklautstärke ausgewählt, dann erhöhe die Lautstärke mit der rechten Pfeiltaste
                        if (isConfirmedSound)
                        {
                            //currentSoundVolume += volumeStep;
                            //soundtrack.ChangeVolume(currentSoundVolume);
                            soundtrack.Volume += volumeStep;
                            isSelectionConfirmed = true;
                        }
                        else    if (isConfirmedMusic)
                        {
                            //currentMusicVolume += volumeStep;
                            //music.ChangeVolume(currentMusicVolume);
                            music.Volume += volumeStep;
                            isSelectionConfirmed = true;
                        }

                    }
                    else if (keyPressed.Key == ConsoleKey.LeftArrow)
                    {
                        // Wurde der Sound- oder die Musiklautstärke ausgewählt, dann verringer die Lautstärke mit der linken Pfeiltaste
                        if (isConfirmedSound)
                        {
                            //currentSoundVolume -= volumeStep;
                            //soundtrack?.ChangeVolume(currentSoundVolume);
                            soundtrack.Volume -= volumeStep;
                            isSelectionConfirmed = true;
                        }
                        else if (isConfirmedMusic)
                        {
                            //currentMusicVolume -= volumeStep;
                            //music.ChangeVolume(currentMusicVolume);
                            music.Volume -= volumeStep;
                            isSelectionConfirmed = true;
                        }
                    }
                    else if (keyPressed.Key == ConsoleKey.Enter)
                    {
                        //Solange man sich in der Lautstärke Einstellungen befindet, welche sich rechts befinden, soll bei der Enter taste immer zwischen Bestätigen und Selektieren ausgewählt werden, wenn die Enter taste gedrückt wird
                        if (posSelection.x == 1)
                        {
                            isInVolumeSetting = !isInVolumeSetting;

                            if (isInVolumeSetting)
                                isSelectionConfirmed = true;
                            else if (!isInVolumeSetting)
                                isSelectionChanged = true;

                            if (posSelection.Equals(new Vector2(1, 0)) && isInVolumeSetting) //Wenn der Sound bestätigt wurde, dann bestätige dies
                            {
                                isConfirmedSound = true;
                                isConfirmedMusic = false;

                                //RenderVolume(ConsoleColor.Red);
                            }
                            else if (posSelection.Equals(new Vector2(1, 1)) && isInVolumeSetting)
                            {
                                isConfirmedSound = false;
                                isConfirmedMusic = true;

                                //RenderVolume(ConsoleColor.Green, ConsoleColor.Red);
                            }
                            else
                            {
                                isConfirmedSound = false;
                                isConfirmedMusic = false;

                                RenderVolume();
                            }
                        }
                        // Wenn von der linken Seite was ausgewählt wird ...
                        else
                            isSelectionConfirmed = true;
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
                                RenderText(posVolumeSoundText, "Sound", selectedColor);
                            else if (isSelectionConfirmed)
                            {
                                RenderText(posVolumeSoundText, "Sound", confirmedColor);
                            }
                            break;
                        case 1:
                            ReRender();
                            if (isSelectionChanged)
                                RenderText(posVolumeMusicText, "Music", selectedColor);
                            else if (isSelectionConfirmed)
                            {
                                RenderText(posVolumeMusicText, "Music", confirmedColor);
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
                        RenderText(posNoneMusic, "None", selectedColor);
                    }
                    else if (isSelectionConfirmed)
                    {
                        Program.currentPathMusic = string.Empty;
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

                RenderText(posVolumeSoundText, "Sound");
                RenderText(posVolumeMusicText, "Music");

                if (posSelection.Equals(new Vector2(1, 0)) && isInVolumeSetting)
                    RenderVolume(ConsoleColor.Red);
                else if (posSelection.Equals(new Vector2(1, 1)) && isInVolumeSetting)
                    RenderVolume(ConsoleColor.Green, ConsoleColor.Red);
                else
                    RenderVolume();
            }

            static void RenderVolume(ConsoleColor volumeSoundColor = volumeColor, ConsoleColor volumeMusicColor = volumeColor)
            {
                //float volume = currentSoundVolume * 10;
                float volume = soundtrack.Volume * 10;
                for (int i = 0; i < 10; i++)
                {
                    if (i < volume)
                    {
                        RenderText(new Vector2(posVolumeSound.x + i, posVolumeSound.y), "#", volumeSoundColor);
                    }
                    else
                    {
                        RenderText(new Vector2(posVolumeSound.x + i, posVolumeSound.y), "#");
                    }
                }

                //volume = currentMusicVolume * 10;
                volume = music.Volume * 10;
                for (int i = 0; i < 10; i++)
                {
                    if (i < volume)
                    {
                        RenderText(new Vector2(posVolumeMusic.x + i, posVolumeMusic.y), "#", volumeMusicColor);
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
                    music.Play(pathMusic);
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
