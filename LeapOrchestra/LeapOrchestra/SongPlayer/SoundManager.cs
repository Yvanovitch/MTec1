using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LeapOrchestra.SongPlayer
{
    class SoundManager
    {
        public NoteOutputStream noteSender;

        public SoundManager()
        {
            noteSender = new MidiOutputStream();
        }

        public void chooseOutput()
        {
            //Permettre le changement entre stream Midi et stream OSC
        }

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
