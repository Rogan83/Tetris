using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    // Todo: darauf eine "normale" Klasse machen, damit mehrere Instanzen dieser Klasse erstellt werden können
    // und somit auch mehrere Soundtracks parallel abgespielt werden können (z.B. Musik und Soundeffekte)
    static internal class Audio
    {
        static object lockObject = new object();
        static WaveOutEvent waveOut = new WaveOutEvent(); // Erstelle einen neuen WaveOutEvent

        static Timer audioTimer;
        static internal void Play(string path)
        {
            audioTimer?.Dispose();
            audioTimer = new Timer(_ => OnAudioTimerElapsed(path), null, 0, Timeout.Infinite);
        }

        static internal void Stop()
        {
            audioTimer?.Dispose();
            waveOut.Stop();
        }
        static bool isStartAgain = false;
        static bool finish = false;
        static void OnAudioTimerElapsed(string audioPath)
        {
            lock (lockObject)
            {
                
                isStartAgain = true;
                AudioFileReader audioFileReader;

                // Lade die Sounddatei
                if (!File.Exists(audioPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Musikdatei nicht gefunden");
                    // Hier kannst du entscheiden, wie du mit dem Fehler umgehen möchtest
                }
                else
                {
                    // Verbinde den AudioFileReader mit WaveOutEvent
                    audioFileReader = new AudioFileReader(audioPath);
                    {
                        // Initialisiere den WaveOutEvent mit dem AudioFileReader
                        waveOut?.Dispose();
                        waveOut.Init(audioFileReader);

                        // Starte die Wiedergabe
                        waveOut.Play();

                        // Warte, bis die Wiedergabe abgeschlossen ist
                        while (waveOut.PlaybackState == PlaybackState.Playing && finish)
                        {
                            System.Threading.Thread.Sleep(100);
                            finish = false;
                        }
                        finish = true;
                    }
                }
            }
            
        }
    }
}
