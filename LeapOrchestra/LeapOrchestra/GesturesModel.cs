using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Leap;
using LeapOrchestra.Utils;

namespace LeapOrchestra
{
    class GesturesModel
    {
        public string pathSong;

        private Gesture lastGesture;

        public GesturesModel ()
        {
            pathSong = @"D:\Documents\Cours\Orchestra\Wonderful slippery thing.wav";
            lastGesture = Gesture.Invalid;
        }
        
        public void OnGesturesRegistered(GestureList gestures)
        {
            foreach (var gesture in gestures)
            {
                if (gesture.Type != lastGesture.Type && gesture.Type != Gesture.GestureType.TYPESWIPE)
                {
                    Console.WriteLine("Gesture : " + LeapGestures.GestureTypesLookUp[gesture.Type]);
                    if (gesture.Type == Gesture.GestureType.TYPECIRCLE)
                    {
                        //playSong(pathSong);
                    }
                    lastGesture = gesture;
                }
            }
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

    }
}
