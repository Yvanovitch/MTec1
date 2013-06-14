using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LeapOrchestra.SongPlayer
{
    class SoundPlayer
    {
        private string pathSong;
        public NoteOutputStream noteSender;
        
        public SoundPlayer(string path)
        {
            pathSong = path;
            noteSender = new MidiOutputStream();
        }

        public void chooseOutput()
        {
            //Permettre le changement entre stream Midi et stream OSC
        }

        static void playSong(string path)
        {
            System.Media.SoundPlayer song = new System.Media.SoundPlayer();
            song.SoundLocation = path/*@"I:\Tojumaster\Freedom_Or_Random_01_01.wav"*/;
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
