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
    internal class Audio
    {
        AudioFileReader? audioFileReader;
        
        private float _volume;
        internal float Volume
        {
            get => _volume; 
            set
            {
                float vol = value;

                if (vol > 1)
                    vol = 1;
                else if (vol < 0)
                    vol = 0;

                if (audioFileReader != null)
                {
                    audioFileReader.Volume = _volume = vol;
                }
            }
        }

        object lockObject = new object();
        internal WaveOutEvent waveOut { get; set; } = new WaveOutEvent(); // Erstelle einen neuen WaveOutEvent
        bool isStopEndless = false;

        Timer? audioTimer;

        internal Audio()
        {
            string audioPath = "Sounds/Drop.mp3";               // Default sound
            if (File.Exists(audioPath))
                audioFileReader = new AudioFileReader(audioPath);
        }

        internal void Play(string path, bool isEndlessLoop = false)
        {
            //audioTimer?.Dispose();
            Stop();
            audioTimer = new Timer(_ => OnAudioTimerElapsed(path, isEndlessLoop), null, 0, Timeout.Infinite);
        }

        internal void Stop()
        {
            audioTimer?.Dispose();
            waveOut.Stop();
            isStopEndless = true;
        }

        void OnAudioTimerElapsed(string audioPath, bool isEndlessLoop)
        {
            lock (lockObject)
            {
                // Lade die Sounddatei
                if (!File.Exists(audioPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Musikdatei nicht gefunden");
                }
                else
                {
                    // Verbinde den AudioFileReader mit WaveOutEvent
                    audioFileReader = new AudioFileReader(audioPath);
                    audioFileReader.Volume = Volume;
                    // Initialisiere den WaveOutEvent mit dem AudioFileReader
                    waveOut?.Dispose();
                    if (audioFileReader != null)
                    {
                        waveOut?.Init(audioFileReader);
                        // Starte die Wiedergabe
                        waveOut?.Play();
                    }

                    // Warte, bis die Wiedergabe abgeschlossen ist
                    if (!isEndlessLoop)
                        return;
                    else
                        isStopEndless = false;

                    while (!isStopEndless)
                    {
                        // Spiele den Soundeffekt erneut ab
                        if (audioFileReader == null)
                            return;

                        waveOut?.Play();

                        // Warte, bis der Soundeffekt beendet ist
                        while (waveOut?.PlaybackState != PlaybackState.Stopped)
                        {
                            Thread.Sleep(100);
                        }
                        // Stoppe die Wiedergabe, um sicherzustellen, dass sie zurückgesetzt wird
                        waveOut.Stop();

                        // Setze die Position des Audio-Readers auf den Anfang
                        audioFileReader.Position = 0;
                    }
                }
            }
        }
    }
}
