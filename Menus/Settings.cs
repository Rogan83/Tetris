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
        #region fields
        static bool isInSettingMenu = true;
        static bool isSelectionChanged = false;
        static bool isSelectionConfirmed = false;
        static bool isInVolumeSetting = false;

        static bool isConfirmedSound = false;
        static bool isConfirmedMusic = false;

        static float volumeStep = .1f;                // Der Schritt, mit dem die Lautstärke verändert werden soll

        static ConsoleColor selectedColor = ConsoleColor.Yellow;
        static ConsoleColor confirmedColor = ConsoleColor.Red;
        const ConsoleColor volumeColor = ConsoleColor.Green;

        static int posSelectionEndColumn1 = 3;              // Definiert, wie groß die max. Größe von der 1, der 2. Spalte und der Reihe ist. (inkl. der Zahl 0)
        static int posSelectionEndColumn2 = 1;
        static int posSelectionEndRow = 1;

        static Vector2 posSelection = new Vector2(0, 0);
        static Vector2 PosSelection
        {
            get => posSelection; 
            set
            {
                int endPosY;
                int endPosX;

                posSelection = value;

                if (posSelection.x == 0)
                    endPosY = posSelectionEndColumn1;
                else
                    endPosY = posSelectionEndColumn2;
                endPosX = posSelectionEndRow;


                if (posSelection.y > endPosY)
                    posSelection.y = 0;
                else if (posSelection.y < 0)
                    posSelection.y = endPosY;

                if (posSelection.x > endPosX)
                    posSelection.x = 0;
                else if (posSelection.x < 0)
                    posSelection.x = endPosX;
            }
        }

        static Vector2 posMusicA            = new Vector2(0, 8);
        static Vector2 posMusicB            = new Vector2(0, 9);
        static Vector2 posMusicC            = new Vector2(0, 10);
        static Vector2 posMusicOff          = new Vector2(0, 11);

        static Vector2 posVolumeSoundText   = new Vector2(20, 9);
        static Vector2 posVolumeMusicText   = new Vector2(20, 10);
        static Vector2 posVolumeSound       = new Vector2(30, 9);
        static Vector2 posVolumeMusic       = new Vector2(30, 10);
        #endregion

        #region properties
        static internal Audio soundtrack { get; set; } = new Audio();
        static internal Audio music { get; set; } = new Audio();
        #endregion

        #region methods
        static internal void InitSettingsMenu()
        {
            isInSettingMenu = true;
            
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Settings");
            Console.WriteLine(String.Empty);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Use the arrow keys to navigate into the menu.");
            Console.WriteLine("You can also use the tab key to navigate to the left and right side.");
            Console.WriteLine("Use the enter key to confirm your choice. ");
            Console.WriteLine("When you confirm the volume from the sound or music, you can change it with the left- or right arrow.");

            Console.Write("Press Backspace to return to the ");

            if (Program.previousGamestate == PreviousGameState.MainMenu)                // Wenn man sich vorher im Main Menü befand
                Console.Write("Main Menu.");
            else if (Program.previousGamestate == PreviousGameState.GameOverMenu)       // Wenn man sich vorher im Game Over Menü befand
                Console.Write("Game Over Menu.");

            // Initialsiere das Menü
            isSelectionChanged = true;
            HighlightText(PosSelection);
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

                        PosSelection = new Vector2(PosSelection.x, ++PosSelection.y);

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.UpArrow)
                    {
                        if (isInVolumeSetting)      //Solange man sich in den Einstellungen befindet, wo man die Lautstärke verändert, dann ignoriere diese Taste
                            continue;

                        PosSelection = new Vector2(PosSelection.x, --PosSelection.y);

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.Tab)
                    {
                        if (isInVolumeSetting)          //Solange man sich in den Einstellungen befindet, wo man die Lautstärke verändert, dann ignoriere diese Taste
                            continue;

                        PosSelection = new Vector2(++PosSelection.x, PosSelection.y);
                        //Resette die Y Position beim wechseln der Spalten
                        PosSelection.y = 0;

                        isSelectionChanged = true;
                    }
                    else if (keyPressed.Key == ConsoleKey.RightArrow)
                    {
                        if (isInVolumeSetting)
                        {
                            // Wurde der Sound- oder die Musiklautstärke ausgewählt, dann erhöhe die Lautstärke mit der rechten Pfeiltaste
                            if (isConfirmedSound)
                            {
                                soundtrack.Volume += volumeStep;
                                soundtrack.Play("Sounds/Drop.mp3");
                                Properties.Settings.Default.VolumeSoundtrack = soundtrack.Volume;
                                Properties.Settings.Default.Save();
                                isSelectionConfirmed = true;
                            }
                            else if (isConfirmedMusic)
                            {
                                music.Volume += volumeStep;
                                Properties.Settings.Default.VolumeMusic = music.Volume;
                                Properties.Settings.Default.Save();
                                isSelectionConfirmed = true;
                            }
                        }
                        else
                        {
                            PosSelection = new Vector2(++PosSelection.x, PosSelection.y);
                            //Resette die Y Position beim wechseln der Spalten
                            PosSelection.y = 0;

                            isSelectionChanged = true;
                        }
                    }
                    else if (keyPressed.Key == ConsoleKey.LeftArrow)
                    {
                        if (isInVolumeSetting)
                        {
                            // Wurde der Sound- oder die Musiklautstärke ausgewählt, dann verringer die Lautstärke mit der linken Pfeiltaste
                            if (isConfirmedSound)
                            {
                                soundtrack.Volume -= volumeStep;
                                soundtrack.Play("Sounds/Drop.mp3");
                                Properties.Settings.Default.VolumeSoundtrack = soundtrack.Volume;
                                Properties.Settings.Default.Save();
                                isSelectionConfirmed = true;
                            }
                            else if (isConfirmedMusic)
                            {
                                music.Volume -= volumeStep;
                                Properties.Settings.Default.VolumeMusic = music.Volume;
                                Properties.Settings.Default.Save();
                                isSelectionConfirmed = true;
                            }
                        }
                        else
                        {
                            PosSelection = new Vector2(--PosSelection.x, PosSelection.y);
                            //Resette die Y Position beim wechseln der Spalten
                            PosSelection.y = 0;

                            isSelectionChanged = true;
                        }
                    }
                    else if (keyPressed.Key == ConsoleKey.Enter)
                    {
                        //Solange man sich in der Lautstärke Einstellungen befindet, welche sich rechts befinden, soll bei der Enter taste immer zwischen Bestätigen und Selektieren ausgewählt werden, wenn die Enter taste gedrückt wird
                        if (PosSelection.x == 1)
                        {
                            isInVolumeSetting = !isInVolumeSetting;

                            if (isInVolumeSetting)
                                isSelectionConfirmed = true;
                            else if (!isInVolumeSetting)
                            {
                                isSelectionChanged = true;
                                //isConfirmedSound = false;
                                //isConfirmedMusic = false;
                            }

                            if (PosSelection.Equals(new Vector2(1, 0)) && isInVolumeSetting) //Wenn der Sound bestätigt wurde, dann bestätige dies
                            {
                                isConfirmedSound = true;
                                isConfirmedMusic = false;
                            }
                            else if (PosSelection.Equals(new Vector2(1, 1)) && isInVolumeSetting)
                            {
                                isConfirmedSound = false;
                                isConfirmedMusic = true;
                            }
                        }
                        // Wenn ein Musikstück ausgewählt wurde, welche sich auf der linken Seite befindet, dann bestätige diese Eingabe, damit diese entsprechend
                        // farblich dargestellt wird
                        else
                            isSelectionConfirmed = true;
                    }
                }
                // Stellt den Text, in Abhängigkeit, ob man sich nur mit den Cursor darüber befindet oder ob man diesen ausgewählt hat, farblich unterschiedlich dar.
                // Die Logik dazu befindet sich in der Methode "HighlightText". Setze anschließend den Flag zurück, damit diese Methode nur dann ausgeführt wird, wenn
                // man sich entweder im Menü navigiert oder etwas ausgewählt wird, da dort das Flag wieder auf true gesetzt wird.
                if (isSelectionChanged)
                {
                    HighlightText(PosSelection);
                    isSelectionChanged = false;
                }
                else if (isSelectionConfirmed)
                {
                    HighlightText(PosSelection);
                    isSelectionConfirmed = false;
                }
            }
            #region local methods
            static void HighlightText(Vector2 cursorPos)
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
                                PlayMusic(Program.PathMusicA);
                            }
                            break;
                        case 1:
                            ReRender();
                            if (isSelectionChanged)
                                RenderText(posMusicB, "Music B", selectedColor);
                            if (isSelectionConfirmed)
                            { 
                                RenderText(posMusicB, "Music B", confirmedColor);
                                PlayMusic(Program.PathMusicB);
                            }
                            break;
                        case 2:
                            ReRender();
                            if (isSelectionChanged)
                                RenderText(posMusicC, "Music C", selectedColor);
                            if (isSelectionConfirmed)
                            {
                                RenderText(posMusicC, "Music C", confirmedColor);
                                PlayMusic(Program.PathMusicC);
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
                // Rendert das ganze Menü neu
                static void ReRender()          // Render das ganze Menü neu
                {
                    RenderText(posMusicA, "Music A");
                    RenderText(posMusicB, "Music B");
                    RenderText(posMusicC, "Music C");
                    RenderText(posMusicOff, "Music off");

                    RenderText(posVolumeSoundText, "Sound");
                    RenderText(posVolumeMusicText, "Music");

                    if (PosSelection.Equals(new Vector2(1, 0)) && isInVolumeSetting)
                        RenderVolume(ConsoleColor.Red);
                    else if (PosSelection.Equals(new Vector2(1, 1)) && isInVolumeSetting)
                        RenderVolume(ConsoleColor.Green, ConsoleColor.Red);
                    else
                        RenderVolume();
                }
                static void RenderAndPlayMusicC()
                {
                    RenderText(posMusicA, "Music A");
                    RenderText(posMusicB, "Music B");
                    RenderText(posMusicC, "Music C");

                    if (isSelectionChanged)
                    {
                        RenderText(posMusicOff, "Music off", selectedColor);
                    }
                    else if (isSelectionConfirmed)
                    {
                        Properties.Settings.Default.MusicPath = string.Empty;
                        Properties.Settings.Default.Save();
                        music.Stop();
                        RenderText(posMusicOff, "Music off", confirmedColor);
                    }
                }
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
                if (Program.previousGamestate == PreviousGameState.MainMenu)
                    music.Play(pathMusic);

                Properties.Settings.Default.MusicPath = pathMusic;
                Properties.Settings.Default.Save();
            }

            static void GoBack()
            {
                Console.Clear();
                
                isInSettingMenu = false;

                if (Program.previousGamestate == PreviousGameState.MainMenu) 
                    MainMenu.InitMainMenu();
                else if (Program.previousGamestate == PreviousGameState.GameOverMenu)
                    GameOverMenu.InitGameOverMenu();
            }

            static void RenderText(Vector2 pos, string text, ConsoleColor color = ConsoleColor.White)
            {
                Console.ForegroundColor = color;
                Console.SetCursorPosition(pos.x, pos.y);
                Console.WriteLine(text);
            }
            #endregion
        }
    }
    #endregion
}
