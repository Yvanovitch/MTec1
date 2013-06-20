/*
 * Quand on n'a pas chargé de fichier midi, on envoi des bangs
 * On peut choisir entre envoyer les signaux (note ou bang) en OSC ou MIDI
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NAudio.Midi;
using System.IO;

namespace LeapOrchestra.SongPlayer
{
    class SoundManager 
    {
        public NoteOutputStream noteSender;
        private SEND_MODE playMode;
        private MidiFileReader reader;
        public event Action<int> sendTempo;
        

        enum SEND_MODE{ MIDI_NOTE, MIDI_BANG};

        public SoundManager()
        {
            noteSender = new MidiOutputStream();
            choosePlayMode();
        }

        public void readMidiFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Fichier inexistant");
                playMode = SEND_MODE.MIDI_BANG;
                return;
            }
            
            reader = new MidiFileReader(path);
            reader.SendNote += noteSender.SendNote;
            reader.SendProgramChange += noteSender.SendProgramChange;
            reader.analyzeProgramChange();
            Console.WriteLine("Midi File Loaded");
            playMode = SEND_MODE.MIDI_NOTE;
            return;
        }

        public int getTempo()
        {
            if (reader == null)
            {
                return 0;
            }
            else
            {
                return (int)reader.Tempo;
            }
        }
        public void chooseOutput(NoteOnEvent note)
        {
            //Permettre le changement entre stream Midi et stream OSC
            playMode = SEND_MODE.MIDI_BANG;
        }

        public void choosePlayMode ()
        {
            /*Permettre de choisir entre :
             * Envoyer des bangs dans les temps
             * Envoyer les notes à l'intérieur des temps synchronisées
             */
            playMode = SEND_MODE.MIDI_NOTE;
        }

        public void throwBang()
        {
            if (playMode == SEND_MODE.MIDI_NOTE)
            {
                reader.throwBang();
                sendTempo(this.getTempo());
            }
            else
            {
                noteSender.SendBang();
            }
        }

        public void Process()
        {
            while (true)
            {
                if (playMode == SEND_MODE.MIDI_NOTE)
                {
                    reader.playNote();
                    Thread.Sleep(10);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        //Mis de côté, ne sert à rien
        static void playSong(string path)
        {
            //path = @"D:\Documents\Cours\Orchestra\Wonderful slippery thing.wav");
            
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Path undefined");
                return;
            }
            
            System.Media.SoundPlayer song = new System.Media.SoundPlayer();
            song.SoundLocation = path;
            song.LoadAsync();
            song.PlayLooping();
            Console.WriteLine("Press Enter to Quit");
            ConsoleKeyInfo KeyInfo = Console.ReadKey();
            while (KeyInfo.Key != ConsoleKey.Enter)
            {
                KeyInfo = Console.ReadKey();
                Thread.Sleep((4 * 60 + 28) * 1000);
            }
            song.Stop();
            return;
        }

        public void Close()
        {
            noteSender.Close();
        }
    }
}
