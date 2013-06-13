using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;
using LeapOrchestra.Utils;

namespace LeapOrchestra
{
    class GesturesModel
    {
        public void OnGesturesRegistered(GestureList gestures)
        {
            foreach (var gesture in gestures)
            {
                Console.WriteLine("Gesture : " + LeapGestures.GestureTypesLookUp[gesture.Type]);
            }
        }
    }
}
