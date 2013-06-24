using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapOrchestra.Sensor
{
    class LeapModel
    {
        private Vector previousVelocity;
        private Vector previousPosition;

        public event Action sendBang;

        public LeapModel()
        {
            previousPosition = new Vector(0, 0, 0);
            previousVelocity = new Vector(-10, 0, 0);
        }

        public void OnFrameRegistered(Frame frame)
        {
            Finger finger = frame.Fingers[0];
            Hand hand = frame.Hands[0];
            
             if ( ( (previousVelocity.x < 0 && hand.PalmVelocity.x > 0) ||
                (previousVelocity.x > 0 && hand.PalmVelocity.x < 0) ) &&
                previousPosition.DistanceTo(hand.PalmPosition) > 70 )
            {
                sendBang();
                //Console.WriteLine("Bang");
                //Console.Beep(440, 300);
                previousPosition = hand.PalmPosition;
                previousVelocity = hand.PalmVelocity;
                
            }

            if (hand.PalmPosition.y > 300)
            {
                //Console.Beep(500, 300);
            }
        }
    }
}
