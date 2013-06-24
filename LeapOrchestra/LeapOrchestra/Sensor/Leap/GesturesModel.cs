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
        private Gesture lastGesture;

        public GesturesModel ()
        {
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

        

    }
}
