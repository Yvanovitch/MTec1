using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapOrchestra
{
    class LeapModel
    {
        public void OnFingersRegistered(FingerList fingers)
        {

            Console.WriteLine(fingers[0].TipPosition.x);
        }
    }
}
