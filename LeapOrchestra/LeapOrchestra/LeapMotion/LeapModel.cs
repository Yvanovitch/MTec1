using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapOrchestra.Sensor
{
    class LeapModel
    {
        private int previousSpeed1;
        private int previousSpeed2;
        private Vector previousVelocity;
        private Vector previousPosition;
        private int top;
        private long temp;
        private long tempo;
        private long previousTimestamp;

        public event Action sendBang;

        public LeapModel()
        {
            previousSpeed1 = 0;
            previousSpeed2 = 0;
            top = 0;
            temp = 0;
            tempo = 0;
            previousPosition = new Vector(0, 0, 0);
            previousVelocity = new Vector(-10, 0, 0);
            previousTimestamp = 0;
        }

        public void OnFrameRegistered(Frame frame)
        {
            Finger finger = frame.Fingers[0];
            Hand hand = frame.Hands[0];
            /*var tipVelocity = (int)finger.TipVelocity.Magnitude;
            
            if (previousSpeed1 < tipVelocity && previousSpeed1 < previousSpeed2 && tipVelocity < 500 && tempo > 100000)
            {
                top = 0;
            }
            else
            {
                top = 1;
            }

            tempo = frame.Timestamp - temp;

            if (top == 0)
            {
                Console.WriteLine(tempo);
                
                temp = frame.Timestamp;

                sendBang(); //On joue la note midi
            }

            previousSpeed2 = previousSpeed1;
            previousSpeed1 = tipVelocity;
             */

            //Solution YVAN 
            
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
            
            
            //Console.WriteLine("position : "+hand.PalmPosition.y);

            if (hand.PalmPosition.y > 300)
            {
                //Console.Beep(500, 300);
            }


            /*if (frame.Timestamp - previousTimestamp < 150)
                return;

            if ((previousVelocity.Magnitude - hand.PalmVelocity.Magnitude <  50 ) &&
                hand.PalmVelocity.Magnitude < 60 &&
                previousPosition.DistanceTo(hand.PalmPosition) > 130)
            {
                //sendBang();
                Console.WriteLine("Bang");
                Console.Beep(440, 100);
                if (hand.PalmVelocity.Magnitude < 30)
                    previousVelocity = hand.PalmVelocity;
                previousTimestamp = frame.Timestamp;
            }

            if(hand.PalmVelocity.Magnitude < 30)
                previousVelocity = hand.PalmVelocity;
            */
        }
    }
}
