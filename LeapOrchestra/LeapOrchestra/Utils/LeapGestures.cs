using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapOrchestra.Utils
{
    public static class LeapGestures
    {
        
        
        public static readonly Dictionary<Gesture.GestureType, string> GestureTypesLookUp = new Dictionary<Gesture.GestureType, string>()
                                     {
                                         {Gesture.GestureType.TYPEKEYTAP,"Tap gesture"},
                                         {Gesture.GestureType.TYPECIRCLE, "Circle gesture"},
                                         {Gesture.GestureType.TYPESWIPE, "Swipe gesture"},
                                         {Gesture.GestureType.TYPESCREENTAP, "Screen tap"}
                                     };
    }
}
