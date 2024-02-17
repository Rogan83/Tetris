﻿using NAudio.Wave;
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
        object lockObject = new object();
        internal WaveOutEvent waveOut { get; set; } = new WaveOutEvent(); // Erstelle einen neuen WaveOutEvent
        bool isStopEndless = false;

        Timer audioTimer;
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
                    
                    // Initialisiere den WaveOutEvent mit dem AudioFileReader
                    waveOut?.Dispose();
                    waveOut.Init(audioFileReader);

                    // Starte die Wiedergabe
                    waveOut.Play();

                    // Warte, bis die Wiedergabe abgeschlossen ist

                    if (!isEndlessLoop)
                        return;
                    else
                        isStopEndless = false;

                    while (!isStopEndless)
                    {
                        // Spiele den Soundeffekt erneut ab
                        waveOut.Play();

                        // Warte, bis der Soundeffekt beendet ist
                        while (waveOut.PlaybackState != PlaybackState.Stopped)
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
