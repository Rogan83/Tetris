using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    
    static internal class Audio
    {
        
        static WaveOutEvent waveOut;

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

        static void OnAudioTimerElapsed(string audioPath)
        {
            AudioFileReader audioFileReader;

            // Erstelle einen neuen WaveOutEvent
            //using (var waveOut = new WaveOutEvent())
            waveOut = new WaveOutEvent();
            // Lade die Sounddatei
            if (!File.Exists(audioPath))
            {
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
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
