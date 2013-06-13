using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapOrchestra
{
    class LeapModel
    {
        private Vector previousPos;

        public LeapModel()
        {
            previousPos = new Vector(0, 0, 0);
        }
        
        public void OnFingersRegistered(FingerList fingers)
        {

            Console.WriteLine("Actuel : "+fingers[0].TipPosition.x+" previous : "+previousPos.x);

            previousPos = fingers[0].TipPosition;
        }
    }
}
