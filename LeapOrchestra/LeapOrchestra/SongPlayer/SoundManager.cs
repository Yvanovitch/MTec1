using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NAudio.Midi;

namespace LeapOrchestra.SongPlayer
{
    class SoundManager
    {
        public NoteOutputStream noteSender;
        private SEND_MODE playMode;
        private MidiFileReader reader;

        enum SEND_MODE{ MIDI_NOTE, MIDI_BANG};

        public SoundManager()
        {
            noteSender = new MidiOutputStream();
            choosePlayMode();
        }

        public void readMidiFile(string path)
        {
            reader = new MidiFileReader(path);
            reader.SendNote += noteSender.SendNote;
        }

        public void chooseOutput(NoteOnEvent note)
        {
            //Permettre le changement entre stream Midi et stream OSC
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
            if (playMode == SEND_MODE.MIDI_BANG)
            {
                noteSender.SendBang();
            }
            else
            {
                reader.throwBang();
            }
        }

        public void Process()
        {
            reader.playNote();
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
