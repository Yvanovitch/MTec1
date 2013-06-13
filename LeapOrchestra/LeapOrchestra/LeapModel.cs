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
        private int allerRetour;
        private int temp;
        private long temp2;
        private long tempo;

        public LeapModel()
        {
            previousPos = new Vector(0, 0, 0);
            allerRetour = 0;
            temp = 0;
            temp2 = 0;
            tempo = 0;
        }

        public void OnFrameRegistered(Frame frame)
        {
            Finger finger = 
            OnFingersRegistered(frame.Fingers, frame);
        }

        public void OnFingersRegistered(FingerList fingers, Frame frame)
        {
            if (previousPos.x <= fingers[0].TipPosition.x)
            {
                allerRetour = 0;
            }
            else
            {
                allerRetour = 1;
            }

            if (allerRetour != temp && frame.Timestamp - temp2 > 200000)
            {
                if (allerRetour == 0)
                {
                    Console.WriteLine("Aller");
                }
                else
                {
                    Console.WriteLine("Retour");
                }
                tempo = frame.Timestamp - temp2;
                Console.WriteLine(tempo);
                temp2 = frame.Timestamp;
            }

            previousPos = fingers[0].TipPosition;
            temp = allerRetour;
        }
    }
}
